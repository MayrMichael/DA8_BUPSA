using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;


namespace Brenn_und_Plasmaschneidanlage
{
    /// <summary>
    /// Interaktionslogik für NewSubprogram.xaml
    /// </summary>
    public partial class NewSubprogram : Window
    {
        private List<string> wrongNames;

        private string name;
        private string path;
        
        public NewSubprogram(List<string> names)
        {
            InitializeComponent();
            wrongNames = names;
            txtContent.ToolTip = "Bitte einen Namen eingeben";

        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void RbNewSub_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSearch.Visibility = Visibility.Collapsed;
                gbContent.Header = "Neues Unterprogramm";
                txtContent.Text = "";
                rbImport.IsChecked = true;
            }
            catch (Exception) { }
 
        }

        private void RbExistingSub_Checked(object sender, RoutedEventArgs e)
        {
            btnSearch.Visibility = Visibility.Visible;
            gbContent.Header = "Vorhandenes Unterprogramm";
            txtContent.Text = Properties.Settings.Default.subProgramDirectoryPath;
            txtContent.ToolTip = Properties.Settings.Default.subProgramDirectoryPath;
            txtContent.Background = Brushes.White;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Schneidunterprogramm (*.ucut)|*.ucut|Alle Dateien (*.*)|*.*",
                InitialDirectory = @Properties.Settings.Default.subProgramDirectoryPath
            };
            if (openFileDialog.ShowDialog() == true)
            {
                txtContent.Text = openFileDialog.FileName;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void TxtContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isSpellRight = false;
            bool isNameRight = false;

            if (rbNewSub.IsChecked == true)
            {
                foreach (string name in wrongNames)
                {
                    if (name == txtContent.Text.Trim(' '))
                    {
                        txtContent.ToolTip = "Der aktuelle Name wird schon verwendet";
                        isNameRight = false;
                        break;
                    }
                    else
                    {
                        txtContent.ToolTip = "";
                        isNameRight = true;
                    }
                        
                }

                Regex regex = new Regex(@"^[a-zA-Z0-9_]+$");

                // Compare a string against the regular expression
                if (!regex.IsMatch(txtContent.Text))
                {
                    if (txtContent.ToolTip.ToString() != "")
                        txtContent.ToolTip += Environment.NewLine + "Mindestens ein Zeichen ist ungültig!";
                    else
                        txtContent.ToolTip = "Mindestens ein Zeichen ist ungültig!";

                }
                else
                    isSpellRight = true;

                if (isNameRight && isSpellRight && txtContent.Text != "")
                {
                    txtContent.Background = Brushes.White;
                    btnOK.IsEnabled = true;
                    txtContent.ToolTip = "Name des neuen Unterprogramms";
                }
                else if(txtContent.Text == "")
                {
                    txtContent.Background = Brushes.White;
                    txtContent.ToolTip = "Bitte einen Namen eingeben";
                    btnOK.IsEnabled = false;
                }
                else
                {
                    btnOK.IsEnabled = false;
                    txtContent.Background = Brushes.Red;
                }

                name = txtContent.Text;
                path = "";
                   
            }
            else
            {
                string[] splitPath = txtContent.Text.Split('\\');
                string nameForCheck = splitPath[splitPath.Length - 1].Split('.')[0];

                txtContent.ToolTip = txtContent.Text;


                foreach (string name in wrongNames)
                {
                    if (name == nameForCheck)
                    {
                        isNameRight = false;
                        break;
                    }
                    else
                    {
                        isNameRight = true;
                    }

                }

                if (isNameRight)
                {
                    btnOK.IsEnabled = true;
                    txtContent.Background = Brushes.White;
                }
                else if (nameForCheck == "")
                {
                    btnOK.IsEnabled = false;
                    txtContent.Background = Brushes.White;
                }   
                else
                {
                    btnOK.IsEnabled = false;
                    txtContent.Background = Brushes.Red;
                    txtContent.ToolTip += Environment.NewLine + "Der aktuelle Name wird schon verwendet";
                }

                name = nameForCheck;
                path = txtContent.Text;
            }

        }

        public Program Subprogram
        {
            get { return new Program(name, rbImport.IsChecked == true, false, "", path); }
        }
    }
}
