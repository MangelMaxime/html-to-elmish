module App.Navbar

open Elmish

type Config =
    { IndentationSize : int
      IndentWith : string }

type Model =
    { Config : Config }

type Msg =
    | ChangeIndentationSize of int
    | UseSpaces
    | UseTabulation
    | ChooseSample of string

type ExternalMsg =
    | NoOp
    | LoadSample of string
    | ConfigChanged

let init _ =
    { Config =
        { IndentationSize = 4
          IndentWith = " " } }, Cmd.none

let update model =
    function
    | ChangeIndentationSize newSize ->
        { model with Config =
                        { model.Config with IndentationSize = newSize } }, Cmd.none, ConfigChanged

    | UseSpaces ->
        { model with Config =
                        { model.Config with IndentWith = " " } }, Cmd.none, ConfigChanged

    | UseTabulation ->
        { model with Config =
                        { model.Config with IndentWith = "\t" } }, Cmd.none, ConfigChanged

    | ChooseSample sampleCode ->
        model, Cmd.none, LoadSample sampleCode

let helloWorld =
    """<span>Hello world</span>"""

let fulmaMediaObject =
    """<article class="media">
  <figure class="media-left">
    <p class="image is-64x64">
      <img src="https://bulma.io/images/placeholders/128x128.png">
    </p>
  </figure>
  <div class="media-content">
    <div class="field">
      <p class="control">
        <textarea class="textarea" placeholder="Add a comment..."></textarea>
      </p>
    </div>
    <nav class="level">
      <div class="level-left">
        <div class="level-item">
          <a class="button is-info">Submit</a>
        </div>
      </div>
      <div class="level-right">
        <div class="level-item">
          <label class="checkbox">
            <input type="checkbox"> Press enter to submit
          </label>
        </div>
      </div>
    </nav>
  </div>
</article>"""

let fulmaBox =
    """<div class="box">
  <article class="media">
    <div class="media-left">
      <figure class="image is-64x64">
        <img src="https://bulma.io/images/placeholders/128x128.png" alt="Image">
      </figure>
    </div>
    <div class="media-content">
      <div class="content">
        <p>
          <strong>John Smith</strong> <small>@johnsmith</small> <small>31m</small>
          <br>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean efficitur sit amet massa fringilla egestas. Nullam condimentum luctus turpis.
        </p>
      </div>
      <nav class="level is-mobile">
        <div class="level-left">
          <a class="level-item">
            <span class="icon is-small"><i class="fas fa-reply"></i></span>
          </a>
          <a class="level-item">
            <span class="icon is-small"><i class="fas fa-retweet"></i></span>
          </a>
          <a class="level-item">
            <span class="icon is-small"><i class="fas fa-heart"></i></span>
          </a>
        </div>
      </nav>
    </div>
  </article>
</div>"""

let foundationTopBar =
    """<div class="top-bar">
  <div class="top-bar-left">
    <ul class="dropdown menu" data-dropdown-menu>
      <li class="menu-text">Site Title</li>
      <li>
        <a href="#">One</a>
        <ul class="menu vertical">
          <li><a href="#">One</a></li>
          <li><a href="#">Two</a></li>
          <li><a href="#">Three</a></li>
        </ul>
      </li>
      <li><a href="#">Two</a></li>
      <li><a href="#">Three</a></li>
    </ul>
  </div>
  <div class="top-bar-right">
    <ul class="menu">
      <li><input type="search" placeholder="Search"></li>
      <li><button type="button" class="button">Search</button></li>
    </ul>
  </div>
</div>"""

let boostrapNavbar =
    """<nav class="navbar navbar-expand-lg navbar-light bg-light">
  <a class="navbar-brand" href="#">Navbar</a>
  <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
    <span class="navbar-toggler-icon"></span>
  </button>

  <div class="collapse navbar-collapse" id="navbarSupportedContent">
    <ul class="navbar-nav mr-auto">
      <li class="nav-item active">
        <a class="nav-link" href="#">Home <span class="sr-only">(current)</span></a>
      </li>
      <li class="nav-item">
        <a class="nav-link" href="#">Link</a>
      </li>
      <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          Dropdown
        </a>
        <div class="dropdown-menu" aria-labelledby="navbarDropdown">
          <a class="dropdown-item" href="#">Action</a>
          <a class="dropdown-item" href="#">Another action</a>
          <div class="dropdown-divider"></div>
          <a class="dropdown-item" href="#">Something else here</a>
        </div>
      </li>
      <li class="nav-item">
        <a class="nav-link disabled" href="#">Disabled</a>
      </li>
    </ul>
    <form class="form-inline my-2 my-lg-0">
      <input class="form-control mr-sm-2" type="search" placeholder="Search" aria-label="Search">
      <button class="btn btn-outline-success my-2 my-sm-0" type="submit">Search</button>
    </form>
  </div>
</nav>"""

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Components

let private navbarItem dispatch =
    fun text sampleCode ->
        Navbar.Item.a [ Navbar.Item.Props [ OnClick (fun _ -> ChooseSample sampleCode |> dispatch)] ]
            [ str text ]

let view dispatch =
    let viewNavbarItem = navbarItem dispatch
    fun model ->
        Navbar.navbar [ Navbar.IsFixedTop ]
            [ Navbar.Brand.div [ ]
                [ Navbar.Item.a [ ]
                    [ strong [ ]
                        [ str "Html to Elmish" ] ] ]
              Navbar.Item.div [ Navbar.Item.HasDropdown
                                Navbar.Item.IsHoverable ]
                [ Navbar.Link.a [ ]
                    [ str "Samples" ]
                  Navbar.Dropdown.div [ ]
                    [ viewNavbarItem "Hello world" helloWorld
                      viewNavbarItem "Bootstrap: Navbar" boostrapNavbar
                      viewNavbarItem "Fulma: Box" fulmaBox
                      viewNavbarItem "Fulma: Media Object" fulmaMediaObject
                      viewNavbarItem "Foundation: Top Bar" foundationTopBar ] ] ]
