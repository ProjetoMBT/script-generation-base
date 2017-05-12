using System.IO;
using ClaferLanguage;
using PlugSpl.Atlas;
using System.Windows.Forms;

namespace FeatureModelEditor.Clafer
{
    public class ClaferFileFormat: IFileFormat
    {
        public void SaveTo(AtlasFeatureModel atlas, string filename)
        {
            MessageBox.Show("Save function for Clafer language was not implemented.");
        }

        public AtlasFeatureModel LoadFrom(string filename)
        {
            TextReader reader = new StreamReader(filename);
            ClaferParserCoeus coeus = new ClaferParserCoeus(reader.ReadToEnd());

            AtlasFeatureModel atlas = coeus.Atlas;
            reader.Close();
            return atlas;
        }

        public string GetFilter()
        {
            return "Clafer Feature Model (*.cfr)|*.cfr";
        }
    }
}
