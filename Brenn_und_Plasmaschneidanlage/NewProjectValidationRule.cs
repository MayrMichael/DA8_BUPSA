using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Brenn_und_Plasmaschneidanlage
{
    class NewProjectValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value , CultureInfo cultureInfo)
        {
            string name = value as string;

            if (name != null && name != "")
            {
                string pattern = @"^[a-zA-Z0-9_]+$";
                Regex regex = new Regex(pattern);

                if (!regex.IsMatch(name))
                    return new ValidationResult(false , "Mindestens ein Zeichen ist ungültig! Nur a-z, A-Z, 0-9 und _ sind gültige Zeichen");
                else
                    return ValidationResult.ValidResult;
            }
            else
                return new ValidationResult(false , "Name vergeben");
        }
    }
}
