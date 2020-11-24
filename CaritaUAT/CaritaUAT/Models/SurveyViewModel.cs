using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CaritaUAT.Models
{

    public class SurveyViewModel
    {

        public ObservableCollection<SurveyAnswer> Items { get; set; }

        public SurveyViewModel()
        {
            Items = new ObservableCollection<SurveyAnswer>();
        }

    }

}