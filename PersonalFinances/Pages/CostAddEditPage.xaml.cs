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
using Windows.UI.ViewManagement;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CostAddEditPage : Page
    {
        ObservableCollection<Costs> costsCollection;
        ObservableCollection<Purse> purseCollection;
        Costs cost;
        CostCategories costCategoriesSealed;
        Purse purse;
        Purse purseSealed;
        Currency currency;
        int id;
        public CostAddEditPage()
        {
            this.InitializeComponent();
            this.Loaded += CostAddEdit_Loaded;
        }

        private void CostAddEdit_Loaded(object sender, RoutedEventArgs e)
        {
            using(PFContext db = new PFContext())
            {
                purseListCB.ItemsSource = db.Purse.ToList();
                costCategorListCB.ItemsSource = db.CostCategories.ToList();
            }
            if(purseSealed != null && costCategoriesSealed != null)
            {
                purseListCB.SelectedValue = purseSealed.Id;
                costCategorListCB.SelectedValue = costCategoriesSealed.Id;
                //curAbbrev.Text = 
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                id = (int)e.Parameter;
                using(PFContext db = new PFContext())
                {
                    cost = db.Costs.FirstOrDefault(c => c.Id == id);
                }
            }

            if(cost != null)
            {
                Costs c;
                headerBlock.Text = "Редактировать платеж";

                costSum.Text = cost.Summa.ToString();
                dateCost.Date = Convert.ToDateTime(cost.DateOperation);
                costComment.Text = cost.Comment;
                // ComboBox
                using (PFContext db = new PFContext())
                {
                    costsCollection = new ObservableCollection<Costs>(db.Costs.Include(x => x.Purse)
                        .Include(y => y.CostCategories).ToList());
                    //purseCollection = new ObservableCollection<Purse>(db.Purse)

                    c = costsCollection.FirstOrDefault(x => x.Id == id);
                    purseSealed = c.Purse;
                    costCategoriesSealed = c.CostCategories;
                }
            }
            else
            {
                dateCost.Date = DateTime.Today;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Purse pList = purseListCB.SelectedItem as Purse;
            CostCategories ccList = costCategorListCB.SelectedItem as CostCategories;
            double sum;

            DateTimeOffset date = (DateTimeOffset)dateCost.Date;
            string dateFormat = date.Date.ToString("dd.MM.yyyy");


            if(pList == null)
            {
                errorText.Text = "Выберите счет";
                return;
            }
            if(!Double.TryParse(ConvertToStringFormat(costSum.Text),out sum))
            {
                errorText.Text = "Некоректная сумма";
                return;
            }
            if(sum < 0)
            {
                errorText.Text = "Некоректная сумма";
                return;
            }
            if(ccList == null)
            {
                errorText.Text = "Выберите категорию";
                return;
            }
            //if(costComment.Text.Length == 0)
            //{
            //    costComment.Text = " ";
            //}
            //dateFormat = dateFormat.ToString("");
            using(PFContext db = new PFContext())
            {
                if(cost != null)
                {
                    int purseId_Old = cost.PurseId;
                    double summaCost_Old = cost.Summa;
                    currency = db.Currency.FirstOrDefault(c => c.Id == pList.CurrencyId);

                    if(pList.Id == purseId_Old && summaCost_Old == sum) // неизменный кошелек и сумма
                    {
                        cost.Summa = sum;
                        cost.DateOperation = dateFormat;
                        cost.PurseId = pList.Id;
                        cost.CostCategoriesId = ccList.Id;
                        cost.Comment = costComment.Text;
                        cost.Purse = pList;
                        cost.CostCategories = ccList;
                        cost.CurrencyId = pList.CurrencyId;
                        cost.Currency = currency;
                    }
                    else if(pList.Id == purseId_Old && summaCost_Old != sum) // изменяется только сумма
                    {
                        Purse purseChangeSumElement = db.Purse.FirstOrDefault(p => p.Id == pList.Id);
                        purseChangeSumElement.Balance = purseChangeSumElement.Balance - (sum - summaCost_Old);
                        db.Purse.Update(purseChangeSumElement);

                        cost.Summa = sum;
                        cost.DateOperation = dateFormat;
                        cost.PurseId = pList.Id;
                        cost.CostCategoriesId = ccList.Id;
                        cost.Comment = costComment.Text;
                        cost.Purse = pList;
                        cost.CostCategories = ccList;
                        cost.CurrencyId = pList.CurrencyId;
                        cost.Currency = currency;
                    }    
                    
                    else if(pList.Id != purseId_Old) //изменяется кошелек
                    {
                        Purse purseChangeOld;
                        Purse purseChangeNew;

                        purseChangeOld = db.Purse.FirstOrDefault(p => p.Id == purseId_Old);
                        purseChangeOld.Balance = purseChangeOld.Balance + summaCost_Old;
                        db.Purse.Update(purseChangeOld);

                        purseChangeNew = db.Purse.FirstOrDefault(p => p.Id == pList.Id);
                        purseChangeNew.Balance = purseChangeNew.Balance - sum;
                        db.Purse.Update(purseChangeNew);

                        cost.Summa = sum;
                        cost.DateOperation = dateFormat;
                        cost.PurseId = pList.Id;
                        cost.CostCategoriesId = ccList.Id;
                        cost.Comment = costComment.Text;
                        cost.Purse = pList;
                        cost.CostCategories = ccList;
                        cost.CurrencyId = pList.CurrencyId;
                        cost.Currency = currency;


                    }

                    db.Costs.Update(cost);

                }
                else
                {
                    currency = db.Currency.FirstOrDefault(c => c.Id == pList.CurrencyId);
                    Costs costNew = new Costs
                    {
                        Summa = sum,
                        DateOperation = dateFormat,
                        PurseId = pList.Id,
                        CostCategoriesId = ccList.Id,
                        Comment = costComment.Text,
                        Purse = pList,
                        CostCategories = ccList,
                        CurrencyId = pList.CurrencyId,                        
                        Currency = currency
					};
                    db.Purse.Attach(pList);
                    db.CostCategories.Attach(ccList);
                    db.Currency.Attach(currency);
                    /* Update Purse*/
                    purse = db.Purse.FirstOrDefault(p => p.Id == costNew.PurseId);
                    purse.Balance = purse.Balance - costNew.Summa;
                    db.Purse.Update(purse); 
                                       
                    db.Costs.Add(costNew);
                }
                db.SaveChanges();
            }
            GoToPreviousPage();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }
        private void GoToPreviousPage()
        {
            if (Frame.CanGoBack)
                Frame.Navigate(typeof(OperationsPage), 0);
            else
                Frame.Navigate(typeof(OperationsPage));
        }

        private void addPurseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PursePageAddEdit));
        }

        private void addCostCategorButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CostCategoriesAddEditPage));
        }

        private void purseListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Purse purseElemComboBox = purseListCB.SelectedItem as Purse;
            Currency currencyForCB;
            if(purseElemComboBox != null)
            {
                using(PFContext db = new PFContext())
                {
                    currencyForCB = db.Currency.FirstOrDefault(c => c.Id == purseElemComboBox.CurrencyId);
                }
                curAbbrev.Text = currencyForCB.Abbreviation;
            }
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
