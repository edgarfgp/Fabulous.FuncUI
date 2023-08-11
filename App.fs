namespace MyApp

open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Controls
    
type Components =
    static member Counter() =
        Component (fun ctx ->
            let state = ctx.useState 0
    
            DockPanel.create [
                DockPanel.children [
                    Button.create [
                        Button.onClick (fun _ -> state.Set(state.Current - 1))
                        Button.content "Decrement"
                    ]
                    Button.create [
                        Button.onClick (fun _ -> state.Set(state.Current + 1))
                        Button.content "Increment"
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

type IFuncUIControl =
    inherit IFabControl
    
module FuncUIControl =
    let WidgetKey = Widgets.registerWithFactory(fun _ -> Components.Counter())
  
[<AutoOpen>]
module FuncUIControlBuilders =
    
    type Fabulous.Avalonia.View with
        static member FuncUIControl<'msg>() =
            WidgetBuilder<'msg, IFuncUIControl>(
                FuncUIControl.WidgetKey,
                AttributesBundle(
                    StackList.empty(),
                    ValueNone,
                    ValueNone
                ))
                      
open type Fabulous.Avalonia.View

module App =
    let view _ =
        (VStack() {            
            FuncUIControl()
        })
            .center()
    
#if MOBILE
    let app model = SingleViewApplication(view model)
#else
    let app model = DesktopApplication(Window(view model))
#endif
    let program = Program.stateless app
