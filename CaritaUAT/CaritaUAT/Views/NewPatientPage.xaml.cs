using CaritaUAT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CaritaUAT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPatientPage : ContentPage
    {

        public Patient Patient { get; set; }

        public NewPatientPage()
        {
            Patient = new Patient();

            Patient.PatPNr = Data.CaritaUATdb.GetNewPatPNr();

            InitializeComponent();
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            int j = -1;

            CaritaUAT.Data.CaritaUATdb.PatientAddOrUpdate(Patient);
            Navigation.PopAsync();


        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}