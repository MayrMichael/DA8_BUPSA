using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryGCode
{
    public class GCode
    {
        private List<GCodeLine> allGCodeLines = new List<GCodeLine>();

        public GCode(string code,List<string> subprograms, bool isSubProgram)
        {
            ParserGCode parserGCode = new ParserGCode();

            allGCodeLines = parserGCode.Parse(code,subprograms,isSubProgram);
        }

        public List<string> ErrorList()
        {
            List<string> errorList = new List<string>();

            foreach(GCodeLine gCodeLine in allGCodeLines)
            {
                if (!(gCodeLine.Error.Count < 1))
                    foreach(GCodeLine.ErrorGCode errorGCode in gCodeLine.Error)
                        errorList.Add(errorGCode.ToString());
            }
            return errorList;
        }

        
    }

    public class GCodeLine
    {
        public int LineNumber { get; set; }
        //public string Line;
        public List<string> Command { get; set; } = new List<string>();
        public double? XPos { get; set; }
        public double? YPos { get; set; }
        public double? SEG  { get; set; }
        public double? ANG  { get; set; }
        public double? ZPos { get; set; }
        public double? IPos { get; set; }
        public double? JPos { get; set; }
        public double? CRNumber { get; set; }
        public double? PNumber { get; set; }
        public string SubPrgName { get; set; }
        public int? PartsX { get; set; }
        public int? PartsY { get; set; }
        public double? SubPrgDistanceX { get; set; }
        public double? SubPrgDistanceY { get; set; }
        public List<ErrorGCode> Error { get; set; } = new List<ErrorGCode>();

        private const string SEPERATOR = ";";

        #region GCodeLine
        public GCodeLine(int lineNumber)
        {
            this.LineNumber = lineNumber;
            this.XPos = null;
            this.YPos = null;
            this.SEG = null;
            this.ANG = null;
            this.ZPos = null;
            this.IPos = null;
            this.JPos = null;
            this.CRNumber = null;
            this.PNumber = null;
            this.SubPrgName = null;
            this.PartsX = null;
            this.PartsY = null;
            this.SubPrgDistanceX = null;
            this.SubPrgDistanceY = null;
        }

        public GCodeLine(int lineNumber, string command, double xPos, double yPos)
        {
            this.LineNumber = lineNumber;
            this.Command.Add(command);
            this.XPos = xPos;
            this.YPos = yPos;
            this.SEG = null;
            this.ANG = null;
            this.ZPos = null;
            this.IPos = null;
            this.JPos = null;
            this.CRNumber = null;
            this.PNumber = null;
            this.SubPrgName = null;
            this.PartsX = null;
            this.PartsY = null;
            this.SubPrgDistanceX = null;
            this.SubPrgDistanceY = null;
        }

        public GCodeLine(int lineNumber, string command, double xPos, double yPos, double crNumber)
        {
            this.LineNumber = lineNumber;
            this.Command.Add(command);
            this.XPos = xPos;
            this.YPos = yPos;
            this.SEG = null;
            this.ANG = null;
            this.ZPos = null;
            this.IPos = null;
            this.JPos = null;
            this.CRNumber = crNumber;
            this.PNumber = null;
            this.SubPrgName = null;
            this.PartsX = null;
            this.PartsY = null;
            this.SubPrgDistanceX = null;
            this.SubPrgDistanceY = null;
        }

        public GCodeLine(int lineNumber, string command, double xPos, double yPos, double iPos, double jPos)
        {
            this.LineNumber = lineNumber;
            this.Command.Add(command);
            this.XPos = xPos;
            this.YPos = yPos;
            this.SEG = null;
            this.ANG = null;
            this.ZPos = null;
            this.IPos = IPos;
            this.JPos = JPos;
            this.CRNumber = null;
            this.PNumber = null;
            this.SubPrgName = null;
            this.PartsX = null;
            this.PartsY = null;
            this.SubPrgDistanceX = null;
            this.SubPrgDistanceY = null;
        } 
        #endregion

        public static void CopyInside(ref GCodeLine gCodeLine1, GCodeLine gCodeLine2)
        {
            if (gCodeLine1.LineNumber != gCodeLine2.LineNumber)
                throw new Exception("LineNumber is not equal");

            #region Copy Commands
            if (gCodeLine2.Command.Count() != 0)
                foreach (string command in gCodeLine2.Command)
                    gCodeLine1.Command.Add(command); 
            #endregion

            #region Copy X,Y,Z
            if (gCodeLine1.XPos != null && gCodeLine2.XPos != null)
                throw new Exception("XPos has in both variables a value");
            else
                   if (gCodeLine1.XPos == null)
                gCodeLine1.XPos = gCodeLine2.XPos;

            if (gCodeLine1.YPos != null && gCodeLine2.YPos != null)
                throw new Exception("YPos has in both variables a value");
            else
                if (gCodeLine1.YPos == null)
                gCodeLine1.YPos = gCodeLine2.YPos;

            if (gCodeLine1.ZPos != null && gCodeLine2.ZPos != null)
                throw new Exception("ZPos has in both variables a value");
            else
                if (gCodeLine1.ZPos == null)
                gCodeLine1.ZPos = gCodeLine2.ZPos; 
            #endregion

            #region Copy Circle
            if (gCodeLine1.IPos != null && gCodeLine2.IPos != null)
                throw new Exception("IPos has in both variables a value");
            else
                    if (gCodeLine1.IPos == null)
                gCodeLine1.IPos = gCodeLine2.IPos;

            if (gCodeLine1.JPos != null && gCodeLine2.JPos != null)
                throw new Exception("JPos has in both variables a value");
            else
                if (gCodeLine1.JPos == null)
                gCodeLine1.JPos = gCodeLine2.JPos;

            if (gCodeLine1.CRNumber != null && gCodeLine2.CRNumber != null)
                throw new Exception("CRNumber has in both variables a value");
            else
                if (gCodeLine1.CRNumber == null)
                gCodeLine1.CRNumber = gCodeLine2.CRNumber;
            #endregion

            #region Copy Break;
            if (gCodeLine1.PNumber != null && gCodeLine2.PNumber != null)
                throw new Exception("PNumber has in both variables a value");
            else
                    if (gCodeLine1.PNumber == null)
                gCodeLine1.PNumber = gCodeLine2.PNumber; 
            #endregion

            #region Copy SubProgram
            if (gCodeLine1.SubPrgName != null && gCodeLine2.SubPrgName != null)
                throw new Exception("SubProgramName has in both variables a value");
            else
                    if (gCodeLine1.SubPrgName == null)
                gCodeLine1.SubPrgName = gCodeLine2.SubPrgName;

            if (gCodeLine1.PartsX != null && gCodeLine2.PartsX != null)
                throw new Exception("PartsX has in both variables a value");
            else
               if (gCodeLine1.PartsX == null)
                gCodeLine1.PartsX = gCodeLine2.PartsX;

            if (gCodeLine1.PartsY != null && gCodeLine2.PartsY != null)
                throw new Exception("PartsY has in both variables a value");
            else
              if (gCodeLine1.PartsY == null)
                gCodeLine1.PartsY = gCodeLine2.PartsY;

            if (gCodeLine1.SubPrgDistanceX != null && gCodeLine2.SubPrgDistanceX != null)
                throw new Exception("SubDistanceX has in both variables a value");
            else
              if (gCodeLine1.SubPrgDistanceX == null)
                gCodeLine1.SubPrgDistanceX = gCodeLine2.SubPrgDistanceX;

            if (gCodeLine1.SubPrgDistanceY != null && gCodeLine2.SubPrgDistanceY != null)
                throw new Exception("SubDistanceY has in both variables a value");
            else
              if (gCodeLine1.SubPrgDistanceY == null)
                gCodeLine1.SubPrgDistanceY = gCodeLine2.SubPrgDistanceY;
            #endregion


            #region Copy Error
            for (int i = 0; i < gCodeLine2.Error.Count(); i++)
                gCodeLine1.Error.Add(gCodeLine2.Error[i]); 
            #endregion

        }

        public enum GCodeErrorCodes
        {
            //Command was not recognized
            WrongCommand,
            WrongGCommand,
            WrongMCommand,
            //Converting to number faild
            XPos,
            YPos,
            IPos,
            JPos,
            CRNumber,
            PNumber,
            ANGNumber,
            SEGNumber,
            IJAndCR,
            TXNumber,
            TYNumber,
            LXNumber,
            LYNumber,
            //Missing command
            MissingCommand,
            //
            MissingG00OrG01,
            MissingXCommand,
            MissingYCommand,
            MissingPCommand,
            MissingLXCommand,
            MissingLYCommand,
            MissingTXCommand,
            MissingTYCommand,
            ToManyCommands,
            ToolCompensationNotDeactivate,
            PrgHasFinished
        }

        public struct ErrorGCode
        {
            public ErrorGCode(int tokenNumberLine, GCodeErrorCodes errorCode, string errorToken = "")
            {
                this.ErrorTokenNumberLine = tokenNumberLine;
                this.ErrorCode = errorCode;
                this.ErrorToken = errorToken;
            }

            public int ErrorTokenNumberLine { get; private set; }

            public GCodeErrorCodes ErrorCode { get; private set; }

            public string ErrorToken { get; private set; }

            public override string ToString()
            {
                string errorGCodeText = "";

                errorGCodeText += ErrorTokenNumberLine.ToString() + SEPERATOR;
                errorGCodeText += ErrorCode.ToString() + SEPERATOR;
                errorGCodeText += ErrorToken + SEPERATOR;

                return errorGCodeText;
            }
        }

        public override string ToString()
        {
            string gcodeLineText = "";

            gcodeLineText += LineNumber.ToString()+";";

            foreach(string command in Command)
            {
                gcodeLineText += command + ";";
            }

            gcodeLineText += XPos.ToString() + SEPERATOR
             + YPos.ToString() + SEPERATOR
             + ZPos.ToString() + SEPERATOR
             + IPos.ToString() + SEPERATOR
             + JPos.ToString() + SEPERATOR
             + CRNumber.ToString() + SEPERATOR;
            
            foreach(ErrorGCode error in Error)
            {
                gcodeLineText += error.ToString() + SEPERATOR;
            }
            

            return gcodeLineText;
        }
    }
}
