using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextEditor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fontSettings();
        }
        private void fontSettings()
        {
            fontFamilyCB.ItemsSource = Fonts.SystemFontFamilies.OrderBy(x => x.Source);
            fontSizeCB.ItemsSource = new List<double>() { 2.0, 4.0, 6.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 18.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0 };
        }

        private void fontFamilyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontFamilyCB.SelectedItem != null)
            {
                editorRTB.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamilyCB.SelectedItem);
            }
        }
        private void fontSizeCB_TextChanged(object sender, TextChangedEventArgs e)
        {
            editorRTB.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeCB.Text);
        }
        private void fontColorBTN_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextRange range = new TextRange(editorRTB.Selection.Start, editorRTB.Selection.End);
                range.ApplyPropertyValue(FlowDocument.ForegroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)));
            }
        }
        private void fontBackgroundColorBTN_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if(colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextRange range = new TextRange(editorRTB.Selection.Start, editorRTB.Selection.End);
                range.ApplyPropertyValue(FlowDocument.BackgroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B)));
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //To Microsoft.Win32 i System.Windows.Forms jest, aby można było dodać using Microsoft.Win32, żeby działał colordialog, bo inaczej visual się kłóci, bo w WPF nie ma colordialog i trzeba użyć opcji dostępnych w windows forms
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Wszystkie pliki (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(editorRTB.Document.ContentStart, editorRTB.Document.ContentEnd);
                range.Save(fileStream, System.Windows.Forms.DataFormats.Rtf);
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Wszystkie pliki (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(editorRTB.Document.ContentStart, editorRTB.Document.ContentEnd);
                range.Load(fileStream, System.Windows.Forms.DataFormats.Rtf);
            }
        }

        private void editorRTB_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object obj = editorRTB.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            fontFamilyCB.SelectedItem = obj;
            obj = editorRTB.Selection.GetPropertyValue(Inline.FontSizeProperty);
            fontSizeCB.SelectedItem = obj.ToString();
        }

        private void leftBTN_Checked(object sender, RoutedEventArgs e)
        {
            centerBTN.IsChecked = false;
            rightBTN.IsChecked = false;
            justifyBTN.IsChecked = false;
        }

        private void centerBTN_Checked(object sender, RoutedEventArgs e)
        {
            leftBTN.IsChecked = false;
            rightBTN.IsChecked = false;
            justifyBTN.IsChecked = false;
        }

        private void rightBTN_Checked(object sender, RoutedEventArgs e)
        {
            centerBTN.IsChecked = false;
            leftBTN.IsChecked = false;
            justifyBTN.IsChecked = false;
        }

        private void justifyBTN_Checked(object sender, RoutedEventArgs e)
        {
            centerBTN.IsChecked = false;
            leftBTN.IsChecked = false;
            rightBTN.IsChecked = false;
        }

        private void imageBTN_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.FileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if(dlg.ShowDialog() == true)
            {
                string file = dlg.FileName;
                Bitmap bitmap = new Bitmap(file);
                System.Windows.Clipboard.SetDataObject(bitmap);
                editorRTB.Paste();
            }
        }
        private void slowaBTN_Click(object sender, RoutedEventArgs e)
        {
            int ileZnakow = editorRTB.Selection.Text.Length;
            string text = editorRTB.Selection.Text;
            string ileSlow = text.Split(new char[] { ' ' } , StringSplitOptions.RemoveEmptyEntries).Length.ToString();
            ileZnakowLBL.Content = "Znaki: " + ileZnakow + "; Słowa: "+ileSlow;

        }
        private void dataBTN_Click(object sender, RoutedEventArgs e)
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            editorRTB.AppendText(date);
            editorRTB.CaretPosition = editorRTB.CaretPosition.DocumentEnd;
        }

        private void czasBTN_Click(object sender, RoutedEventArgs e)
        {
            string time = DateTime.Now.ToString("HH:mm");
            editorRTB.AppendText(time);
            editorRTB.CaretPosition = editorRTB.CaretPosition.DocumentEnd;
        }

        private void szukajBTN_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(editorRTB.Document.ContentStart, editorRTB.Document.ContentEnd);
            textRange.ClearAllProperties();
            ileZnalezioneLBL.Content = "";

            string editorTekst = textRange.Text;
            string szukanyTekst = szukajSlowoTB.Text;

            if (string.IsNullOrWhiteSpace(editorTekst) || string.IsNullOrWhiteSpace(szukanyTekst))
            {
                szukajSlowoTB.Background = new SolidColorBrush(Colors.OrangeRed);
                ileZnalezioneLBL.Content = "Brak tekstu do wyszukania.";
            }

            else
            {
                Regex regex = new Regex(szukanyTekst);
                int liczZnalezione = Regex.Matches(editorTekst, regex.ToString()).Count;

                for (TextPointer startPointer = editorRTB.Document.ContentStart;
                            startPointer.CompareTo(editorRTB.Document.ContentEnd) <= 0;
                                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward))
                {
                    if (startPointer.CompareTo(editorRTB.Document.ContentEnd) == 0)
                    {
                        break; //koniec tekstu
                    }

                    string parsedString = startPointer.GetTextInRun(LogicalDirection.Forward);
                    int index = parsedString.IndexOf(szukanyTekst);

                    if (index >= 0) 
                    {
                        startPointer = startPointer.GetPositionAtOffset(index);

                        if (startPointer != null)
                        {
                            TextPointer koniecPointer = startPointer.GetPositionAtOffset(szukanyTekst.Length);
                            TextRange znalezionyTekst = new TextRange(startPointer, koniecPointer);

                            znalezionyTekst.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));

                        }
                    }
                }
                if (liczZnalezione > 0)
                {
                    ileZnalezioneLBL.Content = "Znaleziono: " + liczZnalezione;
                    szukajSlowoTB.Background = new SolidColorBrush(Colors.YellowGreen);
                }
                else
                {
                    ileZnalezioneLBL.Content = "Nie znaleziono pasującego tekstu.";
                    szukajSlowoTB.Background = new SolidColorBrush(Colors.OrangeRed);
                }
            }
        }

        private void szukajSlowoTB_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            szukajSlowoTB.Background = new SolidColorBrush(Colors.White);
            szukajSlowoTB.Text = "";
        }
    }
}
