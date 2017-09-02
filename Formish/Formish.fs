namespace Formish

open Xamarin.Forms


type App () = 
    inherit Application ()
    do 
        base.MainPage <- SimpleCounter2.page
        SimpleCounter2.run ()
