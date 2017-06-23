using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PersonalFinances.Pages;
using Windows.Storage;
using Windows.UI.ViewManagement;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PersonalFinances
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings;
        ApplicationView view;
        bool isOpenMenuPanel = false;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            //вид меню 
            this.SizeChanged += (s, e) =>
            {
                if(e.NewSize.Width < 500)
                {
                    splitViewMenu.DisplayMode = SplitViewDisplayMode.Inline;
                    splitViewMenu.IsPaneOpen = false;
                    isOpenMenuPanel = false;
                }
                else if (e.NewSize.Width < 800)
                {
                    splitViewMenu.DisplayMode = SplitViewDisplayMode.CompactInline;
                    splitViewMenu.IsPaneOpen = false;
                    isOpenMenuPanel = false;
                }
                else
                {
                    splitViewMenu.IsPaneOpen = true;
                    isOpenMenuPanel = true;
                }
                    
                    
            };
            //по умолчанию открываем страницу OperationsPage.xaml
            myFrame.Navigate(typeof(OperationsPage));
            TitleTextBlock.Text = "Операции";
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            view = ApplicationView.GetForCurrentView();
            localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["isFullScreenMode"];

            if (value != null)
            {
                if (value.ToString() == "true")
                {
                    view.TryEnterFullScreenMode();
                }
                else if (value.ToString() == "false")
                {
                    view.ExitFullScreenMode();
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (operations.IsSelected)
            {
                myFrame.Navigate(typeof(OperationsPage));
                TitleTextBlock.Text = "Операции";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (currency.IsSelected)
            {
                myFrame.Navigate(typeof(CurrencyPage));
                TitleTextBlock.Text = "Валюта";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (purse.IsSelected)
            {
                myFrame.Navigate(typeof(PursePage));
                TitleTextBlock.Text = "Счета";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (categories.IsSelected)
            {
                myFrame.Navigate(typeof(CategoriesPage));
                TitleTextBlock.Text = "Категории расходов/доходов";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (reportCosts.IsSelected)
            {
                myFrame.Navigate(typeof(ReportPage));
                TitleTextBlock.Text = "Отчет о расходах";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (reportIncomes.IsSelected)
            {
                myFrame.Navigate(typeof(ReportIncomePage));
                TitleTextBlock.Text = "Отчет о доходах";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (reportCostsIncomes.IsSelected)
            {
                myFrame.Navigate(typeof(ReportCostsIncomesPage));
                TitleTextBlock.Text = "Очет о р/д";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (accumulation.IsSelected)
            {
                myFrame.Navigate(typeof(AccumulationPage));
                TitleTextBlock.Text = "Цели";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
            else if (settings.IsSelected)
            {
                myFrame.Navigate(typeof(SettingsPage));
                TitleTextBlock.Text = "Настройки";
                if (splitViewMenu.IsPaneOpen && isOpenMenuPanel == false)
                    splitViewMenu.IsPaneOpen = false;
            }
        }

        private void HumbugerButton_Click(object sender, RoutedEventArgs e)
        {
            splitViewMenu.IsPaneOpen = !splitViewMenu.IsPaneOpen;
        }

        private void report_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (reportCosts.Visibility == Visibility.Visible)
            {
                reportCosts.Visibility = Visibility.Collapsed;
                reportIncomes.Visibility = Visibility.Collapsed;
                reportCostsIncomes.Visibility = Visibility.Collapsed;
            }
            else
            {
                reportCosts.Visibility = Visibility.Visible;
                reportIncomes.Visibility = Visibility.Visible;
                reportCostsIncomes.Visibility = Visibility.Visible;
            }
        }
    }
}
