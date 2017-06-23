using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using PersonalFinances.Models;
using Windows.Storage;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CategoriesPage : Page
    {
        CostCategories costCategorElement;
        SourceOfIncome sourceOfIncomeElement;
        ObservableCollection<CostCategories> costCategorCollection;
        ObservableCollection<SourceOfIncome> sourceOfIncomeCollection;
        ApplicationDataContainer localSettings;
        public CategoriesPage()
        {
            this.InitializeComponent();
            this.Loaded += CategoriesPage_Loaded;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                int index = (int)e.Parameter;
                pivot.SelectedIndex = index;
            }
        }
        private void CategoriesPage_Loaded(object sender, RoutedEventArgs e)
        {
            using(PFContext db = new PFContext())
            {
                costCategorCollection = new ObservableCollection<CostCategories>(db.CostCategories.ToList());
                sourceOfIncomeCollection = new ObservableCollection<SourceOfIncome>(db.SourceOfIncome.ToList());                
            }

            localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["sortCategoriesSettings"];

            if(value != null)
            {
                if(value.ToString() == "byNameDesc")
                {
                    SortCategoriesByNameDesc(costCategorCollection, sourceOfIncomeCollection);
                }
                else
                {
                    SortCategoriesByName(costCategorCollection, sourceOfIncomeCollection);
                }
            }
            else
            {
                costCategorList.ItemsSource = costCategorCollection;
                sourceOfIncomeList.ItemsSource = sourceOfIncomeCollection;
            }

        }

        private void BtnAddIncomeCategor_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SourceOfIncomeAddEditPage));
        }
        private void BtnAddCostCategor_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CostCategoriesAddEditPage));
        }

        private void costCategorList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            costCategorListFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            costCategorElement = ((FrameworkElement)e.OriginalSource).DataContext as CostCategories;
        }

        private void costCategorList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            costCategorListFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            costCategorElement = ((FrameworkElement)e.OriginalSource).DataContext as CostCategories;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (costCategorElement != null)
                Frame.Navigate(typeof(CostCategoriesAddEditPage), costCategorElement.Id);
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteCostCategor = new ContentDialog()
            {
                Title = "Подтверждение действия",
                Content = "Вы действительно хотите удалить дынную категорию?" + "\n" +"Удалятся все операции, связанные с данной категорией.",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };
            ContentDialogResult result = await deleteCostCategor.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                DeleteCostCategorItem(costCategorElement);
            }
            else
                deleteCostCategor.Hide();
        }

        private void sourceOfIncomeList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            sourceOfIncomeFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            sourceOfIncomeElement = ((FrameworkElement)e.OriginalSource).DataContext as SourceOfIncome;

        }

        private void sourceOfIncomeList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            sourceOfIncomeFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            sourceOfIncomeElement = ((FrameworkElement)e.OriginalSource).DataContext as SourceOfIncome;
        }

        private void EditSourceOfIncome_Click(object sender, RoutedEventArgs e)
        {
            if (sourceOfIncomeElement != null)
                Frame.Navigate(typeof(SourceOfIncomeAddEditPage), sourceOfIncomeElement.Id);
        }

        private async void DeleteSourceOfIncome_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteSourceOfIncome = new ContentDialog()
            {
                Title = "Подтверждение действия",
                Content = "Вы действительно хотите удалить дынную категорию?" + "\n" + "Удалятся все операции, связанные с данной категорией.",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };
            ContentDialogResult result = await deleteSourceOfIncome.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                DeleteSourceOfIncomeItem(sourceOfIncomeElement);
            }
            else
                deleteSourceOfIncome.Hide();
        }

        private void DeleteSourceOfIncomeItem(SourceOfIncome sourceOfIncome)
        {
            Purse purseChangeBalance;
            using (PFContext db = new PFContext())
            {
                foreach(Income i in db.Income)
                {
                    if (i.SourceOfIncomeId == sourceOfIncome.Id)
                    {
                        db.Income.Remove(i);
                        purseChangeBalance = db.Purse.FirstOrDefault(p => p.Id == i.PurseId);
                        purseChangeBalance.Balance = purseChangeBalance.Balance - i.Summa;
                        db.Purse.Update(purseChangeBalance);
                    }
                }
                db.SourceOfIncome.Remove(sourceOfIncome);
                db.SaveChanges();
                sourceOfIncomeList.ItemsSource = db.SourceOfIncome.ToList();
            }
        }

        private void DeleteCostCategorItem(CostCategories costCategor)
        {
            Purse purseChangeBalance;
            using (PFContext db = new PFContext())
            {
                foreach (Costs c in db.Costs)
                {
                    if (c.CostCategoriesId == costCategor.Id)
                    {
                        db.Costs.Remove(c);
                        purseChangeBalance = db.Purse.FirstOrDefault(p => p.Id == c.PurseId);
                        purseChangeBalance.Balance = purseChangeBalance.Balance + c.Summa;
                        db.Purse.Update(purseChangeBalance);
                    }                        
                }
                db.CostCategories.Remove(costCategor);
                db.SaveChanges();
                costCategorList.ItemsSource = db.CostCategories.ToList();
            }
        }

        private void sortCategorByName_Click(object sender, RoutedEventArgs e)
        {
            SortCategoriesByName(costCategorCollection, sourceOfIncomeCollection);
        }

        private void sortCategorByNameDesc_Click(object sender, RoutedEventArgs e)
        {
            SortCategoriesByNameDesc(costCategorCollection, sourceOfIncomeCollection);
        }

        private void SortCategor_Click(object sender, RoutedEventArgs e)
        {
            sortFlyotMenu.ShowAt((AppBarButton)sender);
        }

        private void SortCategoriesByName(ObservableCollection<CostCategories> CC, ObservableCollection<SourceOfIncome> sOfI)
        {
            var sortCostCategor = from c in CC
                            orderby c.Name
                            select c;
            var sortSourceOfIncome = from s in sOfI
                                     orderby s.Name
                                     select s;


            sortCategorByName.IsChecked = true;
            sortCategorByNameDesc.IsChecked = false;
            
            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortCategoriesSettings"] = "byName";

            costCategorList.ItemsSource = sortCostCategor;
            sourceOfIncomeList.ItemsSource = sortSourceOfIncome;
        }
        private void SortCategoriesByNameDesc(ObservableCollection<CostCategories> CC, ObservableCollection<SourceOfIncome> sOfI)
        {
            var sortCostCategor = from c in CC
                                  orderby c.Name descending
                                  select c;
            var sortSourceOfIncome = from s in sOfI
                                     orderby s.Name descending
                                     select s;


            sortCategorByName.IsChecked = false;
            sortCategorByNameDesc.IsChecked = true;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortCategoriesSettings"] = "byNameDesc";

            costCategorList.ItemsSource = sortCostCategor;
            sourceOfIncomeList.ItemsSource = sortSourceOfIncome;
        }
    }
}
