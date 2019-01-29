using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Brenn_und_Plasmaschneidanlage
{
    /// <summary>
    /// Interaktionslogik für RenameSubprogram.xaml
    /// </summary>
    public partial class RenameSubprogram : Window
    {
        private List<string> wrongNames;
        private string newName;

        public RenameSubprogram(string oldName,List<string> names)
        {
            InitializeComponent();
            wrongNames = names;
            txtOld.Text = oldName;
            txtNew.Focus();
            windowRenameSub.Title = oldName + " umbenenen";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnNew_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isNameRight = false;
            bool isSpellRight = false;


            foreach (string name in wrongNames)
            {
                if (name == txtNew.Text.Trim(' '))
                {
                    txtNew.ToolTip = "Der aktuelle Name wird schon verwendet";
                    isNameRight = false;
                    break;
                }
                else
                {
                    txtNew.ToolTip = "";
                    isNameRight = true;
                }

            }

            Regex regex = new Regex(@"^[a-zA-Z0-9_]+$");

            // Compare a string against the regular expression
            if (!regex.IsMatch(txtNew.Text))
            {
                if (txtNew.ToolTip.ToString() != "")
                    txtNew.ToolTip += Environment.NewLine + "Mindestens ein Zeichen ist ungültig!";
                else
                    txtNew.ToolTip = "Mindestens ein Zeichen ist ungültig!";

            }
            else
                isSpellRight = true;

            if (isNameRight && isSpellRight && txtNew.Text != "")
            {
                txtNew.Background = Brushes.White;
                btnOK.IsEnabled = true;
                txtNew.ToolTip = "Neuer Name des neuen Unterprogramms";
                newName = txtNew.Text;
            }
            else if (txtNew.Text == "")
            {
                txtNew.Background = Brushes.White;
                txtNew.ToolTip = "Bitte einen Namen eingeben";
                btnOK.IsEnabled = false;
            }
            else
            {
                btnOK.IsEnabled = false;
                txtNew.Background = Brushes.Red;
            }
        }

        public string NewName
        {
            get { return newName; }
        }
    }
}
