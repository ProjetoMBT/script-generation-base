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
            ogma.ParserMethod(@"C:\Users\Marcelo\Dropbox\CePES\OgmaOATSParser\OgmaOATSParser\GLC\script.java");
        }
    }
}
