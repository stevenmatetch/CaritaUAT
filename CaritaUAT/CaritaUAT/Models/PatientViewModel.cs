using System.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using CaritaUAT.Data;
using System.Windows.Input;
using Telerik.XamarinForms.DataControls.ListView.Commands;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Spreadsheet.Model.DataValidation;
using CaritaUAT.Views;

namespace CaritaUAT.Models
{
    class PatientViewModel
    {
        public ObservableCollection<Patient> Items { get; set; }

        public PatientViewModel()
        {
            Items = new ObservableCollection<Patient>();
            foreach (var pat in CaritaUAT.Data.CaritaUATdb.Patients)
            {
                if (pat.ImagePath == null)
                {
                    pat.ImagePath = ImageSource.FromResource("CaritaUAT.Icons.placeholder.png", typeof(Patient).GetTypeInfo().Assembly);
                }
                Items.Add(pat);
            }
        }

    }

}
