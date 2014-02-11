namespace SemanticReleaseNotes

type ReleaseNotes() =
    let mutable summary = ""
    let mutable items = ([] |> List.toSeq : seq<string>)

    member this.Summary 
        with get() = summary
        and set(value) = summary <- value

    member this.Items 
        with get() = items
        and set(value) = items <- value
