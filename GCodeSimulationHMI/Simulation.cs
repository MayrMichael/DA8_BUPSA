using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GCodeSimulationHMI
{
    class Simulation 
    {
        LinesVisual3D Toolmoves = new LinesVisual3D();
        List<LinesVisual3D> moves = new List<LinesVisual3D>();

        public void DrawLine(LinesVisual3D lines, double x_start, double y_start, double z_start, double x_stop, double y_stop, double z_stop)
        {
            lines.Points.Add(new Point3D(x_start, y_start, z_start));
            lines.Points.Add(new Point3D(x_stop, y_stop, z_stop));
        }

        public List<LinesVisual3D> Build_3D_Model(double x_start, double y_start, double z_start, double x_stop, double y_stop, double z_stop)
        {
            DrawLine(Toolmoves, x_start,  y_start,  z_start,  x_stop,  y_stop,  z_stop);

            Toolmoves.Thickness = 1;
            Toolmoves.Color = Colors.Red;

            moves.Add(Toolmoves);

            return moves;
        }
    }
}
