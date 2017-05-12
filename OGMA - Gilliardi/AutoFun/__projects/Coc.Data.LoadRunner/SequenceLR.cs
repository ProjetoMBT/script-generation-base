using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.LoadRunner.SequenceModel;
using Coc.Modeling.Uml;
using System.Windows;

namespace Coc.Data.LoadRunner
{
    public class SequenceLR
    {
        public static List<Scenario> GenerateSequenceModel(UmlModel model, List<String[][]> testCases)
        {
            foreach(String[][] s in testCases)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    for (int j = 0; j < s[i].Length; j++)
                    {
                        foreach (UmlActivityDiagram actDiagram in model.Diagrams.OfType<UmlActivityDiagram>())
                        {
                            foreach (UmlTransition transition in actDiagram.UmlObjects.OfType<UmlTransition>())
                            {
                                String input = s[i][j];
                                if (input.Equals(transition.GetTaggedValue("TDACTION")))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}