using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OgmaOATSParser
{
    internal class InputQueue : ICloneable
    {
        private List<string> queue;
        private int pointer;

        public int Pointer
        {
            get { return pointer; }
            set { pointer = value; }
        }

        public InputQueue(string input)
        {
            input = input.Replace(System.Environment.NewLine, " ");
            input = input.Replace("\t", " ");
            queue = input.Split(' ').ToList();
            queue.RemoveAll(item => item.Equals(""));
            pointer = 0;
            #if DEBUG
            PrintTokenList("TokenizationListFirstMoment.txt");
            #endif
        }

        private InputQueue(InputQueue oldQueue)
        {
            this.pointer = oldQueue.Pointer;
            this.queue = new List<string>();

            foreach (String value in oldQueue.queue)
            {
                this.queue.Add(value);
            }
        }

        internal void SplitItem(string border)
        {
            string itemToSplit = queue[pointer];
            itemToSplit = itemToSplit.Replace(border, border + "$" + border);

            List<string> newItems = itemToSplit.Split(border.ToCharArray()).ToList();
            newItems.RemoveAll(item => item.Equals(""));

            for (int i = 0; i < newItems.Count; i++)
            {
                if (newItems[i].Equals("$"))
                    newItems[i] = border;
            }

            queue.InsertRange(pointer + 1, newItems);
            queue.RemoveAt(pointer);
        }

        public string Pop()
        {
            try
            {
                string toReturn = queue[pointer];
                pointer++;
                return toReturn;
            }
            catch (Exception)
            {
                pointer++;
                return "";
            }
        }

        /// <summary>
        /// This method is internal to the functioning of the Lexical Analyser. Use with caution.
        /// </summary>
        internal void DecrementPointer()
        {
            pointer--;
        }

        public object Clone()
        {
            return new InputQueue(this);
        }

        #if DEBUG
        /// <summary>
        /// Method used for testing the class.
        /// </summary>
        internal void PrintTokenList(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (string item in queue)
                {
                    writer.WriteLine(item);
                }
            }
        }

        internal string Peek()
        {
            try
            {
                return queue[pointer];
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endif
    }
}
