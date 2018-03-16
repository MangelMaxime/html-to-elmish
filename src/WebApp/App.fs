module App

open Elmish
open Fable.Import
open Fable.Core
open Monaco
open Fable.SimpleXml

type EditorState =
    | Loading
    | Loaded

type Model =
    { HtmlCode : string
      FSharpCode : string
      HtmlEditorState : EditorState
      FSharpEditorState : EditorState }

let init _ =
    { HtmlCode = """<div class="container">
  <div class="notification">
    This container is <strong>centered</strong> on desktop.
  </div>
</div>"""
      FSharpCode = ""
      HtmlEditorState = Loading
      FSharpEditorState = Loading }, Cmd.none

type Msg =
    | HtmlEditorLoaded
    | FSharpEditorLoaded
    | OnHtmlChange of string

let attributesToString (indentationLevel : int) (attributes : Map<string,string>) =
    let indentation = String.replicate indentationLevel "\t"

    attributes
    |> Map.toList
    |> List.map (fun (key, value) ->
        key + " " + value
    )
    |> String.concat ("\n" + indentation)
    |> (fun str ->
        "[ " + str + " ]"
    )

let rec xmlElementToElmish (indentationLevel : int) (xmlElement : XmlElement)  =
    match xmlElement with
    | { IsTextNode = true
        Content = content } ->
            "str \"" + content + "\""
    | { Name = tagName
        Attributes = attributes
        Children = children } ->
            let childrenStr =
                children
                |> List.map (xmlElementToElmish (indentationLevel + 1))
                |> String.concat "\n"
                |> (fun str ->
                    "[ " + str + " ]"
                )

            let indentation = String.replicate indentationLevel "\t"

            indentation + tagName + " " + (attributesToString indentationLevel attributes) + "\n" + childrenStr

let update model =
    function
    | OnHtmlChange htmlCode ->
        let newModel =
            match SimpleXml.tryParseElement htmlCode with
            | None ->
                printfn "Failed to parse the htmlCode"
                { model with HtmlCode = htmlCode }
            | Some xmlElement ->
                { model with HtmlCode = htmlCode
                             FSharpCode = xmlElementToElmish 0 xmlElement }

        newModel, Cmd.none

    | HtmlEditorLoaded ->
        { model with HtmlEditorState = Loaded }, Cmd.none

    | FSharpEditorLoaded ->
        { model with FSharpEditorState = Loaded }, Cmd.none

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Components
open Fulma.Layouts
open Fulma.Extensions

module Monaco =

    open Fable.Core.JsInterop

    type Props =
        | Width of obj
        | Height of obj
        | Value of obj
        | DefaultValue of obj
        | Language of string
        | Theme of string
        | Options  of Monaco.Editor.IEditorConstructionOptions
        | OnChange of (obj * obj -> unit)
        | EditorWillMount of (Monaco.IExports -> unit)
        | EditorDidMount of (Monaco.Editor.IEditor * Monaco.IExports -> unit)
        | RequireConfig of obj

    let inline editor (props: Props list) : React.ReactElement =
        ofImport "default" "react-monaco-editor" (keyValueList CaseRules.LowerFirst props) []

module Editor =

    open Fable.Core.JsInterop

    type Props =
        | OnChange of (string -> unit)
        | Value of string
        | Language of string
        | IsReadOnly of bool
        | EditorDidMount of (unit -> unit)

    let inline editor (props: Props list) : React.ReactElement =
        ofImport "default" "./js/Editor.js" (keyValueList CaseRules.LowerFirst props) []

let view dispatch =
    (fun model ->
        let isLoading =
            match model with
            | { HtmlEditorState = Loading; FSharpEditorState = Loading } -> true
            | _ -> false

        div [ ]
            [ Navbar.navbar [ Navbar.IsFixedTop ]
                [ Navbar.Brand.div [ ]
                    [ str "Html to Elmish" ] ]
              div [ Class "page-content" ]
                [ PageLoader.pageLoader [ PageLoader.IsActive isLoading ]
                    [ ]
                  Columns.columns [ Columns.IsGapless
                                    Columns.Props [ Style [ CSSProp.Width "100%" ] ] ]
                    [ Column.column [ ]
                        [ Editor.editor [ Editor.OnChange (OnHtmlChange >> dispatch)
                                          Editor.Value model.HtmlCode
                                          Editor.EditorDidMount (fun _ -> dispatch HtmlEditorLoaded) ] ]
                      Column.column [ ]
                        [ Editor.editor [ Editor.Language "fsharp"
                                          Editor.IsReadOnly true
                                          Editor.Value model.FSharpCode
                                          Editor.EditorDidMount (fun _ -> dispatch FSharpEditorLoaded) ] ] ]
                ]
            ]
    )

open Elmish.React
open Elmish.HMR

Program.mkProgram init update view
|> Program.withHMR
|> Program.withReact "elmish-app"
|> Program.run
