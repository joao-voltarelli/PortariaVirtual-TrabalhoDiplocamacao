using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
    class Substring
    {
        public static string Java(string s, int beginIndex, int endIndex)
        {
            // simulates Java substring function
            int len = endIndex - beginIndex;
            return s.Substring(beginIndex, len);
        }
    }
}
