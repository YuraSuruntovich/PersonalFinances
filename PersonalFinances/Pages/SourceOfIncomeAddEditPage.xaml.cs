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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace PersonalFinances.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SourceOfIncomeAddEditPage : Page
    {
        SourceOfIncome income;
        int id;
        public SourceOfIncomeAddEditPage()
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
                    income = db.SourceOfIncome.FirstOrDefault(c => c.Id == id);
                }
            }

            if (income != null)
            {
                headerBlock.Text = "Редактировать категорию доходов";
                nameSourceOfIncome.Text = income.Name;
            }
        }
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (nameSourceOfIncome.Text.Length == 0)
            {
                errorText.Text = "Введите название";
                return;
            }

            using (PFContext db = new PFContext())
            {
                if (income != null)
                {
                    income.Name = nameSourceOfIncome.Text;
                    db.SourceOfIncome.Update(income);
                }
                else
                {
                    SourceOfIncome incomeNew = new SourceOfIncome
                    {
                        Name = nameSourceOfIncome.Text
                    };
                    db.SourceOfIncome.Add(incomeNew);
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
                Frame.Navigate(typeof(MainPage));
        }
    }
}
