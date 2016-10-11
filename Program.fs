open HtmlAgilityPack
open System.IO
open HttpFs.Client
open System.Text
open System
open Hopac
open System.Threading

let addToInstapaper username password (urls : string list) =
    for url in urls do
        try
            let response = 
                Request.createUrl Post "https://www.instapaper.com/api/add"
                |> Request.queryStringItem "username" username
                |> Request.queryStringItem "password" password
                |> Request.queryStringItem "url" url
                |> getResponse
                |> run
            if (response.statusCode <> 200 && response.statusCode <> 201) then
                printfn "Something bad happened: %i" response.statusCode
            else
                printfn "Committed url %s to instapaper" url
            Thread.Sleep(2500) // don't beat up Instapaper
        with
            | ex -> printfn "Exception: %s" (ex.GetBaseException()).Message
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
    
    // Re-globalize the URL
    let baseUrl = "http://edmunds.com"
    let fullUrls = List.map(fun x -> baseUrl + x) urls

    let username = "foobar@whatwhat.com"
    let password = "PASSWORD GOES HERE"

    // Write to file
    addToInstapaper username password fullUrls
    0
