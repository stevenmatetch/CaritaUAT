using CaritaUAT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.XamarinForms.DataControls.ListView.Commands;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CaritaUAT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HealthCardsPage : ContentPage
    {

        public HealthCardsPage()
        {
            InitializeComponent();

        }


        private void radListViewPatients_SelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {


        }

        private void radListViewPatients_ItemTapped(object sender, Telerik.XamarinForms.DataControls.ListView.ItemTapEventArgs e)
        {
            if (e.Item != null)
            {
                CaritaUAT.Models.Patient thisPat = (CaritaUAT.Models.Patient)e.Item;

                HealthCardPatientPage patPage = new HealthCardPatientPage(thisPat);
                Navigation.PushAsync(patPage);

            }

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            NewPatientPage patPage = new NewPatientPage();
            Navigation.PushAsync(patPage);
        }
        private void Update_Clicked(object sender, EventArgs e)
        {
            radListViewPatients.ItemsSource = new PatientsHealthCardsViewModel().Items;
        }

        private void radListViewPatients_RefreshRequested(object sender, Telerik.XamarinForms.DataControls.ListView.PullToRefreshRequestedEventArgs e)
        {
            radListViewPatients.ItemsSource = new PatientsHealthCardsViewModel().Items;
            radListViewPatients.EndRefresh();
        }
    }
}