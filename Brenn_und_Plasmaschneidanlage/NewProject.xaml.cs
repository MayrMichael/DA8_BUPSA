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
using System.IO;
using System.Collections;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Brenn_und_Plasmaschneidanlage
{
    /// <summary>
    /// Interaktionslogik für NewProject.xaml
    /// </summary>
    public partial class NewProject : MetroWindow
    {
        private string standardDirectoryPath = @Properties.Settings.Default.standardDirectoryPath;




        private Dictionary<string, string> dictSubProgramsPath = new Dictionary<string, string>();
        private List<string> mySubProgramList = new List<string>();
        string currentItemText;
        int currentItemIndex;

        const string PRGEXTENSION = ".pcut";

        const string SUBPRGEXTENSION = ".ucut";

        public NewProject()
        {
            InitializeComponent();

            DataContext = this;

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(@Properties.Settings.Default.subProgramDirectoryPath);
                FileInfo[] files = directoryInfo.GetFiles("*" + SUBPRGEXTENSION); //Getting Text files
                foreach (FileInfo file in files)
                {
                    string fileName = file.Name.Replace(file.Extension , "");
                    dictSubProgramsPath.Add(fileName , file.FullName);
                    mySubProgramList.Add(fileName);
                }
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Der Standardorder für die Unterprogramme konnte nicht gefunden werden!" + Environment.NewLine + "Bitte kontrollieren Sie den Verzeichnisname!" +
                    Environment.NewLine + "Verzeichnispfad: " + @Properties.Settings.Default.subProgramDirectoryPath , "Fehler" , MessageBoxButton.OK , MessageBoxImage.Error);
            }

            lbPossibleSubPrograms.ItemsSource = mySubProgramList;

            if (mySubProgramList.Count == 0)
            {
                lbSubPrograms.IsEnabled = false;
                lbPossibleSubPrograms.IsEnabled = false;
                btnExport.IsEnabled = false;
                btnImport.IsEnabled = false;
            }
            else
            {
                btnImport.IsEnabled = true;
            }


            txtFilePath.Text = standardDirectoryPath;

            txtName.Focus();

            btnExport.IsEnabled = false;
        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {

            if (lbPossibleSubPrograms.SelectedIndex != -1)
            {


                // Find the right item and it's value and index
                currentItemText = lbPossibleSubPrograms.SelectedValue.ToString();
                currentItemIndex = lbPossibleSubPrograms.SelectedIndex;


            }
            else
            {
                currentItemText = lbPossibleSubPrograms.Items[0].ToString();
                currentItemIndex = 0;
            }

            lbSubPrograms.Items.Add(currentItemText);
            if (mySubProgramList != null)
            {
                mySubProgramList.RemoveAt(currentItemIndex);
            }

            if (mySubProgramList.Count == 0)
                btnImport.IsEnabled = false;

            btnExport.IsEnabled = true;

            // Refresh data binding
            ApplyDataBinding();

        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            // Find the right item and it's value and index

            btnImport.IsEnabled = true;

            if (lbSubPrograms.SelectedIndex != -1)
            {
                currentItemText = lbSubPrograms.SelectedValue.ToString();
                currentItemIndex = lbSubPrograms.SelectedIndex;

                // LeftListBox.Items.Add(RightListBox.SelectedItem);
                lbSubPrograms.Items.RemoveAt(lbSubPrograms.Items.IndexOf(lbSubPrograms.SelectedItem));
            }
            else
            {
                if (lbSubPrograms.Items.Count != 0)
                {
                    currentItemText = lbSubPrograms.Items[0].ToString();
                    currentItemIndex = 0;

                    // LeftListBox.Items.Add(RightListBox.SelectedItem);
                    lbSubPrograms.Items.RemoveAt(0);
                }
            }

            if (lbSubPrograms.Items.Count == 0)
                btnExport.IsEnabled = false;

            // Add RightListBox item to the ArrayList
            mySubProgramList.Add(currentItemText);

            mySubProgramList.Sort();

            // Refresh data binding
            ApplyDataBinding();

        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                standardDirectoryPath = dialog.FileName + @"\";
                this.Focus();
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private string _ProjectName;

        public string ProjectName
        {
            set { _ProjectName = value; }
            get { /*return txtName.Text;*/ return _ProjectName; }
        }

        public string ProjectPath
        {
            get { return @txtFilePath.Text.ToString(); }
        }

        public List<string> SubProgramsToImportPath
        {
            get
            {
                List<string> subProgramsToImportPath = new List<string>();

                foreach(string subProgram in lbSubPrograms.Items)
                {
                    subProgramsToImportPath.Add(dictSubProgramsPath[subProgram]);
                }

                return subProgramsToImportPath;
            }
        }

        public List<string> SubProgramsToImportName
        {
            get
            {
                List<string> subProgramsToImportName = new List<string>();

                foreach (string subProgram in lbSubPrograms.Items)
                {
                    subProgramsToImportName.Add(subProgram);
                }

                return subProgramsToImportName;
            }
        }

        public bool Import
        {
            get { return (bool)rbImport.IsChecked; }
        }

        private void ApplyDataBinding()
        {
            lbPossibleSubPrograms.ItemsSource = null;
            // Bind ArrayList with the ListBox
            lbPossibleSubPrograms.ItemsSource = mySubProgramList;
        }

        private void MetroWindow_Loaded(object sender , RoutedEventArgs e)
        {
            this.txtName.Text = "";
        }
    }
}
