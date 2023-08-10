namespace MyApp







open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Controls
    
type Components =
    static member Counter () =
        Component (fun ctx ->
            let state = ctx.useState 0
    
            DockPanel.create [
                DockPanel.children [
                    Button.create [
                        Button.onClick (fun _ -> state.Set(state.Current - 1))
                        Button.content "click to decrement"
                    ]
                    Button.create [
                        Button.onClick (fun _ -> state.Set(state.Current + 1))
                        Button.content "click to increment"
                    ]
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.text (string state.Current)
                    ]
                ]
            ]
        )
        
open Fabulous
open Fabulous.Avalonia
open Fabulous.StackAllocatedCollections.StackList


type IFunUiControl =
    inherit IFabControl
    
module FunUiControl =
    let WidgetKey = Widgets.registerWithFactory(fun _ -> Components.Counter())
  
[<AutoOpen>]
module FunUiControlBuilder =
    
    type Fabulous.Avalonia.View with
        static member FunUiControl<'msg>() =
            WidgetBuilder<'msg, IFunUiControl>(
                FunUiControl.WidgetKey,
                AttributesBundle(
                    StackList.empty(),
                    ValueNone,
                    ValueNone
                ))
            
            
open type Fabulous.Avalonia.View

module App =
    type Model =
        { Count: int; Step: int; TimerOn: bool }

    type Msg =
        | Increment
        | Decrement
        | Reset
        | SetStep of float
        | TimerToggled of bool
        | TimedTick

    let initModel = { Count = 0; Step = 1; TimerOn = false }
    
    let timerCmd () =
        async {
            do! Async.Sleep 200
            return TimedTick
        }
        |> Cmd.ofAsyncMsg

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | Increment ->
            { model with
                Count = model.Count + model.Step },
            Cmd.none
        | Decrement ->
            { model with
                Count = model.Count - model.Step },
            Cmd.none
        | Reset -> initModel, Cmd.none
        | SetStep n -> { model with Step = int(n + 0.5) }, Cmd.none
        | TimerToggled on -> { model with TimerOn = on }, (if on then timerCmd() else Cmd.none)
        | TimedTick ->
            if model.TimerOn then
                { model with
                    Count = model.Count + model.Step },
                timerCmd()
            else
                model, Cmd.none

    let view model =
        (VStack() {
            TextBlock($"%d{model.Count}").centerText()
            
            FunUiControl()

            Button("Increment", Increment).centerHorizontal()

            Button("Decrement", Decrement).centerHorizontal()

            (HStack() {
                TextBlock("Timer").centerVertical()

                ToggleSwitch(model.TimerOn, TimerToggled)
            })
                .margin(20.)
                .centerHorizontal()

            Slider(1., 10, float model.Step, SetStep)

            TextBlock($"Step size: %d{model.Step}").centerText()

            Button("Reset", Reset).centerHorizontal()

        })
            .center()

    
#if MOBILE
    let app model = SingleViewApplication(view model)
#else
    let app model = DesktopApplication(Window(view model))
#endif

    
    let program = Program.statefulWithCmd init update app
