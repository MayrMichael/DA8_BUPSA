using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brenn_und_Plasmaschneidanlage.ViewModel.Commands
{
    class ShortCutCommands
    {
        public static readonly RoutedUICommand SaveAll = new RoutedUICommand("Alles Speichern" , "SaveAll" , typeof(CustomCommands) , 
            new InputGestureCollection() { new KeyGesture(Key.S , ModifierKeys.Control | ModifierKeys.Shift) });

        public static readonly RoutedUICommand SaveAs = new RoutedUICommand("Speichern unter" , "SaveAs" , typeof(CustomCommands) , 
            new InputGestureCollection() { new KeyGesture(Key.F2 , ModifierKeys.Alt) });

        public static readonly RoutedUICommand Exit = new RoutedUICommand("Beenden" , "Exit" , typeof(CustomCommands) , 
            new InputGestureCollection() { new KeyGesture(Key.F4 , ModifierKeys.Alt) });

        public static readonly RoutedUICommand NewFromTemplate = new RoutedUICommand("Neu aus Vorlage" , "NewFromTemplate" , typeof(CustomCommands) , 
            new InputGestureCollection() { new KeyGesture(Key.N , ModifierKeys.Control | ModifierKeys.Alt) });

        public static readonly RoutedUICommand NewSubprogram = new RoutedUICommand("Unterprogramm" , "NewSubprogram" , typeof(CustomCommands) , 
            new InputGestureCollection() { new KeyGesture(Key.N , ModifierKeys.Control | ModifierKeys.Shift) });

        public static readonly RoutedUICommand Update = new RoutedUICommand("Unterprogramme aktualisieren" , "Update" , typeof(CustomCommands) , 
            new InputGestureCollection() { new KeyGesture(Key.A , ModifierKeys.Control) });
    }
}
