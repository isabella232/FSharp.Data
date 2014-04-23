﻿#if INTERACTIVE
#load "SetupTesting.fsx"
SetupTesting.generateSetupScript __SOURCE_DIRECTORY__ "FSharp.Data.DesignTime"
#load "__setup__FSharp.Data.DesignTime__.fsx"
#else
module internal Test
#endif

open System
open System.IO
open System.Net
open ProviderImplementation

//alow test cases that access the network to work when you're behind a proxy
WebRequest.DefaultWebProxy.Credentials <- CredentialCache.DefaultNetworkCredentials

let (++) a b = Path.Combine(a, b)
let resolutionFolder = __SOURCE_DIRECTORY__ ++ ".." ++ "tests" ++ "FSharp.Data.Tests" ++ "Data"
let outputFolder = __SOURCE_DIRECTORY__ ++ ".." ++ "tests" ++ "FSharp.Data.DesignTime.Tests" ++ "expected"
let assemblyName = "FSharp.Data.dll"

type Platform = Net40 | Portable7 | Portable47

let dump signatureOnly ignoreOutput platform saveToFileSystem (inst:TypeProviderInstantiation) =
    let runtimeAssembly = 
        match platform with
        | Net40 -> __SOURCE_DIRECTORY__ ++ ".." ++ "bin" ++ assemblyName
        | Portable7 -> __SOURCE_DIRECTORY__ ++ ".." ++ "bin" ++ "portable7" ++ assemblyName
        | Portable47 -> __SOURCE_DIRECTORY__ ++ ".." ++ "bin" ++ "portable47" ++ assemblyName    
    inst.Dump resolutionFolder (if saveToFileSystem then outputFolder else "") runtimeAssembly signatureOnly ignoreOutput
    |> Console.WriteLine

let dumpAll inst = 
    dump false false Net40 false inst
    dump false false Portable7 false inst
    dump false false Portable47 false inst

Html { Sample = "list_of_counties_wikipedia.html"
       PreferOptionals = false
       IncludeLayoutTables = false
       MissingValues = "NaN,NA,#N/A,:"
       Culture = "" 
       ResolutionFolder = "" }
|> dumpAll

Json { Sample = "optionals.json"
       SampleIsList = false
       RootName = ""
       Culture = "" 
       ResolutionFolder = "" }
|> dumpAll

Xml { Sample = "JsonInXml.xml"
      SampleIsList = true
      Global = false
      Culture = "" 
      ResolutionFolder = "" }
|> dumpAll

Csv { Sample = "AirQuality.csv"
      Separators = ";" 
      Culture = "" 
      InferRows = Int32.MaxValue
      Schema = ""
      HasHeaders = true
      IgnoreErrors = false
      AssumeMissingValues = false
      PreferOptionals = false
      Quote = '"'
      MissingValues = "NaN,NA,#N/A,:"
      CacheRows = true
      ResolutionFolder = "" }
|> dumpAll

let testCases = 
    __SOURCE_DIRECTORY__ ++ ".." ++ "tests" ++ "FSharp.Data.DesignTime.Tests" ++ "SignatureTestCases.config"
    |> File.ReadAllLines
    |> Array.map TypeProviderInstantiation.Parse

for testCase in testCases do
    dump false false Net40 true testCase
