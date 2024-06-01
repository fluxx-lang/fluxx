using System;
using System.Reflection;
using System.Threading.Tasks;
using Faml.App;
using ReactiveData;
using TypeTooling;
using TypeTooling.Xaml.XamarinForms;
using Xamarin.Forms;

[assembly: TypeToolingProvider(typeof(XamarinFormsXamlTypeToolingProvider))]

namespace Faml.XamarinForms {
    public class FamlXamarinFormsApplication : FamlApplication {
        public static FamlXamarinFormsApplication Instance;

        private readonly Application _application;
        private IReactive<ContentPage>? _currentPage;

        public static void Init(Application application, Assembly assembly, string resourceFileName) {
            if (Instance != null)
                throw new Exception("Faml.XamarinForms was already initialized");

            Instance = new FamlXamarinFormsApplication(application, assembly, resourceFileName);
        }

        public FamlXamarinFormsApplication(Application application, Assembly assembly, string resourceFileName) {
            _application = application;

            Initialize(assembly, resourceFileName);

            //appConnection.SetVisualizer(new FormsVisualizer());
            //appConnection.Start();
        }

        protected override void AddTypeToolingProviders(Program program, FamlProject project) {
            project.AddTypeToolingProvider(new XamarinFormsXamlTypeToolingProvider(program.RootProject.TypeToolingEnvironment));
        }

        public void SetCurrentPage(string functionName, Args args) {
            SetCurrentPage( ContentPageFunction.Invoke(Program, functionName, args) );
        }

        public void SetCurrentPageToWelcome() {
            var welcomePage = new ContentPage() {
                Content = new StackLayout() {
                    Children = {
                        new Label() {
                            Text = "Welcome!",
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        }
                    }
                }
            };

            SetCurrentPage(new ReactiveConstant<ContentPage>(welcomePage));
        }

        private void SetCurrentPage(IReactive<ContentPage> contentPage) {
            if (_currentPage != null)
                _currentPage.Changed -= OnCurrentPageChanged;

            _currentPage = contentPage;

            _currentPage.Changed += OnCurrentPageChanged;
            _application.MainPage = new NavigationPage(_currentPage.Value);
        }

        public void OnCurrentPageChanged() {
            Device.BeginInvokeOnMainThread(() => {
                _application.MainPage = new NavigationPage(_currentPage.Value);
            });
        }

        public async Task NavigateTo(Page page) {
            await _application.MainPage.Navigation.PushAsync(page);
        }
    }
}
