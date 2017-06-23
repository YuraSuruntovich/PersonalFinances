﻿#pragma checksum "C:\Users\yuraa\Desktop\PersonalFinances\PersonalFinances\Pages\ReportIncomePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E4DFF0C52961B0E3D9017F67AF0EFE27"
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
    partial class ReportIncomePage : 
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

        private class ReportIncomePage_obj23_Bindings :
            global::Windows.UI.Xaml.IDataTemplateExtension,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IReportIncomePage_Bindings
        {
            private global::PersonalFinances.Models.Income dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);
            private bool removedDataContextHandler = false;

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.TextBlock obj24;
            private global::Windows.UI.Xaml.Controls.TextBlock obj25;
            private global::Windows.UI.Xaml.Controls.TextBlock obj26;
            private global::Windows.UI.Xaml.Controls.TextBlock obj27;

            public ReportIncomePage_obj23_Bindings()
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
                 global::PersonalFinances.Models.Income data = args.NewValue as global::PersonalFinances.Models.Income;
                 if (args.NewValue != null && data == null)
                 {
                    throw new global::System.ArgumentException("Incorrect type passed into template. Based on the x:DataType global::PersonalFinances.Models.Income was expected.");
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
                        this.SetDataRoot(args.Item as global::PersonalFinances.Models.Income);
                        if (!removedDataContextHandler)
                        {
                            removedDataContextHandler = true;
                            ((global::Windows.UI.Xaml.Controls.StackPanel)args.ItemContainer.ContentTemplateRoot).DataContextChanged -= this.DataContextChangedHandler;
                        }
                        this.initialized = true;
                        break;
                }
                this.Update_((global::PersonalFinances.Models.Income) args.Item, 1 << (int)args.Phase);
                return nextPhase;
            }

            public void ResetTemplate()
            {
            }

            // IReportIncomePage_Bindings

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

            // ReportIncomePage_obj23_Bindings

            public void SetDataRoot(global::PersonalFinances.Models.Income newDataRoot)
            {
                this.dataRoot = newDataRoot;
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::PersonalFinances.Models.Income obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_SourceOfIncome(obj.SourceOfIncome, phase);
                        this.Update_Summa(obj.Summa, phase);
                        this.Update_Currency(obj.Currency, phase);
                        this.Update_DateOperation(obj.DateOperation, phase);
                    }
                }
            }
            private void Update_SourceOfIncome(global::PersonalFinances.Models.SourceOfIncome obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_SourceOfIncome_Name(obj.Name, phase);
                    }
                }
            }
            private void Update_SourceOfIncome_Name(global::System.String obj, int phase)
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
                    this.SortIncome = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 14 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.SortIncome).Click += this.SortIncome_Click;
                    #line default
                }
                break;
            case 2:
                {
                    this.BtnShowFilters = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 16 "..\..\..\Pages\ReportIncomePage.xaml"
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
                    this.sortIncomesByDateDesc = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 20 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortIncomesByDateDesc).Click += this.sortIncomesByDateDesc_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.sortIncomesByDate = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 22 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortIncomesByDate).Click += this.sortIncomesByDate_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.sortIncomesBySymmaDesc = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 24 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortIncomesBySymmaDesc).Click += this.sortIncomesBySymmaDesc_Click;
                    #line default
                }
                break;
            case 7:
                {
                    this.sortIncomesBySumma = (global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)(target);
                    #line 26 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem)this.sortIncomesBySumma).Click += this.sortIncomesBySumma_Click;
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
                    #line 59 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.buttonAccept).Click += this.buttonAccept_Click;
                    #line default
                }
                break;
            case 10:
                {
                    this.isAllDate = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 52 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.isAllDate).Tapped += this.isAllDate_Tapped;
                    #line default
                }
                break;
            case 11:
                {
                    this.dateStart = (global::Windows.UI.Xaml.Controls.CalendarDatePicker)(target);
                    #line 54 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CalendarDatePicker)this.dateStart).DateChanged += this.dateStart_DateChanged;
                    #line default
                }
                break;
            case 12:
                {
                    this.datefinish = (global::Windows.UI.Xaml.Controls.CalendarDatePicker)(target);
                    #line 56 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CalendarDatePicker)this.datefinish).DateChanged += this.datefinish_DateChanged;
                    #line default
                }
                break;
            case 13:
                {
                    this.isAllIncomesCategories = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 45 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.isAllIncomesCategories).Tapped += this.isAllIncomesCategories_Tapped;
                    #line default
                }
                break;
            case 14:
                {
                    this.incomesCategoriesCB = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 47 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.incomesCategoriesCB).SelectionChanged += this.incomesCategoriesCB_SelectionChanged;
                    #line default
                }
                break;
            case 15:
                {
                    this.isAllPurse = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 38 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.isAllPurse).Tapped += this.isAllPurse_Tapped;
                    #line default
                }
                break;
            case 16:
                {
                    this.purseListCB = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 40 "..\..\..\Pages\ReportIncomePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.purseListCB).SelectionChanged += this.purseListCB_SelectionChanged;
                    #line default
                }
                break;
            case 17:
                {
                    this.rootPivot = (global::Windows.UI.Xaml.Controls.Pivot)(target);
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
                    this.ColumnChartIncome = (global::WinRTXamlToolkit.Controls.DataVisualization.Charting.Chart)(target);
                }
                break;
            case 21:
                {
                    this.PieChartIncome = (global::WinRTXamlToolkit.Controls.DataVisualization.Charting.Chart)(target);
                }
                break;
            case 22:
                {
                    this.incomeList = (global::Windows.UI.Xaml.Controls.ListView)(target);
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
                    ReportIncomePage_obj23_Bindings bindings = new ReportIncomePage_obj23_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot((global::PersonalFinances.Models.Income) element23.DataContext);
                    element23.DataContextChanged += bindings.DataContextChangedHandler;
                    global::Windows.UI.Xaml.DataTemplate.SetExtensionInstance(element23, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

