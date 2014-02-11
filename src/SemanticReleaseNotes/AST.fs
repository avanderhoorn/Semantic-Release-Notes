module AST

type Items = Items of string list

//ddd

type Text = Text of string
type Priority = Priority of int
type Heading = Heading of Priority * Text

type Note = 
    | Section of Heading 
    | OrderedItem of Priority * Text
    | UnorderedItem of Text
    | Summary of Text 

type ReleaseNotes = ReleaseNotes of (Note list)

(*
let ToObjectModel(ReleaseNotes n) =
    let (Summary summary, Items items) = n
    let a = new SemanticReleaseNotes.ReleaseNotes()
    a.Summary <- summary
    a
*)
