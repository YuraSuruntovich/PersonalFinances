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
using System.Runtime.InteropServices;
using Windows.Web.Http;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Windows.System;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>

    public sealed partial class CurrencyEditPage : Page
    {
        int id;
        Currency currency;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public CurrencyEditPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                id = (int)e.Parameter;
                using (PFContext db = new PFContext())
                {
                    currency = db.Currency.FirstOrDefault(c => c.Id == id);
                }

                if(currency != null)
                {
                    rateBox.Text = currency.Rate.ToString();
                    scaleCur.Text = currency.CurScale.ToString();
                    abrevCur.Text = currency.Abbreviation;
                }
            }
        }
       
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string value = ConvertToStringFormat(rateBox.Text);
            double finalRate;
            if (!Double.TryParse(ConvertToStringFormat(value), out finalRate))
            {
                errorText.Text = "Некоректный курс";
                return;
            }
            if(finalRate <= 0)
            {
                errorText.Text = "Некоректный курс";
                return;
            }

            using(PFContext db = new PFContext())
            {
                currency.Rate = finalRate;
                db.Currency.Update(currency);

                db.SaveChanges();
            }
            GoToPreviousPage();
        }
        private async void getOfficialRate_1_Click(object sender, RoutedEventArgs e)
        {
            if (IsConnect())
            {
                ring1.IsActive = true;
                double curs;
                var client = new HttpClient();
                DateTime thisDay = DateTime.Today;
                string date = thisDay.ToString("yyyy") + "-" + thisDay.ToString("MM") + "-" + thisDay.ToString("dd");
                try
                {
                    var resVal = await client.GetStringAsync(new Uri("http://www.nbrb.by/API/ExRates/Rates/" + currency.CurIdNatBank + "?onDate=" + date + "&Periodicity=0"));
                    dynamic y = Newtonsoft.Json.JsonConvert.DeserializeObject(resVal);
                    curs = y.Cur_OfficialRate;
                    rateBox.Text = Math.Round(curs,2).ToString();

                    ring1.IsActive = false;
                    errorText.Text = "";
                }
                catch
                {
                    ring1.IsActive = false;
                    errorText.Text = "Невозможно загрузить курс";
                }
            }
            else
            {
                ContentDialog settingDialog = new ContentDialog()
                {
                    Title = "Вы не подключены",
                    Content = "Нет подключения к Интернету.\n" + "- Убедитесь, что не включен режим 'в самолете'.\n" +
                                 "- Проверьте, включен ли беспроводной коммутатор.",
                    PrimaryButtonText = "Перейти к параметрам сети",
                    SecondaryButtonText = "Отмена"
                };

                ContentDialogResult result = await settingDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                }
                else
                    settingDialog.Hide();
            }
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
                Frame.Navigate(typeof(CurrencyPage));
        }
        private bool IsConnect()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }
    }
}
