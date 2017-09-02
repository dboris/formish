namespace Formish

open System.ComponentModel
open Xamarin.Forms


[<AutoOpen>]
module ViewModel =

    let bindPageContext (page : Page) viewModel =
        page.BindingContext <- viewModel
        viewModel

    type ViewModelBase () =
        let propertyChanged = new Event<PropertyChangedEventHandler, PropertyChangedEventArgs> ()

        interface INotifyPropertyChanged with
            [<CLIEvent>] member __.PropertyChanged = propertyChanged.Publish

        member self.OnPropertyChanged propName =
            propertyChanged.Trigger (self, new PropertyChangedEventArgs (propName))

[<AutoOpen>]
module Extensions =
    open Elmish.Core

    module Program =
        let withRootViewModel mkRootVM (program : Program<_,_,_,_>) =
            let mutable rootViewModel = None
            let setState m d =
                match rootViewModel with
                | None ->
                    let vm = mkRootVM m d
                    rootViewModel <- vm |> Some
                    Device.BeginInvokeOnMainThread (fun () -> program.view m d vm)
                | Some vm ->
                    Device.BeginInvokeOnMainThread (fun () -> program.view m d vm)
            { program with setState = setState }