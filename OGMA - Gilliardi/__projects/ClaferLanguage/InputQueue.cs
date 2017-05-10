using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClaferLanguage
{
    public class InputQueue
    {
        private string[] queue;
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
            queue = input.Split(' ');
            List<string> newQueue = new List<string>();

            foreach (string entry in queue)
            {
                if (!entry.Equals(""))
                {
                    newQueue.Add(entry);
                }
            }

            queue = newQueue.ToArray();

            pointer = 0;
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
        /// This method is internal to the functioning of the Clafer Lexical Analyser. Use with caution.
        /// </summary>
        public void DecrementPointer()
        {
            pointer--;
        }
    }
}