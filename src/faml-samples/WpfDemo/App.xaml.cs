using System.Reflection;
using System.Windows;
using Faml;
using Faml.Wpf;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        void Application_Startup(object sender, StartupEventArgs e) {
            MainWindow mainWindow = new MainWindow();

            Assembly thisAssembly = typeof(App).GetTypeInfo().Assembly;
            FamlWpfApplication.Init(this, mainWindow, thisAssembly, "MainWindow.faml");
            
            FamlWpfApplication.Instance.SetCurrentPage("MainWindow.mainPage", new Args());

            FamlWpfApplication.Instance.AppToDevEnvConnection.Start();
            //AppMqttClient appMqttClient = new AppMqttClient("myapp", FamlWpfApplication.Instance.GetProgram(), "127.0.0.1");
            //appMqttClient.SetExampleRenderer(new UwpExampleRenderer(this));
            //appMqttClient.SetObjectRenderer(new UwpUiElementRenderer(this));

            mainWindow.Show();
        }
    }
}
