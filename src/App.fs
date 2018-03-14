module App

open Elmish
open Fable.Import
open Monaco

type Model =
    { Temp : string }

let init _ =
    { Temp = "" }, Cmd.none

type Msg =
    | NoOp

let update model =
    function
    | NoOp -> model, Cmd.none


open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Components
open Fulma.Layouts

let subView dispatch =
    (fun arg1 ->
        str "I am a subView"
    )

// let createEditor(domElement) =

//     let options = jsOptions<Monaco.Monaco.Editor.IEditorConstructionOptions>(fun o ->
//         let minimapOptions =  jsOptions<Monaco.Editor.IEditorMinimapOptions>(fun oMinimap ->
//             oMinimap.enabled <- Some false
//         )
//         o.language <- Some "html"
//         o.fontSize <- Some 14.
//         o.theme <- Some "vs-dark"
//         o.value <- Some "<h1>Maxime</h1>"
//         o.minimap <- Some minimapOptions
//     )

//     let services = createEmpty<Monaco.Editor.IEditorOverrideServices>
//     let ed = Monaco.editor.create(domElement, options, services)
//     ed

// let mutable htmlEditor = Unchecked.defaultof<Monaco.Editor.IEditor>

module Monaco =

    open Fable.Core
    open Fable.Core.JsInterop

    type Props =
        | Width of obj
        | Height of obj
        | Value of obj
        | DefaultValue of obj
        | Language of string
        | Theme of string
        | Options  of Monaco.Monaco.Editor.IEditorConstructionOptions
        | OnChange of (obj * obj -> unit)
        | EditorWillMount of (obj -> unit)
        | EditorDidMount of (obj * obj -> unit)
        | RequireConfig of obj

    let inline editor (props: Props list) : React.ReactElement =
        ofImport "default" "react-monaco-editor" (keyValueList CaseRules.LowerFirst props) []

open Fable.Core.JsInterop

let view dispatch =
    let subView = subView dispatch

    let editorOptions = jsOptions<Monaco.Editor.IEditorConstructionOptions> (fun o ->
        let minimapOptions = jsOptions<Monaco.Editor.IEditorMinimapOptions>(fun oMinimap ->
                oMinimap.enabled <- Some false
            )

        o.minimap <- Some minimapOptions
    )

    (fun model ->
        div [ ]
            [ Navbar.navbar [ Navbar.IsFixedTop ]
                [ ]
              div [ Class "page-content" ]
                [ Monaco.editor [
                      createObj [
                          "url" ==> "/libs/requirejs/require.js"
                          "paths" ==> createObj [
                              "vs" ==> "/libs/vs"
                          ]
                      ] |> Monaco.RequireConfig
                      Monaco.Options editorOptions
                    ]
                ]
            ]
    )

open Elmish.React
open Elmish.HMR

Program.mkProgram init update view
|> Program.withHMR
|> Program.withReact "elmish-app"
|> Program.run
