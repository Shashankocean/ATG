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
        public string business_name, phone_number, asset_number, location;
        Entry BusinessEntry, PhoneEntry, AssetEntry, LocationEntry;
        Label title;
        public ReceiptDemoView() : base()
        {
            title = new Label { Text = "Receipt" };
            printBtn = new Button { Text = "Print" };
            printBtn.Clicked += PrintBtn_Clicked;

            BusinessEntry = new Entry { Placeholder = "Business name" };
            PhoneEntry = new Entry { Placeholder = "Phone number" };
            AssetEntry = new Entry { Placeholder = "Asset number" };
            LocationEntry = new Entry { Placeholder = "Location" };
            Children.Add(BusinessEntry);
            Children.Add(PhoneEntry);
            Children.Add(AssetEntry);
            Children.Add(LocationEntry);

            Children.Add(title);
            Children.Add(printBtn);
        }

		private void PrintBtn_Clicked(object sender, EventArgs e)
		{
            business_name = BusinessEntry.Text;
            phone_number = PhoneEntry.Text;
            asset_number = AssetEntry.Text;
            location = LocationEntry.Text;
            if (string.IsNullOrEmpty(business_name) && string.IsNullOrEmpty(phone_number) && string.IsNullOrEmpty(location) && string.IsNullOrEmpty(asset_number))
            {
                    title.Text = "Some fields are blank";
            }
            else if (asset_number.Length != 11)
            {
                title.Text = "Asset number should be 11 digit";
            }
            else
            {
                Regex regex = new Regex(@"^[0-9]+$");
                if (regex.IsMatch(asset_number))
                {

                    if (CheckPrinter())
                    {
                        printBtn.IsEnabled = false;
                        new Task(new Action(() =>
                        {
                            PrintLineMode();
                        })).Start();
                    }
                
                }
                else
                {
                    title.Text = "Enter numbers (asset)";
                }
                
            }
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

        "^ XA~TA000~JSN ^ LT0 ^ MNW ^ MTD ^ PON ^ PMN ^ LH0,0 ^ JMA ^ PR5,5~SD10 ^ JUS ^ LRN ^ CI0 ^ XZ"+
        "^ XA"+
        "^ MMT"+
        "^ PW422"+
        "^ LL0269"+
        "^ LS0"+
        "^ BY2,2,160 ^ FT219,41 ^ BUR,,Y,N"+
        "^ FD{0} ^ FS"+
        "^ FT68,7 ^ A0R,17,16 ^ FH\\^ FDLocation:  ^ FS"+
        "^ FT103,8 ^ A0R,17,16 ^ FH\\^ FDAsset: ^ FS"+
        "^ FT137,8 ^ A0R,17,16 ^ FH\\^ FDPhone:^ FS"+
        "^ FT172,8 ^ A0R,17,16 ^ FH\\^ FDBusiness: ^ FS"+
        "^ FT66,85 ^ A0R,20,19 ^ FH\\^ FD{1} ^ FS"+
        "^ FT100,59 ^ A0R,20,19 ^ FH\\^ FD{2} ^ FS"+
        "^ FT169,79 ^ A0R,20,19 ^ FH\\^ FD{3} ^ FS"+
        "^ FT134,59 ^ A0R,20,19 ^ FH\\^ FD{4} ^ FS"+
        "^ PQ1,0,1,Y ^ XZ";

            //int headerHeight = 325;

            ////DateTime date = new DateTime();
            ////string sdf = "yyyy/MM/dd";
            ////string dateString = date.ToString(sdf);

            string header = string.Format(tmpHeader,asset_number, location,asset_number,business_name,phone_number);

            printerConnection.Write(GetBytes(header));

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