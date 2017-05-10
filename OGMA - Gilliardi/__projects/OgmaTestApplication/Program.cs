using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmaOATSParser;

namespace OgmaTestApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Ogma ogma = new Ogma();
            String sblabs = "sblabs";
            ogma.ParserMethod(@"C:\Users\COC-7-01\Desktop\head\__projects\OgmaOATSParser\GLC\script3.java", ref sblabs,null);
        }
    }
}
