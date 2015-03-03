/* Author:

*/

if (typeof String.prototype.endsWith !== 'function') {
    String.prototype.endsWith = function(suffix) {
        return this.indexOf(suffix, this.length - suffix.length) !== -1;
    };
}

if (typeof String.prototype.titleize !== 'function') {
    String.prototype.titleize = function() {
      var words = this.split(' ')
      var array = []
      for (var i=0; i<words.length; ++i) {
        array.push(words[i].charAt(0).toUpperCase() + words[i].toLowerCase().slice(1))
      }
      return array.join(' ')
    };
}

var processSyntax = (function () {
    var linkProcessor = {
            pattern : /\[\[(\S+)\]\[(\S+)\]\]/i,
            process : function (input, obj) {
                var match =  this.pattern.exec(input);
                        
                if (match) {
                    if ($.isArray(obj)) {
                        obj.push(match[1]);
                        obj.push(match[2]);
                    }
                    else
                        obj[match[1]] = match[2];
                }
                        
                return obj;
            }
        },
        lineProcessor = {
            options : [{ 
                pattern : /^# ([\w\s*]*)/i,
                test : function (input) {
                    return this.pattern.test(input);
                },
                process : function (obj, input) {
                    var title = this.pattern.exec(input),
                        links = linkProcessor.process(input, {}),
                        feature = { items : [] };
                            
                    feature.name = title[1];
                    if (links.icon)
                        feature.icon = links.icon;
                            
                    // Store results 
                    if (!obj.sections)
                        obj.sections = []; 
                    obj.sections.push(feature);
                }
            },
            {
                pattern : /^ [\-\+\*]|([123])\. /i, 
                categoryPattern : /\+([\w-]+)/i,
                test : function (input) {
                    return this.pattern.test(input);
                },
                process : function (obj, input) {
                    var priority = this.pattern.exec(input),
                        links = linkProcessor.process(input, []),
                        item = {};
                            
                    if (priority && !isNaN(priority[1])) 
                        item.priority =  priority[1]; 
                    input = input.replace(this.pattern, '');
                    
                    // handle categories
                    
                    if(!item.categories) {
                        item.categories = [];
                    }
                    
                    var category;
                    while(category = this.categoryPattern.exec(input)) {
                        item.categories.push(category[1].replace('-', ' ').titleize());
                        var replacement = category[1];
                        if(input.endsWith(category[1])) {
                            replacement = '';
                        }
                        input = input.replace(this.categoryPattern, replacement);
                    }
                    
                    if (links.length > 0) {
                        item.taskId = links[0];
                        item.teskLink = links[1];
                        input = input.replace(linkProcessor.pattern, '');
                    }
                    item.summary = input.trim();

                    // Store results 
                    if (!obj.sections) {
                        if (!obj.items)
                            obj.items = [];
                        obj.items.push(item);
                    }
                    else 
                        obj.sections[obj.sections.length - 1].items.push(item); 
                }
            }],
            primary : {
                pattern : /^[a-zA-Z0-9]/i,
                process : function (obj, input, nextInput) { 
                    var item = obj;
                    if (obj.sections) 
                        item = obj.sections[obj.sections.length - 1];

                    if (!item.summary)
                        item.summary = ''; 

                    input = input.trim();
                    if (input === '' && nextInput && this.pattern.test(nextInput))
                        input = '\n\n'; 
                            
                    item.summary += input; 
                }
            }
        },
        process = function (raw) {
            var result = {},
                rawLines = raw.split('\n');

            for (var rawLineIndex in rawLines) {
                var rawLine = rawLines[rawLineIndex],
                    matched = false;

                // Process the line
                for (var optionIndex in lineProcessor.options) {
                    var option = lineProcessor.options[optionIndex];
                            
                    if (option.test(rawLine)) {
                        option.process(result, rawLine);
                        matched = true;
                    }
                }
                if (!matched)
                    lineProcessor.primary.process(result, rawLine, rawLines[+rawLineIndex + 1]);
            }
            
            return result;
        };
             
    return {
        process : process  
    };
})();
      
var formatSyntax = (function () {
    var formatHtmlParser = [
            {
                pattern : /\*\*([\S ]+)\*\*|\_\_([\S ]+)\_\_/ig,
                process : function(val) {
                    return val.replace(this.pattern, '<strong>$1$2</strong>');
                }
            },
            {
                pattern : /\_([\S ]+)\_|\*([\S ]+)\*/ig,
                process : function(val) {
                    return val.replace(this.pattern, '<em>$1$2</em>');
                }
            },
            {
                pattern : /\`([\S ]+)\`/ig,
                process : function(val) {
                    return val.replace(this.pattern, '<code>$1</code>');
                }
            },
            {
                pattern : /\[([\S ]+)\]\(([\S ]+)\)/ig,
                process : function(val) {
                    return val.replace(this.pattern, '<a href="$2">$1</a>');
                }
            }
        ],
        processString = function (val) {
            if (val) {
                for (var fomatterIndex in formatHtmlParser) 
                    val = formatHtmlParser[fomatterIndex].process(val);
                val = val.replace(/\n/g, '<br />');
            }
            else
                val = '';
            return val;
        },
        processList = function (items) { 
            var result = '',
                hasPriorities = false;

            for (var itemIndex in items) {
                var item = items[itemIndex],
                    attr = '';
                    
                if (item.priority) {
                    attr = ' data-content="' + item.priority + '"';
                    hasPriorities = true;
                }
                result += '<li' + attr + '>';
                if(item.categories) {
                    result += '{';
                    for(var categoryIndex in item.categories) {
                        if(categoryIndex > 0) {
                            result += ', ';
                        }
                        result += item.categories[categoryIndex];
                    }
                    result += '}';
                }
                
                var summary = item.summary;
                for (var fomatterIndex in formatHtmlParser) 
                    summary = formatHtmlParser[fomatterIndex].process(summary);
                result += summary;
                    
                if (item.taskId)
                    result += ' <a href="' + item.teskLink + '">' + item.taskId + '</a>';
                result += '</li>';
            }

            result =  '<ul' + (hasPriorities ? ' class="custom"' : '') + '>' + result + '</ul>';
                    
            return result;
        },
        process = function (val) {
            var result = '';
              
            result += '<div>' + processString(val.summary) + processList(val.items) + '</div>';
            for (var featureIndex in val.sections) {
                var feature = val.sections[featureIndex];
                
                result += '<div><h1>' + feature.name + '</h1>' + processString(feature.summary) + processList(feature.items) + '</div>';
            }

            return result; 
        };
            
    return {
        process : process
    };
})();
      
var formatJson = (function () {
    var process = function(val) {
        var result = '',
            stack = [],
            stackTop = { indent: '' },
            strLen = val.length,
            char = '',
            indentStr = '    ',
            newLine = '\r\n';
        
        for (var i = 0; i < strLen; i++) {
            char = val[i];
            
            if (char == '{' || char == '[') {
                stack.push(stackTop = {
                    isArray: char == '[',
                    isOutterArray: char == '[' && val[i + 1] == '[',
                    indent: stackTop.indent + (!stackTop.isOutterArray ? indentStr : '')
                }); 
            }
            
            if (char == '}' || (char == ']' && stackTop.isOutterArray))
                result += newLine + (stack.length > 1 ? stack[stack.length - 2].indent : '');
             
            result += ((char == ']' && !stackTop.isOutterArray) || char == ':' ? ' ' : '') + char + (char == '[' || char == ':' || (char == ',' && stackTop.isArray) ? ' ' : '');
             
            if ((char == ',' && (!stackTop.isArray || stackTop.isOutterArray)) || char == '{' || (char == '[' && stackTop.isOutterArray))
                result += newLine + stackTop.indent;
            
            if (char == '}' || char == ']') {
                stack.pop();
                stackTop = stack[stack.length - 1];
            } 
        }

        return result;
    };
            
    return {
        process : process
    };
})();
  
var process = function(scope, text) { 
    var data = processSyntax.process(text),
        stringData = formatJson.process(JSON.stringify(data)),
        stringHtml = formatSyntax.process(data);

    scope.find('.result').html(stringHtml);
    scope.find('.object').html(stringData); 
};

var navigate = function (evt) {
    $(location.hash).attr('checked', 'checked');
};

$(function() {

    if (location.hash)
        navigate();

    $(window).on('hashchange', navigate);

    $('.code').each(function(){
        var scope = $(this).closest('.container'),
            text = scope.find('.code').text();
        process(scope, text);
    });

    $('.tabs a').click(function () {
        var item = $(this),
            index = item.attr('data-tab'),
            ul = item.closest('ul'),
            ulNext = ul.next();

        ul.find('.active').removeClass('active');
        ulNext.find('.active').removeClass('active');

        ul.find('a[data-tab="' + index + '"]').addClass('active');
        ulNext.find('li[data-tab="' + index + '"]').addClass('active'); 
    });

    var execute = function() {
        var scope = $('.execute').closest('.section-editor'),
            text = scope.find('.editor').val();
        process(scope, text);
        scope.find('.result-container').show();
    };
    $('.execute').click(function() {
        execute();
    });
    execute();
});


