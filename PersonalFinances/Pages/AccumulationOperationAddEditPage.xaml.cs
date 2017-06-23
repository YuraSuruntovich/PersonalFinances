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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AccumulationOperationAddEditPage : Page
    {
        int id;
        Accumulation accumulation;
        ObservableCollection<Purse> purseCollection;
        Currency curObjForCurAbrevv;
        public AccumulationOperationAddEditPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                id = (int)e.Parameter;
                using (PFContext db = new PFContext())
                {
                    accumulation = db.Accumulation.FirstOrDefault(a => a.Id == id);
                    curObjForCurAbrevv = db.Currency.FirstOrDefault(c => c.Id == accumulation.CurrencyId);
                    purseCollection = new ObservableCollection<Purse>(db.Purse.Where(p => p.CurrencyId == accumulation.CurrencyId).ToList());
                }
            }
            if (accumulation != null)
            {
                if(purseCollection.Count == 0)
                {
                    errorText.Text = "Нет доступных счетов";
                }
                else
                {
                    purseListCB.ItemsSource = purseCollection;
                }
                curAbbrev.Text = curObjForCurAbrevv.Abbreviation;
            }
            else
                GoToPreviousPage();
                
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            double sum;
            Purse purseElement = purseListCB.SelectedItem as Purse;

            if(purseElement == null)
            {
                errorText.Text = "Не выбран счет";
                return;
            }
            if (!Double.TryParse(ConvertToStringFormat(accumulationSum.Text), out sum))
            {
                errorText.Text = "Некоректная сумма";
                return;
            }
            if (sum < 0)
            {
                errorText.Text = "Некоректная сумма";
                return;
            }

            using(PFContext db = new PFContext())
            {
                Purse purseUpdate;
                Currency currrencyElement = db.Currency.FirstOrDefault(c => c.Id == purseElement.CurrencyId);
                AccumulationOperation accumulationOperation = new AccumulationOperation
                {
                    AccumulationId = accumulation.Id,
                    Summa = sum,
                    PurseId = purseElement.Id,
                    CurrencyId = currrencyElement.Id,
                    Purse = purseElement,
                    Currency = currrencyElement,
                    Accumulation = accumulation
                };
                db.Purse.Attach(purseElement);
                db.Currency.Attach(currrencyElement);
                db.Accumulation.Attach(accumulation);
                /* Update Purse */
                purseUpdate = db.Purse.FirstOrDefault(p => p.Id == accumulationOperation.PurseId);
                purseUpdate.Balance = purseUpdate.Balance - accumulationOperation.Summa;
                db.Purse.Update(purseUpdate);
                /* Update Accumulation */
                accumulation.CurrentSumma += accumulationOperation.Summa;
                db.Update(accumulation);

                db.AccumulationOperation.Add(accumulationOperation);
                db.SaveChanges();
            }
            GoToPreviousPage();
            
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
        private void GoToPreviousPage()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(AccumulationPage));
        }
    }
}
