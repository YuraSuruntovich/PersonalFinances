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
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PersonalFinances.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CurrencyPage : Page
    {
        Currency currencyElement;
        ObservableCollection<Currency> cList;
        public CurrencyPage()
        {
            this.InitializeComponent();
            this.Loaded += CurrencyPage_Loaded;
        }

        private void CurrencyPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (PFContext db = new PFContext())
            {
                cList = new ObservableCollection<Currency>(db.Currency.Where(c => c.Id != 1).ToList());
                currencyList.ItemsSource = cList;

            }
        }

        private void currencyList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            currencyListFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            currencyElement = ((FrameworkElement)e.OriginalSource).DataContext as Currency;
        }

        private void currencyList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            currencyListFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            currencyElement = ((FrameworkElement)e.OriginalSource).DataContext as Currency;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (currencyElement != null)
                Frame.Navigate(typeof(CurrencyEditPage), currencyElement.Id);
        }
    }
}
