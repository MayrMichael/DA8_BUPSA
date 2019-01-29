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
using HelixToolkit.Wpf;

namespace GCodeSimulationHMI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<LinesVisual3D> moves;

        public MainWindow()
        {
            InitializeComponent();

            Simulation sm = new Simulation();

            moves = sm.Build_3D_Model(0, 10, 0, 100, 50, -20);

            foreach (LinesVisual3D m in moves)
            {
                viewport.Children.Remove(m);
                viewport.Children.Add(m);
            }
        }
    }
}
