using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.Atlas;

/* V2: First version of Coeus. It is complete for V2. It has the full capacity of the Lexical Analyser, but is 
 * unable to make use of most of its classes. The parser is currently able to read a .cfr file and return an Atlas object.
 * 
 * V3 (prediction): The most important aspect that must be explored from Clafer are the parsing of an Atlas structure
 * into a .cfr file. Aside from this, it is important for Coeus to be able to parse cardinality and constraint
 * properties, but these have not yet been implemented in V2.
 **/
namespace ClaferLanguage
{
    // Coeus: Greek titan that represents the axis of the universe.
    public class ClaferParserCoeus
    {
        private ClaferLexicalAnalyser analyser = new ClaferLexicalAnalyser();
        InputQueue entry;

        private AtlasFeatureModel atlas;
	    public AtlasFeatureModel Atlas
	    {
		    get { return atlas;}
	    }

        public ClaferParserCoeus(string input)
        {
            entry = new InputQueue(input);
            ClaferClassNode lex = analyser.BuildLexicon(entry);
            entry.Pointer = 0;

            BuildAtlas(lex);
        }

        private void BuildAtlas(ClaferClassNode lex)
        {
            atlas = new AtlasFeatureModel();

            if (lex.Type == ClaferClasses.Module)
            {
                lex = lex.Derivations.First();
                if (lex.Type == ClaferClasses.ListDeclaration)
                {
                    ReadDeclaration(lex.Derivations[0]);
                }
            }
        }
        private string ReadIdent(ClaferClassNode ident)
        {
            return entry.Pop();
        }
        private bool ReadAbstract(ClaferClassNode abstractNode)
        {
            if (abstractNode.Derivations.First().Type == ClaferClasses.ReservedWord)
            {
                entry.Pop();
                return true;
            }
            return false;
        }
        private AtlasConnectionType ReadGCard(ClaferClassNode gCard)
        {
            if (gCard.Derivations.First().Type == ClaferClasses.ReservedWord)
            {
                string switchKey = entry.Pop();
                switch (switchKey)
                {
                    case "mux":
                        return AtlasConnectionType.Optional;

                    case "xor":
                        return AtlasConnectionType.Alternative;

                    case "or":
                        return AtlasConnectionType.OrRelation;
                }
            }
            return AtlasConnectionType.Mandatory;
        }
        private void ReadDeclaration(ClaferClassNode declaration)
        {
            if (declaration.Type == ClaferClasses.Declaration)
            {
                ClaferClassNode clafer = declaration.Derivations.First();

                if (clafer.Type == ClaferClasses.Clafer)
                {
                    ReadClafer(clafer);
                }
            }
        }
        private void ReadClafer(ClaferClassNode clafer)
        {
            bool isAbstract = ReadAbstract(clafer.Derivations[0]);
            string name = ReadIdent(clafer.Derivations[2]);

            AtlasFeature feature = new AtlasFeature(name);
            feature.IsAbstract = isAbstract;

            atlas.CreateFeatureModel(feature);

            ReadElements(clafer.Derivations[5], feature);
        }
        private void ReadElements(ClaferClassNode elements, AtlasFeature feature)
        {
            if (elements.Type == ClaferClasses.Elements && elements.Derivations[0].Type != ClaferClasses.Epsilon)
            {
                entry.Pop();
                ReadListElement(elements.Derivations[1], feature);
                entry.Pop();
            }
        }
        private void ReadListElement(ClaferClassNode listElement, AtlasFeature feature)
        {
            if (listElement.Type == ClaferClasses.ListElement)
            {
                if (listElement.Derivations[0].Type != ClaferClasses.Epsilon)
                {
                    ReadClafer(listElement.Derivations[0].Derivations[0], feature);
                    ReadListElement(listElement.Derivations[1], feature);
                }
            }
        }
        private void ReadClafer(ClaferClassNode clafer, AtlasFeature feature)
        {
            bool isAbstract = ReadAbstract(clafer.Derivations[0]);
            AtlasConnectionType type = ReadGCard(clafer.Derivations[1]);
            string name = ReadIdent(clafer.Derivations[2]);

            AtlasFeature newFeature = new AtlasFeature(name);
            newFeature.IsAbstract = isAbstract;

            atlas.AddFeature(newFeature, feature, type);

            ReadElements(clafer.Derivations[5], newFeature);
        }
    }
}