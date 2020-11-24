using CaritaUAT.Data;
using CaritaUAT.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CaritaUAT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HealthCardPatientPage : ContentPage
    {
        public Patient _patient { get; set; }


        public ObservableCollection<Survey> Items { get; set; }

        public HealthCardPatientPage(Patient patient)
        {

            _patient = patient;
            if (_patient.Surveys == null)
            {
                _patient.Surveys = CaritaUATdb.GetPatientSurveys(_patient);
            }

            Items = new ObservableCollection<Survey>();
            foreach (var sur in _patient.Surveys)
            {
                Items.Add(sur);
            }


            InitializeComponent();

            this.Title = _patient.Dsc + " " + _patient.PNR;

        }


        private void radListViewSurveys_ItemTapped(object sender, Telerik.XamarinForms.DataControls.ListView.ItemTapEventArgs e)
        {
            if (e.Item != null)
            {
                Survey survey = (CaritaUAT.Models.Survey)e.Item;
                SurveyEditPage surveyPage = new SurveyEditPage(_patient, survey);
                Navigation.PushAsync(surveyPage);
            }

        }

        private void AddHealthCard_Clicked(object sender, EventArgs e)
        {
            SurveyForm thisForm;

            try
            {
                thisForm = CaritaUATdb.SurveyForms.First(x => x.Typ == "HLA");
            }
            catch
            {
                DisplayAlert("Fel", "Inga hälsokortformulär sparade i appen!", "Ok");
                return;
            }

            Survey survey = CaritaUATdb.CreateSurveyFromForm(thisForm);

            survey.PatPNr = _patient.PatPNr;

            SurveyEditPage surveyPage = new SurveyEditPage(_patient, survey);
            Navigation.PushAsync(surveyPage);
        }
        private void AddMouthCard_Clicked(object sender, EventArgs e)
        {

            // OHA

            SurveyForm thisForm;

            try
            {
                thisForm = CaritaUATdb.SurveyForms.First(x => x.Typ == "OHA");
            }
            catch
            {
                DisplayAlert("Fel", "Inga munvårdskort sparade i appen!", "Ok");
                return;
            }

            Survey survey = CaritaUATdb.CreateSurveyFromForm(thisForm);

            survey.PatPNr = _patient.PatPNr;

            SurveyEditPage surveyPage = new SurveyEditPage(_patient, survey);
            Navigation.PushAsync(surveyPage);

        }


        private void radListViewSurveys_RefreshRequested(object sender, Telerik.XamarinForms.DataControls.ListView.PullToRefreshRequestedEventArgs e)
        {

            _patient.Surveys = CaritaUATdb.GetPatientSurveys(_patient);


            Items = new ObservableCollection<Survey>();
            foreach (var sur in _patient.Surveys)
            {
                Items.Add(sur);
            }

            radListViewSurveys.ItemsSource = Items;
            radListViewSurveys.EndRefresh();

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            _patient.Surveys = CaritaUATdb.GetPatientSurveys(_patient);
            Items = new ObservableCollection<Survey>();
            foreach (var sur in _patient.Surveys)
            {
                Items.Add(sur);
            }
            radListViewSurveys.ItemsSource = Items;
        }
    }
}