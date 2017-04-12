using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Coc.Data.ControlStructure;
using Coc.Data.Interfaces;

namespace OgmaOATSParser
{
    /*
     * This is Ogma, an Oracle ATS script parser built through the use of a simple Context-Free Grammar. 
     * In this header we will explain how Ogma functions in two over-arching steps. Within the code, you will
     * find more detailed descriptions of each step.
     * 
     * It is important to note that Ogma is 'not' built for performance. Our goal was to make it as readable
     * as possible so as to enable the comprehension of our approach. Therefore, bear in mind that if applied
     * to extensive scripts in daily use, it may be found lacking.
     * 
     * The reason for this component to be documented in this manner, rather than in the usual documentation 
     * format used by the development team responsible for the overarching PLeTs tool, is the development team's
     * understanding that the use of Formal Languages is not as widely understood as other areas of programming.
     * Therefore, we hope that anyone who is assigned to maintain or expand this component will be able to
     * quickly come to grips with it through these instructions.
     * 
     * 1º: The first step in Ogma's parsing method is the tokenization of a given Oracle ATS script. To do this,
     * the class Lexical Analyser is used. Within it, a single, continuous string representing the entire script
     * is broken down into tokens that are then analysed one by one, following a basic CFG description. As a result
     * of this phase, a tree structure is built with the entire derivation of each token within the script.
     * 
     * 2º: Given that the Lexical and Syntactic analyses are put together in the Lexical Analyser phase, the
     * structure that is returned from it is assumed to be syntactically correct. Therefore, all that is left is the
     * Semantic analysis of the script. By making use of the built lexicon, as well as the input script itself,
     * Ogma goes on to analyse each token in the script in regards to its semantic representation, that is, the actual
     * values are assigned to an UML model structure.
     * 
     * It is important to note that Ogma does NOT validate the values of the script. That is, given a script with a 
     * correct lexical and syntactic structure, the semantic analysis is done merely in regards of assigning values
     * from the script to an UML model. The validation of these values must be done on the UML model structure itself.
     */
    public class Ogma : Parser
    {
        private LexicalAnalyser analyser;
        private InputQueue entry;

        #region Control Methods
            public Ogma()
            {
                analyser = new LexicalAnalyser();
            }

            /*
             * This simple method is the starting point of Ogma's parsing process, building the lexicon (lex) and
             * making a call to the first rule of the CFG.
             */
            public List<ModelingStructure> ParserMethod(string path)
            {
                LoadFile(path);
                ClassNode lex = analyser.BuildLexicon(entry);
                entry.Pointer = 0;

                throw new NotImplementedException();
            }

            private void LoadFile(string path)
            {
                TextReader reader = new StreamReader(path);
                entry = new InputQueue(reader.ReadToEnd());
            }
        #endregion
    }
}