using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using PlugSpl.DataStructs.ProductConfigurator;
using PlugSpl.DataStructs.ComponentPoolManager;
using ComponentPoolManager;
using System.Windows;
using System.Xml.Linq;
using System.Collections;
using System.Reflection;

/* V2: First version of Freyr. The four primary files required by Visual Studio are already being created.
 * V2 (pending): 
 *      Freyr does NOT REPLACE TAGS ON COMPILATION.     
 * V3 (prediction): Freyr is not a priority in V3. It is likely to remain in maintenance only.
 * V3 (reality): Yeah right. Gotta burn this to the ground and build it up again.
 * 
 * 
 * 
 * V3.0 (proposal): Welp, here we go.
 * 
 * - First of all, this thing is a mess. All of the permanent string parts of it will go into auxiliary methods that will be 
 * called solely for that. That way, anything that requires modifying won't be lost amidst piles of hard code.
 *      Amendment: I'll turn them into regions and keep them in the methods, so as not to create unnecessary ones.
 *          DONE
 * 
 * - Second, the way the methods are constructed needs to be modified. They are an inner, very specific function of Interface
 * construction, and therefore they can have their own method.
 * 
 * - Third, the way a project's output type is to be selected must be made variable, rather than hard code.
 * 
 * - Fourth, ifdef. Self-explanatory, but it's so that the optionals can be implemented accordingly with the minVar in Bragi and Danu.
 * 
 * - Fifth, code-replacement is a mess. I gotta figure out how to make it less awful.
 * 
 * - Sixth, comments. This is a very complicated and hard-to-read class by nature, I need to add more comments to make it easier to read.
 *      DONE
 **/

namespace ComponentPoolManager.CodeHandler
{
    // Freyr: Norse god of kingship, fertility, prosperity, sunshine and fair weather. Yes, we like C#.
    public class FreyrCSharpCodeGenerator : IPoolManagerCodeHandler
    {
        /// <summary>
        /// Generates the project file (.csproj) for a component.
        /// CURRENT: The current status is that all projects generated will become libraries. Unknown whether that is to be modified.
        /// </summary>
        /// <param name="comp">The component that is to be written.</param>
        public static void GenerateProjectFile(DanuComponent comp)
        {
            #region OBSOLETE
            //string currentExecutingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            //FileInfo f = new FileInfo(currentExecutingAssemblyLocation);
            //System.Environment.CurrentDirectory = f.Directory.FullName;
            #endregion

            // Creation of the file.
            //TODO Check whether at this point the folder is already created or not.
            string filename = comp.Name + ".csproj";
            XmlWriter writer = XmlWriter.Create("./Source/" + comp.Name + "/" + filename);

            #region Stable as of 3.0
            // This section need only be changed if ever we desire to change the versions with which to compile.
            writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            writer.WriteAttributeString("ToolsVersion", "4.0");
            writer.WriteAttributeString("DefaultTargets", "Build");
            writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
            #endregion

            #region Hardcoded
            writer.WriteStartElement("PropertyGroup");

            writer.WriteStartElement("Configuration");
            writer.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
            writer.WriteValue("Debug");
            writer.WriteEndElement();

            writer.WriteStartElement("Platform");
            writer.WriteAttributeString("Condition", " '$(Platform)' == '' ");
            writer.WriteValue("AnyCPU");
            writer.WriteEndElement();
            #endregion

            //TODO Figure out what these are.
            writer.WriteElementString("ProductVersion", "8.0.30703");
            writer.WriteElementString("SchemaVersion", "2.0");

            // Gets the GUID from the structure, which should already have been generated prior to call.
            writer.WriteElementString("ProjectGuid", comp.Specification.GUID);

            // "Library" must become some sort of variable if the OutputType ever has to change.
            writer.WriteElementString("OutputType", "Library");

            #region Hardcoded
            writer.WriteElementString("AppDesignerFolder", "Properties");
            writer.WriteElementString("RootNamespace", comp.Name);
            writer.WriteElementString("AssemblyName", comp.Name);
            #endregion

            //TODO Figure out what these are.
            writer.WriteElementString("TargetFrameworkVersion", "v4.0");
            writer.WriteElementString("FileAlignment", "512");

            writer.WriteEndElement();

            #region Compile Parameters
            /* TODO Turn these into variables so that the output path can be picked, the constants can be defined, the compile parameters
             * can be defined, etc.
             */
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
            #endregion

            #region References
            //TODO Figure these out, whether they're references, includes or what.
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
            #endregion

            #region Project Classes?
            //TODO Are these the classes that will be compiled? Gotta check on that.
            writer.WriteStartElement("ItemGroup");
            writer.WriteStartElement("Compile");
            writer.WriteAttributeString("Include", comp.Name + ".cs");
            writer.WriteEndElement();

            //TODO Need to check on whether this stuff is affected by optional or not.
            foreach (EshuInterface io in comp.Specification.Interfaces)
            {
                writer.WriteStartElement("Compile");
                writer.WriteAttributeString("Include", io.Name + ".cs");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Compile");
            writer.WriteAttributeString("Include", "Properties\\AssemblyInfo.cs");
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region Direct Refenreces?
            //TODO Check if these are the references to the classes from other projects or what. If not, well damn.
            if (comp.Sockets.Count != 0)
            {
                writer.WriteStartElement("ItemGroup");

                foreach (DanuSocket so in comp.Sockets)
                {
                    writer.WriteStartElement("ProjectReference");
                    DanuInterfaceObject io = so.InterfaceUsed;
                    writer.WriteAttributeString("Include", "..\\" + io.Eshu.ImplementingParents.First().Name + "\\" + io.Eshu.ImplementingParents.First().Name + ".csproj");
                    writer.WriteElementString("Project", io.Eshu.Parent.Parent.GUID);
                    writer.WriteElementString("Name", io.Eshu.Parent.Name);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            #endregion

            //TODO Figure this out.
            writer.WriteStartElement("Import");
            writer.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.Close();
        }

        /// <summary>
        /// Generates the project file (.csproj) for compiling the final product.
        /// CURRENT: This thing's a mess. It probably shouldn't even exist. Gotta check on it.
        /// </summary>
        /// <param name="pool">The entire pool with every single component. This thingamajig causes trouble.</param>
        /// <param name="pathname">The path to the config file, which is a glorified txt.</param>
        /// <param name="extraFiles">A list containing extra files to be added to the project file</param>
        /// <returns>No idea why it has to return anything...</returns>
        public static List<DanuComponent> GenerateProductProject(DanuProductConfigurator pool, string pathname, List<string> extraFiles)
        {
            #region OBSOLETE
            //string currentExecutingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            //FileInfo f = new FileInfo(currentExecutingAssemblyLocation);
            //System.Environment.CurrentDirectory = f.Directory.FullName;
            #endregion

            // List of components that have been chosen in the config.
            List<DanuComponent> chosenOnes = new List<DanuComponent>();
            // Frankenstein list of classes to be compiled.
            List<string> classNames = new List<string>();
            // Frankenstein list of references for the executable project.
            List<string> referenceNames = new List<string>();
            //TODO No idea.
            List<string> embeddedResource = new List<string>();
            // Reader for the config.
            XmlTextReader reader = new XmlTextReader(pathname);

            #region Finding correct components
            // Fills in the chosenOnes list with the correct components.
            /* TODO IMPORTANT: This does NOT remove the connections with unused interfaces which MAY (probably do) cause the entire
             * project to be compiled every single time. Which would be bad.
             */
            while (reader.Read())
            {
                if (!reader.Name.Equals("Component") && !reader.Name.Equals("Root"))
                {
                    chosenOnes.Add(pool.GetComponent(reader.ReadContentAsString()));
                }
            }
            #endregion

            #region Reading Component .csproj
            // Reader for each component's .csproj.
            //TODO This here code reeks some'in foul. I gotta see if this can't be eliminated.
            XmlReader compReader;

            /* TODO This entire thing is used to read the .csproj of a single component, going through all of the chosen components
             * and picking out their references, includes and all that, in order to create a gigantic Frankenstein csproj. This thing
             * is monstruous and hurts my eyes, as well as the pride of humanity.
             */
            foreach (DanuComponent comp in chosenOnes)
            {
                compReader = new XmlTextReader("./Source/" + comp.Name + "/" + comp.Name + ".csproj");

                while (compReader.Read())
                {
                    if (compReader.Name.Equals("Reference") && !referenceNames.Contains(compReader["Include"]))
                    {
                        if (compReader["Include"].Contains(".cs"))
                        {
                            referenceNames.Add(comp.Name + "\\" + compReader["Include"]);
                        }
                        else
                        {
                            referenceNames.Add(compReader["Include"]);
                        }
                    }
                    if (compReader.Name.Equals("Compile") && !compReader["Include"].Equals("Properties\\AssemblyInfo.cs"))
                    {
                        if (compReader["Include"].Contains(".cs"))
                        {
                            classNames.Add(comp.Name + "\\" + compReader["Include"]);
                        }
                        else
                        {
                            classNames.Add(compReader["Include"]);
                        }
                    }
                    if (compReader.Name.Equals("EmbeddedResource") && !compReader["Include"].Equals("Properties\\AssemblyInfo.cs"))
                    {
                        //TODO Why has this been tampered with? Must examine more carefully if Frankenstein.csproj continues to exist.
                        //if (compReader["Include"].Contains(".cs"))
                        //{
                        embeddedResource.Add(comp.Name + "\\" + compReader["Include"]);
                        //}
                        //else
                        //{
                        //    classNames.Add(compReader["Include"]);
                        //}
                    }
                }

                compReader.Close();
            }

            //adding extraFiles to theirs right place
            foreach (string ext in extraFiles)
            {
                if (!ext.Contains("AssemblyInfo.cs"))
                {
                    if (ext.EndsWith(".cs"))
                    {
                        if (classNames.FindIndex(x => x.Equals(ext, StringComparison.OrdinalIgnoreCase)) <= -1)
                        {
                            classNames.Add(ext);
                            System.Diagnostics.Debug.WriteLine("cs " + ext);
                        }
                    }
                    else //if (ext.EndsWith(".xml"))
                    {
                        if (embeddedResource.FindIndex(x => x.Equals(ext, StringComparison.OrdinalIgnoreCase)) <= -1)
                        {
                            embeddedResource.Add(ext);
                            System.Diagnostics.Debug.WriteLine("etc " + ext);
                        }
                    }
                }
            }
            #endregion

            // Beginning of Frankenstein.csproj writing.
            string filename = "PlugSPLCompileProject.csproj";
            /* TODO Check on whether this repository is really necessary. It gets filled in with everything that'll be used,
             * basically creating a Frankenstein folder for Frankenstein.csproj.
             */
            Directory.CreateDirectory("Repository");
            XmlWriter writer = XmlWriter.Create("./Repository/" + filename);

            #region Stable as of 3.0
            // This section need only be changed if ever we desire to change the versions with which to compile.
            writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
            writer.WriteAttributeString("ToolsVersion", "4.0");
            writer.WriteAttributeString("DefaultTargets", "Build");
            writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
            #endregion

            #region Hardcoded
            writer.WriteStartElement("PropertyGroup");

            writer.WriteStartElement("Configuration");
            writer.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
            writer.WriteValue("Debug");
            writer.WriteEndElement();

            writer.WriteStartElement("Platform");
            writer.WriteAttributeString("Condition", " '$(Platform)' == '' ");
            writer.WriteValue("AnyCPU");
            writer.WriteEndElement();
            #endregion

            //TODO Figure out what these are.
            writer.WriteElementString("ProductVersion", "8.0.30703");
            writer.WriteElementString("SchemaVersion", "2.0");

            // Creates a new GUID, since Frankie doesn't really have a formal structure, it's just a malformed beast.
            writer.WriteElementString("ProjectGuid", Guid.NewGuid().ToString());

            /* TODO IMPORTANT!!! This here be the line that decides the OutputType of the FINAL PRODUCT. It MUST be modified in order
             * to work with ANYTHING other than a Console Application.
             */
            writer.WriteElementString("OutputType", "Exe");

            #region Hardcoded
            writer.WriteElementString("AppDesignerFolder", "Properties");
            writer.WriteElementString("RootNamespace", pool.Root.Specification.Classes.First().Name);
            writer.WriteElementString("AssemblyName", pool.Root.Specification.Classes.First().Name);
            #endregion

            //TODO Figure out what these are.
            writer.WriteElementString("TargetFrameworkVersion", "v4.0");
            writer.WriteElementString("FileAlignment", "512");

            writer.WriteEndElement();

            #region Compile Parameters
            /* TODO Turn these into variables so that the output path can be picked, the constants can be defined, the compile parameters
             * can be defined, etc. Very important here, being that this is currently our final output, our "poster boy".
             */
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
            #endregion

            #region References
            // Frankie's refs being written.
            //TODO Gotta double-check this for consistency and duplicity.
            writer.WriteStartElement("ItemGroup");
            foreach (string reference in referenceNames)
            {
                writer.WriteStartElement("Reference");
                writer.WriteAttributeString("Include", reference);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            #endregion

            #region Project Classes?
            /* All of the classes in every project chosen, and possibly a few more if I did something wrong in the above code.
             * Which is likely.
             */
            writer.WriteStartElement("ItemGroup");
            foreach (string className in classNames)
            {
                writer.WriteStartElement("Compile");
                writer.WriteAttributeString("Include", className);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            #endregion

            #region Resources?
            //TODO Figure this whole region out. I have fairly little idea what this is, given that we haven't really worked with it.
            writer.WriteStartElement("ItemGroup");
            foreach (string resource in embeddedResource)
            {
                writer.WriteStartElement("EmbeddedResource");
                writer.WriteAttributeString("Include", resource);
                writer.WriteEndElement();
            }
            //TODO So much tampering with my code without explicit justification. Gonna start whipping people 'round here.
            /*  <ItemGroup>
                <EmbeddedResource Include="Templates\globals.h" />
                <EmbeddedResource Include="Templates\vuser_end.c" />
                <EmbeddedResource Include="Templates\vuser_init.c" />
              </ItemGroup>
              <ItemGroup>
                <EmbeddedResource Include="Templates\default.cfg" />
                <EmbeddedResource Include="Templates\default.usp" />
                <EmbeddedResource Include="Templates\scenarioTemplate.lrs" />
                <EmbeddedResource Include="Templates\script.usr" />
              </ItemGroup>*/
            writer.WriteEndElement();
            #endregion

            //TODO Figure this out.
            writer.WriteStartElement("Import");
            writer.WriteAttributeString("Project", "$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
            writer.WriteEndElement();

            writer.Close();

            return chosenOnes;
        }

        /// <summary>
        /// Replaces the string for the declaration of an interface.
        /// CURRENT: MUST be modified in order for optionals to work.
        /// </summary>
        /// <param name="parentClassName">Name of the class that'll be read and re-written.</param>
        /// <param name="socketClassName">Name of the class being used in this product to implement the interface.</param>
        /// <param name="interfaceName">Name of the interface to be implemented</param>
        /// <returns>The entire text of the parentClass, which will have to be re-written. Amateurism abounds.</returns>
        public static string ReplaceInterface(string parentClassName, string socketClassName, string interfaceName)
        {
            #region OBSOLETE
            //string currentExecutingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            //FileInfo f = new FileInfo(currentExecutingAssemblyLocation);
            //System.Environment.CurrentDirectory = f.Directory.FullName;
            #endregion

            // Reading the text of the class which will be substituted.
            TextReader reader = new StreamReader("./Repository/" + parentClassName + ".cs");
            string parentClassText = reader.ReadToEnd();

            //TODO This bit right here is currently hardcoded to not allow optionals. It has be changed. Likely to use ifdef.
            string subject = "public " + interfaceName + " instance" + interfaceName + " = null;";
            string newSubject = "public " + interfaceName + " instance" + interfaceName + " = new " + socketClassName + "." + socketClassName + "();";

            parentClassText = parentClassText.Replace(subject, newSubject);

            reader.Close();

            return parentClassText;
        }

        /// <summary>
        /// Generates the so-called "main class" of a component.
        /// CURRENT: Unsure whether this method is quite what we need.
        /// </summary>
        /// <param name="comp">Component to which the class belongs.</param>
        public static void GenerateMainClass(DanuComponent comp)
        {
            #region OBSOLETE
            //string currentExecutingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            //FileInfo f = new FileInfo(currentExecutingAssemblyLocation);
            //System.Environment.CurrentDirectory = f.Directory.FullName;
            #endregion

            string filename = comp.Name + ".cs";

            /* TODO Tamper tamper tamper... Is there any certainty this file exists beforehand? And if there is, why not check? 
             * Must examine.
             */
            //if(!File.Exists(filename))
            //    File.Create(filename).Close();

            // Initializing the class writer.
            TextWriter writer = new StreamWriter("./Source/" + comp.Name + "/" + filename);

            // "Using" section of the cs code.
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");

            /* Adds "using" clauses for any sockets this component uses. Theoretically, it will allow this class to implement the
             * interfaces of which this is a specialization.
             */
            //TODO Must check on the logic of this part, as well as its usefulness with optional.
            foreach (DanuSocket so in comp.Sockets)
            {
                DanuInterfaceObject io = so.InterfaceUsed;
                writer.WriteLine("using " + io.Eshu.Parent.Name + ";");
            }

            //Hardcode
            writer.WriteLine("");

            writer.WriteLine("namespace " + comp.Name);
            writer.WriteLine("{");
            writer.WriteLine("\t// This is the core class of the " + comp.Name + " component.");
            writer.Write("\tpublic class " + comp.Name);

            // Analyses the class for specialization requirements and adds implementations to the interfaces in question.
            /* TODO I'm still unsure as to whether it is possible for a class to implement more than one interface through diagram
             * manipulation. It seems illogical to me, as that would create a dependency we do not really foresee.
             */
            if (comp.Sockets.Count != 0)
            {
                writer.Write(" : ");
                foreach (DanuSocket so in comp.Sockets)
                {
                    if (!so.Equals(comp.Sockets.First()))
                        writer.Write(", ");
                    writer.Write(so.InterfaceUsed.Name);
                }
                writer.Write("\n");
            }

            writer.WriteLine("\t{");

            // Declaration of the interface instances for use within the code. This bit is tricksy with optionals.
            foreach (DanuInterfaceObject io in comp.Interfaces)
            {
                writer.WriteLine("\t\t//REPLACE");
                writer.WriteLine("\t\tpublic " + io.Name + " instance" + io.Name + " = " + "null;");
                writer.WriteLine("\t\t//REPLACE");
            }

            writer.WriteLine("\t\t");

            //TODO Should anything go in here? How will we check if the programmer did the ifdefs correctly?
            if (comp.Sockets.Count == 0)
            {
                writer.WriteLine("\t\tpublic static void Main(string[] args)");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\t");
                writer.WriteLine("\t\t}");
                writer.WriteLine("\t\t");
            }

            // Implementation of the methods, if this class implements interfaces.
            //TODO I wonder, if it has a return type, should we write some sort of auto-code to throw an exception or return null?
            foreach (DanuSocket so in comp.Sockets)
            {
                foreach (EshuMethod method in so.InterfaceUsed.Eshu.Signature)
                {
                    writer.Write("\t\t" + "public " + method.ReturnType + " " + method.Name + "(");
                    foreach (EshuProperty parameter in method.Parameters)
                    {
                        if (!parameter.Equals(method.Parameters.First()))
                            writer.Write(", ");
                        writer.Write(parameter.Type + " " + parameter.Name);
                    }
                    writer.Write(")\n");
                    writer.WriteLine("\t\t{");
                    writer.WriteLine("\t\t\t");
                    writer.WriteLine("\t\t}");
                    writer.WriteLine("\t\t");
                }
            }

            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Close();
        }

        /// <summary>
        /// Generates an interface's code.
        /// CURRENT: Working fairly well, surprisingly.
        /// </summary>
        /// <param name="io">Interface to be generated.</param>
        /// <param name="path">Where to write it.</param>
        /// <param name="parent">What component it belongs to.</param>
        public static void GenerateInterface(DanuInterfaceObject io, string path, DanuComponent parent)
        {
            // NOTE: This is probably the tidiest method in this class. Best not tamper much with it, lest we wake some dragons.

            #region OBSOLETE
            //string currentExecutingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            //FileInfo f = new FileInfo(currentExecutingAssemblyLocation);
            //System.Environment.CurrentDirectory = f.Directory.FullName;
            #endregion

            // Setting up the filewriter.
            string filename = io.Name + ".cs";
            //TODO Fishy tampering. I wonder how he's creating these files beforehand.
            //File.Create(filename).Close();
            TextWriter writer = new StreamWriter(path + filename);

            // Basic "using" clauses.
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");

            writer.WriteLine("");

            writer.WriteLine("namespace " + parent.Name);
            writer.WriteLine("{");
            writer.WriteLine("\tpublic interface " + io.Name);
            writer.WriteLine("\t{");

            // Declaration of the methods this interface signature defines.
            foreach (EshuMethod method in io.Eshu.Signature)
            {
                writer.Write("\t\t" + method.ReturnType + " " + method.Name + "(");
                foreach (EshuProperty parameter in method.Parameters)
                {
                    if (!parameter.Equals(method.Parameters.First()))
                        writer.Write(", ");
                    writer.Write(parameter.Type + " " + parameter.Name);
                }
                writer.Write(");\n");
            }

            writer.WriteLine("\t\t");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Close();
        }

        /// <summary>
        /// Generates the assemblyinfo file for a component.
        /// CURRENT: Pretty useless, not to mention broken.
        /// </summary>
        /// <param name="comp">Component in question.</param>
        public static void GenerateAssemblyInfo(DanuComponent comp)
        {
            #region OBSOLETE
            //string currentExecutingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            //FileInfo f = new FileInfo(currentExecutingAssemblyLocation);
            //System.Environment.CurrentDirectory = f.Directory.FullName;
            #endregion

            // Creates the file writer.
            string filename = "AssemblyInfo.cs";
            //TODO Tampering
            //File.Create(filename).Close();
            TextWriter writer = new StreamWriter("./Source/" + comp.Name + "/Properties/" + filename);

            //TODO Hm? What are these?
            writer.WriteLine("using System.Reflection;");
            writer.WriteLine("using System.Runtime.CompilerServices;");
            writer.WriteLine("using System.Runtime.InteropServices;");

            //TODO I wonder if we shouldn't auto-fill these somehow. Maybe get some information from the user's computer?
            writer.WriteLine("[assembly: AssemblyTitle(\"" + comp.Name + "\")]");
            writer.WriteLine("[assembly: AssemblyDescription(\"\")]");
            writer.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
            writer.WriteLine("[assembly: AssemblyCompany(\"\")]");
            writer.WriteLine("[assembly: AssemblyProduct(\"" + comp.Name + "\")]");
            writer.WriteLine("[assembly: AssemblyCopyright(\"\")]");
            writer.WriteLine("[assembly: AssemblyTrademark(\"\")]");
            writer.WriteLine("[assembly: AssemblyCulture(\"\")]");
            writer.WriteLine("[assembly: ComVisible(false)]");
            /* TODO My goodness, this is old code. Gotta take the GUID from the component's attribute, otherwise it'll be impossible
             * to ever compile these libraries on their own.
             */
            writer.WriteLine("[assembly: Guid(\"0fe9c50c-c8b9-4798-9ed4-a764e0615052\")]");
            writer.WriteLine("[assembly: AssemblyVersion(\"1.0.0.0\")]");
            writer.WriteLine("[assembly: AssemblyFileVersion(\"1.0.0.0\")]");

            writer.Close();
        }

        #region Secondary Methods
        public string GetName()
        {
            return "CSharp Code Generator for Microsoft Visual Studio 10";
        }

        public void GenerateCode(DanuProductConfigurator danu)
        {

            string projectLowerName = danu.Root.Name;


            string currentLocation = Assembly.GetExecutingAssembly().Location;

            foreach (DanuComponent comp in danu.Components)
            {
                Directory.CreateDirectory("./Source/" + comp.Name);
                GenerateMainClass(comp);
                GenerateProjectFile(comp);
                Directory.CreateDirectory("./Source/" + comp.Name + "/Properties");
                GenerateAssemblyInfo(comp);
                foreach (DanuInterfaceObject io in comp.Interfaces)
                {
                    GenerateInterface(io, "./Source/" + comp.Name + "/", comp);
                }
            }
        }
        #endregion
    }
}
