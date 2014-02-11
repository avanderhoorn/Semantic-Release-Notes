namespace SemanticReleaseNotes.ObjectModel
 
open System
open System.Collections.Generic

type Item() =
    let mutable priority = new Nullable<int>()
    let mutable summary = ""
    let mutable category = ""
    
    member this.Priority 
        with get() = priority
        and set(value) = priority <- value

    member this.Summary 
        with get() = summary
        and set(value) = summary <- value

    member this.Category 
        with get() = category
        and set(value) = category <- value
    
type ReleaseNotes() =
    let mutable summary = ""
    let mutable items = new List<Item>()
    member this.Summary 
        with get() = summary
        and set value = summary <- value
    member this.Items
        with get() = items
        and set value = items <- value
