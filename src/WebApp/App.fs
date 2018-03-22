module App.Main

open Elmish
open Fable.Import
open Fable.Core
open Monaco
open HtmlConverter.Converter

type EditorState =
    | Loading
    | Loaded

type Model =
    { HtmlCode : string
      FSharpCode : string
      HtmlEditorState : EditorState
      FSharpEditorState : EditorState
      Navbar : App.Navbar.Model }

type Msg =
    | HtmlEditorLoaded
    | FSharpEditorLoaded
    | OnHtmlChange of string
    | NavbarMsg of App.Navbar.Msg
    | UpdateFSharpCode

let init _ =
    let (navbarModel, navbarMsg) = App.Navbar.init ()
    { HtmlCode = ""
      FSharpCode = ""
      HtmlEditorState = Loading
      FSharpEditorState = Loading
      Navbar = navbarModel }, Cmd.map NavbarMsg navbarMsg

let update model =
    function
    | OnHtmlChange htmlCode ->
        { model with HtmlCode = htmlCode}, Cmd.ofMsg UpdateFSharpCode

    | HtmlEditorLoaded ->
        { model with HtmlEditorState = Loaded }, Cmd.none

    | FSharpEditorLoaded ->
        { model with FSharpEditorState = Loaded }, Cmd.none

    | NavbarMsg navbarMsg ->
        let (navbarModel, navbarMsg, externalMessage) = App.Navbar.update model.Navbar navbarMsg

        let newModel, extraMsg =
            match externalMessage with
            | App.Navbar.ExternalMsg.NoOp -> model, Cmd.none

            | App.Navbar.ExternalMsg.LoadSample htmlCode ->
                { model with HtmlCode = htmlCode }, Cmd.ofMsg UpdateFSharpCode

            | App.Navbar.ExternalMsg.ConfigChanged ->
                model, Cmd.ofMsg UpdateFSharpCode

        { newModel with Navbar = navbarModel }, Cmd.batch [ Cmd.map NavbarMsg navbarMsg
                                                            extraMsg ]

    | UpdateFSharpCode ->
        { model with FSharpCode =
                        htmlToElmish
                            model.Navbar.Config.IndentationSize
                            model.Navbar.Config.IndentWith
                            model.HtmlCode }, Cmd.none


open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.Components
open Fulma.Layouts
open Fulma.Extensions
open Fulma.BulmaClasses

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
    let viewNavbar = App.Navbar.view (NavbarMsg >> dispatch)
    (fun model ->
        let isLoading =
            match model with
            | { HtmlEditorState = Loading; FSharpEditorState = Loading } -> true
            | _ -> false

        div [ ]
            [ viewNavbar model.Navbar
              div [ Class "page-content" ]
                [ PageLoader.pageLoader [ PageLoader.IsActive isLoading
                                          PageLoader.Color IsWhite ]
                    [ span [ Class "title" ]
                        [ str "We are getting everything ready for you" ] ]
                  Columns.columns [ Columns.IsGapless
                                    Columns.IsMultiline ]
                    [ Column.column [ Column.Width(Column.All, Column.IsHalf) ]
                        [ Message.message [ ]
                            [ Message.body [ ]
                                [ div [ ClassName Bulma.Properties.Alignment.HasTextCentered ]
                                    [ str "Type or paste HTML code" ] ] ] ]
                      Column.column [ Column.Width(Column.All, Column.IsHalf) ]
                        [ Message.message [ ]
                            [ Message.body [ ]
                                [ div [ ClassName Bulma.Properties.Alignment.HasTextCentered ]
                                    [ str "F# code compatible with Elmish" ] ] ] ] ]
                  Columns.columns [ Columns.IsGapless
                                    Columns.IsMultiline
                                    Columns.Props [ Style [ Height "100%" ] ] ]
                    [ Column.column [ Column.Width(Column.All, Column.IsHalf) ]
                        [ Editor.editor [ Editor.OnChange (OnHtmlChange >> dispatch)
                                          Editor.Value model.HtmlCode
                                          Editor.EditorDidMount (fun _ -> dispatch HtmlEditorLoaded) ] ]
                      Column.column [ Column.Width(Column.All, Column.IsHalf) ]
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
