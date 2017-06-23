using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using PersonalFinances.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Windows.Storage;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PursePage : Page
    {
        ObservableCollection<Purse> purseCollection;
        Purse purseElement;
        ApplicationDataContainer localSettings;

        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        public PursePage()
        {
            this.InitializeComponent();
            this.Loaded += PursePage_Loaded;
            dt.Tick += RefreshErrorText;
            dt.Start();
        }

        private void RefreshErrorText(object sender, object e)
        {
            if (errorText.Text.Length > 1)
                errorText.Text = "";
        }

        private void PursePage_Loaded(object sender, RoutedEventArgs e)
        {
            using (PFContext db = new PFContext())
            {
                purseCollection = new ObservableCollection<Purse>(db.Purse.Include(x => x.Currency).ToList());
            }

            localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["sortPurseSetting"];

            if(value != null)
            {
                if (value.ToString() == "byName")
                {
                    SortPurseName(purseCollection);
                }
                else if (value.ToString() == "byBalanceDesc")
                {
                    SortPurseBalanceDesc(purseCollection);
                }
                else
                    SortPurseBalance(purseCollection);
            }
            else
                purseList.ItemsSource = purseCollection;

        }

        private void BtnAddPurse_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PursePageAddEdit));
        }

        private void BtnEditPurse_Click(object sender, RoutedEventArgs e)
        {
            if(purseList.SelectedItem != null)
            {
                Purse purse = purseList.SelectedItem as Purse;
                if (purse != null)
                    Frame.Navigate(typeof(PursePageAddEdit), purse.Id);
            }
            else
                errorText.Text = "Не выбран элемент";
        }

        private async void BtnDelPurse_Click(object sender, RoutedEventArgs e)
        {
            if (purseList.SelectedItem != null)
            {
                ContentDialog deletePurse = new ContentDialog()
                {
                    Title = "Подтверждение действия",
                    Content = "Вы действительно хотите удалить данный счет?" + "\n" + "Удалятся все операции, связанные с данным счетом.",
                    PrimaryButtonText = "Удалить",
                    SecondaryButtonText = "Отмена"
                };

                ContentDialogResult result = await deletePurse.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    Purse purse = purseList.SelectedItem as Purse;
                    if (purse != null)
                    {
                        DeletePurseItem(purse);
                    }
                }
                else
                    deletePurse.Hide();
            }
            else
                errorText.Text = "Не выбран элемент";
        }

        
        private void DeletePurseItem(Purse p)
        {
            using (PFContext db = new PFContext())
            {
                try
                {
                    foreach (Costs c in db.Costs)
                    {
                        if (c.PurseId == p.Id)
                            db.Costs.Remove(c);
                    }

                    foreach (Income i in db.Income)
                    {
                        if (i.PurseId == p.Id)
                            db.Income.Remove(i);
                    }
                    foreach (Displacement d in db.Displacement)
                    {
                        if (d.PurseId1 == p.Id || d.PurseId2 == p.Id)
                            db.Displacement.Remove(d);
                    }
                    foreach(AccumulationOperation a in db.AccumulationOperation)
                    {
                        if (a.PurseId == p.Id)
                            db.AccumulationOperation.Remove(a);
                    }

                    db.Purse.Remove(p);
                    db.SaveChanges();
                    purseList.ItemsSource = db.Purse.ToList();
                }
                catch
                {
                    errorText.Text = "Удаление не возможно.Повторите попытку позже";
                }
            }
        }

        private void purseList_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            purseListFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            purseElement = ((FrameworkElement)e.OriginalSource).DataContext as Purse;
        }
      
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (purseElement != null)
                Frame.Navigate(typeof(PursePageAddEdit), purseElement.Id);
        }

        private void purseList_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            purseListFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            purseElement = ((FrameworkElement)e.OriginalSource).DataContext as Purse;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deletePurse = new ContentDialog()
            {
                Title = "Подтверждение действия",
                Content = "Вы действительно хотите удалить данный счет?" + "\n" + "Удалятся все операции, связанные с данным счетом.",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };

            ContentDialogResult result = await deletePurse.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (purseElement != null)
                {
                    DeletePurseItem(purseElement);
                }
            }
            else
                deletePurse.Hide();
        }

        private void SortPurse_Click(object sender, RoutedEventArgs e)
        {
            sortFlyotMenu.ShowAt((AppBarButton)sender);
        }

        private void SortPurseName(ObservableCollection<Purse> PC)
        {
            var sortPurse = from p in PC
                            orderby p.Name
                            select p;

            sortPurseByName.IsChecked = true;
            sortPurseByBalance.IsChecked = false;
            sortPurseByBalanceDesc.IsChecked = false;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortPurseSetting"] = "byName";

            purseList.ItemsSource = sortPurse;
            
        }
        private void SortPurseBalanceDesc(ObservableCollection<Purse> PC)
        {
            var sortPurse = from p in PC
                            orderby p.Balance*p.Currency.Rate/p.Currency.CurScale descending
                            select p;

            sortPurseByName.IsChecked = false;
            sortPurseByBalance.IsChecked = false;
            sortPurseByBalanceDesc.IsChecked = true;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortPurseSetting"] = "byBalanceDesc";

            purseList.ItemsSource = sortPurse;
        }
        private void SortPurseBalance(ObservableCollection<Purse> PC)
        {
            var sortPurse = from p in PC
                            orderby p.Balance * p.Currency.Rate / p.Currency.CurScale
                            select p;

            sortPurseByName.IsChecked = false;
            sortPurseByBalance.IsChecked = true;
            sortPurseByBalanceDesc.IsChecked = false;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortPurseSetting"] = "byBalance";

            purseList.ItemsSource = sortPurse;
        }

        private void sortPurseByName_Click(object sender, RoutedEventArgs e)
        {
            SortPurseName(purseCollection);
        }

        private void sortPurseByBalanceDesc_Click(object sender, RoutedEventArgs e)
        {
            SortPurseBalanceDesc(purseCollection); //по убыванию
        }

        private void sortPurseByBalance_Click(object sender, RoutedEventArgs e)
        {
            SortPurseBalance(purseCollection);
        }
    }
}
