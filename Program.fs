open HtmlAgilityPack
open System.IO
open HttpFs.Client
open System.Text
open System
open Hopac
open System.Threading

let createInstapaperRequest username password url =
    Request.createUrl Post "https://www.instapaper.com/api/add"
    |> Request.queryStringItem "username" username
    |> Request.queryStringItem "password" password
    |> Request.queryStringItem "url" url

let addToInstapaper username password (urls : string list) =
    for url in urls do
        try
            let request = createInstapaperRequest username password url
            let resp = request |> getResponse |> run
            printfn "Committed %s and got StatusCode=%d" url resp.statusCode
        with
            | ex -> printfn "Exception %s" (ex.GetBaseException()).Message
    ()

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
        // Distinct, because even though Instapaper handles that, why send the requests?
        |> Seq.distinct
        |> List.ofSeq
    
    // Re-globalize the URL
    let baseUrl = "http://edmunds.com"
    let fullUrls = List.map(fun x -> baseUrl + x) urls


    // Write to file
    addToInstapaper username password fullUrls
    0
