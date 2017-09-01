namespace Formish

open System
open System.ComponentModel
open Xamarin.Forms

module SimpleCounterPage =
    let notSubscribed () = Diagnostics.Debug.WriteLine "Handler not subscribed"

    type PageViewModel () =
        let mutable count = 0
        let mutable incrementCommand = new Command<_> (fun () -> notSubscribed ())
        let mutable decrementCommand = new Command<_> (fun () -> notSubscribed ())

        let propertyChanged = new Event<PropertyChangedEventHandler, PropertyChangedEventArgs> ()

        interface INotifyPropertyChanged with
            [<CLIEvent>] member __.PropertyChanged = propertyChanged.Publish

        member self.Count
            with get () = count
            and set value =
                if value <> count then
                    count <- value
                    propertyChanged.Trigger (self, new PropertyChangedEventArgs ("Count"))

        member self.IncrementCommand 
            with get () = incrementCommand
            and set value =
                incrementCommand <- value
                propertyChanged.Trigger (self, new PropertyChangedEventArgs ("IncrementCommand"))

        member self.DecrementCommand
            with get () = decrementCommand
            and set value =
                decrementCommand <- value
                propertyChanged.Trigger (self, new PropertyChangedEventArgs ("DecrementCommand"))

        member self.Setup (initialCount, dispatch, incrementAction, decrementAction) =
            self.Count <- initialCount
            self.IncrementCommand <- new Command<_> (fun _ -> dispatch incrementAction)
            self.DecrementCommand <- new Command<_> (fun _ -> dispatch decrementAction)

    type Page (viewModel) as self =
        inherit ContentPage (Title = "Simple Counter")

        let label = Label (XAlign = TextAlignment.Center)
        let incButton = Button (Text = "Increment")
        let decButton = Button (Text = "Decrement")
        let stack = StackLayout (VerticalOptions = LayoutOptions.Center)
        do
            self.BindingContext <- viewModel
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
    type Msg = Increment | Decrement

    let init () =
        { x = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Increment -> { model with x = model.x + 1 }, Cmd.none
        | Decrement -> { model with x = model.x - 1 }, Cmd.none

    let pageViewModel = SimpleCounterPage.PageViewModel ()
    let page = SimpleCounterPage.Page (pageViewModel)
    let subscribeViewModel initial =
        let sub dispatch =
            pageViewModel.Setup (initial.x, dispatch, Increment, Decrement)
        Cmd.ofSub sub

    let view model dispatch =
        Device.BeginInvokeOnMainThread (fun () -> pageViewModel.Count <- model.x)

    let run () =
        Program.mkProgram init update view
        |> Program.withSubscription subscribeViewModel
        |> Program.withConsoleTrace
        |> Program.run