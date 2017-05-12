/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package argoparser;

import Objects.UmlModel;
import OgmaOATSParser.Parser;
import Servico.ArgoUMLtoAstahXML;
import Servico.PopulateStructureModel;
import Servico.ReaderDocumentXML;
import java.util.logging.Logger;
import javax.swing.JOptionPane;

public class ArgoParser
{

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args)
    {
        try
        {
            //args = new String[2];
            //args[0] = "D:\\Development\\CePES\\Plets Workspace\\script.java";
            //args[1] = "JAVA";
            if (args.length == 2)
            {
                PopulateStructureModel psm = new PopulateStructureModel();
                Parser p = new Parser();
                ReaderDocumentXML reader = new ReaderDocumentXML();
                ArgoUMLtoAstahXML exporter = new ArgoUMLtoAstahXML();
                UmlModel model = null;
                String path = args[0];//.split("\n\n\n\n\n")[0];
                String type = args[1];//.split("\n\n\n\n\n")[1];
                //String path = "C:\\Users\\COC-7-01\\Desktop\\head\\__projects\\OgmaOATSParser\\GLC\\script.java";
                //String type = "JAVA";
                //UmlModel model = psm.methodPopulateModel(reader.methodReaderXML("C:\\Users\\COC-7-01\\Desktop\\head\\__output\\plets\\Models\\ModeloTeste_Argo.xmi"));
                switch (type)
                {
                    case "ARGO":
                        model = psm.methodPopulateModel(reader.methodReaderXML(path));
                        break;
                    case "JAVA":
                        model = p.ReadScript(path);
                        break;
                }
                exporter.ToXmi(model);
            }
            else
            {
                JOptionPane.showMessageDialog(null, "Erro na execução do .JAR:" +
                " argsLen: " + args.length, "ArgoParser", JOptionPane.ERROR_MESSAGE);
            }
        }
        catch (Exception e)
        {
            JOptionPane.showMessageDialog(null, "Erro na execução do .JAR" + e, "ArgoParser", JOptionPane.ERROR_MESSAGE);
        }
    }
}
