using System;
using System.Collections.Generic;
using Coc.Data.Interfaces;
using Coc.Data.Xmi.AbstractValidator;
using Coc.Data.AbstractSequenceGenerator;
using Coc.Data.ControlStructure;
using Coc.Data.AbstractParser;
using Coc.Data.ControlAndConversionStructures;
using System.Xml;

namespace Coc.Apps.PLeTs.ControlUnit
{    
    public class ControlUnit
    {
        #region Attributes
        private Validator validator;
        private SequenceGenerator sequenceGenerator;
        private StructureCollection structureCollection;
        private Parser parser;
        private ParsedStructureExporter exporter;
        private ScriptGenerator scriptGenerator;
        private String path;
        public String Name { get; set; }
        //public GeneralUseStructure generalStructure { get; set; }
        public List<GeneralUseStructure> listgeneralStructure { get; set; }
        public int TestCaseCount { get; set; }
        #endregion

        //public Tuple<List<GeneralUseStructure>, StructureType> listGeneralStructure;

        #region Constructor
        public ControlUnit(StructureType type)
        {
            exporter = ParsedStructureExporterFactory.CreateExporter();
            validator = ValidatorFactory.CreateValidator();
            sequenceGenerator = SequenceGeneratorFactory.CreateSequenceGenerator(type);
            scriptGenerator = ScriptGeneratorFactory.CreateScriptGenerator();
            this.structureCollection = new StructureCollection();
        }

        public ControlUnit()
        {
            // TODO: Complete member initialization
        }
        #endregion

        //public string ToolName() { return null; }
        //public string ToolVersion() { return null; }
        //TODO: Button Activation/Deactivation sequence parameters. Different GUI?
        //TODO: Function-specific structure holder component to externalize structure variable definition from ControlUnit

        #region Public Methods
        public void LoadModelingStructure(String path, String parserType)
        {
            parser = ParserFactory.CreateParser(parserType);
            String name = "";
            this.path = path;
            ResetAttributes();
            structureCollection = parser.ParserMethod(path, ref name, null);
            this.Name = name;
        }

       

        public XmlDocument ExportParsedStructure()
        {
            return exporter.ToXmi(structureCollection.listGeneralStructure);
        }

        public List<KeyValuePair<String, Int32>> ValidateModel(String filename)
        {
            return validator.Validate(structureCollection.listGeneralStructure, filename);
        }

        public void GenerateSequence(StructureType type)
        {
            int tcCount = 0;
            //transforma em grafo/fsm
            //aplica dfs/hsi
            //retorna uma lista de TestPlan.
            listgeneralStructure = sequenceGenerator.GenerateSequence(structureCollection.listGeneralStructure, ref tcCount, type);

            this.TestCaseCount = tcCount + 1; //soma 1 em função do TestCase geral;
        }


        public void GenerateScript(String path)
        {
            //scriptGenerator.GenerateScript(structureCollection.listGeneralStructure, path);
            scriptGenerator.GenerateScript(listgeneralStructure, path);
        }

      
        #endregion

        #region Private Methods
        private void ResetAttributes()
        {
           // generalStructure = null;
            TestCaseCount = 0;
        }
        #endregion
    }
}
