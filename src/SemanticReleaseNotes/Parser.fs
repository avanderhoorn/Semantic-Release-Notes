module Parser

open System
open FParsec

open AST
open SemanticReleaseNotes.Visitors

// convenience type for locking down generic type inference issues
// from: http://www.quanttec.com/fparsec/tutorial.html#fs-value-restriction
type private State = unit

let private ws = spaces 

let private ch c = pchar c
let private ch_ws c = ch c .>> ws
let private ws_ch_ws c = ws >>. ch c .>> ws

let private str s = pstring s
let private str_ws s = str s .>> ws
let private ws_str_ws s = ws >>. str s .>> ws

// make this compiler directive condition true to trace the parsers
#if xDEBUG
let (<!>) (p: Parser<_,_>) label : Parser<_,_> =
    fun stream ->
        printfn "%A: Entering %s" stream.Position label
        let reply = p stream
        printfn "%A: Leaving %s (%A)" stream.Position label reply.Status
        reply
#else
let (<!>) (p: Parser<_,_>) label : Parser<_,_> =
    fun stream -> p stream
#endif

(*
let private anyStringUntil_ch c = manySatisfy ((<>) c)
let private between_ch cStart cEnd p = between (ch cStart) (ch cEnd) p
let private string_between_ch cStart cEnd = between_ch cStart cEnd (anyStringUntil_ch cEnd) //<!> "stringSurroundedBy"
//let private quotedString = string_between_ch '"' '"'
let private hexn n = manyMinMaxSatisfy n n isHex
let private guidFromTuple t =
    let (((a,b),c),d),e = t
    let s = sprintf "%s-%s-%s-%s-%s" a b c d e
    Guid.Parse(s)
let private guid : Parser<Guid,State> = (hexn 8 .>> ch '-' .>>. hexn 4 .>> ch '-' .>>. hexn 4 .>> ch '-' .>>. hexn 4 .>> ch '-' .>>. hexn 12) |>> guidFromTuple //<!> "guid"
let private guid_between_str sStart sEnd = between (str sStart) (str sEnd) guid

let private emptyIfNone items = 
    match items with
    | Some x -> x
    | None -> []
*)

//let private restOfLineAsText skipNewline = restOfLine skipNewline |>> Text <!> "text"

// -------
let private newline_as_string = newline |>> fun x -> x.ToString()
let private eof_as_string = eof >>. preturn ""
let private eol = newline_as_string <|> eof_as_string

let private priority = ws >>. pint32 .>> skipString ". " |>> Priority <!> "item priority"

let private hn = many1Satisfy (fun c -> match c with '#' -> true | _ -> false) |>> (fun h -> h.Length) |>> Priority <!> "heading priority"
let private name = restOfLine true <!> "name"
let private heading = ws >>. hn .>>. name <!> "heading"

let private empty_line = newline >>. preturn "" <!> "empty line"
let private summaryLine = lookAhead (ws >>. noneOf "-+*#123456789") >>. many1CharsTill (noneOf "\r\n") eol <!> "summary line"
let private join (s:string) (list:string list) = String.Join(s, list)
let private summary = many1 (summaryLine <|> empty_line) |>> join "\n" |>> Summary <!> "summary"

let private unorderedItem = tuple3 (preturn None) (ws >>. anyOf "-+*" >>. ws >>. restOfLine true |>> Summary) (preturn None) <!> "unordered item"
let private orderedItem = tuple3 (priority |>> Some) (restOfLine true |>> Summary) (preturn None) <!> "ordered item" 
let private item = (attempt orderedItem <|> unorderedItem) |>> Item
let private items = many1 (attempt item) |>> Items <!> "items"

let private resolveSectionTuple (optHeading:(Priority*string) option) (summary:Summary option) (items:Items option) =
                let (optPriority,optName) = match optHeading with 
                                           | Some(p,n) -> (Some(p),Some(Name n))
                                           | None -> (None,None)
                (optName, optPriority, summary, items)

// Ensure that a section has at least one of name, summary, or items
let private ensureSectionResultIsValid (p: Parser<Name option*Priority option*Summary option*Items option,State>) =
    fun stream ->
        let reply = p stream
        if reply.Status = ReplyStatus.Ok then
            let (name,priority,summary,items) = reply.Result
            if name.IsSome || summary.IsSome || items.IsSome then
                Reply(reply.Result)
            else
                Reply(Error, expected "Either a heading, a summary paragraph, or a list of items.") 
        else
            Reply(reply.Status,reply.Error)

let private sectionContent = pipe3 (opt heading) (opt summary) (opt items) resolveSectionTuple 
let private section = ensureSectionResultIsValid sectionContent |>> Section <!> "section"

let private releaseNotes = ws >>. many1 section .>> ws .>> eof |>> ReleaseNotes <!> "release notes"

let private parser = releaseNotes .>> eof 

exception ParseError of string * ParserError

type ParseException (message:string, context:ParserError) =
    inherit ApplicationException(message, null)
    let Context = context

let ParseAST str =
    match run parser str with
    | Success(result, _, _)   -> result 
    | Failure(errorMsg, errorContext, _) -> raise (new ParseException(errorMsg, errorContext))

let Parse str = ParseAST str |> ASTToObjectModelVisitor.Visit

let PrettyPrint a = sprintf "%A" a

#if DEBUG
let Test p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

let Run p str =
    match run p str with
    | Success(result, _, _)   -> result
    | Failure(errorMsg, errorContext, _) -> raise (new ParseException(errorMsg, errorContext))
#endif