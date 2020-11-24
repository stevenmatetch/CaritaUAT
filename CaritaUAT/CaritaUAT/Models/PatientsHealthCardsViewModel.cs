using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace CaritaUAT.Models
{
    class PatientsHealthCardsViewModel
    {
        public ObservableCollection<Patient> Items { get; set; }

        public PatientsHealthCardsViewModel()
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
