using System;
using System.Collections.Generic;

namespace Coc.Data.ReadXlsx
{
    public struct Tupla
    {
        public String valor1;
        public String valor2;

        public Tupla(String valor1, String valor2)
        {
            this.valor1 = valor1;
            this.valor2 = valor2;
        }

        public override bool Equals(Object obj)
        {
            Tupla innerTupla = (Tupla)obj;
            if (innerTupla.valor1.Equals(this.valor1) && innerTupla.valor2.Equals(this.valor2))
                return true;
            else
                return false;
        }
    }

    public class KeywordDictionary
    {
        private Dictionary<Tupla, String> keywords;

        public KeywordDictionary()
        {
            keywords = new Dictionary<Tupla, String>();
        }

        public void AddKeyword(String tdobject, String tdaction, String keyword)
        {
            if (!keywords.ContainsKey(new Tupla(tdobject, tdaction)))
            {
                keywords.Add(new Tupla(tdobject, tdaction), keyword);
            }
            else if (tdobject.Equals("All Objects") && (tdaction.Equals("exists") || tdaction.Equals("existstimeout")))
            {
                String aux = GetKeyword(tdobject, tdaction);
                if (!aux.Equals("check"))
                {
                    keywords[new Tupla(tdobject, tdaction)] = "check";
                }
            }
        }

        public String GetKeyword(String tdobject, String tdaction)
        {
            if (!keywords.ContainsKey(new Tupla(tdobject, tdaction)))
            {
                if (keywords.ContainsKey(new Tupla("All Objects", tdaction)))
                {
                    return keywords[new Tupla("All Objects", tdaction)];
                }
                else
                {
                    return null;
                }
            }
            return keywords[new Tupla(tdobject, tdaction)];
        }
    }
}