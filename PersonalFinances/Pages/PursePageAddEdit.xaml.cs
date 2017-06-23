using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class PursePageAddEdit : Page
    {
        ObservableCollection<Purse> purseCollection;
        List<Currency> currency;
        Purse purse;
        Currency curSealed;
        int id;
        public PursePageAddEdit()
        {
            this.InitializeComponent();
            this.Loaded += PursePageAddEdit_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                id = (int)e.Parameter;
                using(PFContext db = new PFContext())
                {
                    purse = db.Purse.FirstOrDefault(c => c.Id == id);
                }
            }

            if(purse != null)
            {
                Purse p;
                headerBlock.Text = "Редактировать счет";

                purseNameBox.Text = purse.Name;
                purseBalanceBox.Text = purse.Balance.ToString();
                using (PFContext db = new PFContext())
                {
                    purseCollection = new ObservableCollection<Purse>(db.Purse.Include(x => x.Currency).ToList());
                    p = purseCollection.FirstOrDefault(x => x.Id == id);
                    curSealed = p.Currency;
                }
            }
        }
        private void PursePageAddEdit_Loaded(object sender, RoutedEventArgs e)
        {
            using (PFContext db = new PFContext())
            {
                currency = db.Currency.ToList();
            }
            currencyList.ItemsSource = currency;
            // for edit
            if(curSealed != null)
            {
                currencyList.SelectedValue = curSealed.Id;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string value = ConvertToStringFormat(purseBalanceBox.Text);
            double balance;
            Currency cur = currencyList.SelectedItem as Currency;
           
            if (purseNameBox.Text.Length == 0)
            {
                errorText.Text = "Введите имя счета";
                return;
            }
            if (!Double.TryParse(value, out balance))
            {
                errorText.Text = "Некоректный баланс";
                return;
            }
            if (cur == null)
            {
                errorText.Text = "Выберите валюту";
                return;
            }

            using (PFContext db = new PFContext())
            {

                if (purse != null)
                {
                    purse.Name = purseNameBox.Text;
                    purse.Balance = balance;
                    purse.CurrencyId = cur.Id;
                    purse.Currency = cur;
                    db.Purse.Update(purse);
                }
                else
                {
                    Purse purseNew = new Purse
                    {
                        Name = purseNameBox.Text,
                        Balance = balance,
                        Currency = cur,
                        CurrencyId = cur.Id
                    };
                    db.Currency.Attach(cur);
                    db.Purse.Add(purseNew);
                }
                db.SaveChanges();
            }
            GoToPreviousPage();
        }

        private void GoToPreviousPage()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(PursePage));
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
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
