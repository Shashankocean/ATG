/********************************************************************************************************** 
 * CONFIDENTIAL AND PROPRIETARY 
 *
 * The source code and other information contained herein is the confidential and the exclusive property of
 * ZIH Corporation and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corporation 2016
 *
 * ALL RIGHTS RESERVED 
 *********************************************************************************************************/

/*********************************************************************************************************
File:   LineDemoView.cs

Descr:  A ZPL reciept printing demo. Shows how to put together a dynamic, variable length printout page 
        in ZPL.

Date: 03/8/16 
Updated:
**********************************************************************************************************/
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin_LinkOS_Developer_Demo
{
    public class ReceiptDemoView : BaseDemoView
    {
        Button printBtn;
        public string business_name, phone_number, asset_number, location,date,due_date,date_time;
        Entry BusinessEntry, PhoneEntry, AssetEntry, LocationEntry;
        Label title;
        public ReceiptDemoView() : base()
        {
            
            
            var buttonStyle = new Style(typeof(Button))
            {
                Setters = {

                new Setter { Property = Button.TextColorProperty,   Value = Color.White },
                new Setter { Property = Button.WidthRequestProperty, Value=200 },
                new Setter { Property = Button.BorderRadiusProperty,   Value = 20 },
                new Setter { Property = Button.HorizontalOptionsProperty,   Value = LayoutOptions.Center },
                new Setter { Property = Button.BackgroundColorProperty,   Value = Color.FromHex("#03bc00") },
                new Setter { Property = Button.BorderColorProperty,   Value = Color.Lime }
            }
            };

            title = new Label { Text = " ",TextColor=Color.Red };
            printBtn = new Button { Text = "Print", Style =buttonStyle };
            printBtn.Clicked += PrintBtn_Clicked;

            BusinessEntry = new Entry { Placeholder = "Business name", TextColor = Color.FromHex("#0a0a0a"),PlaceholderColor=Color.FromHex("#7b7c7c"),BackgroundColor= Color.FromHex("#f2f2f2") };
            var BusinessEntry2 = new Label {Text="" };
            PhoneEntry = new Entry { Placeholder = "Phone number", Keyboard = Keyboard.Numeric, TextColor=Color.FromHex("#0a0a0a"), PlaceholderColor = Color.FromHex("#7b7c7c"), BackgroundColor = Color.FromHex("#f2f2f2") };
            AssetEntry = new Entry { Placeholder = "Asset number", Keyboard= Keyboard.Numeric, TextColor = Color.FromHex("#0a0a0a"), PlaceholderColor = Color.FromHex("#7b7c7c"), BackgroundColor = Color.FromHex("#f2f2f2") };
            AssetEntry.Focused += AssetEntry_Focused;
            LocationEntry = new Entry { Placeholder = "Location", TextColor = Color.FromHex("#0a0a0a"), PlaceholderColor = Color.FromHex("#7b7c7c"), BackgroundColor = Color.FromHex("#f2f2f2") };

            //StackLayout stk = new StackLayout {
            //    Spacing=25,
            //};
            //stk.Children.Add(BusinessEntry2);
            //stk.Children.Add(BusinessEntry);
            //stk.Children.Add(PhoneEntry);
            //stk.Children.Add(AssetEntry);
            //stk.Children.Add(LocationEntry);
            //stk.Children.Add(title);
            //stk.Children.Add(printBtn);
            //Content = stk;
            base.stk.Children.Add(BusinessEntry);
            base.stk.Children.Add(PhoneEntry);
            base.stk.Children.Add(AssetEntry);
            base.stk.Children.Add(LocationEntry);
            base.stk.Children.Add(title);
            base.stk.Children.Add(printBtn);
            base.stk.Children.Add(BusinessEntry2);


        }

        private async void AssetEntry_Focused(object sender, FocusEventArgs e)
        {
           
            List<business_DB> list = await business_data.db_connection.get_asset();
            if (list.Count !=0)
            {
                business_DB business = list[0];
                long asset = Int64.Parse(business.asset_number);
                ++asset;
                AssetEntry.Text = leftPadding(asset.ToString());
            }
            else
            {
                AssetEntry.Text = "00000000001";
            }
        }
        public string leftPadding(string s1)
        {
            try
            {
                int length = s1.Length;
                for (int i = length; i < 11; i++)
                {
                    s1 = "0" + s1;
                }
                return s1;
            }
            catch (Exception ex)
            {
                return " " + ex.Message;
            }
        }
        private async void PrintBtn_Clicked(object sender, EventArgs e)
		{
           
            DateTime datetime = DateTime.Now;
            date_time = datetime.ToString();
            if (string.IsNullOrEmpty(BusinessEntry.Text) || string.IsNullOrEmpty(PhoneEntry.Text) || string.IsNullOrEmpty(LocationEntry.Text) || string.IsNullOrEmpty(AssetEntry.Text))
            {
                    title.Text = "Some fields are blank";
            }
            else if (AssetEntry.Text.Length != 11)
            {
                title.Text = "Asset number should be 11 digit";
            }
            else
            {
                Regex regex = new Regex(@"^[0-9]+$");
                if (regex.IsMatch(AssetEntry.Text) && regex.IsMatch(PhoneEntry.Text))
                {
                    //ReprintView rp = new ReprintView();
                    //if (!await rp.DisplayAlert("Conform", "Are you sure you want to print?", "Yes", "No"))
                    //{
                    //    return;
                    //}
                    if (myprinterG())
                    {
                        await business_data.db_connection.SaveBusinessAsync(new business_DB() { business_name = BusinessEntry.Text, location = LocationEntry.Text, phone_number = PhoneEntry.Text, asset_number = AssetEntry.Text, date = datetime.ToString("dd-MM-yyyy"), due_date = datetime.AddMonths(6).ToString("dd-MM-yyyy"), date_time = date_time });
                        check_printer(BusinessEntry.Text, PhoneEntry.Text, AssetEntry.Text, LocationEntry.Text, datetime.ToString("dd-MM-yyyy"), datetime.AddMonths(6).ToString("dd-MM-yyyy"));
                    }
                }
                else
                {
                    title.Text = "Enter numbers (Asset & Phone number)";
                }   
            }
		}
        public bool myprinterG()
        {
            if (myPrinterG == null)
            {
                ShowAlert("No printer selected", "Printer");
                return false;
            }
            return true;
        }
        public void check_printer(string businessname,string phonenumber,string assetnumber,string location,string date, string duedate)
        {
           
            if (myPrinterG !=null)
            {
                myPrinter = myPrinterG;
            }
            business_name = businessname;
            phone_number = phonenumber;
            asset_number = assetnumber;
            this.location = location;
            this.date = date;
            due_date = duedate;
           //title.Text = business_name + ":" + phone_number + ":" + asset_number + ":" + this.location + ":" + this.date + ":" + due_date;
           
            if (CheckPrinter())
            {
                printBtn.IsEnabled = false;
                new Task(new Action(() =>
                {
                    PrintLineMode();
                })).Start();
            }
            title.Text = " ";
            BusinessEntry.Text = "";
            AssetEntry.Text = "";
            PhoneEntry.Text = "";
            LocationEntry.Text = "";
        }
		private void PrintLineMode()
        {
            IConnection connection = null;
            try
            {
                connection = myPrinter.Connection;
                connection.Open();
                IZebraPrinter printer = ZebraPrinterFactory.Current.GetInstance(connection);
                if ((!CheckPrinterLanguage(connection)) || (!PreCheckPrinterStatus(printer)))
                {
                    resetPage();
                    return;
                }
                sendZplReceipt(connection);
                if (PostPrintCheckStatus(printer)) { };
                    //ShowAlert("Receipt printed.");
            }
            catch (Exception ex)
            {
                // Connection Exceptions and issues are caught here
                ShowErrorAlert(ex.Message);
            }
            finally
            {
                if ((connection != null) && (connection.IsConnected))
                    connection.Close();
                resetPage();
            }
        }

        private void resetPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                printBtn.IsEnabled = true;
            });
        }

        private void sendZplReceipt(IConnection printerConnection)
        {

            /*
             This routine is provided to you as an example of how to create a variable length label with user specified data.
             The basic flow of the example is as follows

                Header of the label with some variable data
                Body of the label
                    Loops thru user content and creates small line items of printed material
                Footer of the label

             As you can see, there are some variables that the user provides in the header, body and footer, and this routine uses that to build up a proper ZPL string for printing.
             Using this same concept, you can create one label for your receipt header, one for the body and one for the footer. The body receipt will be duplicated as many items as there are in your variable data

             */

            String tmpHeader =
            /*
             Some basics of ZPL. Find more information here : http://www.zebra.com

             ^XA indicates the beginning of a label
             ^PW sets the width of the label (in dots)
             ^MNN sets the printer in continuous mode (variable length receipts only make sense with variably sized labels)
             ^LL sets the length of the label (we calculate this value at the end of the routine)
             ^LH sets the reference axis for printing. 
                You will notice we change this positioning of the 'Y' axis (length) as we build up the label. Once the positioning is changed, all new fields drawn on the label are rendered as if '0' is the new home position
             ^FO sets the origin of the field relative to Label Home ^LH
             ^A sets font information 
             ^FD is a field description
             ^GB is graphic boxes (or lines)
             ^B sets barcode information
             ^XZ indicates the end of a label
             */

            //1// "^XA^MMT^PW422^LL0269^LS0^BY2,2,160^FT219,41^BUR,,Y,N^FD12345678987^FS^FO68,7^A0R,17,16^FDLocation:  ^FS^FO103,8^A0R,17,16^FDAsset: ^FS^FO137,8^A0R,17,16^FDPhone:^FS^FO172,8^A0R,17,16^FDBusiness: ^FS^FO66,85^A0R,20,19^FD{0}^FS^FO100,59^A0R,20,19^FD12345678910^FS^FO169,79^A0R,20,19^FDName of the business i^FS^FO134,59^A0R,20,19^FD123456789102^FS^PQ1,0,1,Y^XZ";
            //dyn v1"^XA^MNN^PW422^LL0269^LH0,0^BY3,2,61^FT71,70^BUN,,Y,N^FD"+asset_number+"^FS^FO16,246^A0N,17,16^FDDue Date: ^FS^FO16,225^A0N,17,16^FDDate: ^FS^FO16,198^A0N,17,16^FDLocation:  ^FS^FO16,170^A0N,17,16^FDAsset: ^FS^FO16,140^A0N,17,16^FDPhone: ^FS^FO16,111^A0N,17,16^FDBusiness: ^FS^FO85,198^A0N,20,19^FD"+location+"^FS^FO60,170^A0N,20,19^FD"+asset_number+"^FS^FO80,111^A0N,20,19^FD"+business_name+"^FS^FO68,140^A0N,20,19^FD"+phone_number+"^FS^FO60,225^A0N,17,19^FD"+date+"^FS^FO81,246^A0N,17,16^FD"+due_date+"^FS^XZ";
            //stat v1"^XA^MNN^PW422^LL0269^LH0,0^BY3,2,61^FT71,70^BUN,,Y,N^FD12345678987^FS^FO16,246^A0N,17,16^FDDue Date: ^FS^FO16,225^A0N,17,16^FDDate: ^FS^FO16,198^A0N,17,16^FDLocation:  ^FS^FO16,170^A0N,17,16^FDAsset: ^FS^FO16,140^A0N,17,16^FDPhone: ^FS^FO16,111^A0N,17,16^FDBusiness: ^FS^FO85,198^A0N,20,19^FDBangalore location^FS^FO60,170^A0N,20,19^FD12345678987^FS^FO80,111^A0N,20,19^FDName of the business i^FS^FO68,140^A0N,20,19^FD123456789102^FS^FO60,225^A0N,17,19^FDDate: ^FS^FO81,246^A0N,17,16^FDDue Date: ^FS^XZ";
            "^XA^MNN^PW400^LL0216^LH0^BY1,2,77^FT221,59^BUR,,Y,N^FD"+asset_number+"^FS^FB210,1,0,C^FO342,0^A0R,24,24^FD"+business_name+"^FS^FB210,1,0,C^FO55,0^A0R,24,24^FDNext Due Date^FS^FB210,1,0,C^FO35,0^A0R,20,20^FD"+due_date+"^FS^FB210,1,0,C^FO115,0^A0R,24,24^FDTest Date^FS^FB210,1,0,C^FO95,0^A0R,20,20^FD"+date+"^FS^FB210,1,0,C^FO155,0^A0R,20,20^FD"+location+"^FS^FB210,1,0,C^FO175,0^A0R,24,24^FDLocation^FS^FB210,1,0,C^FO314,0^A0R,20,20^FD"+phone_number+"^FS^PQ1,0,1,Y^XZ";
            // stat v2"^XA^MNN^PW422^LL0269^LH0,0^BY3,2,61^FT71,70^BUN,,Y,N^FD12345678987^FS^FO16,190^A0N,17,16^FDDate:  ^FS^FO235,190^A0N,17,16^FDDue Date:  ^FS^FO16,165^A0N,17,16^FDPhone: ^FS^FO245,165^A0N,19,16^FDAsset: ^FS^FO16,138^A0N,17,16^FDLocation: ^FS^FO16,111^A0N,17,16^FDBusiness: ^FS^FO50,189^A0N,20,19^FDDate^FS^FO300,189^A0N,20,19^FD31-03-2017^FS^FO62,164^A0N,20,19^FD92345678987^FS^FO285,165^A0N,20,19^FD12345678987^FS^FO80,110^A0N,20,19^FDName of the business i^FS^FO78,137^A0N,20,19^FDBangalore Location^FS^XZ";
            //int headerHeight = 325;

            ////DateTime date = new DateTime();
            ////string sdf = "yyyy/MM/dd";
            ////string dateString = date.ToString(sdf);

            // string header = string.Format(tmpHeader,asset_number, location,asset_number,business_name,phone_number);

            printerConnection.Write(GetBytes(tmpHeader));

            //int heightOfOneLine = 40;

            //Double totalPrice = 0;

            //Dictionary<string, string> itemsToPrint = createListOfItems();

            //foreach(string productName in itemsToPrint.Keys)
            //{
            //    string price;
            //    itemsToPrint.TryGetValue(productName, out price);

            //    String lineItem = "^XA^POI^LL40" + "^FO50,10" + "\r\n" + "^A0,N,28,28" + "\r\n" + "^FD{0}^FS" + "\r\n" + "^FO280,10" + "\r\n" + "^A0,N,28,28" + "\r\n" + "^FD${1}^FS" + "^XZ";
            //    Double tempPrice;
            //    Double.TryParse(price, out tempPrice);
            //    totalPrice += tempPrice;
            //    String oneLineLabel = String.Format(lineItem, productName, price);

            //    printerConnection.Write(GetBytes(oneLineLabel));

            //}

            //long totalBodyHeight = (itemsToPrint.Count + 1) * heightOfOneLine;

            //long footerStartPosition = headerHeight + totalBodyHeight;

            //string tPrice = Convert.ToString(Math.Round((totalPrice), 2));

            //String footer = String.Format("^XA^POI^LL600" + "\r\n" +

            //"^FO50,1" + "\r\n" + "^GB350,5,5,B,0^FS" + "\r\n" +

            //"^FO50,15" + "\r\n" + "^A0,N,40,40" + "\r\n" + "^FDTotal^FS" + "\r\n" +

            //"^FO175,15" + "\r\n" + "^A0,N,40,40" + "\r\n" + "^FD${0}^FS" + "\r\n" +

            //"^FO50,130" + "\r\n" + "^A0,N,45,45" + "\r\n" + "^FDPlease Sign Below^FS" + "\r\n" +

            //"^FO50,190" + "\r\n" + "^GB350,200,2,B^FS" + "\r\n" +

            //"^FO50,400" + "\r\n" + "^GB350,5,5,B,0^FS" + "\r\n" +

            //"^FO50,420" + "\r\n" + "^A0,N,30,30" + "\r\n" + "^FDThanks for choosing us!^FS" + "\r\n" +

            //"^FO50,470" + "\r\n" + "^B3N,N,45,Y,N" + "\r\n" + "^FD0123456^FS" + "\r\n" + "^XZ", tPrice);

            //printerConnection.Write(GetBytes(footer));

        }

        private Dictionary<string, string> createListOfItems()
        {
            String[] names = { "Microwave Oven", "Sneakers (Size 7)", "XL T-Shirt", "Socks (3-pack)", "Blender", "DVD Movie" };
            String[] prices = { "79.99", "69.99", "39.99", "12.99", "34.99", "16.99" };
            Dictionary<string, string> retVal = new Dictionary<string, string>();

            for (int ix = 0; ix < names.Length; ix++)
            {
                retVal.Add(names[ix], prices[ix]);
            }
            return retVal;
        }
    }
}