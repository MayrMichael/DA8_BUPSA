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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management;
using System.Xml;
using System.IO;
using LibraryGCode;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;


namespace Brenn_und_Plasmaschneidanlage
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        ProjectManager projectManager = new ProjectManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = projectManager;
            Test();
        }
        
        async Task Test()
        {
            await this.ShowMessageAsync("This is the title" , "Some message");
        }

        private const string SUBDISTANC = "   ";
        private const string MAINDISTANC = " ";

        #region Menu New File
        private void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {

                if (projectManager.AskSave())
                {
                    NewProject newProject = new NewProject();

                    if (newProject.ShowDialog() == true)
                    {
                        if (newProject.DialogResult == true)
                        {
                            //lbProjectTree.Items.Clear();

                            projectManager.Clear();

                            projectManager.ProjectName = newProject.ProjectName;
                            projectManager.ProjectPath = newProject.ProjectPath;

                            projectManager.Programs.Add(new Program("Main", true, true, Environment.NewLine + "M30", ""));
                            //lbProjectTree.Items.Add(MAINDISTANC + projectManager.Programs[0].ProgramName);


                            for (int i = 0; i < newProject.SubProgramsToImportName.Count; i++)
                            {
                                if (File.Exists(newProject.SubProgramsToImportPath[i]))
                                {
                                    Program program = new Program(newProject.SubProgramsToImportName[i]);

                                    XmlReader reader = new XmlTextReader(newProject.SubProgramsToImportPath[i]);

                                    while (reader.Read())
                                    {
                                        if (reader.NodeType == XmlNodeType.Element)
                                        {
                                            while (reader.MoveToNextAttribute())
                                            {
                                                switch (reader.Name)
                                                {
                                                    case "name":
                                                        program.ProgramName = reader.Value;
                                                        break;
                                                    case "content":
                                                        program.ProgramContent = reader.Value;
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    program.IsMain = false;
                                    program.IsSave = false;

                                    if (newProject.Import)
                                    {
                                        program.IsImport = true;
                                        program.ProgramPath = "";
                                    }
                                    else
                                    {
                                        program.IsImport = false;
                                        program.ProgramPath = newProject.SubProgramsToImportPath[i];
                                    }


                                    projectManager.Programs.Add(program);
                                    //lbProjectTree.Items.Add(SUBDISTANC + newProject.SubProgramsToImportName[i]);
                                }
                                else
                                {
                                    MessageBox.Show("Unterprogram: " + newProject.SubProgramsToImportPath[i] + Environment.NewLine + "Pfad: " + newProject.SubProgramsToImportPath[i] + Environment.NewLine + "konnte nicht hinzugefügt werden!!!"
                                        + Environment.NewLine + "Bitte überprüfen Sie die Datei", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }

                            projectManager.CurrentSelectedProgram = 0;

                            //lbProjectTree.SelectedIndex = projectManager.CurrentSelectedProgram;

                           // lblProjectName.Content = projectManager.ProjectName;

                            tbDevelopment.IsEnabled = true;
                            tbDevelopment.Focus();
                        }
                    } 
                }   
        }

        private void NewFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region Menu New Subprogram
        private void NewSubprogram_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<string> programsName = new List<string>();

            foreach (Program program in projectManager.Programs)
            {
                programsName.Add(program.ProgramName);
            }


            NewSubprogram newSubprogram =  new NewSubprogram(programsName);
            newSubprogram.ShowDialog();
            if(newSubprogram.DialogResult == true)
            {
                //projectManager.Programs.Add(newSubprogram.GetSubprogram());
                //lbProjectTree.Items.Add(SUBDISTANC + newSubprogram.Subprogram.ProgramName);
                projectManager.Update();
            }
        }

        private void NewSupprogram_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (projectManager.ProjectName != null && projectManager.ProjectName != "")
                e.CanExecute = true;
            else
                e.CanExecute = false;
        } 
        #endregion

        #region Menu New From Template
        private void NewFromTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void NewFromTemplate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }
        #endregion

        #region Menu Open File
        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (projectManager.AskSave())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Multiselect = false,
                    Filter = "Schneidprogramm (*.pcut)|*.pcut|Textdatei (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    XmlReader xr = new XmlTextReader(openFileDialog.FileName);

                    //lbProjectTree.Items.Clear();

                    projectManager.Clear();

                    try
                    {
                        string name = "";
                        string content = "";
                        bool isMain = false;
                        bool isImport = false;

                        int i = 0;

                        while (xr.Read())
                        {
                            if (xr.NodeType == XmlNodeType.Element)
                            {
                                if (xr.AttributeCount > 0)
                                {
                                    while (xr.MoveToNextAttribute())
                                    {
                                        switch (xr.Name)
                                        {
                                            case "name":
                                                name = xr.Value;
                                                break;
                                            case "content":
                                                content = xr.Value;
                                                break;
                                            case "isMain":
                                                isMain = Convert.ToBoolean(xr.Value);
                                                break;
                                            case "isImport":
                                                isImport = Convert.ToBoolean(xr.Value);
                                                break;
                                        }
                                    }
                                    if (isImport)
                                        projectManager.Programs.Add(new Program(name, isImport, isMain, content, ""));
                                    else
                                        projectManager.Programs.Add(new Program(name, isImport, isMain, "", content));

                                    //if (name == "Main")
                                    //    lbProjectTree.Items.Add(MAINDISTANC + name);
                                    //else
                                    //    lbProjectTree.Items.Add(SUBDISTANC + name);
                                    i++;
                                }
                                else
                                {
                                    if (xr.Depth == 0)
                                    {
                                        projectManager.ProjectName = xr.Name;
                                    }
                                }
                            }  
                        }

                        xr.Close();

                        for(int k = 0;k < projectManager.Programs.Count ;k++)
                        {
                            projectManager.Programs[k].Save();
                        }

                        projectManager.ProjectPath = openFileDialog.FileName;

                        projectManager.CurrentSelectedProgram = 0;

                        projectManager.Update();

                        lbProjectTree.SelectedIndex = projectManager.CurrentSelectedProgram;

                        lblProjectName.Content = projectManager.ProjectName;

                        tbDevelopment.IsEnabled = true;
                        tbDevelopment.Focus();

                    }
                    catch (XmlException)
                    {
                        MessageBox.Show("Datei konnte nicht gelesen werden!!!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        projectManager.Clear();
                        tbDevelopment.IsEnabled = false;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ein unbekanter Fehler ist aufgetreten!!!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        tbDevelopment.IsEnabled = false;
                        projectManager.Clear();
                    }
                    finally
                    {
                        if (xr != null)
                            xr.Close();
                    }
                }
            }
        }

        private void OpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
                e.CanExecute = true;
        }
        #endregion

        #region Menu Save File
        private void SaveFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            projectManager.SaveCurrentProgram();
        }

        private void SaveFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (projectManager.ProjectName != null && projectManager.ProjectName != "")
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
        #endregion

        #region Menu Save All
        private void SaveAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            projectManager.SaveAll(); 
        }

        private void SaveAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (projectManager.ProjectName != null && projectManager.ProjectName != "")
                e.CanExecute = true;
            else
                e.CanExecute = false;
        } 
        #endregion

        #region Menu Save As
        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (projectManager.ProjectName != null && projectManager.ProjectName != "")
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
        #endregion

        #region Menu Update
        private void Update_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            projectManager.Update();
        }

        private void Update_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (projectManager.ProjectName != null && projectManager.ProjectName != "")
            {
                bool isShortcut = false;

                foreach (Program program in projectManager.Programs)
                {
                    if (!program.IsImport)
                        isShortcut = true;
                }
                if (isShortcut)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;

            }
            else
                e.CanExecute = false;
        } 
        #endregion

        #region Menu Exit
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Shutdown();

            //string[] dialogtextExit = { "Sollen die Dateien gespeichert werden?","Sollen die Datei gespeichert werden?"};

            //foreach (Program program in projectManager.Programs)
            //{
            //    if (!program.IsSave)
            //    {
            //        dialogtext += program.ProgramName + Environment.NewLine;
            //    }
            //}
        }

        void Shutdown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams =
                     mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown",
                                               mboShutdownParams, null);
            }
        }

        

        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion


        private void TxtProgramContent_TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Program prg = projectManager.Programs[projectManager.CurrentSelectedProgram];

            prg.ProgramContent = txtProgramContent.Text;

            projectManager.Programs[projectManager.CurrentSelectedProgram] = prg;

            //lbxError.Items.Clear();

            //string[] lines = txtProgramContent.Text.ToUpper().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            //foreach (string s in lines)
            //{
            //    lbxError.Items.Add(s);
            //}

    

        }

        private void LbProjectTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (lbProjectTree.SelectedIndex != -1)
            //{
            //    lblProg.Content = lbProjectTree.SelectedItem.ToString().Trim(' ');

            //    for(int i= 0;i < projectManager.Programs.Count();i++)
            //    {
            //        if (projectManager.Programs[i].ProgramName.ToString() == lbProjectTree.SelectedItem.ToString().Trim(' '))
            //        {
            //            projectManager.CurrentSelectedProgram = i;
            //            txtProgramContent.Text = projectManager.Programs[i].ProgramContent;
            //            if (!projectManager.Programs[projectManager.CurrentSelectedProgram].IsImport)
            //                txtProgramContent.IsReadOnly = true;
            //            else
            //                txtProgramContent.IsReadOnly = false;
            //        }
            //    }


            //}

            if (projectManager.CurrentSelectedProgram != -1)
            {
                try
                {
                    //lblProg.Content = projectManager.Programs[projectManager.CurrentSelectedProgram].ProgramName;
                    txtProgramContent.Text = projectManager.Programs[projectManager.CurrentSelectedProgram].ProgramContent;
                    if (!projectManager.Programs[projectManager.CurrentSelectedProgram].IsImport)
                        txtProgramContent.IsReadOnly = true;
                    else
                        txtProgramContent.IsReadOnly = false;
                }
                catch (Exception){ }
            }

        }

        private void BtnDeleteSub_Click(object sender, RoutedEventArgs e)
        {
            if (lbProjectTree.SelectedIndex == 0)
                return;

            string deleteName = lbProjectTree.SelectedItem.ToString().Trim(' ');

            MessageBoxResult messageBoxResult = MessageBox.Show("Soll das Unterprogramm: " + deleteName + " gelöcht werden?", "Frage", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            //lbProjectTree.Items.RemoveAt(lbProjectTree.SelectedIndex);

            for (int i = 0; i < projectManager.Programs.Count; i++)
            {
                if(projectManager.Programs[i].ProgramName == deleteName)
                {
                    projectManager.Programs.RemoveAt(i);
                    lbProjectTree.SelectedIndex = i - 1;
                }
            }
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            //TODO:Rename fertigstellen
            //List<string> programsName = new List<string>();
            //string oldName = lbProjectTree.SelectedItem.ToString().Trim(' ');

            //foreach (Program program in projectManager.Programs)
            //{
            //    programsName.Add(program.ProgramName);
            //}

            //RenameSubprogram renameSubprogram = new RenameSubprogram(oldName, programsName);
            //renameSubprogram.ShowDialog();
            //if(renameSubprogram.DialogResult == true)
            //{
            //    for (int i = 0; i < projectManager.Programs.Count; i++)
            //    {
            //        if (projectManager.Programs[i].ProgramName == oldName)
            //        {
            //            projectManager.Programs[i].ProgramName.Replace(oldName,renameSubprogram.NewName);
            //            (Label)lbProjectTree.SelectedItem
            //        }
            //    }
            //}

        }

        private void lbxError_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
 

        private void txtProgramContent_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            //Program prg = projectManager.Programs[projectManager.CurrentSelectedProgram];

            //prg.ProgramContent = txtProgramContent.Text;

            //projectManager.Programs[projectManager.CurrentSelectedProgram] = prg;

            //if (projectManager.CurrentSelectedProgram != -1)
            //{
            //    if (txtProgramContent.Text != "" && txtProgramContent.Text != null)
            //    {
            //        List<string> subPrgNames = new List<string>();

            //        foreach (Program program in projectManager.Programs)
            //            subPrgNames.Add(program.ProgramName);

            //        GCode gCode = new GCode(txtProgramContent.Text, subPrgNames, !projectManager.Programs[projectManager.CurrentSelectedProgram].IsMain);
            //        List<string> lines = new List<string>();
            //        lines = gCode.ErrorList();
            //        txtError.Text = "";
            //        foreach (string s in lines)
            //        {
            //            txtError.Text += s + Environment.NewLine;
            //        }
            //    } 
            //}

            txtLineNumber.Text = "";

            for (int i = 0 ; i < txtProgramContent.LineCount ; i++)
            {
                txtLineNumber.Text += " " + (i + 1).ToString() + Environment.NewLine;


            }
        }
    }
    public static class CustomCommands
    {
        public static readonly RoutedUICommand SaveAll  = new RoutedUICommand("Alles Speichern", "SaveAll", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });
        public static readonly RoutedUICommand SaveAs = new RoutedUICommand("Speichern unter","SaveAs", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F2, ModifierKeys.Alt) });
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Beenden","Exit", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Alt) });
        public static readonly RoutedUICommand NewFromTemplate = new RoutedUICommand("Neu aus Vorlage", "NewFromTemplate", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Alt) });
        public static readonly RoutedUICommand NewSubprogram = new RoutedUICommand("Unterprogramm", "NewSubprogram", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift) });
        public static readonly RoutedUICommand Update = new RoutedUICommand("Unterprogramme aktualisieren", "Update", typeof(CustomCommands), new InputGestureCollection() { new KeyGesture(Key.A, ModifierKeys.Control) });
    }    
}

