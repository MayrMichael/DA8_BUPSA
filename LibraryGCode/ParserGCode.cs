using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryGCode
{
    public class ParserGCode
    {
        private LexerGCode lexerGCode;

        private const string EOF = LexerGCode.EOF;
        private const string EOL = LexerGCode.EOL;

        private bool multiLineG01G00 = false;

        private bool PrgHasFinished = false;

        private string multiLineCommand = "";

        private bool toolcompensationActivated = false;

        private bool _isSubProgram;

        private List<string> _subPrograms;

        private List<GCodeLine> gCodeLines = new List<GCodeLine>();

        public List<GCodeLine> Parse(string code,List<string> subPrograms, bool isSubProgram)
        {
            gCodeLines.Clear();

            _isSubProgram = isSubProgram;
            _subPrograms = subPrograms;

            lexerGCode = new LexerGCode(code);

            HandleCode();

            return gCodeLines;
        }

        private void HandleCode()
        {
            bool finish = false;
            bool SubPrgFound = false;
            string token;
            do
            {
                token = lexerGCode.GetNextToken();

                GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

                switch (token.Substring(0, 1))
                {
                    case "G":
                        if (PrgHasFinished)
                            gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentLineNumber, GCodeLine.GCodeErrorCodes.PrgHasFinished, token));
                        GCodeLine.CopyInside(ref gCodeLine, HandleGFunction(token));
                        break;
                    case "M":
                        if (PrgHasFinished)
                            gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentLineNumber, GCodeLine.GCodeErrorCodes.PrgHasFinished, token));
                        GCodeLine.CopyInside(ref gCodeLine, HandleMFunction(token));
                        multiLineG01G00 = false;
                        break;
                    case "X":
                    case "Y":
                        if (PrgHasFinished)
                            gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentLineNumber, GCodeLine.GCodeErrorCodes.PrgHasFinished, token));
                        GCodeLine.CopyInside(ref gCodeLine, HandleG00G01(token));

                        if (!multiLineG01G00)
                            gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.MissingG00OrG01, token));
                        else
                            gCodeLine.Command.Add(multiLineCommand);
                        break;
                    default:
                        switch (token)
                        {
                            case EOL:
                                break;
                            case EOF:
                                finish = true;
                                break;
                            default:
                                foreach(string subPrg in _subPrograms)
                                    if(token == subPrg)
                                    {
                                        if (PrgHasFinished)
                                            gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentLineNumber, GCodeLine.GCodeErrorCodes.PrgHasFinished, token));

                                        GCodeLine.CopyInside(ref gCodeLine, HandleSubProgram(token));
                                        SubPrgFound = true;
                                        break;
                                    }
                                        
                                if(!SubPrgFound)
                                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(token));                                
                                break;
                        }
                        break;
                }
                gCodeLines.Add(gCodeLine);
                Debugger.Log(1, "Test", "Hallo");

            } while (!finish);


        }

        private GCodeLine HandleGFunction(string token)
        {
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            multiLineG01G00 = false;

            switch (token)
            {
                case "G00":
                case "G0":
                    gCodeLine.Command.Add("G0");
                    GCodeLine.CopyInside(ref gCodeLine, HandleG00G01());
                    multiLineG01G00 = true;
                    multiLineCommand = "G0";
                    break;
                case "G01":
                case "G1":
                    gCodeLine.Command.Add("G1");
                    GCodeLine.CopyInside(ref gCodeLine, HandleG00G01());
                    multiLineG01G00 = true;
                    multiLineCommand = "G1";
                    break;
                case "G02":
                case "G2":
                    gCodeLine.Command.Add("G2");
                    GCodeLine.CopyInside(ref gCodeLine, HandleG02G03());
                    break;
                case "G03":
                case "G3":
                    gCodeLine.Command.Add("G3");
                    GCodeLine.CopyInside(ref gCodeLine, HandleG02G03());
                    break;
                case "G4":
                case "G04":
                    gCodeLine.Command.Add("G4");
                    GCodeLine.CopyInside(ref gCodeLine, HandleG04());
                    break;
                case "G40":
                    gCodeLine.Command.Add("G40");
                    if (!multiLineG01G00)
                        gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber,GCodeLine.GCodeErrorCodes.MissingG00OrG01));
                    GCodeLine.CopyInside(ref gCodeLine, HandleG00G01());
                    toolcompensationActivated = false;
                    break;
                case "G41":
                    gCodeLine.Command.Add("G41");
                    if (!multiLineG01G00)
                        gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.MissingG00OrG01));
                    GCodeLine.CopyInside(ref gCodeLine, HandleG00G01());
                    toolcompensationActivated = true;
                    break;
                case "G42":
                    gCodeLine.Command.Add("G42");
                    if (!multiLineG01G00)
                        gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.MissingG00OrG01));
                    GCodeLine.CopyInside(ref gCodeLine, HandleG00G01());
                    toolcompensationActivated = true;
                    break;
                case "G53":
                    gCodeLine.Command.Add("G53");
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(lexerGCode.GetNextToken()));
                    break;
                case "G54":
                    gCodeLine.Command.Add("G54");
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(lexerGCode.GetNextToken()));
                    break;
                case "G55":
                    gCodeLine.Command.Add("G55");
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(lexerGCode.GetNextToken()));
                    break;
                case "G56":
                    gCodeLine.Command.Add("G56");
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(lexerGCode.GetNextToken()));
                    break;
                case "G57":
                    gCodeLine.Command.Add("G57");
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(lexerGCode.GetNextToken()));
                    break;
                case "G58":
                    gCodeLine.Command.Add("G58");
                    GCodeLine.CopyInside(ref gCodeLine, HandleG58G59());
                    break;
                case "G59":
                    gCodeLine.Command.Add("G59");
                    GCodeLine.CopyInside(ref gCodeLine, HandleG58G59());
                    break;
                case "G90":
                    gCodeLine.Command.Add("G55");
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(lexerGCode.GetNextToken()));
                    break;
                case "G91":
                    gCodeLine.Command.Add("G55");
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(lexerGCode.GetNextToken()));
                    break;
                default:
                    //Error
                    gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.WrongGCommand, token));
                    GCodeLine.CopyInside(ref gCodeLine, HandleComment(token));
                    break;
            }

            return gCodeLine;
        }
        
        private GCodeLine HandleMFunction(string token)
        {
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            switch (token)
            {
                case "M00":
                case "M0":
                    gCodeLine.Command.Add("M0");
                    break;
                case "M02":
                case "M2":
                    gCodeLine.Command.Add("M2");
                    break;
                case "M09":
                case "M9":
                    gCodeLine.Command.Add("M9");
                    break;
                case "M10":
                    gCodeLine.Command.Add("M10");
                    break;
                case "M11":
                    gCodeLine.Command.Add("M11");
                    break;
                case "M12":
                    gCodeLine.Command.Add("M12");
                    break;
                case "M17":
                    GCodeLine.CopyInside(ref gCodeLine, HandleM17());
                    break;
                case "M30":
                    GCodeLine.CopyInside(ref gCodeLine, HandleM30());
                    break;
                default:
                    //Error
                    gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.WrongMCommand, token));
                    break;
            }

            token = lexerGCode.GetNextToken();

            if (token != EOL)
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleComment(token)); 
            }

            return gCodeLine;
        }

        private GCodeLine HandleM17()
        {
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            gCodeLine.Command.Add("M17");

            PrgHasFinished = true;

            if (toolcompensationActivated)
            {
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.ToolCompensationNotDeactivate));
            }

            if(!_isSubProgram)
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.WrongMCommand));

            return gCodeLine;
        }

        private GCodeLine HandleM30()
        {
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            gCodeLine.Command.Add("M30");

            PrgHasFinished = true;

            if (toolcompensationActivated)
            {
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.ToolCompensationNotDeactivate));
            }

            if (_isSubProgram)
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.WrongMCommand));

            return gCodeLine;
        }

        private GCodeLine HandleComment(string token)
        {
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            bool comment = false;

            while (token != EOL)
            {
                if (token.StartsWith("#"))
                {
                    //Comment
                    comment = true;
                }

                if (!comment)
                {
                    multiLineG01G00 = false;
                    //Error
                    gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.WrongCommand, token));
                }

                token = lexerGCode.GetNextToken();
            }

            return gCodeLine;
        }

        private GCodeLine HandleSubProgram(string token)
        {
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            gCodeLine.Command.Add("SubPrg");
            gCodeLine.SubPrgName = token;

            token = lexerGCode.GetNextToken();

            #region TX
            if (token.StartsWith("TX"))
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleTX(token));
                token = lexerGCode.GetNextToken();
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingTXCommand));
            } 
            #endregion

            #region TY
            if (token.StartsWith("TY"))
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleTY(token));
                token = lexerGCode.GetNextToken();
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingTYCommand));
            }
            #endregion

            #region LX
            if (token.StartsWith("LX"))
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleLX(token));
                token = lexerGCode.GetNextToken();
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingLXCommand));
            }
            #endregion

            #region LY
            if (token.StartsWith("LY"))
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleLY(token));
                token = lexerGCode.GetNextToken();
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingLYCommand));
            } 
            #endregion

            if (token != EOL)
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleComment(token));
            }

            return gCodeLine;
        }

        private GCodeLine HandleG00G01(string token = "")
        {

            if (token == "")
            {
                token = lexerGCode.GetNextToken(); 
            }

            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            bool oneFound = false;
            bool checkXY = false;
            int commandCounter = 0;

            if(token == "G41")
            {
                gCodeLine.Command.Add("G41");
                token = lexerGCode.GetNextToken();
                toolcompensationActivated = true;
            }

            if (token == "G42")
            {
                gCodeLine.Command.Add("G42");
                token = lexerGCode.GetNextToken();
                toolcompensationActivated = true;
            }

            if (token.StartsWith("X"))
            {
                oneFound = true;
                checkXY = true;
                commandCounter++;
                GCodeLine.CopyInside(ref gCodeLine, HandleX(token));
                token = lexerGCode.GetNextToken();
            }

            if (token.StartsWith("Y"))
            {
                oneFound = true;
                checkXY = true;
                commandCounter++;
                GCodeLine.CopyInside(ref gCodeLine, HandleY(token));
                token = lexerGCode.GetNextToken();
            }

            if(token.StartsWith("SEG"))
            {
                oneFound = true;
                commandCounter++;
                GCodeLine.CopyInside(ref gCodeLine, HandleSEG(token));
                token = lexerGCode.GetNextToken();
            }

            if (token.StartsWith("ANG"))
            {
                commandCounter++;
                GCodeLine.CopyInside(ref gCodeLine, HandleANG(token));
                token = lexerGCode.GetNextToken();
            }

            if (!oneFound)
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingCommand));
            }

            if(commandCounter > 2)
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.ToManyCommands));
            }
            else if(commandCounter < 2 && !checkXY && oneFound)
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingCommand));
            }

            if (token != EOL)
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleComment(token));
            }

            return gCodeLine;
        }

        private GCodeLine HandleG02G03()
        {
            string token = lexerGCode.GetNextToken();

            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            bool oneFoundXY = false;
            bool oneFoundIJ = false;
            bool CircleDefinitionIJ = false;
            bool CircleDefinitionCR = false;

            if (token.StartsWith("X"))
            {
                oneFoundXY = true;
                GCodeLine.CopyInside(ref gCodeLine, HandleX(token));
                token = lexerGCode.GetNextToken();
            }

            if (token.StartsWith("Y"))
            {
                oneFoundXY = true;
                GCodeLine.CopyInside(ref gCodeLine, HandleY(token));
                token = lexerGCode.GetNextToken();
            }

            if (!oneFoundXY)
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingCommand));
            }

            if (token.StartsWith("I"))
            {
                oneFoundIJ = true;
                CircleDefinitionIJ = true;
                GCodeLine.CopyInside(ref gCodeLine, HandleX(token));
                token = lexerGCode.GetNextToken();
            }

            if (token.StartsWith("J"))
            {
                oneFoundIJ = true;
                CircleDefinitionIJ = true;
                GCodeLine.CopyInside(ref gCodeLine, HandleY(token));
                token = lexerGCode.GetNextToken();
            }

            if (token.StartsWith("CR"))
            {
                CircleDefinitionCR = true;
                GCodeLine.CopyInside(ref gCodeLine, HandleCR(token));
                token = lexerGCode.GetNextToken();
            }

            if (!oneFoundIJ && !CircleDefinitionCR)
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.MissingCommand));
            }

            if (CircleDefinitionIJ && CircleDefinitionCR)
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.IJAndCR));
            }

            if (token != EOL)
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleComment(token));
            }

            return gCodeLine;
        }

        private GCodeLine HandleG04()
        {
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            string token = lexerGCode.GetNextToken();

            if (token.StartsWith("P"))
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleP(token));
                token = lexerGCode.GetNextToken();
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingPCommand));
            }

            if (token != EOL)
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleComment(token));
            }

            return gCodeLine;
        }

        private GCodeLine HandleG58G59()
        {
            string token;

            token = lexerGCode.GetNextToken();


            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);


            if (token.StartsWith("X"))
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleX(token));
                token = lexerGCode.GetNextToken();
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingXCommand));
            }

            if (token.StartsWith("Y"))
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleY(token));
                token = lexerGCode.GetNextToken();
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber
                    , GCodeLine.GCodeErrorCodes.MissingYCommand));
            }

            if (token != EOL)
            {
                GCodeLine.CopyInside(ref gCodeLine, HandleComment(token));
            }

            return gCodeLine;
        }

        private GCodeLine HandleY(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(1).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.YPos = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.YPos, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleX(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(1).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.XPos = number;
            }

            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.XPos, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleSEG(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(3).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.SEG = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.SEGNumber, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleANG(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(3).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.ANG = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.ANGNumber, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleI(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(1).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.IPos = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.IPos, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleJ(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(1).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.JPos = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.JPos, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleCR(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(1).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.CRNumber = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.CRNumber, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleP(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(1).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.PNumber = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.PNumber, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleTX(string token)
        {
            int number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            if (int.TryParse(token.Substring(2).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.PartsX = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.TXNumber, token));
            }


            return gCodeLine;
        }

        private GCodeLine HandleTY(string token)
        {
            int number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);

            if (int.TryParse(token.Substring(2).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.PartsY = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.TYNumber, token));
            }


            return gCodeLine;
        }

        private GCodeLine HandleLX(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(2).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.SubPrgDistanceX = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.LXNumber, token));
            }
            return gCodeLine;
        }

        private GCodeLine HandleLY(string token)
        {
            double number;
            GCodeLine gCodeLine = new GCodeLine(lexerGCode.CurrentLineNumber);
            if (Double.TryParse(token.Substring(2).Replace(".", ","), out number))
            {
                //number will be secured
                gCodeLine.SubPrgDistanceY = number;
            }
            else
            {
                //Error
                gCodeLine.Error.Add(new GCodeLine.ErrorGCode(lexerGCode.CurrentTokenNumber, GCodeLine.GCodeErrorCodes.LYNumber, token));
            }
            return gCodeLine;
        }

        //TODO: Die anderen Handle(n) fertigstellen
    }
}
