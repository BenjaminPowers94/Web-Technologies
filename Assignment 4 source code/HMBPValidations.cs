using System.Text.RegularExpressions;
using System.Globalization;

namespace HMBPClassLibrary
{
    public static class HMBPValidations
    {
        public static string HMBPCapitalize(string str)
        {
            string myStr = "";

            if (str != "" || str != null) //checks if input string is not empty or null
            {
                myStr = str.ToLower().Trim(); //converts to lowercase and trims each string beggining and end
                myStr = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(myStr); //Capitlizes first word in each new word
            }

            return myStr;
        }
        public static string HMBPExtractDigits(string str)
        {
            string myStr = "";

            if (str != "" || str != null) //checks if input string is not empty or null 
            {
                foreach (char c in str) //for each character in string
                {
                    if (int.TryParse(c.ToString(), out int tempInt)) //if parses to int works, add char to return string
                    {
                        myStr += c.ToString();
                    }
                }
            }

            return myStr;
        }
        public static bool HMBPPostalCodeValidation(string str)
        {
            // Regex for canadian postal code
            Regex pattern = new Regex(@"^[ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ] ?\d[ABCEGHJKLMNPRSTVWXYZ]\d$", RegexOptions.IgnoreCase);
            bool retBool = false;

            if (pattern.IsMatch(str)) //if matched 
            {
                retBool = true; //set return to true
            }
            else
            {
                retBool = HMBPZipCodeValidation(ref str); // checks to see if american 
            }

            return retBool;
        }
        public static string HMBPPostalCodeFormat(string str)
        {
            string myStr = "";

            if (str != "" || str != null) //checks if input string is not empty or null
            {
                if (str[3] != ' ') //if 4th character isn't a space
                {
                    myStr = str.Insert(3, " "); //add space in 4th character space
                    myStr = myStr.ToUpper(); //converts string to upper
                }
                else
                {
                    myStr = str.ToUpper(); //converts string to upper
                }
            }

            return myStr;
        }
        public static bool HMBPZipCodeValidation(ref string str)
        {
            bool retBool = false;
            string myStr = "";

            if (str != "" || str != null) //if input string isn't empty or null
            {
                myStr = HMBPExtractDigits(str); //extracts digits from string

                if (myStr.Length == 5) //if returned string size is 5
                {
                    str = myStr; //set into original string
                    retBool = true;
                }
                else if (myStr.Length == 9) //if size is 9
                {
                    str = myStr.Insert(5, "-"); //add dash at char location 6
                    retBool = true;
                }
                else
                {
                    retBool = false;
                }
            }
            else
            {
                retBool = false;
                str = "";
            }

            return retBool;
        }

        public static bool isEmpty(string input)
        {
            if (input == null || input == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
