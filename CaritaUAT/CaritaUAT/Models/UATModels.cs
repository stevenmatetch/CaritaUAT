using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace CaritaUAT.Models
{

    public class Patient
    {
        public Patient()
        {
            PatPNrInternal = -1;
            PatPNr = -1;
        }


        [PrimaryKey, AutoIncrement]
        public int PatPNrInternal { get; set; }
        public int PatPNr { get; set; }
        public int AcmPNr { get; set; }

        [NotNull]
        public string Dsc { get; set; }
        [NotNull]
        public string PNR { get; set; }

        public string FirstName { get; set; }

        [Ignore]
        public Accomodation Accomodation { get; set; }

        [JsonProperty(PropertyName = "OhaL-tt")]
        [Ignore]
        public List<Survey> Surveys { get; set; }

        /* View modeller */
        [Ignore]
        public ImageSource ImagePath { get; set; }
        [Ignore]
        public string GetAccomodationName
        {
            get
            {
                if (Accomodation == null) return "Boende saknas";
                return Accomodation.Dsc;
            }
        }
        [Ignore]
        public string GetSearchStr
        {
            get
            {
                return Dsc + " " + PNR;
            }
        }

        [Ignore]
        public string LastName
        {
            get
            {
                if (Dsc == null || Dsc == "" || Dsc.IndexOf(',') == 0) return "";
                if (Dsc.IndexOf(',') < 0) return Dsc;
                return Dsc.Substring(0, Dsc.IndexOf(','));
            }
            set
            {
                Dsc = value + ", " + FirstName;
            }
        }


    }

    public class Accomodation
    {
        [PrimaryKey, AutoIncrement]
        public int AcmPNrInternal { get; set; }
        public int AcmPNr { get; set; }
        public string AcmPId { get; set; }
        public string Dsc { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Email { get; set; }
        public string Txt { get; set; }
        public bool BrwHidden { get; set; }

        [JsonProperty(PropertyName = "Patient-tt")]
        [Ignore]
        public List<Patient> Patients { get; set; }
    }


    public class Survey
    {
        [PrimaryKey, AutoIncrement]
        public int SurveyNrInternal { get; set; }
        public int SeqLNr { get; set; }
        public int PatPNr { get; set; }
        public int EcoPNr { get; set; }
        public int TeaPNr { get; set; }
        public int EmpPNr { get; set; }
        public int EmpPNrSign { get; set; }
        public DateTime? DatSign { get; set; }
        public DateTime? Dat { get; set; }
        public int OhaSNr { get; set; }
        public int PatPANr { get; set; }
        public int RfvSNr { get; set; }
        public int SeqThrL { get; set; }
        public string NameNurse { get; set; }
        public string NameHead { get; set; }
        public DateTime? DatCancel { get; set; }
        public int EmpPNrCancel { get; set; }
        public string Typ { get; set; }
        public string EmpPId { get; set; }
        public string EmpPIdSign { get; set; }
        public string EmpPIdCancel { get; set; }
        public string TypDsc { get; set; }

        [Ignore]
        [JsonProperty(PropertyName = "OhaLA-tt")]
        public List<SurveyAnswer> SurveyAnswers { get; set; }

        [Ignore]
        public Patient Patient { get; set; }
        [Ignore]
        public string FriendlyName { 
            get 
            {
                return TypDsc + " " + ((DateTime)Dat).ToString("yyyy-MM-dd") + " " + EmpPId;
            } 
        }
        public string Info
        {
            get
            {
                if (DatSign != null) return "Signerad"; else return "Påbörjad";
            }
        }
    }

    public class SurveyAnswer
    {
        [PrimaryKey, AutoIncrement]
        public int SurveyAnswersNrInternal { get; set; }
        public int SeqLNr { get; set; }
        public int SubNr { get; set; }
        public string Dsc { get; set; }
        public string OhaTyp { get; set; }
        public bool PrmLog { get; set; }
        public float PrmDec { get; set; }
        public DateTime? PrmDat { get; set; }
        public string PrmChr { get; set; }
        public string Txt { get; set; }
        public string KeyWrd { get; set; }
        public string SelList { get; set; }
        public int Multi { get; set; }
        public bool Man { get; set; }
        public bool SysVar { get; set; }
        public string Sort { get; set; }


        /* View modeller */
        [Ignore]
        public string xName
        {
            get
            {
                return SeqLNr.ToString() + SubNr.ToString();
            }
        }

        [Ignore]
        public bool HeadlineVisible
        {
            get
            {
                return OhaTyp.ToLower() == "top";
            }
        }

        [Ignore]
        public bool TextBoxVisible
        {
            get
            {
                return OhaTyp.ToLower() == "chr";
            }
        }
        [Ignore]
        public string TextBoxText
        {
            get
            {
                string[] posts = PrmChr.Split('¤');
                if (posts.Length > 1) return posts[1];
                return "";
            }
            set
            {
                PrmChr = Dsc + "¤" + value;
            }
        }
        [Ignore]
        public bool NumericVisible
        {
            get
            {
                return OhaTyp.ToLower() == "int";
            }
        }
        [Ignore]
        public int NumericValue
        {
            get
            {
                string[] posts = PrmChr.Split('¤');
                if (posts.Length > 1 && posts[1] != "") return Convert.ToInt32(posts[1]);
                return 0;
            }
            set
            {
                PrmChr = Dsc + "¤" + value.ToString();
            }
        }
        [Ignore]
        public bool SelVisible
        {
            get
            {
                return OhaTyp.ToLower() == "sel";
            }
        }
        [Ignore]
        public Editor _editor { get; set; }
        [Ignore]
        public Telerik.XamarinForms.Input.RadNumericInput _numeric { get; set; }
        [Ignore]
        public List<Telerik.XamarinForms.Primitives.RadCheckBox> _checks { get; set; }
        [Ignore]
        public List<Image> _images { get; set; }
    }


    public class SurveyForm
    {
        [PrimaryKey, AutoIncrement]
        public int OhaPNrInternal { get; set; }
        public int OhaPNr { get; set; }
        public string OhaPId { get; set; }
        public string Dsc { get; set; }
        public string Txt { get; set; }
        public string Typ { get; set; }

        [JsonProperty(PropertyName = "OhaR-tt")]
        [Ignore]
        public List<SurveyFormQuestion> Questions { get; set; }
    }

    public class SurveyFormQuestion
    {
        [PrimaryKey, AutoIncrement]
        public int OhaRNrInternal { get; set; }
        public int OhaRNr { get; set; }
        public string OhaRId { get; set; }
        public string Dsc { get; set; }
        public string Txt { get; set; }
        public string OhaTyp { get; set; }
        public string SelList { get; set; }
        public int Multi { get; set; }
        public bool Man { get; set; }
        public bool SysVar { get; set; }
        public string KeyWrd { get; set; }
        public int OhaPNr { get; set; }
        public string Sort { get; set; }

    }

    public class UATUser
    {
        [PrimaryKey, AutoIncrement]
        public int OprPNrInternal { get; set; }
        public int OprPNr { get; set; }
        public string OprPId { get; set; }
        public string Dsc { get; set; }
        public string LevelMod { get; set; }
        public string PwdChk { get; set; }

    }

}
