namespace SemanticReleaseNotes.Visitors

module ASTToObjectModelVisitor = 

    open AST
    open SemanticReleaseNotes

    let private ItemToObjectModel (item:Item) = 
        let (Item (priority,(Summary summary),category)) = item
        let result = new ObjectModel.Item()
        result.Summary <- summary
        result.Category <- null
        result

    let Visit (ast:ReleaseNotes) : ObjectModel.ReleaseNotes =
        let (ReleaseNotes notes) = ast
        let result = new ObjectModel.ReleaseNotes()
        match notes with
        | (section::tail) -> 
            let (Section (head,priority,summary,items)) = section
            match summary with
            | Some (Summary s) -> result.Summary <- s
            | None -> result.Summary <- null
            match items with
            | Some (Items n) -> 
                n 
                |> List.map ItemToObjectModel 
                |> List.map (fun x -> result.Items.Add(x)) 
                |> ignore
            | None -> result.Items.Clear()
            result
        | [] -> 
            result
