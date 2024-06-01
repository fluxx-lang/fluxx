using System.Reflection;
using Faml;
using Faml.XamarinForms;
using Xamarin.Forms;
using XamarinFormsDemo.Services;
using XamarinFormsDemo.Views;

namespace XamarinFormsDemo
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();

            Assembly thisAssembly = typeof(App).GetTypeInfo().Assembly;
            FamlXamarinFormsApplication.Init(this, thisAssembly, "main.faml");
            FamlXamarinFormsApplication.Instance.SetCurrentPageToWelcome();

            FamlXamarinFormsApplication.Instance.SetCurrentPage("mainPage", new Args());

            FamlXamarinFormsApplication.Instance.AppToDevEnvConnection.Start();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
