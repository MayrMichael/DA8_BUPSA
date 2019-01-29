using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows;
using Brenn_und_Plasmaschneidanlage.ViewModel;

namespace Brenn_und_Plasmaschneidanlage
{
    class Program : ViewModelBase
    {

        private const string SUBDISTANC = "   ";
        private const string MAINDISTANC = " ";

        private string _programName;
        private string _programContent;
        private string _programPath;
        private bool _isSave;
        private bool _isMain;
        private bool _isImport;


        public Program(string programName)
        {
            this.ProgramName = programName;
            this.ProgramPath = "";
            this.ProgramContent = "";
            this.IsImport = false;
            this.IsMain = false;
            this.IsSave = false;
        }


        public Program(string programName , bool isImport , bool isMain = false , string programContent = "" , string programPath = "")
        {
            this.ProgramName = programName;
            this.ProgramPath = programPath;
            this.ProgramContent = programContent;
            this.IsImport = isImport;
            this.IsMain = isMain;
            this.IsSave = false;
        }

        public void Save()
        {
            IsSave = true;
        }

        public void NotSave()
        {
            IsSave = false;
        }

        public void Update()
        {
            if (ProgramPath != "")
            {
                if (File.Exists(ProgramPath))
                {
                    XmlReader reader = new XmlTextReader(ProgramPath);

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "name":
                                        ProgramName = reader.Value;

                                        break;
                                    case "content":
                                        ProgramContent = reader.Value;
                                        break;
                                }
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Die Datei:" + Environment.NewLine + ProgramPath + Environment.NewLine + "konnte nicht gefunden werden!!!" , "Fehler" , MessageBoxButton.OK , MessageBoxImage.Error);
                }
            }
        }

        //Name of the Program
        public string ProgramName
        {
            get { return _programName; }
            set { SetProperty<string>(ref _programName , value); }
        }

        //Content of the Program
        public string ProgramContent
        {
            get { return _programContent; }
            set { SetProperty<string>(ref _programContent , value); }
        }

        //If the Program is an Import there is the Path to Import
        public string ProgramPath
        {
            get { return _programPath; }
            set { SetProperty<string>(ref _programPath , value); }
        }

        //Is true if the program is imported
        public bool IsImport
        {
            get { return _isImport; }
            set { SetProperty<bool>(ref _isImport , value); }
        }

        //Is true if the program is the main program
        public bool IsMain
        {
            get { return _isMain; }
            set { SetProperty<bool>(ref _isMain , value); }
        }

        //is true if the current programm is writen in their xml file
        public bool IsSave
        {
            get { return _isSave; }
            set { SetProperty<bool>(ref _isMain , value); }
        }

    }
}
