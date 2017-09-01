namespace Formish

open System
open System.ComponentModel
open Xamarin.Forms

type Msg = Increment | Decrement

module SimpleCounterPage =

    type PageViewModel (initialCount, dispatch) =
        let mutable count = initialCount
        let incrementCommand = new Command<_> (fun _ -> dispatch Increment)
        let decrementCommand = new Command<_> (fun _ -> dispatch Decrement)
        let propertyChanged = new Event<PropertyChangedEventHandler, PropertyChangedEventArgs> ()

        interface INotifyPropertyChanged with
            [<CLIEvent>] member __.PropertyChanged = propertyChanged.Publish

        member self.Count
            with get () = count
            and set value =
                if value <> count then
                    count <- value
                    propertyChanged.Trigger (self, new PropertyChangedEventArgs ("Count"))

        member __.IncrementCommand with get () = incrementCommand
        member __.DecrementCommand with get () = decrementCommand

    type Page () =
        inherit ContentPage (Title = "Simple Counter")

        let label = Label (XAlign = TextAlignment.Center)
        let incButton = Button (Text = "Increment")
        let decButton = Button (Text = "Decrement")
        let stack = StackLayout (VerticalOptions = LayoutOptions.Center)
        do
            label.SetBinding (Label.TextProperty, "Count")
            incButton.SetBinding (Button.CommandProperty, "IncrementCommand")
            decButton.SetBinding (Button.CommandProperty, "DecrementCommand")
            stack.Children.Add label
            stack.Children.Add incButton
            stack.Children.Add decButton
            base.Content <- stack

module SimpleCounter =
    open Elmish.Core

    type Model = { x : int }

    let init () =
        { x = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Increment -> { model with x = model.x + 1 }, Cmd.none
        | Decrement -> { model with x = model.x - 1 }, Cmd.none

    let mutable pageViewModel = None
    let page = SimpleCounterPage.Page ()

    let setupView initial =
        let sub dispatch =
            let vm = SimpleCounterPage.PageViewModel (initial.x, dispatch)
            pageViewModel <- vm |> Some
            page.BindingContext <- vm
        Cmd.ofSub sub

    let view model dispatch =
        Device.BeginInvokeOnMainThread (fun () -> 
            pageViewModel |> Option.iter (fun vm -> vm.Count <- model.x))

    let run () =
        Program.mkProgram init update view
        |> Program.withSubscription setupView
        |> Program.withConsoleTrace
        |> Program.run