using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Novacode;
using System.Drawing;

namespace FunctionalTool.Data
{
    class Word
    {
        internal static void generateWord()
        {
            using (DocX document = DocX.Create("Test.docx"))
            {
                Paragraph p = document.InsertParagraph();

                p.Append("Hello World").Font(new FontFamily("Arial Black"));

                document.Save();
            }
        }
    }
}
