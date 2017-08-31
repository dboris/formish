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

    type CounterPage () =
        inherit ContentPage ()
        let label = Label (XAlign = TextAlignment.Center)
        let incButton = Button (Text = "Increment")
        let decButton = Button (Text = "Decrement")
        let stack = StackLayout (VerticalOptions = LayoutOptions.Center)
        do
            stack.Children.Add label
            stack.Children.Add incButton
            stack.Children.Add decButton
            base.Content <- stack

        member __.SubscribeHandlers initial =
            let sub dispatch =
                incButton.Clicked.Add (fun _ -> dispatch Increment)
                decButton.Clicked.Add (fun _ -> dispatch Decrement)
            Cmd.ofSub sub

        member __.Counter
            with get () = label.Text
            and set value = label.Text <- value

    let page = CounterPage ()

    let view model dispatch =
        Device.BeginInvokeOnMainThread (fun () -> page.Counter <- sprintf "%i" model.x)

    let run () =
        Program.mkProgram init update view
        |> Program.withSubscription page.SubscribeHandlers
        |> Program.withConsoleTrace
        |> Program.run