using PersonalFinances.Models;
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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AccumulationAddEditPage : Page
    {
        List<Currency> currency;
        Currency curSealed;
        Accumulation accumulation;
        int id;

        public AccumulationAddEditPage()
        {
            this.InitializeComponent();
            this.Loaded += AccumulationAddEditPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                id = (int)e.Parameter;
                using (PFContext db = new PFContext())
                {
                    accumulation = db.Accumulation.FirstOrDefault(a => a.Id == id);
                }
            }

            if(accumulation != null)
            {
                headerBlock.Text = "Редатировать цель";
                accumNameBox.Text = accumulation.Name;
                accumCurrentSum.Text = accumulation.CurrentSumma.ToString();
                accumFinalSum.Text = accumulation.FinalSumma.ToString();
                using(PFContext db = new PFContext())
                {
                    curSealed = db.Currency.FirstOrDefault(c => c.Id == accumulation.CurrencyId);
                }
            }
        }
        private void AccumulationAddEditPage_Loaded(object sender, RoutedEventArgs e)
        {
            using(PFContext db = new PFContext())
            {
                currency = db.Currency.ToList();
            }
            currencyList.ItemsSource = currency;
            //for edit
            if (curSealed != null)
            {
                currencyList.SelectedValue = curSealed.Id;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string currentSum = ConvertToStringFormat(accumCurrentSum.Text);
            string finalSum = ConvertToStringFormat(accumFinalSum.Text);
            Currency cur = currencyList.SelectedItem as Currency;
            double cSumm;
            double fSumm;

            if(accumNameBox.Text.Length == 0)
            {
                errorText.Text = "Введите имя";
                return;
            }
            if (!Double.TryParse(currentSum, out cSumm) || !Double.TryParse(finalSum, out fSumm))
            {
                errorText.Text = "Некоректная сумма";
                return;
            }
            if (cur == null)
            {
                errorText.Text = "Выберите валюту";
                return;
            }

            using(PFContext db = new PFContext())
            {
                if(accumulation != null)
                {
                    accumulation.Name = accumNameBox.Text;
                    accumulation.CurrentSumma = cSumm;
                    accumulation.FinalSumma = fSumm;
                    accumulation.Currency = cur;
                    accumulation.CurrencyId = cur.Id;

                    db.Accumulation.Update(accumulation);
                }
                else
                {
                    Accumulation accumulationNew = new Accumulation
                    {
                        Name = accumNameBox.Text,
                        CurrentSumma = cSumm,
                        FinalSumma = fSumm,
                        CurrencyId = cur.Id,
                        Currency = cur
                    };
                    db.Currency.Attach(cur);
                    db.Accumulation.Add(accumulationNew);
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
                Frame.GoBack();
            else
                Frame.Navigate(typeof(PursePage));
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
