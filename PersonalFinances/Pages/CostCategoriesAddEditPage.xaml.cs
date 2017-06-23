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
    public sealed partial class CostCategoriesAddEditPage : Page
    {
        CostCategories costCategor;
        int id;
        public CostCategoriesAddEditPage()
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
                    costCategor = db.CostCategories.FirstOrDefault(c => c.Id == id);
                }
            }

            if(costCategor != null)
            {
                headerBlock.Text = "Редактировать категорию расходов";
                nameCostCategories.Text = costCategor.Name;
            }
        }

        // Add CostCategories
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if(nameCostCategories.Text.Length == 0)
            {
                errorText.Text = "Введите название";
                return;
            }
            
            using(PFContext db = new PFContext())
            {
                if(costCategor != null)
                {
                    costCategor.Name = nameCostCategories.Text;
                    db.CostCategories.Update(costCategor);
                }
                else
                {
                    CostCategories costCategorNew = new CostCategories
                    {
                        Name = nameCostCategories.Text
                    };
                    db.CostCategories.Add(costCategorNew);
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
