namespace Formish

open System
open Xamarin.Forms
open Elmish.Core


module SimpleCounter2 =

    type Model = { x : int }
    type Msg = Increment | Decrement

    let init () =
        { x = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Increment -> { model with x = model.x + 1 }, Cmd.none
        | Decrement -> { model with x = model.x - 1 }, Cmd.none

    type PageViewModel (initial, dispatch) =
        inherit ViewModelBase ()

        let mutable count = initial.x
        let incrementCommand = new Command<_> (fun _ -> dispatch Increment)
        let decrementCommand = new Command<_> (fun _ -> dispatch Decrement)

        member self.Count 
            with get () = count
            and set value =
                if value <> count 
                then count <- value; base.OnPropertyChanged "Count"

        member __.IncrementCommand with get () = incrementCommand
        member __.DecrementCommand with get () = decrementCommand

    let page = CounterPage ()

    let withRootViewModel vmType (program : Program<_,_,_,_>) =
        let mutable pageViewModel = None
        let setState m d =
            match pageViewModel with
            | None ->
                let vm = vmType (m, d)
                pageViewModel <- vm |> Some
                page.BindingContext <- vm
                Device.BeginInvokeOnMainThread (fun () -> program.view m d vm)
            | Some vm ->
                Device.BeginInvokeOnMainThread (fun () -> program.view m d vm)
        { program with setState = setState }

    let view model dispatch (viewModel : PageViewModel) =
        viewModel.Count <- model.x

    let run () =
        Program.mkProgram init update view
        |> withRootViewModel PageViewModel
        |> Program.withConsoleTrace
        |> Program.run