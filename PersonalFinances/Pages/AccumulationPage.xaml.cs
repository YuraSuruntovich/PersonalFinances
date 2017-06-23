using Microsoft.EntityFrameworkCore;
using PersonalFinances.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AccumulationPage : Page
    {
        ObservableCollection<Accumulation> accumulationCollection;
        Accumulation accumulationElement;
        public AccumulationPage()
        {
            this.InitializeComponent();
            this.Loaded += AccumulationPage_Loaded;
        }

        private void AccumulationPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (PFContext db = new PFContext())
            {
                accumulationCollection = new ObservableCollection<Accumulation>(db.Accumulation
                    .Include(x => x.Currency).ToList());
            }
            accumulationList.ItemsSource = accumulationCollection;
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AccumulationAddEditPage));
        }

        private void EditAccumulation_Click(object sender, RoutedEventArgs e)
        {
            if(accumulationElement != null)
                Frame.Navigate(typeof(AccumulationAddEditPage), accumulationElement.Id);
        }

        private async void DeleteAccumulation_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteAccumulation = new ContentDialog()
            {
                Title = "Подтверждение действия",
                Content = "Вы действительно хотите удалить данную цель?",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };

            ContentDialogResult result = await deleteAccumulation.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (accumulationElement != null)
                {
                    DeleteAccumulationItem(accumulationElement);
                }
            }
            else
                deleteAccumulation.Hide();            
        }

        private void DeleteAccumulationItem(Accumulation a)
        {
            using(PFContext db = new PFContext())
            {
                try
                {
                    foreach(AccumulationOperation ao in db.AccumulationOperation)
                    {
                        if (ao.AccumulationId == a.Id)
                            db.AccumulationOperation.Remove(ao);
                    }

                    db.Accumulation.Remove(a);
                    db.SaveChanges();

                    accumulationCollection = new ObservableCollection<Accumulation>(db.Accumulation
                    .Include(x => x.Currency).ToList());
                    accumulationList.ItemsSource = accumulationCollection;
                }
                catch
                {
                    errorText.Text = "Не возможно удалить цель. Попробуйте позже.";
                }
            }
        }

        private void accumulationList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            accumulationFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            accumulationElement = ((FrameworkElement)e.OriginalSource).DataContext as Accumulation;
        }

        private void accumulationList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            accumulationFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            accumulationElement = ((FrameworkElement)e.OriginalSource).DataContext as Accumulation;
        }

        private void AddAccumulationOperation_Click(object sender, RoutedEventArgs e)
        {
            if (accumulationElement != null)
                Frame.Navigate(typeof(AccumulationOperationAddEditPage), accumulationElement.Id);
        }
    }
}
