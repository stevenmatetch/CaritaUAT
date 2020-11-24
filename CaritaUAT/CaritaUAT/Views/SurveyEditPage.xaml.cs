using CaritaUAT.Data;
using CaritaUAT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Chart;
using Telerik.XamarinForms.Input;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CaritaUAT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyEditPage : ContentPage
    {
        public Patient _patient { get; set; }
        public Survey _survey { get; set; }

        public SurveyEditPage(Patient patient, Survey survey)
        {
            InitializeComponent();

            _patient = patient;
            _survey = survey;

            this.Title = _patient.Dsc + " " + _patient.PNR;

            foreach (var q in _survey.SurveyAnswers)
            {
                Label headline = new Label();
                headline.Text = q.Dsc;
                headline.HorizontalOptions = LayoutOptions.FillAndExpand;
                stackMain.Children.Add(headline);

                switch (q.OhaTyp.ToLower())
                {
                    case "top":
                        headline.BackgroundColor = Color.FromRgb(135, 206, 234); // Samma som default Telerik i Carita
                        headline.FontSize = headline.FontSize * 1.5;
                        headline.FontAttributes = FontAttributes.Bold;
                        break;
                    case "sel":
                        headline.FontSize = headline.FontSize * 1.25;
                        string[] sel = q.PrmChr.Split('¤');
                        if (sel.Length > 1)
                        {
                            if (q.PrmDec == 0)
                            {
                                q._checks = new List<RadCheckBox>();
                                for (int i = 0; i < sel.Length - 1; i++)
                                {
                                    StackLayout stack = new StackLayout();
                                    stack.Orientation = StackOrientation.Horizontal;
                                    string[] vals = sel[i + 1].Split('|');
                                    RadCheckBox button = new RadCheckBox();
                                    q._checks.Add(button);
                                    button.AutomationId = vals[0] + "|" + vals[1];
                                    if (vals[2] == "1") button.IsChecked = true;
                                    Label label = new Label();
                                    label.Text = vals[0];
                                    stack.Children.Add(button);
                                    stack.Children.Add(label);
                                    stackMain.Children.Add(stack);
                                    button.IsCheckedChanged += Button_IsCheckedChanged;
                                }

                            }
                            else
                            {
                                q._checks = new List<RadCheckBox>();
                                for (int i = 0; i < sel.Length - 1; i++)
                                {
                                    StackLayout stack = new StackLayout();
                                    stack.Orientation = StackOrientation.Horizontal;
                                    string[] vals = sel[i + 1].Split('|');
                                    RadCheckBox button = new RadCheckBox();
                                    q._checks.Add(button);
                                    button.AutomationId = vals[0] + "|" + vals[1];
                                    if (vals[2] == "1") button.IsChecked = true;
                                    Label label = new Label();
                                    label.Text = vals[0];
                                    stack.Children.Add(button);
                                    stack.Children.Add(label);
                                    stackMain.Children.Add(stack);
                                }
                            }
                        }
                        break;
                    case "chr":
                        headline.FontSize = headline.FontSize * 1.25;
                        Editor editor = new Editor();
                        q._editor = editor;
                        editor.HorizontalOptions = LayoutOptions.FillAndExpand;
                        editor.HeightRequest = 80;
                        editor.Text = q.TextBoxText;
                        stackMain.Children.Add(editor);
                        break;
                    case "int":
                        headline.FontSize = headline.FontSize * 1.25;
                        RadNumericInput numeric = new RadNumericInput();
                        q._numeric = numeric;
                        numeric.Value = q.NumericValue;
                        numeric.HorizontalOptions = LayoutOptions.Start;
                        stackMain.Children.Add(numeric);
                        break;
                    case "tooth":
                        headline.FontSize = headline.FontSize * 1.25;
                        q._images = new List<Image>();

                        Grid outerGrid = new Grid();
                        outerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        outerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        outerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        outerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        outerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        outerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                        Label newLabel;

                        newLabel = new Label() { Text = "ÖK", HorizontalTextAlignment = TextAlignment.Center };
                        Grid.SetColumn(newLabel, 1);
                        Grid.SetRow(newLabel, 0);
                        outerGrid.Children.Add(newLabel);

                        newLabel = new Label() { Text = "UK", HorizontalTextAlignment = TextAlignment.Center };
                        Grid.SetColumn(newLabel, 1);
                        Grid.SetRow(newLabel, 2);
                        outerGrid.Children.Add(newLabel);

                        newLabel = new Label() { Text = "HÖ", VerticalTextAlignment = TextAlignment.Center };
                        Grid.SetColumn(newLabel, 0);
                        Grid.SetRow(newLabel, 1);
                        outerGrid.Children.Add(newLabel);

                        newLabel = new Label() { Text = "VÄ", VerticalTextAlignment = TextAlignment.Center };
                        Grid.SetColumn(newLabel, 2);
                        Grid.SetRow(newLabel, 1);
                        outerGrid.Children.Add(newLabel);

                        StackLayout innerStack = new StackLayout() { Orientation = StackOrientation.Vertical };
                        Grid.SetColumn(innerStack, 1);
                        Grid.SetRow(innerStack, 1);
                        outerGrid.Children.Add(innerStack);

                        Grid newGrid = new Grid() { ColumnSpacing = 0, RowSpacing = 0 };
                        newGrid.RowDefinitions.Add(new RowDefinition());
                        for (int i = 0; i < 16; i++) newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        Image newImg;
                        for (int i = 0; i < 16; i++)
                        {
                            newImg = new Image() { Source = ImageSource.FromResource("CaritaUAT.Icons.t" + i.ToString() + ".png", typeof(Patient).GetTypeInfo().Assembly) };
                            Grid.SetColumn(newImg, i);
                            newGrid.Children.Add(newImg);
                            switch (q.PrmChr.Substring(i, 1))
                            {
                                case "0":
                                    // Vit
                                    newImg.BackgroundColor = Color.White;
                                    break;
                                case "1":
                                    // Röd
                                    newImg.BackgroundColor = Color.Red;
                                    break;
                                case "2":
                                    // Blå
                                    newImg.BackgroundColor = Color.Blue;
                                    break;
                                case "3":
                                    // Svart
                                    newImg.BackgroundColor = Color.Black;
                                    break;
                            }
                            TapGestureRecognizer newTap = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
                            newTap.Tapped += NewTap_Tapped;
                            newImg.GestureRecognizers.Add(newTap);
                            q._images.Add(newImg);

                        }
                        innerStack.Children.Add(newGrid);

                        newGrid = new Grid() { ColumnSpacing = 0, RowSpacing = 0 };
                        newGrid.RowDefinitions.Add(new RowDefinition());
                        for (int i = 0; i < 16; i++) newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        for (int i = 0; i < 16; i++)
                        {
                            newImg = new Image() { Source = ImageSource.FromResource("CaritaUAT.Icons.t" + (i + 16).ToString() + ".png", typeof(Patient).GetTypeInfo().Assembly) };
                            Grid.SetColumn(newImg, i);
                            newGrid.Children.Add(newImg);
                            switch (q.PrmChr.Substring(i + 16, 1))
                            {
                                case "0":
                                    // Vit
                                    newImg.BackgroundColor = Color.White;
                                    break;
                                case "1":
                                    // Röd
                                    newImg.BackgroundColor = Color.Red;
                                    break;
                                case "2":
                                    // Blå
                                    newImg.BackgroundColor = Color.Blue;
                                    break;
                                case "3":
                                    // Svart
                                    newImg.BackgroundColor = Color.Black;
                                    break;
                            }
                            TapGestureRecognizer newTap = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
                            newTap.Tapped += NewTap_Tapped;
                            newImg.GestureRecognizers.Add(newTap);
                            q._images.Add(newImg);
                        }
                        innerStack.Children.Add(newGrid);
                        stackMain.Children.Add(outerGrid);

                        newGrid = new Grid();
                        newGrid.RowDefinitions.Add(new RowDefinition());
                        newGrid.RowDefinitions.Add(new RowDefinition());
                        newGrid.RowDefinitions.Add(new RowDefinition());
                        newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                        BoxView newBox;
                        newBox = new BoxView() { BackgroundColor = Color.Red, WidthRequest = 16, HeightRequest = 16 };
                        Grid.SetRow(newBox, 0);
                        Grid.SetColumn(newBox, 0);
                        newGrid.Children.Add(newBox);
                        newBox = new BoxView() { BackgroundColor = Color.Blue, WidthRequest = 16, HeightRequest = 16 };
                        Grid.SetRow(newBox, 1);
                        Grid.SetColumn(newBox, 0);
                        newGrid.Children.Add(newBox);
                        newBox = new BoxView() { BackgroundColor = Color.Black, WidthRequest = 16, HeightRequest = 16 };
                        Grid.SetRow(newBox, 2);
                        Grid.SetColumn(newBox, 0);
                        newGrid.Children.Add(newBox);

                        newLabel = new Label() { Text = "Avtagbart" };
                        Grid.SetRow(newLabel, 0);
                        Grid.SetColumn(newLabel, 1);
                        newGrid.Children.Add(newLabel);
                        newLabel = new Label() { Text = "Fastsittande tänder" };
                        Grid.SetRow(newLabel, 1);
                        Grid.SetColumn(newLabel, 1);
                        newGrid.Children.Add(newLabel);
                        newLabel = new Label() { Text = "Saknad tand" };
                        Grid.SetRow(newLabel, 2);
                        Grid.SetColumn(newLabel, 1);
                        newGrid.Children.Add(newLabel);

                        stackMain.Children.Add(newGrid);

                        break;
                }
            }

        }

        private void NewTap_Tapped(object sender, EventArgs e)
        {
            int i;
            foreach (var q in _survey.SurveyAnswers)
            {
                if (q._images != null)
                {
                    i = 0;
                    foreach (var img in q._images)
                    {
                        if (img == sender)
                        {
                            StringBuilder sb = new StringBuilder(q.PrmChr);
                            int val = Convert.ToInt32(sb[i]);
                            val++;
                            if (val > '3') val = '0';
                            switch (val)
                            {
                                case '0': sb[i] = '0'; img.BackgroundColor = Color.White; break;
                                case '1': sb[i] = '1'; img.BackgroundColor = Color.Red; break;
                                case '2': sb[i] = '2'; img.BackgroundColor = Color.Blue; break;
                                case '3': sb[i] = '3'; img.BackgroundColor = Color.Black; break;
                            }
                            q.PrmChr = sb.ToString();
                        }
                        i++;
                    }
                }
            }
        }

        private void Button_IsCheckedChanged(object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            if (e.NewValue == true)
            {
                foreach (var q in _survey.SurveyAnswers)
                {
                    if (q._checks != null)
                    {
                        foreach (var chk in q._checks)
                        {
                            if (chk == sender)
                            {
                                foreach (var unchk in q._checks)
                                {
                                    if (unchk != sender) unchk.IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            foreach (var q in _survey.SurveyAnswers)
            {
                if (q.OhaTyp.ToLower() == "int")
                {
                    q.NumericValue = Convert.ToInt32(q._numeric.Value);
                }
                if (q.OhaTyp.ToLower() == "chr")
                {
                    q.TextBoxText = q._editor.Text;
                }
                if (q.OhaTyp.ToLower() == "sel")
                {
                    q.PrmChr = q.Dsc;
                    for (int i = 0; i < q._checks.Count; i++)
                    {
                        if (q._checks[i].IsChecked == true)
                            q.PrmChr += "¤" + q._checks[i].AutomationId + "|" + "1";
                        else
                            q.PrmChr += "¤" + q._checks[i].AutomationId + "|" + "0";
                    }
                }
            }

            CaritaUATdb.SurveyAddOrUpdate(_survey);

            Navigation.PopAsync();
        }
        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}