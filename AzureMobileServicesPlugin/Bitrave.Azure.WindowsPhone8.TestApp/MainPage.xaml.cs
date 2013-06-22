using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Bitrave.Azure.WindowsPhone8.TestApp.Resources;
using Newtonsoft.Json;

namespace Bitrave.Azure.WindowsPhone8.TestApp
{

    public class ToDoItem
    {
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }

        public override string ToString()
        {
            return Id + "," + Text + "," + Complete;
        }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        private string _azureEndPoint = "http://bitraveservices.azure-mobile.net/";
        private string _applicationKey = "ePYhutMVmiUPWhFAJaRYTJsPFTiuAB20";

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
            azure.LoginAsync(AuthenticationProvider.Facebook, MyCallback);
        }

        private void MyCallback(AzureResponse<MobileServiceUser> obj)
        {
            
        }

        private void OnError(AzureResponse<object> azureResponse)
        {
            
        }



        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}