using System;
using System.ComponentModel;
using System.Windows;

using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Threading;

namespace LibraryGCode
{
    public class SimulationGCode
    {
        //LinesVisual3D Toolmoves = new LinesVisual3D();
        //List<LinesVisual3D> moves = new List<LinesVisual3D>();

        //public void DrawLine(LinesVisual3D lines, double x_start, double y_start, double z_start, double x_stop, double y_stop, double z_stop)
        //{
        //    lines.Points.Add(new Point3D(x_start, y_start, z_start));
        //    lines.Points.Add(new Point3D(x_stop, y_stop, z_stop));
        //}

        

        public List<SimLine> Build3DModel(List <GCodeLine> gCodeLines, SimPoint[] zeroPointOffset)
        {
            List<SimLine> simLines = new List<SimLine>();
            SimPoint workZeroPointOffset = new SimPoint(0, 0, 0);

            int compileUntilLineNumber = SearchForLastRightLine(gCodeLines);
            

            foreach(GCodeLine gCodeLine in gCodeLines)
            {
                if(gCodeLine.LineNumber < compileUntilLineNumber || compileUntilLineNumber == -1)
                {
                    foreach(string command in gCodeLine.Command)
                    {
                        switch(command)
                        {
                            case "G0":
                                break;
                            case "G1":
                                break;
                            case "G2":
                                break;
                            case "G3":
                                break;
                            case "G4":
                                break;
                            case "G40":
                                break;
                            case "G41":
                                break;
                            case "G42":
                                break;
                            case "G53":
                                workZeroPointOffset = new SimPoint(0, 0, 0);
                                break;
                            case "G54":
                                workZeroPointOffset = zeroPointOffset[0];
                                break;
                            case "G55":
                                workZeroPointOffset = zeroPointOffset[1];
                                break;
                            case "G56":
                                workZeroPointOffset = zeroPointOffset[2];
                                break;
                            case "G57":
                                workZeroPointOffset = zeroPointOffset[3];
                                break;
                            case "G58":
                                //workZeroPointOffset.Add(gCodeLine.XPos,gCodeLine.YPos)
                                break;
                            case "G59":
                                break;
                            case "G90":
                                break;
                            case "G91":
                                break;     
                        }
                    }

                    //double x_pos_old = Convert.ToDouble(old_positions.x_pos);
                    //double y_pos_old = Convert.ToDouble(old_positions.y_pos);
                    //double z_pos_old = Convert.ToDouble(old_positions.z_pos);
                    //double xPos = Convert.ToDouble((gCodeLine.XPos.HasValue ? gCodeLine.XPos  : old_positions.x_pos));
                    //double yPos = Convert.ToDouble((gCodeLine.YPos.HasValue ? gCodeLine.YPos  : old_positions.y_pos));
                    //double zPos = Convert.ToDouble((gCodeLine.ZPos.HasValue ? gCodeLine.ZPos  : old_positions.z_pos));
                    //double iPos = Convert.ToDouble((gCodeLine.IPos.HasValue ? gCodeLine.IPos  : Double.NaN));
                    //double crNumber = Convert.ToDouble((gCodeLine.CRNumber.HasValue ? gCodeLine.CRNumber  : Double.NaN));
                    //double jPos = Convert.ToDouble((gCodeLine.JPos.HasValue ? gCodeLine.JPos : Double.NaN));
                    //double jPos = Convert.ToDouble((gCodeLine.JPos.HasValue ? gCodeLine.JPos : Double.NaN));

                    
                }
            }

            return simLines;
        }

        private int SearchForLastRightLine(List<GCodeLine> gCodeLines)
        {
            // -1 the whole code is right
            int lastRightLine = -1;

            for (int i = 0; i < gCodeLines.Count; i++)
            {
                if (gCodeLines[i].Error.Count != 0)
                {
                    return i;
                }
            }

            return lastRightLine;
        }

    }

    public class SimPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public SimPoint()
        {

        }

        public SimPoint(double x,double y, double z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Add(SimPoint simPoint)
        {
            X += simPoint.X;
            Y += simPoint.Y;
            Z += simPoint.Z;
        }

        public void Add(double x, double y, double z)
        {
            X += x;
            Y += y;
            Z += z;
        }
    }

    public class SimLine
    {
        public SimPoint StartPoints { get; set; }
        public SimPoint StopPoints { get; set; }
        public TypeOfLines TypeOfLine { get; set; }

        public SimLine()
        {
            this.StartPoints = null;
            this.StopPoints = null;
        }

        public SimLine(double xStart,double yStart, double xStop, double yStop, double zStart = 0, double zStop = 0)
        {
            this.StartPoints.X = xStart;
            this.StartPoints.Y = yStart;
            this.StartPoints.Z = zStart;

            this.StopPoints.X = xStop;
            this.StopPoints.Y = yStop;
            this.StopPoints.Z = zStop;
        }

        public enum TypeOfLines
        {
            RapidMoves,
            ToolMoves,
            PartMove
        };
    }
}
