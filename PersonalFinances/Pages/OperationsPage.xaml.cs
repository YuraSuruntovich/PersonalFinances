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
using PersonalFinances.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Windows.Storage;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class OperationsPage : Page
    {
        ObservableCollection<Costs> costsCollection;
        ObservableCollection<Income> incomesCollection;
        ObservableCollection<Displacement> displacementCollection;
        Costs costElement;
        Income incomeElement;
        Displacement displacementElemnt;
        ApplicationDataContainer localSettings;

        bool sealedItemCost = false;
        bool sealedItemIncome = false;
        bool sealedItemDisplacement = false;
        double sizeWidthWindow;
        public OperationsPage()
        {
            this.InitializeComponent();
            this.Loaded += OperationsPage_Loaded;
            this.SizeChanged += (s, e) =>
            {
                sizeWidthWindow = e.NewSize.Width;
                if (e.NewSize.Width > 500 && sealedItemCost == true)
                    detailPanel.Visibility = Visibility.Visible;
                else
                    detailPanel.Visibility = Visibility.Collapsed;

                if (e.NewSize.Width > 500 && sealedItemIncome == true)
                    detailPanelIncome.Visibility = Visibility.Visible;
                else
                    detailPanelIncome.Visibility = Visibility.Collapsed;

                if (e.NewSize.Width > 500 && sealedItemDisplacement == true)
                    detailPanelDisplacement.Visibility = Visibility.Visible;
                else
                    detailPanelDisplacement.Visibility = Visibility.Collapsed;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                int index = (int)e.Parameter;
                rootPivot.SelectedIndex = index;
            }
        }
        private void OperationsPage_Loaded(object sender, RoutedEventArgs e)
        {
            using(PFContext db = new PFContext())
            {
                costsCollection = new ObservableCollection<Costs>(db.Costs.Include(x => x.CostCategories)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());
                incomesCollection = new ObservableCollection<Income>(db.Income.Include(x => x.SourceOfIncome)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());
                displacementCollection = new ObservableCollection<Displacement>(db.Displacement.Include(x => x.Currency).ToList());
            }

            localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["sortOperationsSetting"];

            if(value != null)
            {
                if (value.ToString() == "byDateDesc")
                {
                    /* По убыванию(дата) */
                    SortAllDateDesc(costsCollection, incomesCollection, displacementCollection);
                }
                else if (value.ToString() == "byDate")
                {
                    /* По возростанию (дата) */
                    SortAllDate(costsCollection, incomesCollection, displacementCollection);
                }
                else if (value.ToString() == "bySummaDesc")
                {
                    /* По убыванию (сумма) */
                    SortAllSummaDesc(costsCollection, incomesCollection, displacementCollection);
                }
                else if(value.ToString() == "bySumma")
                {
                    /* По возростанию (сумма) */
                    SortAllSumma(costsCollection, incomesCollection, displacementCollection);
                }
            }
            else
            {
                costList.ItemsSource = costsCollection;
                incomeList.ItemsSource = incomesCollection;
                displacementList.ItemsSource = displacementCollection;
            }
        }

        private void BtnAddCost_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CostAddEditPage));
        }
        private void BtnAddIncome_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(IncomeAddEditPage));
        }

        private void costList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(costList.SelectedItem != null)
            {
                sealedItemCost = true;
                Costs costsDetailObj = costList.SelectedItem as Costs;
                Costs costsDetail = costsCollection.FirstOrDefault(c => c.Id == costsDetailObj.Id);


                purseDPC.Text = costsDetail.Purse.Name;
                summaDPC.Text = costsDetail.Summa.ToString();
                abrevDPC.Text = costsDetail.Currency.Abbreviation;
                dateOperatDPC.Text = costsDetail.DateOperation;
                costCategorDPC.Text = costsDetail.CostCategories.Name;
                commentDPC.Text = costsDetail.Comment;

                if (sizeWidthWindow > 500)
                {
                    detailPanel.Visibility = Visibility.Visible;
                }
            }
            
        }

        /* Costs */
        private void costList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            costsFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            costElement = ((FrameworkElement)e.OriginalSource).DataContext as Costs;
        }

        private void costList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            costsFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            costElement = ((FrameworkElement)e.OriginalSource).DataContext as Costs;
        }

        private void EditCost_Click(object sender, RoutedEventArgs e)
        {
            if (costElement != null)
                Frame.Navigate(typeof(CostAddEditPage), costElement.Id);
        }

        private async void DeleteCost_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteCost = new ContentDialog()
            {
                Title = "Подтверждение действия",
                Content = "Вы действительно хотите удалить данную операцию?",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };

            ContentDialogResult result = await deleteCost.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (costElement != null)
                {
                    DeleteCostItem(costElement);
                    if (sealedItemCost)
                    {
                        sealedItemCost = false;
                        //detailPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
                deleteCost.Hide();
        }

        private void DeleteCostItem(Costs c)
        {
            Purse purseChangeBalance;
            using(PFContext db = new PFContext())
            {
                db.Costs.Remove(c);
                purseChangeBalance = db.Purse.FirstOrDefault(p => p.Id == c.PurseId);
                purseChangeBalance.Balance = purseChangeBalance.Balance + c.Summa;
                db.Purse.Update(purseChangeBalance);
                db.SaveChanges();

                costsCollection = new ObservableCollection<Costs>(db.Costs.Include(x => x.CostCategories)
                   .Include(x => x.Purse).Include(x => x.Currency).ToList());
                SortCostDateDesc(costsCollection);
            }
        }

        /* Income */
        private void incomeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (incomeList.SelectedItem != null)
            {
                sealedItemIncome = true;
                Income incomeDetail = incomeList.SelectedItem as Income;

                purseDPI.Text = incomeDetail.Purse.Name;
                summaDPI.Text = incomeDetail.Summa.ToString();
                abrevDPI.Text = incomeDetail.Currency.Abbreviation;
                dateOperatDPI.Text = incomeDetail.DateOperation;
                incomeCategorDPI.Text = incomeDetail.SourceOfIncome.Name;
                commentDPI.Text = incomeDetail.Comment;

                if (sizeWidthWindow > 500)
                {
                    detailPanelIncome.Visibility = Visibility.Visible;
                }
            }
        }

        private void incomeList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            incomesFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            incomeElement = ((FrameworkElement)e.OriginalSource).DataContext as Income;
        }

        private void incomeList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            incomesFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            incomeElement = ((FrameworkElement)e.OriginalSource).DataContext as Income;
        }

        private void EditIncome_Click(object sender, RoutedEventArgs e)
        {
            if (incomeElement != null)
                Frame.Navigate(typeof(IncomeAddEditPage), incomeElement.Id);
        }

        private async void DeleteIncome_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteIncome = new ContentDialog()
            {
                Title = "Подтверждение действия",
                Content = "Вы действительно хотите удалить данную операцию?",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };

            ContentDialogResult result = await deleteIncome.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (incomeElement != null)
                {
                    DeleteIncomeItem(incomeElement);
                    if (sealedItemIncome)
                    {
                        sealedItemIncome = false;
                        detailPanelIncome.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
                deleteIncome.Hide();
        }

        private void DeleteIncomeItem(Income i)
        {
            Purse purseChangeBalance;
            using (PFContext db = new PFContext())
            {
                db.Income.Remove(i);
                purseChangeBalance = db.Purse.FirstOrDefault(p => p.Id == i.PurseId);
                purseChangeBalance.Balance = purseChangeBalance.Balance - i.Summa;
                db.Purse.Update(purseChangeBalance);
                db.SaveChanges();

                incomesCollection = new ObservableCollection<Income>(db.Income.Include(x => x.SourceOfIncome)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());
                SortIncomeDateDesc(incomesCollection);
            }
        }

        private void BtnAddDisplacement_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DisplacementAddEditPage));
        }

        private void displacementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(displacementList.SelectedItem != null)
            {
                Purse purseOut;
                Purse purseIncome;
                Currency currencyOut;
                sealedItemDisplacement = true;

                Displacement displacementDetailPanelObject = displacementList.SelectedItem as Displacement;
                Displacement displacementDetailPanelItem = displacementCollection.FirstOrDefault(d => d.Id == displacementDetailPanelObject.Id);
                using (PFContext db = new PFContext())
                {
                    purseOut = db.Purse.FirstOrDefault(p => p.Id == displacementDetailPanelObject.PurseId1);
                    purseIncome = db.Purse.FirstOrDefault(p => p.Id == displacementDetailPanelObject.PurseId2);
                    currencyOut = db.Currency.FirstOrDefault(c => c.Id == purseOut.CurrencyId);
                }

                namePurseOutDisplacement.Text = purseOut.Name;
                summaOutDisplacement.Text = displacementDetailPanelItem.SummaOut.ToString();
                curAbrevOutDisplacement.Text = currencyOut.Abbreviation;

                namePurseIncomeDisplacement.Text = purseIncome.Name;
                summaIncomeDisplacement.Text = displacementDetailPanelItem.SummaIncome.ToString();
                curAbrevIncomeDisplacement.Text = displacementDetailPanelItem.Currency.Abbreviation;

                dateOperatDPDisplacement.Text = displacementDetailPanelItem.DateOperation;

                curAbrevv1.Text = currencyOut.Abbreviation;
                name2.Text = displacementDetailPanelItem.RateCur1.ToString();

                curAbrevv3.Text = displacementDetailPanelItem.Currency.Abbreviation;
                name4.Text = displacementDetailPanelItem.RateCur2.ToString();

                if(displacementDetailPanelItem.RateCur1 == displacementDetailPanelItem.RateCur2)
                {
                    Rate1DPDisplacement.Visibility = Visibility.Collapsed;
                    Rate2DPDisplacement.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Rate1DPDisplacement.Visibility = Visibility.Visible;
                    Rate2DPDisplacement.Visibility = Visibility.Visible;
                }
                if(currencyOut.Abbreviation == "BYN")
                {
                    Rate1DPDisplacement.Visibility = Visibility.Collapsed;
                }
                else
                    Rate1DPDisplacement.Visibility = Visibility.Visible;
                if (displacementDetailPanelItem.Currency.Abbreviation == "BYN")
                {
                    Rate2DPDisplacement.Visibility = Visibility.Collapsed;
                }
                else
                    Rate2DPDisplacement.Visibility = Visibility.Visible;


                if (sizeWidthWindow > 500)
                {
                    detailPanelDisplacement.Visibility = Visibility.Visible;
                }
            }
        }

        private void displacementList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            displacementFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            displacementElemnt = ((FrameworkElement)e.OriginalSource).DataContext as Displacement;
        }

        private void displacementList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            displacementFlyoutMenu.ShowAt(listView, e.GetPosition(listView));
            displacementElemnt = ((FrameworkElement)e.OriginalSource).DataContext as Displacement;
        }

        private void EditDisplacement_Click(object sender, RoutedEventArgs e)
        {
            if (displacementElemnt != null)
                Frame.Navigate(typeof(DisplacementAddEditPage), displacementElemnt.Id);
        }

        private async void DeleteDisplacement_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDisplacement = new ContentDialog()
            {
                Title = "Подтверждение действия",
                Content = "Вы действительно хотите удалить данную операцию?",
                PrimaryButtonText = "Удалить",
                SecondaryButtonText = "Отмена"
            };

            ContentDialogResult result = await deleteDisplacement.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (displacementElemnt != null)
                {
                    DeleteDisplacementItem(displacementElemnt);
                    if (sealedItemDisplacement)
                    {
                        sealedItemDisplacement = false;
                        detailPanelDisplacement.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
                deleteDisplacement.Hide();
        }

        private void DeleteDisplacementItem(Displacement d)
        {
            Purse purseChangeBalances1;
            Purse purseChangeBalances2;
            using(PFContext db = new PFContext())
            {
                db.Displacement.Remove(d);
                purseChangeBalances1 = db.Purse.FirstOrDefault(p => p.Id == d.PurseId1);
                purseChangeBalances2 = db.Purse.FirstOrDefault(p => p.Id == d.PurseId2);

                purseChangeBalances1.Balance += d.SummaOut;
                purseChangeBalances2.Balance -= d.SummaIncome;

                db.Purse.Update(purseChangeBalances1);
                db.Purse.Update(purseChangeBalances2);

                db.SaveChanges();

                displacementCollection = new ObservableCollection<Displacement>(db.Displacement.Include(x => x.Currency).ToList());
                SortDisplacementDateDesc(displacementCollection);
            }
        }

        #region sort
        private void SortAllDateDesc(ObservableCollection<Costs> c, ObservableCollection<Income> i, ObservableCollection<Displacement> d)
        {
            SortCostDateDesc(c);
            SortIncomeDateDesc(i);
            SortDisplacementDateDesc(d);

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortOperationsSetting"] = "byDateDesc";

            sortOperationsByDateDesc.IsChecked = true;
            sortOperationsByDate.IsChecked = false;
            sortOperationsBySymmaDesc.IsChecked = false;
            sortOperationsBySumma.IsChecked = false;

        }
        private void SortAllDate(ObservableCollection<Costs> c, ObservableCollection<Income> i, ObservableCollection<Displacement> d)
        {
            SortCostDate(c);
            SortIncomeDate(i);
            SortDisplacementDate(d);

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortOperationsSetting"] = "byDate";

            sortOperationsByDateDesc.IsChecked = false;
            sortOperationsByDate.IsChecked = true;
            sortOperationsBySymmaDesc.IsChecked = false;
            sortOperationsBySumma.IsChecked = false;
        }
        private void SortAllSummaDesc(ObservableCollection<Costs> c, ObservableCollection<Income> i, ObservableCollection<Displacement> d)
        {
            SortCostSummaDesc(c);
            SortIncomeSummaDesc(i);
            SortDisplacementSummaDesc(d);

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortOperationsSetting"] = "bySummaDesc";

            sortOperationsByDateDesc.IsChecked = false;
            sortOperationsByDate.IsChecked = false;
            sortOperationsBySymmaDesc.IsChecked = true;
            sortOperationsBySumma.IsChecked = false;
        }
        private void SortAllSumma(ObservableCollection<Costs> c, ObservableCollection<Income> i, ObservableCollection<Displacement> d)
        {
            SortCostSumma(c);
            SortIncomeSumma(i);
            SortDisplacementSumma(d);

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortOperationsSetting"] = "bySumma";

            sortOperationsByDateDesc.IsChecked = false;
            sortOperationsByDate.IsChecked = false;
            sortOperationsBySymmaDesc.IsChecked = false;
            sortOperationsBySumma.IsChecked = true;
        }
        private void SortCostDateDesc(ObservableCollection<Costs> cCollection)
        {
                var sortCosts = from c in cCollection
                                orderby Convert.ToDateTime(c.DateOperation) descending
                                select c;
                costList.ItemsSource = sortCosts;
        }
        private void SortIncomeDateDesc(ObservableCollection<Income> iCollection)
        {
            var sortIncome = from c in iCollection
                            orderby Convert.ToDateTime(c.DateOperation) descending
                            select c;
            incomeList.ItemsSource = sortIncome;
        }
        private void SortDisplacementDateDesc(ObservableCollection<Displacement> dCollection)
        {
            var sortDisplacement = from d in dCollection
                                   orderby Convert.ToDateTime(d.DateOperation) descending
                                   select d;
            displacementList.ItemsSource = sortDisplacement;
        }

        private void SortCostDate(ObservableCollection<Costs> cCollection)
        {
            var sortCosts = from c in cCollection
                            orderby Convert.ToDateTime(c.DateOperation)
                            select c;
            costList.ItemsSource = sortCosts;
        }
        private void SortIncomeDate(ObservableCollection<Income> iCollection)
        {
            var sortIncome = from c in iCollection
                             orderby Convert.ToDateTime(c.DateOperation)
                             select c;
            incomeList.ItemsSource = sortIncome;
        }
        private void SortDisplacementDate(ObservableCollection<Displacement> dCollection)
        {
            var sortDisplacement = from d in dCollection
                                   orderby Convert.ToDateTime(d.DateOperation)
                                   select d;
            displacementList.ItemsSource = sortDisplacement;
        }

        
        private void SortCostSummaDesc(ObservableCollection<Costs> cCollection)
        {
            var sortCosts = from c in cCollection
                            orderby c.Summa*c.Currency.Rate/ c.Currency.CurScale  descending
                            select c;
            costList.ItemsSource = sortCosts;
        }
        private void SortIncomeSummaDesc(ObservableCollection<Income> iCollection)
        {
            var sortIncome = from c in iCollection
                             orderby c.Summa * c.Currency.Rate / c.Currency.CurScale descending
                             select c;
            incomeList.ItemsSource = sortIncome;
        }
        private void SortDisplacementSummaDesc(ObservableCollection<Displacement> dCollection)
        {
            var sortDisplacement = from d in dCollection
                                   orderby d.SummaIncome*d.Currency.Rate/d.Currency.CurScale descending
                                   select d;
            displacementList.ItemsSource = sortDisplacement;
        }

        private void SortCostSumma(ObservableCollection<Costs> cCollection)
        {
            var sortCosts = from c in cCollection
                            orderby c.Summa * c.Currency.Rate / c.Currency.CurScale
                            select c;
            costList.ItemsSource = sortCosts;
        }
        private void SortIncomeSumma(ObservableCollection<Income> iCollection)
        {
            var sortIncome = from c in iCollection
                             orderby c.Summa * c.Currency.Rate / c.Currency.CurScale
                             select c;
            incomeList.ItemsSource = sortIncome;
        }
        private void SortDisplacementSumma(ObservableCollection<Displacement> dCollection)
        {
            var sortDisplacement = from d in dCollection
                                   orderby d.SummaIncome * d.Currency.Rate / d.Currency.CurScale
                                   select d;
            displacementList.ItemsSource = sortDisplacement;
        }
        #endregion

        private void SortOperation_Click(object sender, RoutedEventArgs e)
        {
            sortFlyotMenu.ShowAt((AppBarButton)sender);
        }

        private void sortOperationsByDateDesc_Click(object sender, RoutedEventArgs e)
        {
            SortAllDateDesc(costsCollection, incomesCollection, displacementCollection);
        }

        private void sortOperationsByDate_Click(object sender, RoutedEventArgs e)
        {
            SortAllDate(costsCollection, incomesCollection, displacementCollection);
        }

        private void sortOperationsBySymmaDesc_Click(object sender, RoutedEventArgs e)
        {
            SortAllSummaDesc(costsCollection, incomesCollection, displacementCollection);
        }

        private void sortOperationsBySumma_Click(object sender, RoutedEventArgs e)
        {
            SortAllSumma(costsCollection, incomesCollection, displacementCollection);
        }
    }
}
