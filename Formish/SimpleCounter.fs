namespace Formish

open Xamarin.Forms
open Elmish

module SimpleCounter =

    type Model = { x : int }
    type Msg = Increment | Decrement

    let init () =
        { x = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Increment -> { model with x = model.x + 1 }, Cmd.none
        | Decrement -> { model with x = model.x - 1 }, Cmd.none

    let label = Label (XAlign = TextAlignment.Center)
    let incButton = Button (Text = "Increment")
    let decButton = Button (Text = "Decrement")
    let stack = StackLayout (VerticalOptions = LayoutOptions.Center)
    do
        stack.Children.Add label
        stack.Children.Add incButton
        stack.Children.Add decButton
    let page = ContentPage (Content = stack)

    let view model dispatch =
        Device.BeginInvokeOnMainThread (fun () -> label.Text <- sprintf "%i" model.x)

    let subHandlers initial =
        let sub dispatch =
            incButton.Clicked.Add (fun _ -> dispatch Increment)
            decButton.Clicked.Add (fun _ -> dispatch Decrement)
        Cmd.ofSub sub

    let run () =
        Program.mkProgram init update view
        |> Program.withSubscription subHandlers
        |> Program.withConsoleTrace
        |> Program.run