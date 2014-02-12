using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SemanticReleaseNotes.Tests.TestHelpers;

namespace SemanticReleaseNotes.Tests
{
    /// <summary>
    /// Syntax examples sourced from: http://semanticreleasenotes.org/#Syntax [Sat.Feb.8.2014]
    /// </summary>
    [TestFixture]
    public class Syntax
    {
        private static class SyntaxExamples
        {
            /// <summary>
            /// A summary is one or more paragraphs of text.
            /// </summary>
            public static string Summary = 
@"This is a _project_ summary with two paragraphs. 
Lorem ipsum dolor sit amet consectetuer **adipiscing** elit. 
Aliquam hendreritmi posuere lectus.

Vestibulum `enim wisi` viverra nec fringilla in laoreet
vitae risus. Donec sit amet nisl. Aliquam [semper](?) ipsum
sit amet velit.";

            /// <summary>
            /// Items are indicated via a standard Markdown 
            /// <i>ordered</i> or <i>unordered</i> list.
            /// </summary>
            public static string Items = @"
- This is the _first_ *list* item.
- This is the **second** __list__ item.
- This is the `third` list item.
- This is the [forth](?) list item.";

        }

        /// <summary>
        /// The <see cref="JsonSerializerSettings"/> required to mimic 
        /// the JSON output on http://semanticreleasenotes.org
        /// </summary>
        private static readonly JsonSerializerSettings semanticReleaseNotesJsonSerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

        [Test]
        public void SummaryAST()
        {
            var input = SyntaxExamples.Summary;

            var result = Parser.ParseAST(input);

            //Assert.AreEqual(input.NormalizeLineEndings(),result);
            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void SummaryJSON()
        {
            var input = SyntaxExamples.Summary;

            var result = Parser.Parse(input);

            var json = JsonConvert.SerializeObject(result, semanticReleaseNotesJsonSerializerSettings);

            Approvals.Verify(json);
        }

        [Test]
        public void ItemsAST()
        {
            var input = SyntaxExamples.Items;

            var result = Parser.ParseAST(input);

            Approvals.Verify(Parser.PrettyPrint(result));
        }

        [Test]
        public void ItemsJSON()
        {
            var input = SyntaxExamples.Items;

            var result = Parser.Parse(input);

            var json = JsonConvert.SerializeObject(result, semanticReleaseNotesJsonSerializerSettings);

            Approvals.Verify(json);
        }

        /// <summary>
        /// Priority is indicated via a standard Markdown 
        /// <i>ordered list</i>.
        /// </summary>
        [Test]
        public void PriorityAST()
        {
            var input = @"
1. This is a High priority list item.
1. This is a High priority list item. 
2. This is a Normal priority list item. 
1. This is a High priority list item. 
2. This is a Normal priority list item.
3. This is a Minor priority list item.
3. This is a Minor priority list item. ";

            var result = Parser.ParseAST(input);

            Approvals.Verify(Parser.PrettyPrint(result));
        }

        /// <summary>
        /// A Section can be arbitrary in nature and specific
        /// to the release notes of the application.  Sections
        /// are indicated via a standard Markdown <i>header</i>.
        /// </summary>
        [Test]
        public void SectionsAST()
        {
            var input = @"
# Section
This is the summary for Section.
 - This is a Section scoped first list item.
 - This is a Section scoped second list item.

# Other Section
This is the summary for Other Section.
 - This is a Other Section scoped first list item.
 - This is a Other Section scoped second list item.
";

            var result = Parser.ParseAST(input);

            Approvals.Verify(Parser.PrettyPrint(result));
        }

        /// <summary>
        /// <para>
        /// In some cases we want to have the one document that 
        /// describes many releases. In this case, the syntax 
        /// simply allows you to define a heading which is the 
        /// Version Number. If you use this feature in conjunction 
        /// with sections, Section headers are also altered.
        /// </para>
        /// <para>
        /// For the purposes of interpretation of the version number, 
        /// it is assumed that you are using Semantic Visioning - 
        /// http://semver.org/. Normal Markdown headers are used 
        /// to describe the version number for each release and 
        /// the scope of reach release (which item, etc makes up 
        /// the release).
        /// </para>
        /// </summary>
        [Test]
        public void Release()
        {
            Assert.Inconclusive("Examples not yet specified.");
        }
    }
}