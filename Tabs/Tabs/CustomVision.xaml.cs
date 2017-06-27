using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Plugin.Geolocator;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            TagLabel.Text = "";
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
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

            await postLocationAsync();
            await MakePredictionRequest(file);
        }
		async Task postLocationAsync()
		{

			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;

			var position = await locator.GetPositionAsync(10000);

            celebrityModel model = new celebrityModel()
			{
				Longitude = (float)position.Longitude,
				Latitude = (float)position.Latitude

			};

			await AzureManager.AzureManagerInstance.PostCelebrityInformation(model);
		}
        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "246b8eeddcff4c91b5c521bd5096635b");

            string url = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/models/celebrities/analyze";
            string requestParameters = "model=celebrities";
            string uri = url + "?" + requestParameters;

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                string contentString = await response.Content.ReadAsStringAsync();
                //TagLabel.Text = JsonPrettyPrint(contentString);

                JObject rss = JObject.Parse(contentString);
                List<string> message = new List<string>();
                try
                {
                    foreach (var section in rss["result"]["celebrities"])
                    {
                        message.Add(section["name"].ToString());
                    }
                    foreach (var name in message)
                    {
                        TagLabel.Text += "This celebrity is " + name;
                    }
                }
                catch(Exception e)
                {
                    TagLabel.Text = e.ToString();
                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
		static byte[] GetImageAsByteArray(MediaFile file)
		{
			var stream = file.GetStream();
			BinaryReader binaryReader = new BinaryReader(stream);
			return binaryReader.ReadBytes((int)stream.Length);
		}

        static string JsonPrettyPrint(string json)
        {
			if (string.IsNullOrEmpty(json))
				return string.Empty;

			json = json.Replace(Environment.NewLine, "").Replace("\t", "");

			StringBuilder sb = new StringBuilder();
			bool quote = false;
			bool ignore = false;
			int offset = 0;
			int indentLength = 3;

			foreach (char ch in json)
			{
				switch (ch)
				{
					case '"':
						if (!ignore) quote = !quote;
						break;
					case '\'':
						if (quote) ignore = !ignore;
						break;
				}

				if (quote)
					sb.Append(ch);
				else
				{
					switch (ch)
					{
						case '{':
						case '[':
							sb.Append(ch);
							sb.Append(Environment.NewLine);
							sb.Append(new string(' ', ++offset * indentLength));
							break;
						case '}':
						case ']':
							sb.Append(Environment.NewLine);
							sb.Append(new string(' ', --offset * indentLength));
							sb.Append(ch);
							break;
						case ',':
							sb.Append(ch);
							sb.Append(Environment.NewLine);
							sb.Append(new string(' ', offset * indentLength));
							break;
						case ':':
							sb.Append(ch);
							sb.Append(' ');
							break;
						default:
							if (ch != ' ') sb.Append(ch);
							break;
					}
				}
			}
            return sb.ToString().Trim();
        }
    }
}