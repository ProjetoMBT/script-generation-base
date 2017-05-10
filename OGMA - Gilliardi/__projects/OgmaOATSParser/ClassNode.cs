using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OgmaOATSParser
{
    internal class ClassNode
    {
        private Classes type;

        public Classes Type
        {
            get { return type; }
            set { type = value; }
        }

        private List<ClassNode> derivations;

        public List<ClassNode> Derivations
        {
            get { return derivations; }
            set { derivations = value; }
        }

        public ClassNode(Classes type)
        {
            this.type = type;
            derivations = new List<ClassNode>();
        }

        #if DEBUG
        internal void PrintSyntaxTree(string path)
        {
            StreamWriter writer = new StreamWriter(path);
            PrintSyntaxTree(writer);
        }

        internal void PrintSyntaxTree(StreamWriter writer)
        {
            writer.WriteLine(Type);

            foreach (ClassNode child in Derivations)
            {
                child.PrintSyntaxTree(writer);
            }
        }

        internal void PrintSyntaxTree(string path, InputQueue queue)
        {
            StreamWriter writer = new StreamWriter(path);

            InputQueue newQueue = (InputQueue)queue.Clone();
            newQueue.Pointer = 0;

            PrintSyntaxTree(writer, newQueue, 0);

            writer.Close();
        }

        internal InputQueue PrintSyntaxTree(StreamWriter writer, InputQueue queue, int value)
        {
            string toAppend = "";

            for (int i = 0; i < value; i++)
            {
                toAppend += "\t";
            }

            if (Derivations.Count != 0 || Type.Equals(Classes.Epsilon))
                writer.WriteLine(toAppend + Type);
            else
                writer.WriteLine(toAppend + Type + ": " + queue.Pop());

            foreach (ClassNode child in Derivations)
            {
                queue = child.PrintSyntaxTree(writer, queue, value + 1);
            }

            return queue;
        }
        #endif
    }
}
