using System;
using System.Reflection;
using System.Windows;
using Fluxx.App;
using ReactiveData;
using TypeTooling;
using TypeTooling.Xaml.Wpf;


[assembly: TypeToolingProvider(typeof(WpfXamlTypeToolingProvider))]

namespace Fluxx.Wpf {
    public class FamlWpfApplication : FamlApplication {
        public static FamlWpfApplication Instance;
        
        private readonly Application _application;
        private readonly Window _mainWindow;
        private IReactive<UIElement> _currentPage;


        public static void Init(Application application, Window mainWindow, Assembly resourcesAssembly, string resourceFileName) {
            if (Instance != null)
                throw new Exception("FamlWpfApplication was already initialized");

            Instance = new FamlWpfApplication(application, resourcesAssembly, mainWindow, resourceFileName);
        }

        public FamlWpfApplication(Application application, Assembly assembly, Window mainWindow, string resourceFileName) {
            _application = application;
            _mainWindow = mainWindow;

            Initialize(assembly, resourceFileName);
        }

        protected override void AddTypeToolingProviders(Program program, FamlProject project) {
            project.AddTypeToolingProvider(new WpfXamlTypeToolingProvider(program.RootProject.TypeToolingEnvironment));
        }

        public void SetCurrentPage(string functionName, Args args) {
            if (_currentPage != null)
                _currentPage.Changed -= OnCurrentPageChanged;

            _currentPage = UIElementFunction.Invoke(Program, functionName, args);

            _currentPage.Changed += OnCurrentPageChanged;

            _mainWindow.Content = _currentPage.Value;
        }

        public void OnCurrentPageChanged() {
            _application.Dispatcher.Invoke(() => {
                _mainWindow.Content = _currentPage.Value;
            });
        }

        public void Changed() {            /*{
            Device.BeginInvokeOnMainThread(() => {
                _application.MainPage = new NavigationPage(_currentPage.value);
            });*/
        }
    }
}
