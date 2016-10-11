open HtmlAgilityPack
open System.IO

let dumpUrls filename (urls : string list) =
    use fp = File.CreateText(filename)
    for url in urls do
        fp.WriteLine("{0}", url)

[<EntryPoint>]
let main argv = 
    let doc = new HtmlDocument()
    doc.Load("1989 Yugo GVL Long-Term Road Test - New Updates.html")
    let urls =
        // All links
        doc.DocumentNode.SelectNodes("//a[@href]")
        // Convert the .NET collection into an F# native list
        |> List.ofSeq
        // Get the href contents
        |> List.map(fun a -> a.Attributes.["href"].Value)
        // Then only get long term road test URLs
        |> List.filter(fun url -> url.Contains("long-term-road-test"))
        // And only ones about Yugos...
        |> List.filter(fun url -> url.Contains("yugo"))
        // Just in case there are duplicates
        |> List.distinct
    
    // Re-globalize the URL
    let baseUrl = "http://edmunds.com"
    let fullUrls = List.map(fun x -> baseUrl + x) urls

    // Write to file
    dumpUrls "output.txt" fullUrls
    0
