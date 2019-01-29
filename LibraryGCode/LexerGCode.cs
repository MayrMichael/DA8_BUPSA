using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LibraryGCode
{
    public class LexerGCode
    {
        public int CurrentLineNumber { get; private set; }

        public int CurrentTokenNumber { get; private set; }

        public string CurrentToken { get; private set; }

        public const string EOF = "$EOF";
        public const string EOL = "$EOL";

        private List<string> lines = new List<string>();
        private List<string> tokensEachLine = new List<string>();

        private int lineCounter = 0;

        public LexerGCode(string code)
        {
            //Splits the code in lines

            //code = Regex.Replace(code, @"\s+", " ");
            lines = code.ToUpper().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

            for(int i = 0;i < lines.Count; i++)
            {
                lines[i] = Regex.Replace(lines[i].Trim(' '), @"\s+", " ");
            }

            CurrentLineNumber = 1;
            CurrentTokenNumber = 0;
        }

        public string GetNextToken()
        {
            string currentToken;

            if (CurrentToken == EOL)
            {
                CurrentTokenNumber = 0;
                lineCounter++;
                CurrentLineNumber++;
            }

            if (lineCounter < lines.Count())
            {
                tokensEachLine = lines[lineCounter].Split(' ').ToList();

                if(CurrentTokenNumber < tokensEachLine.Count())
                {
                    currentToken = tokensEachLine[CurrentTokenNumber];
                    CurrentTokenNumber++;
                }
                else
                {
                    currentToken = EOL;
                    CurrentTokenNumber++;
                }

            }
            else
            {
                currentToken = EOF;
            }

            CurrentToken = currentToken;

            if (currentToken == "")
                currentToken = GetNextToken();

            return currentToken;

        } 
    }

}
