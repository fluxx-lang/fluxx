using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using LuxMqttApp;
using LuxXamarinForms;

namespace XamarinFormsRunkeeper.Droid
{
    [Activity(Label = "XamarinFormsRunkeeper", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            var app = new App();
            LoadApplication(app);

            AppMqttClient appMqttClient = new AppMqttClient("myapp", LuxXamarinFormsApplication.Instance.GetMutableProgram());
            appMqttClient.SetObjectRenderer(null /* new RenderControl(app) */);
            appMqttClient.Start();

        }
    }
}

