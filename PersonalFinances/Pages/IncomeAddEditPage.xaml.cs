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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class IncomeAddEditPage : Page
    {
        int id;
        Income income;
        Purse purse;
        Purse purseSealed;
        SourceOfIncome sourceOfIncomeSealed;
        Currency currency;

        ObservableCollection<Income> incomeCollection;
        public IncomeAddEditPage()
        {
            this.InitializeComponent();
            this.Loaded += IncomeAddEditPage_Loaded;
        }

        private void IncomeAddEditPage_Loaded(object sender, RoutedEventArgs e)
        {
            using(PFContext db = new PFContext())
            {
                purseListCB.ItemsSource = db.Purse.ToList();
                incomeCategorListCB.ItemsSource = db.SourceOfIncome.ToList();
            }
            if(purseSealed != null && sourceOfIncomeSealed != null)
            {
                purseListCB.SelectedValue = purseSealed.Id;
                incomeCategorListCB.SelectedValue = sourceOfIncomeSealed.Id;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                id = (int)e.Parameter;
                using(PFContext db = new PFContext())
                {
                    income = db.Income.FirstOrDefault(i => i.Id == id);
                }
            }

            if(income != null)
            {
                Income i;
                headerBlock.Text = "Редактировать доход";

                incomeSum.Text = income.Summa.ToString();
                dateIncome.Date = Convert.ToDateTime(income.DateOperation);
                incomeComment.Text = income.Comment;

                // ComboBox
                using(PFContext db = new PFContext())
                {
                    incomeCollection = new ObservableCollection<Income>(db.Income.Include(x => x.Purse)
                        .Include(y => y.SourceOfIncome).ToList());
                    i = incomeCollection.FirstOrDefault(x => x.Id == id);
                    purseSealed = i.Purse;
                    sourceOfIncomeSealed = i.SourceOfIncome;
                }
            }
            else
            {
                dateIncome.Date = DateTime.Today; 
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Purse pList = purseListCB.SelectedItem as Purse;
            SourceOfIncome sList = incomeCategorListCB.SelectedItem as SourceOfIncome;
            double sum;

            DateTimeOffset date = (DateTimeOffset)dateIncome.Date;
            string dateFormat = date.Date.ToString("dd.MM.yyyy");

            if (pList == null)
            {
                errorText.Text = "Выберите счет";
                return;
            }
            if(!Double.TryParse(ConvertToStringFormat(incomeSum.Text), out sum))
            {
                errorText.Text = "Некоректная сумма";
                return;
            }
            if (sum < 0)
            {
                errorText.Text = "Некоректная сумма";
                return;
            }
            if (sList == null)
            {
                errorText.Text = "Выберите категорию";
                return;
            }
            //if (incomeComment.Text.Length == 0)
            //{
            //    incomeComment.Text = " ";
            //}

            using(PFContext db = new PFContext())
            {
                if(income != null)
                {
                    int purseID_Old = income.PurseId;
                    double summaIncom_Old = income.Summa;
                    currency = db.Currency.FirstOrDefault(c => c.Id == pList.CurrencyId);

                    if(pList.Id == purseID_Old && summaIncom_Old == sum) // неизменный кошелек и сумма
                    {
                        income.Summa = sum;
                        income.DateOperation = dateFormat;
                        income.PurseId = pList.Id;
                        income.SourceOfIncomeId = sList.Id;
                        income.CurrencyId = pList.CurrencyId;
                        income.Comment = incomeComment.Text;
                        income.Purse = pList;
                        income.SourceOfIncome = sList;
                        income.Currency = currency;

                    }
                    else if(pList.Id == purseID_Old && summaIncom_Old != sum) // изменяется только сумма
                    {
                        Purse purseChangeSumElement = db.Purse.FirstOrDefault(p => p.Id == pList.Id);
                        purseChangeSumElement.Balance = purseChangeSumElement.Balance + (sum - summaIncom_Old);
                        db.Purse.Update(purseChangeSumElement);

                        income.Summa = sum;
                        income.DateOperation = dateFormat;
                        income.PurseId = pList.Id;
                        income.SourceOfIncomeId = sList.Id;
                        income.CurrencyId = pList.CurrencyId;
                        income.Comment = incomeComment.Text;
                        income.Purse = pList;
                        income.SourceOfIncome = sList;
                        income.Currency = currency;
                    }
                    else if (pList.Id != purseID_Old)  //изменяется кошелек
                    {
                        Purse purseChangeOld;
                        Purse purseChangeNew;

                        purseChangeOld = db.Purse.FirstOrDefault(p => p.Id == purseID_Old);
                        purseChangeOld.Balance = purseChangeOld.Balance - summaIncom_Old;
                        db.Purse.Update(purseChangeOld);

                        purseChangeNew = db.Purse.FirstOrDefault(p => p.Id == pList.Id);
                        purseChangeNew.Balance = purseChangeNew.Balance + sum;
                        db.Purse.Update(purseChangeNew);

                        income.Summa = sum;
                        income.DateOperation = dateFormat;
                        income.PurseId = pList.Id;
                        income.SourceOfIncomeId = sList.Id;
                        income.CurrencyId = pList.CurrencyId;
                        income.Comment = incomeComment.Text;
                        income.Purse = pList;
                        income.SourceOfIncome = sList;
                        income.Currency = currency;

                    }
                    db.Income.Update(income);
                }
                else
                {
                    currency = db.Currency.FirstOrDefault(c => c.Id == pList.CurrencyId);
                    Income incomeNew = new Income
                    {
                        Summa = sum,
                        DateOperation = dateFormat,
                        PurseId = pList.Id,
                        SourceOfIncomeId = sList.Id,
                        Comment = incomeComment.Text,
                        Purse = pList,
                        SourceOfIncome = sList,
                        CurrencyId = pList.CurrencyId,
                        Currency = currency
                    };
                    db.Purse.Attach(pList);
                    db.SourceOfIncome.Attach(sList);
                    db.Currency.Attach(currency);
                    /* Update Purse */
                    purse = db.Purse.FirstOrDefault(p => p.Id == incomeNew.PurseId);
                    purse.Balance = purse.Balance + incomeNew.Summa;
                    db.Purse.Update(purse);

                    db.Income.Add(incomeNew);
                }
                db.SaveChanges();
            }
            GoToPreviousPage();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }

        private void purseListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Purse purseElemComboBox = purseListCB.SelectedItem as Purse;
            Currency currencyForCB;
            if (purseElemComboBox != null)
            {
                using (PFContext db = new PFContext())
                {
                    currencyForCB = db.Currency.FirstOrDefault(c => c.Id == purseElemComboBox.CurrencyId);
                }
                curAbbrev.Text = currencyForCB.Abbreviation;
            }
        }

        private void addPurseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PursePageAddEdit));
        }

        private void addIncomeCategorButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SourceOfIncomeAddEditPage));
        }

        private void GoToPreviousPage()
        {
            if (Frame.CanGoBack)
                Frame.Navigate(typeof(OperationsPage), 1);
            else
                Frame.Navigate(typeof(OperationsPage));
        }
        public string ConvertToStringFormat(string s)
        {
            char[] symbols = s.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                if (symbols[i] == '.')
                {
                    symbols[i] = ',';
                }
            }
            string str = new string(symbols);

            return str;
        }
    }
}
