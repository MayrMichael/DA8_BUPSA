using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using LibraryGCode;

namespace LibraryGCode_Test
{
    [TestClass]
    public class LexerGCodeTest
    {
        [TestMethod]
        public void LexerGcodeTest1()
        {
            string code = "G01 x10 Y10" + Environment.NewLine + "G02 x10 y20";
            string token;
            LexerGCode lg = new LexerGCode(code);

            string tokenList = "";

            do
            {
                token = lg.GetNextToken();
                tokenList += token + ",";
            }
            while (token != "#EOF");


            string solution = "G01,X10,Y10,#EOL,G02,X10,Y20,#EOL,#EOF,";

            Assert.AreEqual(tokenList, solution);

            //Parser p = new Parser();
            //var res = p.Parse("G01 X10");
            //Assert.AreEqual(res, "asdf");
        }

        [TestMethod]
        public void LexerGcodeTest2()
        {
            GCodeLine gcl1 = new GCodeLine(1);
            GCodeLine gcl2 = new GCodeLine(1);

            gcl1.Command.Add("G01");
            gcl2.Command.Add("M10");

            gcl1.XPos = 10;
            gcl2.YPos = 20;

            gcl2.Error.Add(new GCodeLine.ErrorGCode(5,GCodeLine.GCodeErrorCodes.YPos,"Yp10"));

            GCodeLine.CopyInside(ref gcl1, gcl2);

            GCodeLine gCodeLineTest = new GCodeLine(1,"G01",10,20);
            gCodeLineTest.Command.Add("M10");
            gCodeLineTest.Error.Add(new GCodeLine.ErrorGCode(5, GCodeLine.GCodeErrorCodes.YPos, "Yp10"));

            Assert.AreEqual(gcl1.ToString(), gCodeLineTest.ToString());
        }
    }

    [TestClass]
    public class ParserGCodeTest
    {
        private string ListToString(List<GCodeLine> gCodeLines)
        {
            string text = "";

            foreach(GCodeLine gcl in gCodeLines)
            {
                text += gcl.ToString() + ";";
            }

            return text;
        }

        [TestMethod]
        public void ParserGCode_G1_Correct_G01_X10_Y10_Test()
        {
            ParserGCode p = new ParserGCode();

            List<GCodeLine> result = p.Parse("G01 X10 Y10");
            List<GCodeLine> rightresult = new List<GCodeLine>();

            GCodeLine gCodeLineResult1 = new GCodeLine(1,"G1",10,10);

            rightresult.Add(gCodeLineResult1);
            rightresult.Add(new GCodeLine(2));

            string resulttext = ListToString(result);
            string rightresulttext = ListToString(rightresult);

            Assert.AreEqual(resulttext, rightresulttext);
        }

        [TestMethod]
        public void ParserGCode_G1_Correct_G1_X10_Test()
        {
            ParserGCode p = new ParserGCode();

            List<GCodeLine> result = p.Parse("G1 X10");
            List<GCodeLine> rightresult = new List<GCodeLine>();

            GCodeLine gCodeLineResult1 = new GCodeLine(1);
            gCodeLineResult1.Command.Add("G1");
            gCodeLineResult1.XPos = 10;

            rightresult.Add(gCodeLineResult1);
            rightresult.Add(new GCodeLine(2));

            string resulttext = ListToString(result);
            string rightresulttext = ListToString(rightresult);

            Assert.AreEqual(resulttext, rightresulttext);
        }

        [TestMethod]
        public void ParserGCode_G1_Correct_G01_Y10_52_Test()
        {
            ParserGCode p = new ParserGCode();

            List<GCodeLine> result = p.Parse("G01 Y10,52");
            List<GCodeLine> rightresult = new List<GCodeLine>();

            GCodeLine gCodeLineResult1 = new GCodeLine(1);
            gCodeLineResult1.Command.Add("G1");
            gCodeLineResult1.YPos = 10.52;

            rightresult.Add(gCodeLineResult1);
            rightresult.Add(new GCodeLine(2));

            string resulttext = ListToString(result);
            string rightresulttext = ListToString(rightresult);

            Assert.AreEqual(resulttext, rightresulttext);
        }

        [TestMethod]
        public void ParserGCode_G1_Correct_G01_SEG10_52_ANG45_Test()
        {
            ParserGCode p = new ParserGCode();

            List<GCodeLine> result = p.Parse("G01 SEG10.52 ANG45");
            List<GCodeLine> rightresult = new List<GCodeLine>();

            GCodeLine gCodeLineResult1 = new GCodeLine(1);
            gCodeLineResult1.Command.Add("G1");
            gCodeLineResult1.SEG = 10.52;
            gCodeLineResult1.ANG = 45;

            rightresult.Add(gCodeLineResult1);
            rightresult.Add(new GCodeLine(2));

            string resulttext = ListToString(result);
            string rightresulttext = ListToString(rightresult);

            Assert.AreEqual(resulttext, rightresulttext);
        }

        [TestMethod]
        public void ParserGCode_G1_Correct_G01_G41_X10_X20_Test()
        {
            ParserGCode p = new ParserGCode();

            List<GCodeLine> result = p.Parse("G01 G41 X10 " + Environment.NewLine + "X20");
            List<GCodeLine> rightresult = new List<GCodeLine>();

            GCodeLine gCodeLineResult1 = new GCodeLine(1);
            gCodeLineResult1.Command.Add("G1");
            gCodeLineResult1.Command.Add("G41");
            gCodeLineResult1.XPos = 10;

            GCodeLine gCodeLineResult2 = new GCodeLine(2);
            gCodeLineResult2.Command.Add("G1");
            gCodeLineResult2.XPos = 20;

            rightresult.Add(gCodeLineResult1);
            rightresult.Add(gCodeLineResult2);
            rightresult.Add(new GCodeLine(3));

            string resulttext = ListToString(result);
            string rightresulttext = ListToString(rightresult);

            Assert.AreEqual(resulttext, rightresulttext);
        }


        [TestMethod]
        public void ParserGCode_G1_Error1_Test()
        {
            ParserGCode p = new ParserGCode();

            

            List<GCodeLine> result = p.Parse("G01 hallo wie #gehts");
            List<GCodeLine> rightresult = new List<GCodeLine>();

            GCodeLine gCodeLineResult1 = new GCodeLine(1);

            gCodeLineResult1.Command.Add("G1");
            gCodeLineResult1.Error.Add(new GCodeLine.ErrorGCode(2, GCodeLine.GCodeErrorCodes.MissingCommand));
            gCodeLineResult1.Error.Add(new GCodeLine.ErrorGCode(2, GCodeLine.GCodeErrorCodes.WrongCommand,"HALLO"));
            gCodeLineResult1.Error.Add(new GCodeLine.ErrorGCode(3, GCodeLine.GCodeErrorCodes.WrongCommand,"WIE"));

            rightresult.Add(gCodeLineResult1);
            rightresult.Add(new GCodeLine(2));

            string resulttext = ListToString(result);
            string rightresulttext = ListToString(rightresult);

            Assert.AreEqual(resulttext, rightresulttext);
        }
    }

    [TestClass]
    public class HelpTests
    {
        [TestMethod]
        public void Sample_Test()
        {
            int storage = 0;

            for (int i = 0; i < 5; i++)
            {
                if (i > 2)
                {
                    storage = i;
                    break;
                }
            }

            storage = storage;
        }
    }
}
