using CaritaUAT.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Xamarin.Forms;

namespace CaritaUAT.Data
{


    public class CaritaUATdb
    {

        /*
 * db passphrase dev
 * 
 * #CaritaUAT23# */


        static object locker = new object();

        private static SQLiteConnection database;


        public static bool CreateDatabase(SQLiteConnection conn)
        {
            try
            {
                conn.CreateTable<Models.Accomodation>();
                conn.CreateTable<Models.Patient>();
                conn.CreateTable<Models.Survey>();
                conn.CreateTable<Models.SurveyAnswer>();
                conn.CreateTable<Models.SurveyForm>();
                conn.CreateTable<Models.SurveyFormQuestion>();
                conn.CreateTable<Models.UATUser>();

                conn.CreateIndex("IdxAccomodationAcmPNr", "Accomodation", "AcmPNr", true);
                conn.CreateIndex("IdxPatientPatPNr", "Patient", "PatPNr", false);
                conn.CreateIndex("IdxSurveyAnswerSeqLNr", "SurveyAnswer", "SeqLNr", false);
                conn.CreateIndex("IdxSurveyFormOhaPNr", "SurveyForm", "OhaPNr", true);
                conn.CreateIndex("IdxSurveyFormQuestionOhaPNr", "SurveyFormQuestion", "OhaPNr", false);
                conn.CreateIndex("IdxSurveyFormQuestionOhaRNr", "SurveyFormQuestion", "OhaRNr", true);
                conn.CreateIndex("IdxSurveyPatPNr", "Survey", "PatPNr", false);
                conn.CreateIndex("IdxSurveySeqLNr", "Survey", "SeqLNr", false);
                conn.CreateIndex("IdxUATUserOprPNr", "UATUser", "OprPNr", true);

            } catch
            {
                return false;
            }


            return true;
        }

        public static bool LoadDbData()
        {
            try
            {
                database = DependencyService.Get<ISQLite>().GetConnection();


                _patients = database.Query<Patient>("SELECT * FROM Patient");
                List<Accomodation> acms = database.Query<Accomodation>("SELECT * FROM Accomodation");
                foreach (var pat in _patients)
                {
                    pat.Accomodation = acms.FirstOrDefault(x => x.AcmPNr == pat.AcmPNr);
                }

                _users = database.Query<UATUser>("SELECT * FROM UATUser");

                _surveyforms = database.Query<SurveyForm>("SELECT * From SurveyForm");
                foreach(var frm in _surveyforms)
                {
                    frm.Questions = database.Query<SurveyFormQuestion>("SELECT * FROm SurveyFormQuestion WHERE OhaPNr = ?", new object[] { frm.OhaPNr });
                }


            } catch (Exception ex)
            {
                return false;
            }
             
            return true;
        }

        public static byte[] GetEmptyDatabaseFile()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            var s = a.GetManifestResourceStream("CaritaUAT.Files.CaritaUATdb.db3");

            MemoryStream ms = new MemoryStream();
            s.CopyTo(ms);

            return ms.ToArray();
        }


        #region  Dataset för Integration

        public class DsUATAppDataRoot
        {
            public DsUATAppData dsUATAppData { get; set; }

        }

        public class DsUATAppData
        {
            [JsonProperty(PropertyName = "AcmP-tt")]
            public List<Accomodation> Accomodations { get; set; }

        }

        public class DsUATUsersRoot
        {
            public DsUATUsers dsUATUsers { get; set; }

        }

        public class DsUATUsers
        {
            [JsonProperty(PropertyName = "UATUser-tt")]
            public List<UATUser> Users { get; set; }

        }


        public class DsUATQuestionsRoot
        {
            public DsUATQuestions dsUATQuestions { get; set; }

        }

        public class DsUATQuestions
        {
            [JsonProperty(PropertyName = "OhaP-tt")]
            public List<SurveyForm> SurveyForms { get; set; }

        }

        #endregion

        public static List<Survey> GetPatientSurveys(Patient pat)
        {

            List<Survey> surs = database.Query<Survey>("SELECT * FROM Survey WHERE PatPNr = ?", new object[] { pat.PatPNr });
            foreach (var sur in surs)
            {
                sur.SurveyAnswers = database.Query<SurveyAnswer>("SELECT * FROM SurveyAnswer WHERE SeqLNr = ?", new object[] { sur.SeqLNr });
            }

            return surs;
        }

        public static bool PatientAddOrUpdate(Patient pat)
        {

            int res = -1;

            try
            {
                var thispat = database.Query<Patient>("SELECT * FROM Patient WHERE PatPNr = ?", new object[] { pat.PatPNr });
                if (thispat != null) res = thispat.Count; else res = -1;

                if (res == 0)
                {
                    res = database.Insert(pat);
                }
                else if (res == 1)
                {
                    res = database.Update(pat);
                }
                else
                {
                    return false;
                }
            }
            catch {
                return false; 
            }
            return true;
        }

        public static int GetNewPatPNr()
        {
            int newpatpnr = database.ExecuteScalar<int>("SELECT MIN(PatPNr) FROM Patient");
            if (newpatpnr > 0) newpatpnr = 0;
            newpatpnr--;
            return newpatpnr;
        }
        public static int GetNewSeqLNr()
        {
            int newseqlnr = database.ExecuteScalar<int>("SELECT MIN(SeqLNr) FROM Survey");
            if (newseqlnr > 0) newseqlnr = 0;
            newseqlnr--;
            return newseqlnr;
        }

        public static bool SurveyAddOrUpdate(Survey sur)
        {

            int res = -1;

            try
            {
                var thissur = database.Query<Survey>("SELECT * FROM Survey WHERE SeqLNr = ?", new object[] { sur.SeqLNr });
                if (thissur != null) res = thissur.Count; else res = -1;

                if (res == 0)
                {
                    res = database.Insert(sur);
                }
                else if (res == 1)
                {
                    res = database.Update(sur);
                }
                else
                {
                    return false;
                }

                database.Execute("DELETE FROM SurveyAnswer WHERE SeqLNr = ?", new object[] { sur.SeqLNr });

                if (sur.SurveyAnswers != null)
                {
                    foreach (var surans in sur.SurveyAnswers)
                    {
                        bool res2 = SureyAnswerAddOrUpdate(surans);
                        if (!res2)
                        {
                            res2 = false;
                        }
                    }
                }

            } catch { return false; }
            return true;
        }

        public static bool SureyAnswerAddOrUpdate(SurveyAnswer surans)
        {

            int res = -1;

            try
            {
                var thissurans = database.Query<SurveyAnswer>("SELECT * FROM SurveyAnswer WHERE SeqLNr = ? AND SubNr = ?", new object[] { surans.SeqLNr, surans.SubNr });
                if (thissurans != null) res = thissurans.Count; else res = -1;

                if (res == 0)
                {
                    res = database.Insert(surans);
                }
                else if (res == 1)
                {
                    res = database.Update(surans);
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
            return true;
        }

        public static bool AccomodationAddOrUpdate(Accomodation acc)
        {

            int res = -1;

            try
            {
                var thisacc = database.Query<Accomodation>("SELECT * FROM Accomodation WHERE AcmPNr = ?", new object[] { acc.AcmPNr});
                if (thisacc != null) res = thisacc.Count; else res = -1;

                if (res == 0)
                {
                    res = database.Insert(acc);
                }
                else if (res == 1)
                {
                    res = database.Update(acc);
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
            return true;
        }

        public static bool SurveyFormAddOrUpdate(SurveyForm frm)
        {

            int res = -1;

            try
            {
                var thisfrm = database.Query<SurveyForm>("SELECT * FROM SurveyForm WHERE OhaPNr = ?", new object[] { frm.OhaPNr });
                if (thisfrm != null) res = thisfrm.Count; else res = -1;

                if (res == 0)
                {
                    res = database.Insert(frm);
                }
                else if (res == 1)
                {
                    res = database.Update(frm);
                }
                else
                {
                    return false;
                }

                database.Execute("DELETE FROM SurveyFormQuestion WHERE OhaPNr = ?", new object[] { frm.OhaPNr });

                if (frm.Questions != null)
                {
                    foreach (var frmq in frm.Questions)
                    {
                        bool res2 = SureyFormQuestionAddOrUpdate(frmq);
                        if (!res2)
                        {
                            res2 = false;
                        }
                    }
                }
            }
            catch { return false; }
            return true;
        }

        public static bool SureyFormQuestionAddOrUpdate(SurveyFormQuestion frmq)
        {

            int res = -1;

            try
            {
                var thisfrmq = database.Query<SurveyAnswer>("SELECT * FROM SurveyFormQuestion WHERE OhaRNr = ?", new object[] { frmq.OhaRNr });
                if (thisfrmq != null) res = thisfrmq.Count; else res = -1;

                if (res == 0)
                {
                    res = database.Insert(frmq);
                }
                else if (res == 1)
                {
                    res = database.Update(frmq);
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
            return true;
        }

        public static bool UATUserAddOrUpdate(UATUser opr)
        {

            int res = -1;

            try
            {
                var thisopr = database.Query<SurveyForm>("SELECT * FROM UATUser WHERE OprPNr = ?", new object[] { opr.OprPNr });
                if (thisopr != null) res = thisopr.Count; else res = -1;

                if (res == 0)
                {
                    res = database.Insert(opr);
                }
                else if (res == 1)
                {
                    res = database.Update(opr);
                }
                else
                {
                    return false;
                }

            }
            catch { return false; }
            return true;
        }



        private static List<Patient> _patients;
        public static List<Patient> Patients
        {
            get
            {
                if (_patients == null)
                {
                    _patients = new List<Patient>();
                    //_patients = database.Query<Patient>("SELECT * FROM Patient");
                }
                return _patients;
            }
            set { _patients = value; }
        }


        private static List<SurveyForm> _surveyforms;
        public static List<SurveyForm> SurveyForms
        {
            get
            {
                if (_surveyforms == null)
                {
                    _surveyforms = new List<SurveyForm>();
                   // _surveyforms = database.Query<SurveyForm>("SELECT * FROM SurveyForm");
                }
                return _surveyforms;
            }
            set
            {
                _surveyforms = value;
            }
        }

        private static List<UATUser> _users;
        public static List<UATUser> Users
        {
            get
            {
                if (_users == null)
                {
                    //_users = database.Query<UATUser>("SELECT * FROM UATUser");
                    _users = new List<UATUser>();
                }
                return _users;
            }
            set { _users = value; }
        }

        public static Survey CreateSurveyFromForm(SurveyForm thisForm)
        {
            Survey survey = new Survey();
            survey.SurveyAnswers = new List<SurveyAnswer>();

            survey.SeqLNr = GetNewSeqLNr();
            survey.Dat = DateTime.Now.Date;
            survey.Typ = thisForm.Typ;
            int subnr = 0;

            switch (survey.Typ.ToLower())
            {
                case "hla":
                    survey.TypDsc = "Hälsovårdskort";
                    break;
                case "oha":
                    survey.TypDsc = "Munvårdskort";
                    break;
            }

            foreach (var q in thisForm.Questions)
            {
                subnr++;
                SurveyAnswer newAnswer = new SurveyAnswer();
                newAnswer.SeqLNr = survey.SeqLNr;
                newAnswer.SubNr = subnr;

                newAnswer.Dsc = q.Dsc;
                newAnswer.KeyWrd = q.KeyWrd;
                newAnswer.Man = q.Man;
                newAnswer.Multi = q.Multi;
                newAnswer.OhaTyp = q.OhaTyp;
                newAnswer.SelList = q.SelList;
                newAnswer.Sort = q.Sort;
                newAnswer.SysVar = q.SysVar;
                newAnswer.Txt = q.Txt;                

                switch (q.OhaTyp.ToLower())
                {
                    case "sel":
                        newAnswer.PrmDec = q.Multi;
                        newAnswer.PrmChr = q.Dsc;
                        string[] parts = q.SelList.Split('¤');
                        if (parts.Length > 0)
                        {
                            for (int i = 0; i < parts.Length; i++)
                            {
                                newAnswer.PrmChr += "¤" + parts[i] + "|0";
                            }
                        }
                        break;
                    case "int":
                        newAnswer.PrmChr = q.Dsc + "¤0";
                        break;
                    case "chr":
                        newAnswer.PrmChr = q.Dsc + "¤";
                        break;
                    case "top":
                        newAnswer.PrmChr = q.Dsc;
                        break;
                }

                survey.SurveyAnswers.Add(newAnswer);

            }

            return survey;

        }

    }
}
