namespace Formish

open System.ComponentModel


type ViewModelBase () =
    let propertyChanged = new Event<PropertyChangedEventHandler, PropertyChangedEventArgs> ()

    interface INotifyPropertyChanged with
        [<CLIEvent>] member __.PropertyChanged = propertyChanged.Publish

    member self.OnPropertyChanged propName =
        propertyChanged.Trigger (self, new PropertyChangedEventArgs (propName))
