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
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using PersonalFinances.Models;
using Windows.Storage;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>

    //public class ClassForDiagramm
    //{
    //    public int CategoriesId { get; set; }
    //    public double Summa { get; set; }
    //}

    public struct CostsStruct
    {
        public int Id { get; set; }
        public double Summa { get; set; }
        public string DateOperation { get; set; }
        public int PurseId { get; set; }
        public int CostCategoriesId { get; set; }
        public string CostCategoriesName { get; set; }
        public string Comment { get; set; }
        public int CurrencyId { get; set; }
        public double CurrencyRate { get; set; }
        public int CurrencyScale { get; set; }

        public CostsStruct(int id, double summa, string date_operation, int purse_id, int costsCategor_id,
            string cost_categor_name, string comment, int currency_id, double currency_rate, int currency_scale)
        {
            Id = id;
            Summa = summa;
            DateOperation = date_operation;
            PurseId = purse_id;
            CostCategoriesId = costsCategor_id;
            CostCategoriesName = cost_categor_name;
            Comment = comment;
            CurrencyId = currency_id;
            CurrencyRate = currency_rate;
            CurrencyScale = currency_scale;
        }
    }
    public sealed partial class ReportPage : Page
    {
        ObservableCollection<Purse> purseCollection;
        ObservableCollection<CostCategories> costsCategoriesCollection;
        ObservableCollection<Costs> costsCollection;
        ApplicationDataContainer localSettings;
        List<CostsStruct> costsStructListNoDuplicates;
        List<CostsStruct> costsStructListAll;

        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        public ReportPage()
        {
            this.InitializeComponent();
            this.Loaded += ReportPage_Loaded;
            dt.Tick += RefreshErrorText;
            dt.Start();
        }

        private void RefreshErrorText(object sender, object e)
        {
            if(errorText.Visibility == Visibility.Visible)
            {
                errorText.Visibility = Visibility.Collapsed;
                amountSumma.Visibility = Visibility.Visible;
            }
        }

        private void ReportPage_Loaded(object sender, RoutedEventArgs e)
        {
            // для того, что-бы выбиралось из comboBox в flyoutMenu
            if (Windows.Foundation.Metadata.ApiInformation
                .IsPropertyPresent("Windows.UI.Xaml.FrameworkElement", "AllowFocusOnInteraction"))
            {
                purseListCB.AllowFocusOnInteraction = true;
                costsCategoriesCB.AllowFocusOnInteraction = true;
            }
                
            using (PFContext db = new PFContext())
            {
                purseCollection = new ObservableCollection<Purse>(db.Purse.ToList());
                costsCategoriesCollection = new ObservableCollection<CostCategories>(db.CostCategories.ToList());
                costsCollection = new ObservableCollection<Costs>(db.Costs.Include(x => x.CostCategories)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());

            }

            purseListCB.ItemsSource = purseCollection;
            costsCategoriesCB.ItemsSource = costsCategoriesCollection;            

            amountSumma.Text = "Сумма расходов: " + GetTotalAmount(costsCollection).ToString() + " BYN";
           
            ChangeCollectionForDiagramm(costsCollection);
            if (!SortCostCollection(costsCollection))
            {
                costList.ItemsSource = costsCollection;
            }

            isAllPurse.IsChecked = true;
            isAllCostsCategories.IsChecked = true;
            isAllDate.IsChecked = true;
        }
        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            List<int> listIdForDelete = new List<int>();
            if (isAllPurse.IsChecked == false && purseListCB.SelectedItem == null)
            {
                errorText.Text = "Не выбран счет";
                amountSumma.Visibility = Visibility.Collapsed;
                errorText.Visibility = Visibility.Visible;
                return;
            }
            if (isAllCostsCategories.IsChecked == false && costsCategoriesCB.SelectedItem == null)
            {
                errorText.Text = "Не выбрана категория";
                amountSumma.Visibility = Visibility.Collapsed;
                errorText.Visibility = Visibility.Visible;
                return;
            }
            if (isAllDate.IsChecked == false)
            {
                if (dateStart.Date == null || datefinish.Date == null)
                {
                    errorText.Text = "Не выбрана дата";
                    amountSumma.Visibility = Visibility.Collapsed;
                    errorText.Visibility = Visibility.Visible;
                    return;
                }
                else
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateStart.Date;
                    DateTimeOffset finishDate = (DateTimeOffset)datefinish.Date;
                    string dateFormatStart = startDate.Date.ToString("dd.MM.yyyy");
                    string dateFormatFinish = finishDate.Date.ToString("dd.MM.yyyy");

                    DateTime sDate = Convert.ToDateTime(dateFormatStart);
                    DateTime fDate = Convert.ToDateTime(dateFormatFinish);
                    if (sDate > fDate)
                    {
                        errorText.Text = "Не корректно выбрана дата";
                        amountSumma.Visibility = Visibility.Collapsed;
                        errorText.Visibility = Visibility.Visible;
                        return;
                    }
                }
            }


            using (PFContext db = new PFContext())
            {
                costsCollection = new ObservableCollection<Costs>(db.Costs.Include(x => x.CostCategories)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());
            }

            if (isAllPurse.IsChecked != true)
            {
                Purse purseComboBox = purseListCB.SelectedItem as Purse;
                foreach (Costs c in costsCollection.ToList())
                {
                    if (c.PurseId != purseComboBox.Id)
                    {
                        listIdForDelete.Add(c.Id);
                    }
                }
                /* Удаление не нужных счетов */
                for (int i = 0; i < listIdForDelete.Count; i++)
                {
                    for (int j = 0; j < costsCollection.Count; j++)
                    {
                        if (listIdForDelete[i] == costsCollection[j].Id)
                        {
                            costsCollection.RemoveAt(j);
                        }
                    }
                }
                listIdForDelete.Clear();
            }
            

            if (isAllCostsCategories.IsChecked != true)
            {
                CostCategories costCategoriesComboBox = costsCategoriesCB.SelectedItem as CostCategories;
                foreach (Costs c in costsCollection.ToList())
                {
                    if (c.CostCategoriesId != costCategoriesComboBox.Id)
                    {
                        listIdForDelete.Add(c.Id);
                    }
                }
                /* Удаление не нужных категорий */
                for (int i = 0; i < listIdForDelete.Count; i++)
                {
                    for (int j = 0; j < costsCollection.Count; j++)
                    {
                        if (listIdForDelete[i] == costsCollection[j].Id)
                        {
                            costsCollection.RemoveAt(j);
                        }
                    }
                }
                listIdForDelete.Clear();
            }
            if (isAllDate.IsChecked != true)
            {
                DateTimeOffset startDate = (DateTimeOffset)dateStart.Date;
                DateTimeOffset finishDate = (DateTimeOffset)datefinish.Date;
                string dateFormatStart = startDate.Date.ToString("dd.MM.yyyy");
                string dateFormatFinish = finishDate.Date.ToString("dd.MM.yyyy");

                DateTime sDate = Convert.ToDateTime(dateFormatStart);
                DateTime fDate = Convert.ToDateTime(dateFormatFinish);

                foreach (Costs c in costsCollection.ToList())
                {
                    if (Convert.ToDateTime(c.DateOperation) < sDate || Convert.ToDateTime(c.DateOperation) > fDate)
                    {
                        listIdForDelete.Add(c.Id);
                    }
                }
                /* Удаление не нужных дат */
                for (int i = 0; i < listIdForDelete.Count; i++)
                {
                    for (int j = 0; j < costsCollection.Count; j++)
                    {
                        if (listIdForDelete[i] == costsCollection[j].Id)
                        {
                            costsCollection.RemoveAt(j);
                        }
                    }
                }
                listIdForDelete.Clear();
            }

            if (!SortCostCollection(costsCollection))
                costList.ItemsSource = costsCollection;
            amountSumma.Text = "Сумма расходов: " + GetTotalAmount(costsCollection).ToString() + " BYN";
            ChangeCollectionForDiagramm(costsCollection);
            filterFlyoutMenu.Hide();
        }
        private void ChangeCollectionForDiagramm(ObservableCollection<Costs> collect)
        {
            ObservableCollection<Costs> costCollectionCopy = new ObservableCollection<Costs>(collect);
            var noduplicates = costCollectionCopy.Distinct();

            costsStructListNoDuplicates = new List<CostsStruct>();
            costsStructListAll = new List<CostsStruct>();

            /* не повторяющиеся */
            foreach(Costs c in noduplicates)
            {
                CostsStruct st = new CostsStruct();
                st.Id = c.Id;
                st.Summa = c.Summa;
                st.DateOperation = c.DateOperation;
                st.PurseId = c.PurseId;
                st.CostCategoriesId = c.CostCategoriesId;
                st.CostCategoriesName = c.CostCategories.Name;
                st.Comment = c.Comment;
                st.CurrencyId = c.CurrencyId;
                st.CurrencyRate = c.Currency.Rate;
                st.CurrencyScale = c.Currency.CurScale;

                costsStructListNoDuplicates.Add(st);
            }
            /* Все */
            foreach (Costs c in costCollectionCopy)
            {
                CostsStruct st = new CostsStruct();
                st.Id = c.Id;
                st.Summa = c.Summa;
                st.DateOperation = c.DateOperation;
                st.PurseId = c.PurseId;
                st.CostCategoriesId = c.CostCategoriesId;
                st.CostCategoriesName = c.CostCategories.Name;
                st.Comment = c.Comment;
                st.CurrencyId = c.CurrencyId;
                st.CurrencyRate = c.Currency.Rate;
                st.CurrencyScale = c.Currency.CurScale;

                costsStructListAll.Add(st);
            }

            //не повторяющие в BYN
            for (int i = 0; i < costsStructListNoDuplicates.Count; i++)
            {
                double summa = 0;
                summa = costsStructListNoDuplicates[i].Summa * costsStructListNoDuplicates[i].CurrencyRate / costsStructListNoDuplicates[i].CurrencyScale;
                costsStructListNoDuplicates[i] = new CostsStruct(costsStructListNoDuplicates[i].Id, summa, costsStructListNoDuplicates[i].DateOperation,
                    costsStructListNoDuplicates[i].PurseId, costsStructListNoDuplicates[i].CostCategoriesId, costsStructListNoDuplicates[i].CostCategoriesName, costsStructListNoDuplicates[i].Comment,
                    costsStructListNoDuplicates[i].CurrencyId, costsStructListNoDuplicates[i].CurrencyRate, costsStructListNoDuplicates[i].CurrencyScale);
            }
            /* Все в BYN */
            for (int i = 0; i < costsStructListAll.Count; i++)
            {
                double summa = 0;
                summa = costsStructListAll[i].Summa * costsStructListAll[i].CurrencyRate / costsStructListAll[i].CurrencyScale;
                costsStructListAll[i] = new CostsStruct(costsStructListAll[i].Id, summa, costsStructListAll[i].DateOperation,
                    costsStructListAll[i].PurseId, costsStructListAll[i].CostCategoriesId, costsStructListAll[i].CostCategoriesName, costsStructListAll[i].Comment,
                    costsStructListAll[i].CurrencyId, costsStructListAll[i].CurrencyRate, costsStructListAll[i].CurrencyScale);
            }

            for(int i = 0; i< costsStructListNoDuplicates.Count; i++)
            {
                for(int j=0; j < costsStructListAll.Count; j++)
                {
                    if(costsStructListNoDuplicates[i].Id != costsStructListAll[j].Id)
                    {
                        if(costsStructListNoDuplicates[i].CostCategoriesId == costsStructListAll[j].CostCategoriesId)
                        {
                            double tempSumma = costsStructListAll[j].Summa + costsStructListNoDuplicates[i].Summa;
                            costsStructListNoDuplicates[i] = new CostsStruct(costsStructListNoDuplicates[i].Id, tempSumma, costsStructListNoDuplicates[i].DateOperation,
                                costsStructListNoDuplicates[i].PurseId, costsStructListNoDuplicates[i].CostCategoriesId, costsStructListNoDuplicates[i].CostCategoriesName, costsStructListNoDuplicates[i].Comment,
                                costsStructListNoDuplicates[i].CurrencyId, costsStructListNoDuplicates[i].CurrencyRate, costsStructListNoDuplicates[i].CurrencyScale);
                        }
                    }
                }
            }
            (PieChartCosts.Series[0] as PieSeries).ItemsSource = costsStructListNoDuplicates;
            (ColumnChartCosts.Series[0] as ColumnSeries).ItemsSource = costsStructListNoDuplicates;
        }
   
        private void BtnShowFilters_Click(object sender, RoutedEventArgs e)
        {
            filterFlyoutMenu.ShowAt((AppBarButton)sender);
        }
        private void SortCosts_Click(object sender, RoutedEventArgs e)
        {
            sortFlyotMenu.ShowAt((AppBarButton)sender);
        }
        private void purseListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Purse purseElemComboBox = purseListCB.SelectedItem as Purse;
            if(purseElemComboBox != null)
            {
                isAllPurse.IsChecked = false;
            }
        }

        private void costsCategoriesCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CostCategories costCategoriesElement = costsCategoriesCB.SelectedItem as CostCategories;
            if(costCategoriesElement != null)
            {
                isAllCostsCategories.IsChecked = false;
            }
        }
        private void isAllPurse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Convert.ToBoolean(isAllPurse.IsChecked))
            {
                purseListCB.ItemsSource = null;
                purseListCB.ItemsSource = purseCollection;
            }
        }

        private void isAllCostsCategories_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(Convert.ToBoolean(isAllCostsCategories.IsChecked))
            {
                costsCategoriesCB.ItemsSource = null;
                costsCategoriesCB.ItemsSource = costsCategoriesCollection;
            }
        }

        private void isAllDate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Convert.ToBoolean(isAllDate.IsChecked))
            {
                dateStart.Date = null;
                datefinish.Date = null;
            }
        }

        private void dateStart_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if(dateStart.Date != null)
            {
                isAllDate.IsChecked = false;
            }
        }

        private void datefinish_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if(datefinish.Date != null)
            {
                isAllDate.IsChecked = false;
            }
        }

        private double GetTotalAmount(ObservableCollection<Costs> collection1)
        {
            double sum = 0;
            ObservableCollection<Costs> cc = new ObservableCollection<Costs>(collection1);
            foreach (Costs c in cc)
            {
                double tempVar = c.Summa * c.Currency.Rate / c.Currency.CurScale;
                sum += tempVar;
            }
            sum = Math.Round(sum,2);
            return sum;
        }
        #region Sort
        private bool SortCostCollection(ObservableCollection<Costs> c)
        {
            localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["sortCostsSetting"];

            if (value != null)
            {
                if (value.ToString() == "byDateDesc")
                {
                    /* По убыванию (дата) */
                    SortCostDateDesc(c);
                    return true;
                }
                else if (value.ToString() == "byDate")
                {
                    /* По возростанию (дата) */
                    SortCostDate(c);
                    return true;
                }
                else if (value.ToString() == "bySummaDesc")
                {
                    /* По убыванию (сумма) */
                    SortCostSummaDesc(c);
                    return true;
                }
                else if (value.ToString() == "bySumma")
                {
                    /* По возростанию (сумма) */
                    SortCostSumma(c);
                    return true;
                }
                return false;
            }
            return false;
        }
        private void SortCostDateDesc(ObservableCollection<Costs> cCollection)
        {
            var sortCosts = from c in cCollection
                            orderby Convert.ToDateTime(c.DateOperation) descending
                            select c;
            costList.ItemsSource = sortCosts;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortCostsSetting"] = "byDateDesc";

            sortCostsByDateDesc.IsChecked = true;
            sortCostsByDate.IsChecked = false;
            sortCostsBySymmaDesc.IsChecked = false;
            sortCostsBySumma.IsChecked = false;
        }
        private void SortCostDate(ObservableCollection<Costs> cCollection)
        {
            var sortCosts = from c in cCollection
                            orderby Convert.ToDateTime(c.DateOperation)
                            select c;
            costList.ItemsSource = sortCosts;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortCostsSetting"] = "byDate";

            sortCostsByDateDesc.IsChecked = false;
            sortCostsByDate.IsChecked = true;
            sortCostsBySymmaDesc.IsChecked = false;
            sortCostsBySumma.IsChecked = false;
        }
        private void SortCostSummaDesc(ObservableCollection<Costs> cCollection)
        {
            var sortCosts = from c in cCollection
                            orderby c.Summa * c.Currency.Rate / c.Currency.CurScale descending
                            select c;
            costList.ItemsSource = sortCosts;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortCostsSetting"] = "bySummaDesc";

            sortCostsByDateDesc.IsChecked = false;
            sortCostsByDate.IsChecked = false;
            sortCostsBySymmaDesc.IsChecked = true;
            sortCostsBySumma.IsChecked = false;
        }
        private void SortCostSumma(ObservableCollection<Costs> cCollection)
        {
            var sortCosts = from c in cCollection
                            orderby c.Summa * c.Currency.Rate / c.Currency.CurScale
                            select c;
            costList.ItemsSource = sortCosts;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortCostsSetting"] = "bySumma";

            sortCostsByDateDesc.IsChecked = false;
            sortCostsByDate.IsChecked = false;
            sortCostsBySymmaDesc.IsChecked = false;
            sortCostsBySumma.IsChecked = true;
        }

        

        private void sortCostsByDateDesc_Click(object sender, RoutedEventArgs e)
        {
            SortCostDateDesc(costsCollection);
        }

        private void sortCostsByDate_Click(object sender, RoutedEventArgs e)
        {
            SortCostDate(costsCollection);
        }

        private void sortCostsBySymmaDesc_Click(object sender, RoutedEventArgs e)
        {
            SortCostSummaDesc(costsCollection);
        }

        private void sortCostsBySumma_Click(object sender, RoutedEventArgs e)
        {
            SortCostSumma(costsCollection);
        }

        #endregion

        private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (rootPivot.SelectedIndex == 0)
            //    ReportPage_Loaded(sender,e);
            //else if (rootPivot.SelectedIndex == 1)
            //{
            //    ReportPage_Loaded(sender, e);
            //    ChangeCollectionForDiagramm(costsCollection);
            //}                
            //else if(rootPivot.SelectedIndex == 2)
            //{
            //    ReportPage_Loaded(sender, e);
            //    ChangeCollectionForDiagramm(costsCollection);
            //}
               

        }
    }
}
