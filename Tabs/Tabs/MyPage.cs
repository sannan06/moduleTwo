using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tabs
{
	public partial class CustomVision : ContentPage
	{
		const string subscriptionKey = "246b8eeddcff4c91b5c521bd5096635b";
		const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0";

		public CustomVision()
		{
			InitializeComponent();
		}

		private async void loadCamera(object sender, EventArgs e)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("No Camera", ":( No camera available.", "OK");
				return;
			}

			MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				PhotoSize = PhotoSize.Medium,
				Directory = "Sample",
				Name = $"{DateTime.UtcNow}.jpg"
			});

			if (file == null)
				return;

			image.Source = ImageSource.FromStream(() =>
			{
				return file.GetStream();
			});

			await MakeAnalysisRequest(file);
		}

		static byte[] GetImageAsByteArray(MediaFile file)
		{
			var stream = file.GetStream();
			BinaryReader binaryReader = new BinaryReader(stream);
			return binaryReader.ReadBytes((int)stream.Length);
		}
		async Task MakeAnalysisRequest(MediaFile file)
		{
			var client = new HttpClient();

			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
			string requestParameters = "model=celebrities";
			string uri = uriBase + "?" + requestParameters;

			HttpResponseMessage response;

			byte[] byteData = GetImageAsByteArray(file);

			using (var content = new ByteArrayContent(byteData))
			{

				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(uri, content);
				string contentString = await response.Content.ReadAsStringAsync();

			}

			//Get rid of file once we have finished using it
			file.Dispose();
		}
	}
}