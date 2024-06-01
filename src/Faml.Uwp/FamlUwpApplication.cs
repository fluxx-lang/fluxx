using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Faml.App;
using ReactiveData;
using TypeTooling.Xaml.Uwp;

namespace Faml.Uwp {
    public class FamlUwpApplication : FamlApplication {
        public static FamlUwpApplication Instance;

        private readonly Application _application;
        private IReactive<Page> _currentPage;

        public static void Init(Application application, Assembly assembly, string resourceFileName) {
            if (Instance != null)
                throw new Exception("Faml.XamarinForms was already initialized");

            Instance = new FamlUwpApplication(application, assembly, resourceFileName);
        }

        public FamlUwpApplication(Application application, Assembly assembly, string resourceFileName) {
            _application = application;
            Initialize(assembly, resourceFileName);
        }

        protected override void AddTypeToolingProviders(Program program, FamlProject project) {
            project.AddTypeToolingProvider(new UwpXamlTypeToolingProvider(program.RootProject.TypeToolingEnvironment));
        }

        public async Task InitVisualizer() {
            CoreApplicationView hiddenViewForExamples = CoreApplication.CreateNewView();
            int newViewId = 0;
            await hiddenViewForExamples.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Page hostPage = new Page();
                
                Window.Current.Content = hostPage;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;

                AppToDevEnvConnection.SetVisualizer(new UwpVisualizer(Program, hostPage));
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        public void SetCurrentPage(string functionName, Args args) {
            if (_currentPage != null)
                _currentPage.Changed -= OnCurrentPageChanged;

            _currentPage = PageFunction.Invoke(Program, functionName, args);

            _currentPage.Changed += OnCurrentPageChanged;

            Task.Run(async () => {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => {
                        var rootFrame = Window.Current.Content as Frame;

                        if (rootFrame == null) {
                            // Create a Frame to act as the navigation context and navigate to the first page.
                            rootFrame = new Frame();
                        }

                        rootFrame.Content = _currentPage.Value;
                    });
            }).Wait();
#if false
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page.
                rootFrame = new Frame();
            }

            rootFrame.Content = _currentPage.Value;
#endif
        }

        public void OnCurrentPageChanged() {
            Task.Run(async () => {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () => {
                        var rootFrame = Window.Current.Content as Frame;
                        rootFrame.Content = _currentPage.Value;
                    });
            });
        }

        public void Changed() {            /*{
            Device.BeginInvokeOnMainThread(() => {
                _application.MainPage = new NavigationPage(_currentPage.value);
            });*/
        }

        public async void dataEventHandler(object sender, EventArgs e, IList<object> data) {
           /*^
            foreach (object dataItem in data) {
                if (dataItem is Navigate) {
                    var navigate = ((Navigate) dataItem);

                    Page page = navigate.Content;
                    await _application.MainPage.Navigation.PushAsync(page);
                }
            }
            */
        }
    }
}
