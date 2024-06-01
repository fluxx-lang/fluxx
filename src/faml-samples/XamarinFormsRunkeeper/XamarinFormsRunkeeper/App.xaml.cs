using System.Reflection;
using Lux;
using LuxXamarinForms;
using Xamarin.Forms;

namespace XamarinFormsRunkeeper
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent();

            Assembly thisAssembly = typeof(App).GetTypeInfo().Assembly;
            LuxXamarinFormsApplication.Init(this, thisAssembly, "main.lux");

            LuxXamarinFormsApplication.Instance.SetCurrentPage("mainPage", new Args());

            //MainPage = new XamarinFormsRunkeeper.MainPage();
        }

        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}
