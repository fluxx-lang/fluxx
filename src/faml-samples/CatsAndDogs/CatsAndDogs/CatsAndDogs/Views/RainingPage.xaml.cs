using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CatsAndDogs.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RainingPage : ContentPage
    {
        int count = 0;

        public RainingPage() {
            InitializeComponent();
            updateWeather();
        }

        private void MoreRain_Clicked(object sender, EventArgs e) {
            ++count;
            updateWeather();
        }

        private void updateWeather() {
            if (count == 0) {
                WeatherLabel.Text = "It's not raining.";
            }
            else {
                WeatherLabel.Text = $"It's raining {count} cats and dogs.";
            }
        }
    }
}
