// Program.fs
namespace Test

open System
open System.Reactive.Linq
open ReactiveUI
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Markup.Xaml
open Avalonia.ReactiveUI


type MyControl () as this =
    inherit UserControl ()

    static let TimeProperty =
        AvaloniaProperty.RegisterDirect<MyControl, string>
            ("Time", (fun o -> o.Time), (fun o v -> o.Time <- v))

    let mutable time : string = null
    let tb = TextBlock (Text = "Initial")

    do
        this.Content <- tb

    member __.Time
        with get () = time
        and set value =
            if this.SetAndRaise (TimeProperty, &time, value) then
                tb.Text <- value
                printfn "Time is set: %s" (match value with | null -> "<null>" | _ -> value)


type ClockViewModel () =
    let clock =
        Observable.Interval(TimeSpan.FromSeconds 1.0)
            .Select(fun t -> if t % 2L = 0L then "tick" else "tock")

    member __.Clock = clock


type MainWindowViewModel () =
    let clockModel = ClockViewModel ()

    member __.ClockModel = clockModel


type MainWindow () as this =
    inherit Window ()

    do
        this.InitializeComponent ()
        this.DataContext <- MainWindowViewModel ()

    member private this.InitializeComponent () =
        AvaloniaXamlLoader.Load this


type App () =
    inherit Application()

    override this.Initialize () =
        AvaloniaXamlLoader.Load this

    override __.OnFrameworkInitializationCompleted () =
        base.OnFrameworkInitializationCompleted ()
        match base.ApplicationLifetime with
            | :? IClassicDesktopStyleApplicationLifetime as desktop ->
                desktop.MainWindow <- MainWindow ()
            | _ -> ()


module Test =
    let BuildAvaloniaApp() =
        AppBuilder.Configure<App>()
            .UseReactiveUI()
            .UsePlatformDetect()

    [<EntryPoint>]
    let main (args: string[]) =
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args)
