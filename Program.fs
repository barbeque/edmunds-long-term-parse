open HtmlAgilityPack
open System.IO
open HttpFs.Client
open System.Text
open System
open Hopac
open System.Threading

let createInstapaperRequest username password url =
    Request.createUrl Post "https://www.instapaper.com/api/add"
    |> Request.queryStringItem "url" url
    |> Request.basicAuthentication username password
    |> Request.timeout 3500<ms>
    |> Request.keepAlive false

let addToInstapaper username password (urls : string list) =
    for url in urls do
        printfn "Submitting url %s" url
        try
            let request = createInstapaperRequest username password url
            let resp = request |> Request.responseAsString |> run
            printfn "Committed %s and got Response=%s" url resp
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
        
    let username = "foobar"
    let password = "foobar"

    // Write to file
    addToInstapaper username password fullUrls
    0
