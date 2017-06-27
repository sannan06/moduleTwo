using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Tabs.Droid
{
    [Activity(Label = "Tabs.Droid", Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
			//TabLayoutResource = Resource.Layout.Tabbar;
			//ToolbarResource = Resource.Layout.Toolbar;
			base.Window.RequestFeature(WindowFeatures.ActionBar);
			// Name of the MainActivity theme you had there before.
			// Or you can use global::Android.Resource.Style.ThemeHoloLight
            base.SetTheme(Resource.Style.MyTheme);

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
            Xamarin.FormsMaps.Init(this, bundle);
        }
    }
}
