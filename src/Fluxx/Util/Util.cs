namespace Faml.Util
{
    public class Util
    {
        /// <summary>
        /// Return the input string except with the first character in the string made upper case.
        /// </summary>
        /// <param name="s">string in question</param>
        /// <remarks> s with first character converted to upper case</remarks>
        public static string UpperCaseFirstCharacter(string s)
        {
            if (s.Length == 0)
            {
                return s;
            }

            return s.Substring(0, 1).ToUpper() + s.Substring(1);
        }
    }
}
