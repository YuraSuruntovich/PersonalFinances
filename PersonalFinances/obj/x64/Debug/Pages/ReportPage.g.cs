﻿#pragma checksum "C:\Users\yuraa\Desktop\PersonalFinances\PersonalFinances\Pages\ReportPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B0776040432C54EE48435362ABB72808"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PersonalFinances.Pages
{
    partial class ReportPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        internal class XamlBindingSetters
        {
            public static void Set_Windows_UI_Xaml_Controls_TextBlock_Text(global::Windows.UI.Xaml.Controls.TextBlock obj, global::System.String value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = targetNullValue;
                }
                obj.Text = value ?? global::System.String.Empty;
            }
        };

        private class ReportPage_obj23_Bindings :
            global::Windows.UI.Xaml.IDataTemplateExtension,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IReportPage_Bindings
        {
            private global::PersonalFinances.Models.Costs dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);
            private bool removedDataContextHandler = false;

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.TextBlock obj24;
            private global::Windows.UI.Xaml.Controls.TextBlock obj25;
            private global::Windows.UI.Xaml.Controls.TextBlock obj26;
            private global::Windows.UI.Xaml.Controls.TextBlock obj27;

            public ReportPage_obj23_Bindings()
            {
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 24:
                        this.obj24 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    case 25:
                        this.obj25 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    case 26:
                        this.obj26 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    case 27:
                        this.obj27 = (global::Windows.UI.Xaml.Controls.TextBlock)target;
                        break;
                    default:
                        break;
                }
            }

            public void DataContextChangedHandler(global::Windows.UI.Xaml.FrameworkElement sender, global::Windows.UI.Xaml.DataContextChangedEventArgs args)
            {
                 global::PersonalFinances.Models.Costs data = args.NewValue as global::PersonalFinances.Models.Costs;
                 if (args.NewValue != null && data == null)
                 {
                    throw new global::System.ArgumentException("Incorrect type passed into template. Based on the x:DataType global::PersonalFinances.Models.Costs was expected.");
                 }
                 this.SetDataRoot(data);
                 this.Update();
            }

            // IDataTemplateExtension

            public bool ProcessBinding(uint phase)
            {
                throw new global::System.NotImplementedException();
            }

            public int ProcessBindings(global::Windows.UI.Xaml.Controls.ContainerContentChangingEventArgs args)
            {
                int nextPhase = -1;
                switch(args.Phase)
                {
                    case 0:
                        nextPhase = -1;
                        this.SetDataRoot(args.Item as global::PersonalFinances.Models.Costs);
                        if (!removedDataContextHandler)
                        {
                            removedDataContextHandler = true;
                            ((global::Windows.UI.Xaml.Controls.StackPanel)args.ItemContainer.ContentTemplateRoot).DataContextChanged -= this.DataContextChangedHandler;
                        }
                        this.initialized = true;
                        break;
                }
                this.Update_((global::PersonalFinances.Models.Costs) args.Item, 1 << (int)args.Phase);
                return nextPhase;
            }

            public void ResetTemplate()
            {
            }

            // IReportPage_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            // ReportPage_obj23_Bindings

            public void SetDataRoot(global::PersonalFinances.Models.Costs newDataRoot)
            {
                this.dataRoot = newDataRoot;
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::PersonalFinances.Models.Costs obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_CostCategories(obj.CostCategories, phase);
                        this.Update_Summa(obj.Summa, phase);
                        this.Update_Currency(obj.Currency, phase);
                        this.Update_DateOperation(obj.DateOperation, phase);
                    }
                }
            }
            private void Update_CostCategories(global::PersonalFinances.Models.CostCategories obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_CostCategories_Name(obj.Name, phase);
                    }
                }
            }
            private void Update_CostCategories_Name(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj24, obj, null);
                }
            }
            private void Update_Summa(global::System.Double obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj25, obj.ToString(), null);
                }
            }
            private void Update_Currency(global::PersonalFinances.Models.Currency obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_Currency_Abbreviation(obj.Abbreviation, phase);
                    }
                }
            }
            private void Update_Currency_Abbreviation(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj26, obj, null);
                }
            }
            private void Update_DateOperation(global::System.String obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Windows_UI_Xaml_Controls_TextBlock_Text(this.obj27, obj, null);
                }
            }
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.SortCosts = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 14 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.SortCosts).Click += this.SortCosts_Click;
                    #line default
                }
                break;
            case 2:
                {
                    this.BtnShowFilters = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 16 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.BtnShowFilters).Click += this.BtnShowFilters_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.sortFlyotMenu = (global::Windows.UI.Xaml.Controls.MenuFlyout)(target);
                }
                break;
            case 4:
                {
                    this.sortCostsByDateDesc = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 20 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortCostsByDateDesc).Click += this.sortCostsByDateDesc_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.sortCostsByDate = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 22 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortCostsByDate).Click += this.sortCostsByDate_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.sortCostsBySymmaDesc = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 24 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortCostsBySymmaDesc).Click += this.sortCostsBySymmaDesc_Click;
                    #line default
                }
                break;
            case 7:
                {
                    this.sortCostsBySumma = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 26 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortCostsBySumma).Click += this.sortCostsBySumma_Click;
                    #line default
                }
                break;
            case 8:
                {
                    this.filterFlyoutMenu = (global::Windows.UI.Xaml.Controls.Flyout)(target);
                }
                break;
            case 9:
                {
                    this.buttonAccept = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 59 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.buttonAccept).Click += this.buttonAccept_Click;
                    #line default
                }
                break;
            case 10:
                {
                    this.isAllDate = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 52 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.isAllDate).Tapped += this.isAllDate_Tapped;
                    #line default
                }
                break;
            case 11:
                {
                    this.dateStart = (global::Windows.UI.Xaml.Controls.CalendarDatePicker)(target);
                    #line 54 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CalendarDatePicker)this.dateStart).DateChanged += this.dateStart_DateChanged;
                    #line default
                }
                break;
            case 12:
                {
                    this.datefinish = (global::Windows.UI.Xaml.Controls.CalendarDatePicker)(target);
                    #line 56 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CalendarDatePicker)this.datefinish).DateChanged += this.datefinish_DateChanged;
                    #line default
                }
                break;
            case 13:
                {
                    this.isAllCostsCategories = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 45 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.isAllCostsCategories).Tapped += this.isAllCostsCategories_Tapped;
                    #line default
                }
                break;
            case 14:
                {
                    this.costsCategoriesCB = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 47 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.costsCategoriesCB).SelectionChanged += this.costsCategoriesCB_SelectionChanged;
                    #line default
                }
                break;
            case 15:
                {
                    this.isAllPurse = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 38 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.isAllPurse).Tapped += this.isAllPurse_Tapped;
                    #line default
                }
                break;
            case 16:
                {
                    this.purseListCB = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 40 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.purseListCB).SelectionChanged += this.purseListCB_SelectionChanged;
                    #line default
                }
                break;
            case 17:
                {
                    this.rootPivot = (global::Windows.UI.Xaml.Controls.Pivot)(target);
                    #line 69 "..\..\..\Pages\ReportPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Pivot)this.rootPivot).SelectionChanged += this.rootPivot_SelectionChanged;
                    #line default
                }
                break;
            case 18:
                {
                    this.amountSumma = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 19:
                {
                    this.errorText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 20:
                {
                    this.ColumnChartCosts = (global::WinRTXamlToolkit.Controls.DataVisualization.Charting.Chart)(target);
                }
                break;
            case 21:
                {
                    this.PieChartCosts = (global::WinRTXamlToolkit.Controls.DataVisualization.Charting.Chart)(target);
                }
                break;
            case 22:
                {
                    this.costList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 23:
                {
                    global::Windows.UI.Xaml.Controls.StackPanel element23 = (global::Windows.UI.Xaml.Controls.StackPanel)target;
                    ReportPage_obj23_Bindings bindings = new ReportPage_obj23_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot((global::PersonalFinances.Models.Costs) element23.DataContext);
                    element23.DataContextChanged += bindings.DataContextChangedHandler;
                    global::Windows.UI.Xaml.DataTemplate.SetExtensionInstance(element23, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

