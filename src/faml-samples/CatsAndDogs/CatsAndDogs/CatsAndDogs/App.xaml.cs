using CatsAndDogs.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CatsAndDogs {
    public partial class App : Application {
        public App() {
            InitializeComponent();

            SetMainPage();
        }

        public static void SetMainPage() {
            Current.MainPage = new TabbedPage {
                Children =
                {
                    new NavigationPage(new RainingPage())
                    {
                        Title = "Raining",
                        Icon = Device.OnPlatform("tab_feed.png",null,null)
                    },
                    new NavigationPage(new ItemsPage())
                    {
                        Title = "Browse",
                        Icon = Device.OnPlatform("tab_feed.png",null,null)
                    },
                    new NavigationPage(new AboutPage())
                    {
                        Title = "About",
                        Icon = Device.OnPlatform("tab_about.png",null,null)
                    },
                }
            };

            Current.MainPage.SizeChanged += MainPage_SizeChanged;

            var width = Current.MainPage.Width;
            var height = Current.MainPage.Width;

            Current.MainPage.ForceLayout();

            width = Current.MainPage.Width;
            height = Current.MainPage.Width;
        }

        private static void MainPage_SizeChanged(object sender, System.EventArgs e) {
            var width = Current.MainPage.Width;
            var height = Current.MainPage.Width;
        }
    }
}
