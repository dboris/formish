namespace Formish

open System
open Xamarin.Forms
open Elmish.Core


module SimpleCounter =

    type Model = { x : int }
    type Msg = Increment | Decrement

    let init () =
        { x = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Increment -> { model with x = model.x + 1 }, Cmd.none
        | Decrement -> { model with x = model.x - 1 }, Cmd.none

    type PageViewModel (initialCount, dispatch) =
        inherit ViewModelBase ()

        let mutable count = initialCount
        let incrementCommand = new Command<_> (fun _ -> dispatch Increment)
        let decrementCommand = new Command<_> (fun _ -> dispatch Decrement)

        member self.Count 
            with get () = count
            and set value =
                if value <> count 
                then count <- value; base.OnPropertyChanged "Count"

        member __.IncrementCommand with get () = incrementCommand
        member __.DecrementCommand with get () = decrementCommand

    let mutable pageViewModel = None
    let page = CounterPage ()

    let setupView initial =
        let sub dispatch =
            let vm = PageViewModel (initial.x, dispatch)
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