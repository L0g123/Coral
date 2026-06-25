using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    internal static class Util
    {
        public static string Ellipsis(string str, int cutoff)
        {
            if(str.Length < cutoff) return str;

            return str[0..(cutoff-1)] + '…';
        }
    }
}
