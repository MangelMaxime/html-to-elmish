module App.Main

open Elmish
open Fable.Import
open Fable.Core
open Monaco
open Fable.PowerPack
open HtmlConverter.Converter

type EditorState =
    | Loading
    | Loaded

type Model =
    { HtmlCode : string
      FSharpCode : string
      HtmlEditorState : EditorState
      FSharpEditorState : EditorState }

type Msg =
    | HtmlEditorLoaded
    | FSharpEditorLoaded
    | OnHtmlChange of string
    | UpdateFSharpCode

let init _ =
    { HtmlCode = ""
      FSharpCode = ""
      HtmlEditorState = Loading
      FSharpEditorState = Loading }, Cmd.none

let update model =
    function
    | OnHtmlChange htmlCode ->
        { model with HtmlCode = htmlCode}, Cmd.ofMsg UpdateFSharpCode

    | HtmlEditorLoaded ->
        { model with HtmlEditorState = Loaded }, Cmd.none

    | FSharpEditorLoaded ->
        { model with FSharpEditorState = Loaded }, Cmd.none

    | UpdateFSharpCode ->
        { model with FSharpCode =
                        htmlToElmish model.HtmlCode }, Cmd.none

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.Components
open Fulma.Elements
open Fulma.Layouts
open Fulma.Extensions
open Fulma.BulmaClasses
open Fulma.Extra.FontAwesome

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

module CopyButton =

    open Fable.Core.JsInterop

    type Props =
        | Value of string

    let inline copyButtton (props: Props list) : React.ReactElement =
        ofImport "default" "./js/CopyButton.js" (keyValueList CaseRules.LowerFirst props) []

let private navbarItem dispatch =
    fun text sampleCode ->
        Navbar.Item.a [ Navbar.Item.Props [ OnClick (fun _ -> OnHtmlChange sampleCode |> dispatch)] ]
            [ str text ]

let private navbar dispatch =
    let viewNavbarItem = navbarItem dispatch
    fun _ ->
        Navbar.navbar [ Navbar.IsFixedTop ]
            [ Navbar.Brand.div [ ]
                [ Navbar.Item.a [ ]
                    [ strong [ ]
                        [ str "Html to Elmish" ] ] ]
              Navbar.Start.div [ ]
                [ Navbar.Item.div [ Navbar.Item.HasDropdown
                                    Navbar.Item.IsHoverable ]
                    [ Navbar.Link.a [ ]
                        [ str "Samples" ]
                      Navbar.Dropdown.div [ ]
                        [ viewNavbarItem "Hello world" Samples.helloWorld
                          viewNavbarItem "Bootstrap: Navbar" Samples.boostrapNavbar
                          viewNavbarItem "Fulma: Box" Samples.fulmaBox
                          viewNavbarItem "Fulma: Media Object" Samples.fulmaMediaObject
                          viewNavbarItem "Foundation: Top Bar" Samples.foundationTopBar ] ] ]
              Navbar.End.div [ ]
                [ Navbar.Item.div [ ]
                    [ Button.a [ Button.Props [ Href "https://github.com/MangelMaxime/html-to-elmish" ]
                                 Button.Color IsDark ]
                        [ Icon.faIcon [ ]
                            [ Fa.icon Fa.I.Github ]
                          span [ ]
                            [ str "Github" ] ] ] ] ]

let view dispatch =
    let viewNavbar = navbar dispatch
    (fun model ->
        let isLoading =
            match model with
            | { HtmlEditorState = Loading; FSharpEditorState = Loading } -> true
            | _ -> false

        div [ ]
            [ viewNavbar ()
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
                        [ div [ Class "copy-button" ]
                            [ CopyButton.copyButtton [ CopyButton.Value model.FSharpCode ] ]
                          Editor.editor [ Editor.Language "fsharp"
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
