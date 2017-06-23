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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public class CostsIncomeStatistic
    {
        public string Name { get; set; }
        public double Summa { get; set; }
    }
    public sealed partial class ReportCostsIncomesPage : Page
    {
        ObservableCollection<Purse> purseCollection;
        ObservableCollection<Costs> costsCollection;
        ObservableCollection<Income> incomesCollection;
        List<CostsStruct> costsStructListNoDuplicates;
        List<CostsStruct> costsStructListAll;
        List<IncomesStruct> incomesStructListNoDuplicates;
        List<IncomesStruct> incomesStructListAll;

        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        public ReportCostsIncomesPage()
        {
            this.InitializeComponent();
            this.Loaded += ReportCostsIncomesPage_Loaded;
            dt.Tick += RefreshErrorText;
            dt.Start();
        }

        private void RefreshErrorText(object sender, object e)
        {
            if (errorText.Visibility == Visibility.Visible)
            {
                errorText.Visibility = Visibility.Collapsed;
                amountSumma.Visibility = Visibility.Visible;
            }
        }

        private void ReportCostsIncomesPage_Loaded(object sender, RoutedEventArgs e)
        {
            // для того, что-бы выбиралось из comboBox в flyoutMenu
            if (Windows.Foundation.Metadata.ApiInformation
                .IsPropertyPresent("Windows.UI.Xaml.FrameworkElement", "AllowFocusOnInteraction"))
            {
                purseListCB.AllowFocusOnInteraction = true;
            }

            using(PFContext db = new PFContext())
            {
                purseCollection = new ObservableCollection<Purse>(db.Purse.ToList());
                costsCollection = new ObservableCollection<Costs>(db.Costs.Include(x => x.Purse)
                    .Include(x => x.Currency).Include(x=> x.CostCategories).ToList());
                incomesCollection = new ObservableCollection<Income>(db.Income.Include(x => x.Purse)
                    .Include(x => x.Currency).Include(x=>x.SourceOfIncome).ToList());
            }
            purseListCB.ItemsSource = purseCollection;

            ChangeCollectionForDiagramm(costsCollection, incomesCollection);

            amountSumma.Text = "Доходы - расходы : " 
                + (GetSumma(incomesStructListNoDuplicates) - GetSumma(costsStructListNoDuplicates)).ToString()
                + " BYN";

            isAllPurse.IsChecked = true;
            isAllDate.IsChecked = true;
        }
        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            List<int> listIdForDeleteCosts = new List<int>();
            List<int> listIdForDeleteIncomes = new List<int>();
            #region Validation
            if (isAllPurse.IsChecked == false && purseListCB.SelectedItem == null)
            {
                errorText.Text = "Не выбран счет";
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
                costsCollection = new ObservableCollection<Costs>(db.Costs.Include(x => x.CostCategories)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());
                incomesCollection = new ObservableCollection<Income>(db.Income.Include(x => x.SourceOfIncome)
                    .Include(x => x.Purse).Include(x => x.Currency).ToList());
            }
            if (isAllPurse.IsChecked != true)
            {
                Purse purseComboBox = purseListCB.SelectedItem as Purse;
                foreach (Costs c in costsCollection.ToList())
                {
                    if (c.PurseId != purseComboBox.Id)
                    {
                        listIdForDeleteCosts.Add(c.Id);
                    }
                }
                foreach (Income c in incomesCollection.ToList())
                {
                    if (c.PurseId != purseComboBox.Id)
                    {
                        listIdForDeleteIncomes.Add(c.Id);
                    }
                }
                /* Удаление не нужных счетов */
                for (int i = 0; i < listIdForDeleteCosts.Count; i++)
                {
                    for (int j = 0; j < costsCollection.Count; j++)
                    {
                        if (listIdForDeleteCosts[i] == costsCollection[j].Id)
                        {
                            costsCollection.RemoveAt(j);
                        }
                    }
                }
                for (int i = 0; i < listIdForDeleteIncomes.Count; i++)
                {
                    for (int j = 0; j < incomesCollection.Count; j++)
                    {
                        if (listIdForDeleteIncomes[i] == incomesCollection[j].Id)
                        {
                            incomesCollection.RemoveAt(j);
                        }
                    }
                }
                listIdForDeleteCosts.Clear();
                listIdForDeleteIncomes.Clear();
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
                        listIdForDeleteCosts.Add(c.Id);
                    }
                }
                foreach (Income c in incomesCollection.ToList())
                {
                    if (Convert.ToDateTime(c.DateOperation) < sDate || Convert.ToDateTime(c.DateOperation) > fDate)
                    {
                        listIdForDeleteIncomes.Add(c.Id);
                    }
                }
                /* Удаление не нужных дат */
                for (int i = 0; i < listIdForDeleteCosts.Count; i++)
                {
                    for (int j = 0; j < costsCollection.Count; j++)
                    {
                        if (listIdForDeleteCosts[i] == costsCollection[j].Id)
                        {
                            costsCollection.RemoveAt(j);
                        }
                    }
                }
                for (int i = 0; i < listIdForDeleteIncomes.Count; i++)
                {
                    for (int j = 0; j < incomesCollection.Count; j++)
                    {
                        if (listIdForDeleteIncomes[i] == incomesCollection[j].Id)
                        {
                            incomesCollection.RemoveAt(j);
                        }
                    }
                }

                listIdForDeleteCosts.Clear();
                listIdForDeleteIncomes.Clear();
            }
            ChangeCollectionForDiagramm(costsCollection, incomesCollection);

            amountSumma.Text = "Доходы - расходы : " 
                + (GetSumma(incomesStructListNoDuplicates) - GetSumma(costsStructListNoDuplicates)).ToString()
                + " BYN";

            filterFlyoutMenu.Hide();
        }
        private void ChangeCollectionForDiagramm(ObservableCollection<Costs> cCollect, ObservableCollection<Income> iCollect)
        {
            var noduplicatesCosts = cCollect.Distinct();
            var noduplicatesIncomes = iCollect.Distinct();

            costsStructListNoDuplicates = new List<CostsStruct>();
            costsStructListAll = new List<CostsStruct>();
            incomesStructListNoDuplicates = new List<IncomesStruct>();
            incomesStructListAll = new List<IncomesStruct>();

            /* не повторяющиеся */
            foreach (Costs c in noduplicatesCosts)
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
            foreach (Income c in noduplicatesIncomes)
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
            foreach (Costs c in cCollect)
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
            foreach (Income c in iCollect)
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
            for (int i = 0; i < costsStructListNoDuplicates.Count; i++)
            {
                double summa = 0;
                summa = costsStructListNoDuplicates[i].Summa * costsStructListNoDuplicates[i].CurrencyRate / costsStructListNoDuplicates[i].CurrencyScale;
                costsStructListNoDuplicates[i] = new CostsStruct(costsStructListNoDuplicates[i].Id, summa, costsStructListNoDuplicates[i].DateOperation,
                    costsStructListNoDuplicates[i].PurseId, costsStructListNoDuplicates[i].CostCategoriesId, costsStructListNoDuplicates[i].CostCategoriesName, costsStructListNoDuplicates[i].Comment,
                    costsStructListNoDuplicates[i].CurrencyId, costsStructListNoDuplicates[i].CurrencyRate, costsStructListNoDuplicates[i].CurrencyScale);
            }
            for (int i = 0; i < incomesStructListNoDuplicates.Count; i++)
            {
                double summa = 0;
                summa = incomesStructListNoDuplicates[i].Summa * incomesStructListNoDuplicates[i].CurrencyRate / incomesStructListNoDuplicates[i].CurrencyScale;
                incomesStructListNoDuplicates[i] = new IncomesStruct(incomesStructListNoDuplicates[i].Id, summa, incomesStructListNoDuplicates[i].DateOperation,
                    incomesStructListNoDuplicates[i].PurseId, incomesStructListNoDuplicates[i].IncomeCategoriesId, incomesStructListNoDuplicates[i].IncomeCategoriesName, incomesStructListNoDuplicates[i].Comment,
                    incomesStructListNoDuplicates[i].CurrencyId, incomesStructListNoDuplicates[i].CurrencyRate, incomesStructListNoDuplicates[i].CurrencyScale);
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
            for (int i = 0; i < incomesStructListAll.Count; i++)
            {
                double summa = 0;
                summa = incomesStructListAll[i].Summa * incomesStructListAll[i].CurrencyRate / incomesStructListAll[i].CurrencyScale;
                incomesStructListAll[i] = new IncomesStruct(incomesStructListAll[i].Id, summa, incomesStructListAll[i].DateOperation,
                   incomesStructListAll[i].PurseId, incomesStructListAll[i].IncomeCategoriesId, incomesStructListAll[i].IncomeCategoriesName, incomesStructListAll[i].Comment,
                   incomesStructListAll[i].CurrencyId, incomesStructListAll[i].CurrencyRate, incomesStructListAll[i].CurrencyScale);
            }

            /* Сумирование одинаковых категорий */
            for (int i = 0; i < costsStructListNoDuplicates.Count; i++)
            {
                for (int j = 0; j < costsStructListAll.Count; j++)
                {
                    if (costsStructListNoDuplicates[i].Id != costsStructListAll[j].Id)
                    {
                        if (costsStructListNoDuplicates[i].CostCategoriesId == costsStructListAll[j].CostCategoriesId)
                        {
                            double tempSumma = costsStructListAll[j].Summa + costsStructListNoDuplicates[i].Summa;
                            costsStructListNoDuplicates[i] = new CostsStruct(costsStructListNoDuplicates[i].Id, tempSumma, costsStructListNoDuplicates[i].DateOperation,
                                costsStructListNoDuplicates[i].PurseId, costsStructListNoDuplicates[i].CostCategoriesId, costsStructListNoDuplicates[i].CostCategoriesName, costsStructListNoDuplicates[i].Comment,
                                costsStructListNoDuplicates[i].CurrencyId, costsStructListNoDuplicates[i].CurrencyRate, costsStructListNoDuplicates[i].CurrencyScale);
                        }
                    }
                }
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
            List<CostsIncomeStatistic> statistic = new List<CostsIncomeStatistic>();
            statistic.Add(new CostsIncomeStatistic() { Name = "Расходы", Summa = GetSumma(costsStructListNoDuplicates) });
            statistic.Add(new CostsIncomeStatistic() { Name = "Доходы", Summa = GetSumma(incomesStructListNoDuplicates) });
            (PieChart.Series[0] as PieSeries).ItemsSource = statistic;
            (ColumnChart.Series[0] as ColumnSeries).ItemsSource = statistic;

        }

        private double GetSumma(List<CostsStruct> cst)
        {
            double summa = 0;
            for(int i = 0; i < cst.Count; i++)
            {
                summa += cst[i].Summa;
            }
            return summa;
        }
        private double GetSumma(List<IncomesStruct> ist)
        {
            double summa = 0;
            for (int i = 0; i < ist.Count; i++)
            {
                summa += ist[i].Summa;
            }
            return summa;
        }
        private void BtnShowFilters_Click(object sender, RoutedEventArgs e)
        {
            filterFlyoutMenu.ShowAt((AppBarButton)sender);
        }

        private void purseListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Purse purseElemComboBox = purseListCB.SelectedItem as Purse;
            if (purseElemComboBox != null)
            {
                isAllPurse.IsChecked = false;
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
        private void isAllPurse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Convert.ToBoolean(isAllPurse.IsChecked))
            {
                purseListCB.ItemsSource = null;
                purseListCB.ItemsSource = purseCollection;
            }
        }
    }
}
