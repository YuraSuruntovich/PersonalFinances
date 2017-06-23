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
using Windows.Web.Http;
using System.Runtime.InteropServices;
using Windows.System;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class DisplacementAddEditPage : Page
    {
        ObservableCollection<Displacement> displacementCollection;
        Displacement displacement;
        Purse purseSelect1;
        Purse purseSelect2;
        Purse purseUpdate1;
        Purse purseUpdate2;
        Purse purseSealed1;
        Purse purseSealed2;
        Currency currency1;
        Currency currency2;
        Currency currency;
        int id;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public DisplacementAddEditPage()
        {
            this.InitializeComponent();
            this.Loaded += DisplacementAddEditPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                id = (int)e.Parameter;
                using(PFContext db = new PFContext())
                {
                    displacement = db.Displacement.FirstOrDefault(d => d.Id == id);
                }
            }

            if(displacement != null)
            {
                Displacement d;
                headerBlock.Text = "Редактировать перемещение";

                summaDispl.Text = displacement.SummaOut.ToString();
                dateDispl.Date = Convert.ToDateTime(displacement.DateOperation);
                rate1.Text = displacement.RateCur1.ToString();
                rate2.Text = displacement.RateCur2.ToString();
                // comboBox
                using(PFContext db = new PFContext())
                {
                    displacementCollection = new ObservableCollection<Displacement>(db.Displacement.Include(x => x.Currency));
                    d = displacementCollection.FirstOrDefault(x => x.Id == id);

                    purseSealed1 = db.Purse.FirstOrDefault(p => p.Id == d.PurseId1);
                    purseSealed2 = db.Purse.FirstOrDefault(p => p.Id == d.PurseId2);
                }
            }
        }
        private void DisplacementAddEditPage_Loaded(object sender, RoutedEventArgs e)
        {
            using(PFContext db = new PFContext())
            {
                purse_1ListCB.ItemsSource = db.Purse.ToList();
                purse_2ListCB.ItemsSource = db.Purse.ToList();
            }
            if(purseSealed1 != null && purseSealed2 != null)
            {
                purse_1ListCB.SelectedValue = purseSealed1.Id;
                purse_2ListCB.SelectedValue = purseSealed2.Id;
            }
            dateDispl.Date = DateTime.Today;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            purseSelect1 = purse_1ListCB.SelectedItem as Purse;
            purseSelect2 = purse_2ListCB.SelectedItem as Purse;

            DateTimeOffset date = (DateTimeOffset)dateDispl.Date;
            string dateFormat = date.Date.ToString("dd.MM.yyyy");

            double summa;
            double result;
            double curRate1;
            double curRate2;

            #region Verification
            if (part1.Visibility == Visibility.Collapsed)
            {
                rate1.Text = "1";
            }
            if (part2.Visibility == Visibility.Collapsed)
            {
                rate2.Text = "1";
            }
            if(CurrencesRates.Visibility == Visibility.Collapsed)
            {
                rate1.Text = "1";
                rate2.Text = "1";
            }

            if (purseSelect1 == null || purseSelect2 == null)
            {
                errorText.Text = "Выберите счет";
                return;
            }
            if(purseSelect1.Id == purseSelect2.Id)
            {
                errorText.Text = "Выберите разные счета";
                return;
            }
            if(!Double.TryParse(ConvertToStringFormat(summaDispl.Text), out summa))
            {
                errorText.Text = "Некоректная сумма";
                return;
            }
            if(!Double.TryParse(ConvertToStringFormat(rate1.Text), out curRate1))
            {
                errorText.Text = "Некоректный курс1";
                return;
            }
            if (!Double.TryParse(ConvertToStringFormat(rate2.Text), out curRate2))
            {
                errorText.Text = "Некоректный курс2";
                return;
            }
            #endregion
            curRate1 = Math.Round(curRate1, 2);
            curRate2 = Math.Round(curRate2, 2);
            result = summa * curRate1 / curRate2;
            result = Math.Round(result, 2);

            using(PFContext db = new PFContext())
            {
                if(displacement != null)
                {
                    Purse purse1Return = db.Purse.FirstOrDefault(p => p.Id == displacement.PurseId1);
                    Purse purse2Return = db.Purse.FirstOrDefault(p => p.Id == displacement.PurseId2);
                    Currency currencyForUpdate = db.Currency.FirstOrDefault(c => c.Id == purseSelect2.CurrencyId);

                    purse1Return.Balance += displacement.SummaOut;
                    purse2Return.Balance -= displacement.SummaIncome;

                    db.Purse.Update(purse1Return);
                    db.Purse.Update(purse2Return);

                    displacement.PurseId1 = purseSelect1.Id;
                    displacement.PurseId2 = purseSelect2.Id;
                    displacement.SummaOut = summa;
                    displacement.SummaIncome = result;
                    displacement.DateOperation = dateFormat;
                    displacement.RateCur1 = curRate1;
                    displacement.RateCur2 = curRate2;
                    displacement.CurrencyId = purseSelect2.CurrencyId;
                    displacement.Currency = currencyForUpdate;

                    purseUpdate1 = db.Purse.FirstOrDefault(p => p.Id == displacement.PurseId1);
                    purseUpdate2 = db.Purse.FirstOrDefault(p => p.Id == displacement.PurseId2);

                    purseUpdate1.Balance -= displacement.SummaOut;
                    purseUpdate2.Balance += displacement.SummaIncome;

                    db.Purse.Update(purseUpdate1);
                    db.Purse.Update(purseUpdate2);

                    db.Displacement.Update(displacement);

                }
                else
                {
                    currency = db.Currency.FirstOrDefault(c => c.Id == purseSelect2.CurrencyId);
                    Displacement displacementNew = new Displacement
                    {
                        PurseId1 = purseSelect1.Id,
                        PurseId2 = purseSelect2.Id,
                        SummaOut = summa,
                        SummaIncome = result,
                        DateOperation = dateFormat,
                        RateCur1 = curRate1,
                        RateCur2 = curRate2,
                        CurrencyId = purseSelect2.CurrencyId,
                        //Purse = purseSelect2,
                        Currency = currency
                    };
                    //db.Purse.Attach(purseSelect2);
                    db.Currency.Attach(currency);
                    db.Displacement.Add(displacementNew);
                    /* Update Purse */
                    purseUpdate1 = db.Purse.FirstOrDefault(p => p.Id == displacementNew.PurseId1);
                    purseUpdate2 = db.Purse.FirstOrDefault(p => p.Id == displacementNew.PurseId2);
                    purseUpdate1.Balance -= displacementNew.SummaOut;
                    purseUpdate2.Balance += displacementNew.SummaIncome;
                    db.Purse.Update(purseUpdate1);
                    db.Purse.Update(purseUpdate2);

                    
                }
                db.SaveChanges();
            }
            GoToPreviousPage();

            errorText.Text = "--- Result --- " + result.ToString();

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }

        private void purse_1ListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            purseSelect1 = purse_1ListCB.SelectedItem as Purse;

            using(PFContext db = new PFContext())
            {
                currency1 = db.Currency.FirstOrDefault(c => c.Id == purseSelect1.CurrencyId);
            }
            summaAbrev.Text = currency1.Abbreviation;
            rate1.Text = (currency1.Rate / currency1.CurScale).ToString();

            if (currency1 != null && currency2 != null)
            {
                if (currency1.Id != currency2.Id)
                {
                    abbrevBlock1.Text = currency1.Abbreviation;
                    abbrevBlock2.Text = currency2.Abbreviation;
                    CurrencesRates.Visibility = Visibility.Visible;
                    part1.Visibility = Visibility.Visible;
                    part2.Visibility = Visibility.Visible;

                    if (currency1.Abbreviation == "BYN")
                    {
                        part1.Visibility = Visibility.Collapsed;
                    }
                    else if (currency2.Abbreviation == "BYN")
                    {
                        part2.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        part1.Visibility = Visibility.Visible;
                    }

                }
                else
                {
                    CurrencesRates.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void purse_2ListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            purseSelect2 = purse_2ListCB.SelectedItem as Purse;

            using(PFContext db = new PFContext())
            {
                currency2 = db.Currency.FirstOrDefault(c => c.Id == purseSelect2.CurrencyId);
            }
            rate2.Text = (currency2.Rate / currency2.CurScale).ToString();

            if (currency1 != null && currency2 != null)
            {
                if(currency1.Id != currency2.Id)
                {
                    abbrevBlock1.Text = currency1.Abbreviation;
                    abbrevBlock2.Text = currency2.Abbreviation;
                    CurrencesRates.Visibility = Visibility.Visible;
                    part1.Visibility = Visibility.Visible;
                    part2.Visibility = Visibility.Visible;

                    if (currency2.Abbreviation == "BYN")
                    {
                        part2.Visibility = Visibility.Collapsed;
                    }
                    else if (currency1.Abbreviation == "BYN")
                    {
                        part1.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        part2.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    CurrencesRates.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void getOfficialRate_1_Click(object sender, RoutedEventArgs e)
        {
            if (IsConnect())
            {
                ring1.IsActive = true;
                var client = new HttpClient();
                DateTime thisDay = DateTime.Today;
                DateTime getDate;

                DateTimeOffset dateTime = (DateTimeOffset)dateDispl.Date;
                string date = dateTime.ToString("yyyy") + "-" + dateTime.ToString("MM") + "-" + dateTime.ToString("dd");
                getDate = Convert.ToDateTime(date);

                if (thisDay >= getDate)
                {
                    try
                    {
                        var resVal = await client.GetStringAsync(new Uri("http://www.nbrb.by/API/ExRates/Rates/" + currency1.CurIdNatBank + "?onDate=" + date + "&Periodicity=0"));
                        dynamic y = Newtonsoft.Json.JsonConvert.DeserializeObject(resVal);
                        double curs = y.Cur_OfficialRate / currency1.CurScale;

                        rate1.Text = Math.Round(curs,2).ToString();
                        ring1.IsActive = false;
                        errorText.Text = "";
                    }
                    catch
                    {
                        ring1.IsActive = false;
                        errorText.Text = "Невозможно загрузить курс на эту дату";
                    }
                    
                }
                else
                {
                    ring1.IsActive = false;
                    errorText.Text = "Невозможно загрузить курс на эту дату";
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

        private async void getOfficialRate_2_Click(object sender, RoutedEventArgs e)
        {
            if (IsConnect())
            {
                ring2.IsActive = true;
                var client = new HttpClient();
                DateTime thisDay = DateTime.Today;
                DateTime getDate;

                DateTimeOffset dateTime = (DateTimeOffset)dateDispl.Date;
                string date = dateTime.ToString("yyyy") + "-" + dateTime.ToString("MM") + "-" + dateTime.ToString("dd");
                getDate = Convert.ToDateTime(date);

                if (thisDay >= getDate)
                {
                    try
                    {
                        var resVal = await client.GetStringAsync(new Uri("http://www.nbrb.by/API/ExRates/Rates/" + currency2.CurIdNatBank + "?onDate=" + date + "&Periodicity=0"));
                        dynamic y = Newtonsoft.Json.JsonConvert.DeserializeObject(resVal);

                        double curs = y.Cur_OfficialRate / currency2.CurScale;

                        rate2.Text = Math.Round(curs,2).ToString();
                        ring2.IsActive = false;
                        errorText.Text = "";
                    }
                    catch
                    {
                        ring2.IsActive = false;
                        errorText.Text = "Невозможно загрузить курс на эту дату";
                    }
                }
                else
                {
                    ring2.IsActive = false;
                    errorText.Text = "Невозможно загрузить курс на эту дату";
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

        private bool IsConnect()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }
        private void GoToPreviousPage()
        {
            if (Frame.CanGoBack)
                Frame.Navigate(typeof(OperationsPage), 2);
            else
                Frame.Navigate(typeof(OperationsPage));
        }

        public string ConvertToStringFormat(string s)
        {
            char[] symbols = s.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                if(symbols[i] == '.')
                {
                    symbols[i] = ',';
                }
            }
            string str = new string(symbols);

            return str;
        }

        private void addNewPurseButton1_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PursePageAddEdit));
        }

        private void addNewPurseButton2_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PursePageAddEdit));
        }
    }
}
