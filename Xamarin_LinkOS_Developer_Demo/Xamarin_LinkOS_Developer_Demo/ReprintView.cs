using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace Xamarin_LinkOS_Developer_Demo
{
    public class ReprintView : ContentPage
    {
        public ReprintView()
        {
            Title = "Re print";
           
        }
        protected async override void OnAppearing()
        {
            var template = new DataTemplate(typeof(TextCell));
            template.SetBinding(TextCell.TextProperty, "business_name");
            template.SetBinding(TextCell.DetailProperty, "asset_number");
            ListView business_list = new ListView();
            //business_list.ItemsSource = business_data.business_detail;
            business_list.ItemTemplate = template;
            List<business_DB> list = await business_data.db_connection.get_business_data();
            business_list.ItemsSource = list;
            Content = business_list;

            business_list.ItemSelected += Business_list_ItemSelected;
        }

        async private void Business_list_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
             if(await DisplayAlert("Confirm Re-Print", "Are you sure you want to Re-print ?", "yes", "no"))
            {
                business_DB selected_business=(business_DB) e.SelectedItem;
                ReceiptDemoView rdv = new ReceiptDemoView();
                rdv.check_printer(selected_business.business_name, selected_business.phone_number, selected_business.asset_number, selected_business.location, selected_business.date, selected_business.due_date);
            }
            else
            {
                OnAppearing();
            }
        }
    }
}
