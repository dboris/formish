namespace Formish

open Xamarin.Forms


type App () = 
    inherit Application ()
    do 
        base.MainPage <- SimpleCounter.page
        SimpleCounter.run ()
