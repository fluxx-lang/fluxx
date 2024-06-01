using System.Drawing;
using Windows.UI.Xaml;
using LuxMqttApp;
using Windows.UI.Xaml.Controls;
using Lux;
using LuxUwpApp;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
#if false
            AppMqttClient appMqttClient = new AppMqttClient("myapp", LuxUwpApplication.Instance.GetProgram());
            appMqttClient.SetExampleRenderer(new UwpExampleRenderer(this));
            appMqttClient.SetObjectRenderer(new UwpUiElementRenderer(this));
            appMqttClient.Start();
#endif

            LuxUwpApplication.Instance.SetCurrentPage("main.mainPage", new Args());
        }
    }
}
