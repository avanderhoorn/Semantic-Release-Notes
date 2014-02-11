module AST

// Notes sourced from: http://semanticreleasenotes.org/#Syntax [Sat.Feb.8.2014]

// At its most basic level, SRN should be able to take a paragraph 
// and interpret that as the summary of a given release.
type Summary = Summary of string

// The release note should be able to let you define the importance 
// feature or item that has been defined. 
type Priority = Priority of int

// The release note should be able to indicate what category an item is. 
// For instance, "+New", "+Change", "+Bug-Fix", "+Developer". 
type Category = Category of string

// A release note should be able to show which "items" are included 
// in a given release. Items typically include features, bug fixes, 
// or improvements. 
type Item = Item of Priority option * Summary * Category option
type Items = Items of Item list

// Lets the author group items which represent logical sections within 
// the application. These can be arbitrary in nature and specific to 
// the release notes which application is for. 
type Name = Name of string
type Section = 
    | Section of Name option * Priority option * Summary option * Items option

// TBD: As SRN allows for many releases to be defined within the one 
//      block of text, the system needs to provide a means by which an 
//      individual release can be identified. 
// type Release = ???

type ReleaseNotes = 
    | ReleaseNotes of Section list

(*
let ToObjectModel(ReleaseNotes n) =
    let (Summary summary, Items items) = n
    let a = new SemanticReleaseNotes.ReleaseNotes()
    a.Summary <- summary
    a
*)
