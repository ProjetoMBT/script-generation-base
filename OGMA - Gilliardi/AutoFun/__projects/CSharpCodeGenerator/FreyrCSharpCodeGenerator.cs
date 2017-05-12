using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.DataStructs.ProductConfigurator;
using System.IO;
using System.Xml;

namespace CSharpCodeGenerator
{
    // Freyr: Norse god of kingship, fertility, prosperity, sunshine and fair weather. Yes, we like C#.
    public static class FreyrCSharpCodeGenerator
    {
        public static void GenerateProjectFile(DanuComponent comp)
        {
            string filename = comp.Name + ".csproj";
            XmlWriter writer = XmlWriter.Create(filename);

            writer.WriteStartElement("Project");
            writer.WriteAttributeString("ToolsVersion", "4.0");
            writer.WriteAttributeString("DefaultTargets", "Build");
            writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");

            writer.WriteStartElement("PropertyGroup");

            writer.WriteStartElement("Configuration");
            writer.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
            writer.WriteValue("Debug");
            writer.WriteEndElement();

            writer.WriteStartElement("Platform");
            writer.WriteAttributeString("Condition", " '$(Platform)' == '' ");
            writer.WriteValue("AnyCPU");
            writer.WriteEndElement();

            writer.WriteElementString("ProductVersion", "8.0.30703");
            writer.WriteElementString("SchemaVersion", "2.0");
            writer.WriteElementString("ProjectGuid", "{24279181-0EF3-4F9D-8810-7ECEB097158E}");
            writer.WriteElementString("OutputType", "Library");
            writer.WriteElementString("AppDesignerFolder", "Properties");
            writer.WriteElementString("RootNamespace", comp.Name);
            writer.WriteElementString("AssemblyName", comp.Name);
            writer.WriteElementString("TargetFrameworkVersion", "v4.0");
            writer.WriteElementString("FileAlignment", "512");

            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ");
            writer.WriteElementString("DebugSymbols", "true");
            writer.WriteElementString("DebugType", "full");
            writer.WriteElementString("Optimize", "false");
            writer.WriteElementString("OutputPath", "bin\\Debug\\");
            writer.WriteElementString("DefineConstants", "DEBUG;TRACE");
            writer.WriteElementString("ErrorReport", "prompt");
            writer.WriteElementString("WarningLevel", "4");
            writer.WriteEndElement();

            writer.WriteStartElement("PropertyGroup");
            writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ");
            writer.WriteElementString("DebugType", "pdbonly");
            writer.WriteElementString("Optimize", "true");
            writer.WriteElementString("OutputPath", "bin\\Release\\");
            writer.WriteElementString("DefineConstants", "TRACE");
            writer.WriteElementString("ErrorReport", "prompt");
            writer.WriteElementString("WarningLevel", "4");
            writer.WriteEndElement();

            writer.WriteStartElement("ItemGroup");
            writer.WriteStartElement("Reference");
            writer.WriteAttributeString("Include", "System");
            writer.WriteEndElement();
            writer.WriteStartElement("Reference");
            writer.WriteAttributeString("Include", "System.Core");
            writer.WriteEndElement();
            writer.WriteStartElement("Reference");
            writer.WriteAttributeString("Include", "System.Xml.Linq");
            writer.WriteEndElement();
            writer.WriteStartElement("Reference");
            writer.WriteAttributeString("Include", "System.Data.DataSetExtensions");
            writer.WriteEndElement();
            writer.WriteStartElement("Reference");
            writer.WriteAttributeString("Include", "Microsoft.CSharp");
            writer.WriteEndElement();
            writer.WriteStartElement("Reference");
            writer.WriteAttributeString("Include", "System.Data");
            writer.WriteEndElement();
            writer.WriteStartElement("Reference");
            writer.WriteAttributeString("Include", "System.Xml");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("ItemGroup");
            writer.WriteStartElement("Compile");
            writer.WriteAttributeString("Include", comp.Name + ".cs");
            writer.WriteEndElement();
            writer.WriteStartElement("Compile");
            writer.WriteAttributeString("Include", "Properties\\AssemblyInfo.cs");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("Import");
            writer.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public static void GenerateMainClass(DanuComponent comp)
        {
            string filename = comp.Name + ".cs";
            File.Create(filename).Close();
            TextWriter writer = new StreamWriter(filename);

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");

            writer.WriteLine("");

            writer.WriteLine("namespace " + comp.Name);
            writer.WriteLine("{");
            writer.WriteLine("\t// This is the core class of the " + comp.Name + " component.");
            writer.Write("\tpublic class " + comp.Name);

            if (comp.Sockets.Count != 0)
            {
                writer.Write(" : ");
                foreach (DanuSocket so in comp.Sockets)
                {
                    writer.Write(so.InterfaceUsed.Name + ", ");
                }
                writer.Write("\n");
            }

            writer.WriteLine("\t{");

            foreach (DanuInterfaceObject io in comp.Interfaces)
            {
                writer.WriteLine("\t\tpublic " + io.Name);
            }

            writer.WriteLine("\t\t");
            writer.WriteLine("\t}");
            writer.WriteLine("}");
        }

        public static void GenerateInterface(DanuInterfaceObject io)
        {
            string filename = io.Name + ".cs";
            File.Create(filename).Close();
            TextWriter writer = new StreamWriter(filename);

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");

            writer.WriteLine("");

            writer.WriteLine("namespace " + io.Name);
            writer.WriteLine("{");
            writer.WriteLine("\tpublic interface " + io.Name);
            writer.WriteLine("\t{");
            writer.WriteLine("\t\t");
            writer.WriteLine("\t}");
            writer.WriteLine("}");
        }
    }
}
