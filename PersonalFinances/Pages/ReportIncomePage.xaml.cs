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
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public struct IncomesStruct
    {
        public int Id { get; set; }
        public double Summa { get; set; }
        public string DateOperation { get; set; }
        public int PurseId { get; set; }
        public int IncomeCategoriesId { get; set; }
        public string IncomeCategoriesName { get; set; }
        public string Comment { get; set; }
        public int CurrencyId { get; set; }
        public double CurrencyRate { get; set; }
        public int CurrencyScale { get; set; }

        public IncomesStruct(int id, double summa, string date_operation, int purse_id, int incomeCategor_id,
            string income_categor_name, string comment, int currency_id, double currency_rate, int currency_scale)
        {
            Id = id;
            Summa = summa;
            DateOperation = date_operation;
            PurseId = purse_id;
            IncomeCategoriesId = incomeCategor_id;
            IncomeCategoriesName = income_categor_name;
            Comment = comment;
            CurrencyId = currency_id;
            CurrencyRate = currency_rate;
            CurrencyScale = currency_scale;
        }
    }
    public sealed partial class ReportIncomePage : Page
    {
        ObservableCollection<Purse> purseCollection;
        ObservableCollection<SourceOfIncome> incomesCategoriesCollection;
        ObservableCollection<Income> incomesCollection;
        ApplicationDataContainer localSettings;
        List<IncomesStruct> incomesStructListNoDuplicates;
        List<IncomesStruct> incomesStructListAll;

        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        public ReportIncomePage()
        {
            this.InitializeComponent();
            this.Loaded += ReportIncomePage_Loaded;
            dt.Tick += RefreshErrorText;
            dt.Start();
        }

        private void ReportIncomePage_Loaded(object sender, RoutedEventArgs e)
        {
            // для того, что-бы выбиралось из comboBox в flyoutMenu
            if (Windows.Foundation.Metadata.ApiInformation
                .IsPropertyPresent("Windows.UI.Xaml.FrameworkElement", "AllowFocusOnInteraction"))
            {
                purseListCB.AllowFocusOnInteraction = true;
                incomesCategoriesCB.AllowFocusOnInteraction = true;
            }
            using (PFContext db = new PFContext())
            {
                purseCollection = new ObservableCollection<Purse>(db.Purse.ToList());
                incomesCategoriesCollection = new ObservableCollection<SourceOfIncome>(db.SourceOfIncome.ToList());
                incomesCollection = new ObservableCollection<Income>(db.Income.Include(x => x.SourceOfIncome)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());

            }

            purseListCB.ItemsSource = purseCollection;
            incomesCategoriesCB.ItemsSource = incomesCategoriesCollection;

            amountSumma.Text = "Сумма доходов: " + GetTotalAmount(incomesCollection).ToString() + " BYN";
            ChangeCollectionForDiagramm(incomesCollection);

            if (!SortIncomeCollection(incomesCollection))
            {
                incomeList.ItemsSource = incomesCollection;
            }

            isAllPurse.IsChecked = true;
            isAllIncomesCategories.IsChecked = true;
            isAllDate.IsChecked = true;

        }
        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            List<int> listIdForDelete = new List<int>();
            #region Validation
            if (isAllPurse.IsChecked == false && purseListCB.SelectedItem == null)
            {
                errorText.Text = "Не выбран счет";
                amountSumma.Visibility = Visibility.Collapsed;
                errorText.Visibility = Visibility.Visible;
                return;
            }
            if (isAllIncomesCategories.IsChecked == false && incomesCategoriesCB.SelectedItem == null)
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
            #endregion

            using (PFContext db = new PFContext())
            {
                incomesCollection = new ObservableCollection<Income>(db.Income.Include(x => x.SourceOfIncome)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());
            }

            if (isAllPurse.IsChecked != true)
            {
                Purse purseComboBox = purseListCB.SelectedItem as Purse;
                foreach (Income c in incomesCollection.ToList())
                {
                    if (c.PurseId != purseComboBox.Id)
                    {
                        listIdForDelete.Add(c.Id);
                    }
                }
                /* Удаление не нужных счетов */
                for (int i = 0; i < listIdForDelete.Count; i++)
                {
                    for (int j = 0; j < incomesCollection.Count; j++)
                    {
                        if (listIdForDelete[i] == incomesCollection[j].Id)
                        {
                            incomesCollection.RemoveAt(j);
                        }
                    }
                }
                listIdForDelete.Clear();
            }
            if (isAllIncomesCategories.IsChecked != true)
            {
                SourceOfIncome incomeCategoriesComboBox = incomesCategoriesCB.SelectedItem as SourceOfIncome;
                foreach (Income c in incomesCollection.ToList())
                {
                    if (c.SourceOfIncomeId != incomeCategoriesComboBox.Id)
                    {
                        listIdForDelete.Add(c.Id);
                    }
                }
                /* Удаление не нужных категорий */
                for (int i = 0; i < listIdForDelete.Count; i++)
                {
                    for (int j = 0; j < incomesCollection.Count; j++)
                    {
                        if (listIdForDelete[i] == incomesCollection[j].Id)
                        {
                            incomesCollection.RemoveAt(j);
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

                foreach (Income c in incomesCollection.ToList())
                {
                    if (Convert.ToDateTime(c.DateOperation) < sDate || Convert.ToDateTime(c.DateOperation) > fDate)
                    {
                        listIdForDelete.Add(c.Id);
                    }
                }
                /* Удаление не нужных дат */
                for (int i = 0; i < listIdForDelete.Count; i++)
                {
                    for (int j = 0; j < incomesCollection.Count; j++)
                    {
                        if (listIdForDelete[i] == incomesCollection[j].Id)
                        {
                            incomesCollection.RemoveAt(j);
                        }
                    }
                }
                listIdForDelete.Clear();
            }

            if (!SortIncomeCollection(incomesCollection))
            {
                incomeList.ItemsSource = incomesCollection;
            }
            amountSumma.Text = "Сумма доходов: " + GetTotalAmount(incomesCollection).ToString() + " BYN";
            ChangeCollectionForDiagramm(incomesCollection);
            filterFlyoutMenu.Hide();
        }

        private void RefreshErrorText(object sender, object e)
        {
            if (errorText.Visibility == Visibility.Visible)
            {
                errorText.Visibility = Visibility.Collapsed;
                amountSumma.Visibility = Visibility.Visible;
            }
        }

        private void ChangeCollectionForDiagramm(ObservableCollection<Income> collect)
        {
            ObservableCollection<Income> incomeCollectionCopy = new ObservableCollection<Income>(collect);
            var noduplicates = incomeCollectionCopy.Distinct();

            incomesStructListNoDuplicates = new List<IncomesStruct>();
            incomesStructListAll = new List<IncomesStruct>();

            /* не повторяющиеся */
            foreach (Income c in noduplicates)
            {
                IncomesStruct st = new IncomesStruct();
                st.Id = c.Id;
                st.Summa = c.Summa;
                st.DateOperation = c.DateOperation;
                st.PurseId = c.PurseId;
                st.IncomeCategoriesId = c.SourceOfIncomeId;
                st.IncomeCategoriesName = c.SourceOfIncome.Name;
                st.Comment = c.Comment;
                st.CurrencyId = c.CurrencyId;
                st.CurrencyRate = c.Currency.Rate;
                st.CurrencyScale = c.Currency.CurScale;

                incomesStructListNoDuplicates.Add(st);
            }
            /* Все */
            foreach (Income c in incomeCollectionCopy)
            {
                IncomesStruct st = new IncomesStruct();
                st.Id = c.Id;
                st.Summa = c.Summa;
                st.DateOperation = c.DateOperation;
                st.PurseId = c.PurseId;
                st.IncomeCategoriesId = c.SourceOfIncomeId;
                st.IncomeCategoriesName = c.SourceOfIncome.Name;
                st.Comment = c.Comment;
                st.CurrencyId = c.CurrencyId;
                st.CurrencyRate = c.Currency.Rate;
                st.CurrencyScale = c.Currency.CurScale;

                incomesStructListAll.Add(st);
            }

            //не повторяющие в BYN
            for (int i = 0; i < incomesStructListNoDuplicates.Count; i++)
            {
                double summa = 0;
                summa = incomesStructListNoDuplicates[i].Summa * incomesStructListNoDuplicates[i].CurrencyRate / incomesStructListNoDuplicates[i].CurrencyScale;
                incomesStructListNoDuplicates[i] = new IncomesStruct(incomesStructListNoDuplicates[i].Id, summa, incomesStructListNoDuplicates[i].DateOperation,
                    incomesStructListNoDuplicates[i].PurseId, incomesStructListNoDuplicates[i].IncomeCategoriesId, incomesStructListNoDuplicates[i].IncomeCategoriesName, incomesStructListNoDuplicates[i].Comment,
                    incomesStructListNoDuplicates[i].CurrencyId, incomesStructListNoDuplicates[i].CurrencyRate, incomesStructListNoDuplicates[i].CurrencyScale);
            }
            /* Все в BYN */
            for (int i = 0; i < incomesStructListAll.Count; i++)
            {
                double summa = 0;
                summa = incomesStructListAll[i].Summa * incomesStructListAll[i].CurrencyRate / incomesStructListAll[i].CurrencyScale;
                incomesStructListAll[i] = new IncomesStruct(incomesStructListAll[i].Id, summa, incomesStructListAll[i].DateOperation,
                   incomesStructListAll[i].PurseId, incomesStructListAll[i].IncomeCategoriesId, incomesStructListAll[i].IncomeCategoriesName, incomesStructListAll[i].Comment,
                   incomesStructListAll[i].CurrencyId, incomesStructListAll[i].CurrencyRate, incomesStructListAll[i].CurrencyScale);
            }

            for (int i = 0; i < incomesStructListNoDuplicates.Count; i++)
            {
                for (int j = 0; j < incomesStructListAll.Count; j++)
                {
                    if (incomesStructListNoDuplicates[i].Id != incomesStructListAll[j].Id)
                    {
                        if (incomesStructListNoDuplicates[i].IncomeCategoriesId == incomesStructListAll[j].IncomeCategoriesId)
                        {
                            double tempSumma = incomesStructListAll[j].Summa + incomesStructListNoDuplicates[i].Summa;
                            incomesStructListNoDuplicates[i] = new IncomesStruct(incomesStructListNoDuplicates[i].Id, tempSumma, incomesStructListNoDuplicates[i].DateOperation,
                                   incomesStructListNoDuplicates[i].PurseId, incomesStructListNoDuplicates[i].IncomeCategoriesId, incomesStructListNoDuplicates[i].IncomeCategoriesName, incomesStructListNoDuplicates[i].Comment,
                                   incomesStructListNoDuplicates[i].CurrencyId, incomesStructListNoDuplicates[i].CurrencyRate, incomesStructListNoDuplicates[i].CurrencyScale);
                        }
                    }
                }
            }
            (PieChartIncome.Series[0] as PieSeries).ItemsSource = incomesStructListNoDuplicates;
            (ColumnChartIncome.Series[0] as ColumnSeries).ItemsSource = incomesStructListNoDuplicates;
        }
        private double GetTotalAmount(ObservableCollection<Income> cc)
        {
            double sum = 0;
            
            foreach (Income c in cc)
            {
                double tempVar = c.Summa * c.Currency.Rate / c.Currency.CurScale;
                sum += tempVar;
            }
            sum = Math.Round(sum, 2);
            return sum;
        }

        #region ShowFlyoutMenu
        private void SortIncome_Click(object sender, RoutedEventArgs e)
        {
            sortFlyotMenu.ShowAt((AppBarButton)sender);
        }

        private void BtnShowFilters_Click(object sender, RoutedEventArgs e)
        {
            filterFlyoutMenu.ShowAt((AppBarButton)sender);
        }
        #endregion

        #region SortsButton
        private void sortIncomesByDateDesc_Click(object sender, RoutedEventArgs e)
        {
            SortIncomeDateDesc(incomesCollection);
        }

        private void sortIncomesByDate_Click(object sender, RoutedEventArgs e)
        {
            SortIncomeDate(incomesCollection);
        }

        private void sortIncomesBySymmaDesc_Click(object sender, RoutedEventArgs e)
        {
            SortIncomeSummaDesc(incomesCollection);
        }

        private void sortIncomesBySumma_Click(object sender, RoutedEventArgs e)
        {
            SortIncomeSumma(incomesCollection);
        }
        #endregion
        #region Sort Functions
        private bool SortIncomeCollection(ObservableCollection<Income> c)
        {
            localSettings = ApplicationData.Current.LocalSettings;
            object value = localSettings.Values["sortIncomeSetting"];

            if (value != null)
            {
                if (value.ToString() == "byDateDesc")
                {
                    /* По убыванию (дата) */
                    SortIncomeDateDesc(c);
                    return true;
                }
                else if (value.ToString() == "byDate")
                {
                    /* По возростанию (дата) */
                    SortIncomeDate(c);
                    return true;
                }
                else if (value.ToString() == "bySummaDesc")
                {
                    /* По убыванию (сумма) */
                    SortIncomeSummaDesc(c);
                    return true;
                }
                else if (value.ToString() == "bySumma")
                {
                    /* По возростанию (сумма) */
                    SortIncomeSumma(c);
                    return true;
                }
                return false;
            }
            return false;
        }
        private void SortIncomeSumma(ObservableCollection<Income> iCollection)
        {
            var sortCosts = from i in iCollection
                            orderby i.Summa * i.Currency.Rate / i.Currency.CurScale
                            select i;
            incomeList.ItemsSource = sortCosts;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortIncomeSetting"] = "bySumma";

            sortIncomesByDateDesc.IsChecked = false;
            sortIncomesByDate.IsChecked = false;
            sortIncomesBySymmaDesc.IsChecked = false;
            sortIncomesBySumma.IsChecked = true;
        }
        private void SortIncomeSummaDesc(ObservableCollection<Income> iCollection)
        {
            var sortCosts = from i in iCollection
                            orderby i.Summa * i.Currency.Rate / i.Currency.CurScale descending
                            select i;
            incomeList.ItemsSource = sortCosts;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortIncomeSetting"] = "bySummaDesc";

            sortIncomesByDateDesc.IsChecked = false;
            sortIncomesByDate.IsChecked = false;
            sortIncomesBySymmaDesc.IsChecked = true;
            sortIncomesBySumma.IsChecked = false;
        }
        private void SortIncomeDate(ObservableCollection<Income> iCollection)
        {
            var sortIncome = from c in iCollection
                             orderby Convert.ToDateTime(c.DateOperation)
                             select c;
            incomeList.ItemsSource = sortIncome;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortIncomeSetting"] = "byDate";

            sortIncomesByDateDesc.IsChecked = false;
            sortIncomesByDate.IsChecked = true;
            sortIncomesBySymmaDesc.IsChecked = false;
            sortIncomesBySumma.IsChecked = false;
        }
        private void SortIncomeDateDesc(ObservableCollection<Income> iCollection)
        {
            var sortIncome = from c in iCollection
                             orderby Convert.ToDateTime(c.DateOperation) descending
                             select c;
            incomeList.ItemsSource = sortIncome;

            localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["sortIncomeSetting"] = "byDateDesc";

            sortIncomesByDateDesc.IsChecked = true;
            sortIncomesByDate.IsChecked = false;
            sortIncomesBySymmaDesc.IsChecked = false;
            sortIncomesBySumma.IsChecked = false;
        }
        #endregion

        #region SettingsFilterMenu
        private void purseListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Purse purseElemComboBox = purseListCB.SelectedItem as Purse;
            if (purseElemComboBox != null)
            {
                isAllPurse.IsChecked = false;
            }
        }
        private void incomesCategoriesCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SourceOfIncome incomeCategoriesElement = incomesCategoriesCB.SelectedItem as SourceOfIncome;
            if (incomeCategoriesElement != null)
            {
                isAllIncomesCategories.IsChecked = false;
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
        private void isAllIncomesCategories_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Convert.ToBoolean(isAllIncomesCategories.IsChecked))
            {
                incomesCategoriesCB.ItemsSource = null;
                incomesCategoriesCB.ItemsSource = incomesCategoriesCollection;
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
            if (dateStart.Date != null)
            {
                isAllDate.IsChecked = false;
            }
        }
        private void datefinish_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (datefinish.Date != null)
            {
                isAllDate.IsChecked = false;
            }
        }
        #endregion

    }
}
