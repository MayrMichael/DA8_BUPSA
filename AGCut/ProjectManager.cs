using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AGCut
{
    class ProjectManager : INotifyPropertyChanged
    {
        private string projectName;
        private int currentSelectedProgram;
        private string currentSelectedProgramName;



        public ProjectManager()
        {
            projectName = "";
            currentSelectedProgram = 0;
            currentSelectedProgramName = "";
        }

        public string ProjectName
        {
            get
            {
                return this.projectName;
            }

            set
            {
                if (value != this.projectName)
                {
                    this.projectName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string CurrentSelectedProgramName
        {
            get
            {
                return this.currentSelectedProgramName;
            }
        }

        public string ProjectPath { get; set; }

        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();

        public int CurrentSelectedProgram
        {
            get
            {
                return this.currentSelectedProgram;
            }

            set
            {
                if (value != this.currentSelectedProgram)
                {
                    this.currentSelectedProgram = value;
                    //this.currentSelectedProgramName = Programs[currentSelectedProgram].ProgramName;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Clear()
        {
            this.Programs.Clear();
            this.ProjectName = "";
            this.ProjectPath = "";
            this.CurrentSelectedProgram = 0;
        }

        public void SaveAll()
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(ProjectPath, new UnicodeEncoding());
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement(ProjectName);

            foreach (Program program in Programs)
            {
                WriteXmlElement(program, xmlWriter);
            }


            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            for (int i = 0; i < Programs.Count; i++)
            {
                Programs[i].Save();
            }
        }

        private void WriteXmlElement(Program program, XmlTextWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("program");

            xmlTextWriter.WriteAttributeString("name", program.ProgramName);

            if (program.IsImport)
                xmlTextWriter.WriteAttributeString("content", program.ProgramContent);
            else
                xmlTextWriter.WriteAttributeString("content", program.ProgramPath);


            xmlTextWriter.WriteAttributeString("isMain", program.IsMain.ToString());
            xmlTextWriter.WriteAttributeString("isImport", program.IsImport.ToString());
            xmlTextWriter.WriteEndElement();
        }

        public void SaveCurrentProgram()
        {
            if (File.Exists(ProjectPath))
            {
                List<Program> oldPrograms = new List<Program>();

                XmlReader xr = new XmlTextReader(ProjectPath);

                string oldName = "";
                string oldContent = "";
                bool oldIsMain = false;
                bool oldIsImport = false;

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
                                        oldName = xr.Value;
                                        break;
                                    case "content":
                                        oldContent = xr.Value;
                                        break;
                                    case "isMain":
                                        oldIsMain = Convert.ToBoolean(xr.Value);
                                        break;
                                    case "isImport":
                                        oldIsImport = Convert.ToBoolean(xr.Value);
                                        break;
                                }
                            }
                            if (oldIsImport)
                                oldPrograms.Add(new Program(oldName,oldIsImport, oldIsMain, oldContent, ""));
                            else
                                oldPrograms.Add(new Program(oldName, oldIsImport, oldIsMain, "", oldContent));
                            i++;
                        }
                    }
                }

                xr.Close();

                XmlTextWriter xw = new XmlTextWriter(ProjectPath, new UnicodeEncoding());
                xw.WriteStartDocument();
                xw.WriteStartElement(ProjectName);

                bool CurrenSelectedProgramisSave = false;

                foreach (Program oldProgram in oldPrograms)
                {
                    if (oldProgram.ProgramName != Programs[CurrentSelectedProgram].ProgramName)
                    {
                        WriteXmlElement(oldProgram, xw);
                    }
                    else
                    {
                        WriteXmlElement(Programs[CurrentSelectedProgram], xw);
                        CurrenSelectedProgramisSave = true;
                    }
                }

                if (!CurrenSelectedProgramisSave)
                    WriteXmlElement(Programs[CurrentSelectedProgram], xw);

                xw.WriteEndElement();
                xw.Close();

                Programs[CurrentSelectedProgram].Save();

            }
            else
            {
                SaveAll();
            }
        }

        public bool AskSave()
        {
            if (ProjectName != null && ProjectName != "")
            {
                string dialogtext = "Soll " + Environment.NewLine;


                foreach (Program program in Programs)
                {
                    if (!program.IsSave)
                    {
                        dialogtext += program.ProgramName + Environment.NewLine;
                    }
                }

                dialogtext += "gespeichert werden!!!";


                MessageBoxResult messageBoxResult = MessageBox.Show(dialogtext, "Achtung", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SaveAll();
                    return true;
                }
                else if (messageBoxResult == MessageBoxResult.No)
                    return true;
                else
                    return false; 
            }
            else
                return true;

        }

        public void NotSaveProgram(int number)
        {
            Programs[number].NotSave();
        }

        public void Update()
        {
            for(int i = 0;i < Programs.Count ;i++)
            {
                Programs[i].Update();
            }
        }

    }
}
