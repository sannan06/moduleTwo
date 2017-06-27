using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabs
{
    public partial class AzureTable : ContentPage
    {
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;
        Geocoder geoCoder;

        public AzureTable()
        {
            InitializeComponent();
            geoCoder = new Geocoder();
        }
		
        async void Handle_ClickedAsync(object sender, System.EventArgs e)
		{
			List<celebrityModel> celebrityInformation = await AzureManager.AzureManagerInstance.GetCelebrityInformation();

            foreach (celebrityModel model in celebrityInformation)
			{
				var position = new Position(model.Latitude, model.Longitude);
				var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
				foreach (var address in possibleAddresses)
				model.City = address;
			}

            celebrityList.ItemsSource = celebrityInformation;

		}
    }
}
