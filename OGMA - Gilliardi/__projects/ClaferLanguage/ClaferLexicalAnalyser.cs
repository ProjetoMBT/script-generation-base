using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ClaferLanguage
{
    public class ClaferLexicalAnalyser
    {
        private List<string> reservedWords;
        private InputQueue inputQueue;

        public ClaferLexicalAnalyser()
        {
            reservedWords = new List<string>();

            reservedWords.Add("abstract");
            reservedWords.Add("all");
            reservedWords.Add("disj");
            reservedWords.Add("else");
            reservedWords.Add("enum");
            reservedWords.Add("extends");
            reservedWords.Add("in");
            reservedWords.Add("lone");
            reservedWords.Add("mux");
            reservedWords.Add("no");
            reservedWords.Add("not");
            reservedWords.Add("one");
            reservedWords.Add("opt");
            reservedWords.Add("or");
            reservedWords.Add("some");
            reservedWords.Add("xor");
        }
        public ClaferClassNode BuildLexicon(InputQueue inputQueue)
        {
            this.inputQueue = inputQueue;
            return CheckModule(inputQueue.Pop());
        }

        private ClaferClassNode CheckAbstract(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Abstract);

            if (input.Equals("abstract")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));
            else
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                inputQueue.DecrementPointer();
            }
            return toReturn;
        }
        private ClaferClassNode CheckInteger(string input)
        {
            Regex inte = new Regex(@"^\d$");

            if (inte.IsMatch(input)) return new ClaferClassNode(ClaferClasses.Int);
            throw new FormatException("Input " + input + " is not an Integer");
        }
        private ClaferClassNode CheckExInteger(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ExInteger);

            if (input.Equals("*")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            else
            {
                try
                {
                    toReturn.Derivations.Add(CheckInteger(input));
                }
                catch (FormatException)
                {
                    throw new FormatException("Input " + input + " is not of ExInteger type");
                }
            }
            return toReturn;
        }
        private ClaferClassNode CheckIff(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Iff);

            if (input.Equals("<=>")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            else throw new FormatException("Input " + input + " is not of Iff type");
            return toReturn;
        }
        private ClaferClassNode CheckImplies(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Implies);

            if (input.Equals("=>")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            else throw new FormatException("Input " + input + " is not of Iff type");
            return toReturn;
        }
        private ClaferClassNode CheckAnd(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.And);

            if (input.Equals("&&")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            else throw new FormatException("Input " + input + " is not of And type");
            return toReturn;
        }
        private ClaferClassNode CheckXor(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Xor);

            if (input.Equals("xor")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));
            else throw new FormatException("Input " + input + " is not of Xor type");
            return toReturn;
        }
        private ClaferClassNode CheckOr(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Or);

            if (input.Equals("||")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            else throw new FormatException("Input " + input + " is not of Or type");
            return toReturn;
        }
        private ClaferClassNode CheckNeg(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Neg);

            if (input.Equals("~")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            else throw new FormatException("Input " + input + " is not of Neg type");
            return toReturn;
        }
        private ClaferClassNode CheckQuant(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Quant);

            if (input.Equals("no") || input.Equals("lone") || input.Equals("one") || input.Equals("some"))
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));
            else throw new FormatException("Input " + input + " is not of Quant type");
            return toReturn;
        }
        private ClaferClassNode CheckExQuant(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ExQuant);

            if (input.Equals("all")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));
            else
            {
                try
                {
                    toReturn.Derivations.Add(CheckQuant(input));
                }
                catch (FormatException)
                {
                    throw new FormatException("Input " + input + " is not of ExQuant type");
                }
            }
            return toReturn;
        }
        private ClaferClassNode CheckDisj(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Disj);

            if (input.Equals("disj")) toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));
            else
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                inputQueue.DecrementPointer();
            }
            return toReturn;
        }
        private ClaferClassNode CheckIdent(string input)
        {
            Regex ident = new Regex(@"^[A-Z](\w|')*$");

            if (!reservedWords.Contains(input) && ident.IsMatch(input)) return new ClaferClassNode(ClaferClasses.Ident);
            throw new FormatException("Input " + input + " is not of Ident type");
        }
        private ClaferClassNode CheckEnumId(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.EnumId);

            try
            {
                toReturn.Derivations.Add(CheckIdent(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of EnumID type");
            }

            return toReturn;
        }
        private ClaferClassNode CheckModId(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ModId);

            try
            {
                toReturn.Derivations.Add(CheckIdent(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of ModId type");
            }

            return toReturn;
        }
        private ClaferClassNode CheckLocId(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LocId);

            try
            {
                toReturn.Derivations.Add(CheckIdent(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of LocId type");
            }

            return toReturn;
        }
        private ClaferClassNode CheckListEnumId(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ListEnumId);

            try
            {
                toReturn.Derivations.Add(CheckEnumId(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of ListEnumId type");
            }

            if (inputQueue.Pop().Equals("|"))
            {
                try
                {
                    CheckEnumId(inputQueue.Pop());
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    return toReturn;
                }

                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                inputQueue.DecrementPointer();
                toReturn.Derivations.Add(CheckListEnumId(inputQueue.Pop()));
            }

            return toReturn;
        }
        private ClaferClassNode CheckListLocId(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ListLocId);

            try
            {
                toReturn.Derivations.Add(CheckLocId(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of ListLocId type");
            }

            if (inputQueue.Pop().Equals(","))
            {
                try
                {
                    CheckLocId(inputQueue.Pop());
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    return toReturn;
                }
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                inputQueue.DecrementPointer();
                toReturn.Derivations.Add(CheckListLocId(inputQueue.Pop()));
            }

            return toReturn;
        }
        private ClaferClassNode CheckListModId(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ListModId);

            string input2 = inputQueue.Pop();
            string input3 = inputQueue.Pop();

            if (input2.Equals("/"))
            {
                try
                {
                    toReturn.Derivations.Add(CheckModId(input));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + input3 + " is not of ListModId type");
                }
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                try
                {
                    toReturn.Derivations.Add(CheckListModId(input3));
                }
                catch (FormatException)
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                }
            }
            else
            {
                inputQueue.DecrementPointer();
                inputQueue.DecrementPointer();
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
            }
            return toReturn;
        }
        private ClaferClassNode CheckNCard(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.NCard);

            string input2 = inputQueue.Pop();
            string input3 = inputQueue.Pop();

            if (input2.Equals(".."))
            {
                try
                {
                    toReturn.Derivations.Add(CheckInteger(input));
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + input3 + " is not of NCard type");
                }
                try
                {
                    toReturn.Derivations.Add(CheckExInteger(input3));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + input3 + " is not of NCard type");
                }
                return toReturn;
            }

            inputQueue.DecrementPointer();
            inputQueue.DecrementPointer();
            throw new FormatException("Input " + input + input2 + input3 + " is not of NCard type");
        }
        private ClaferClassNode CheckGNCard(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.GNCard);

            string input2 = inputQueue.Pop();
            string input3 = inputQueue.Pop();

            if (input2.Equals("-"))
            {
                try
                {
                    toReturn.Derivations.Add(CheckInteger(input));
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + input3 + " is not of GNCard type");
                }
                try
                {
                    toReturn.Derivations.Add(CheckExInteger(input3));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + input3 + " is not of GNCard type");
                }
                return toReturn;
            }

            inputQueue.DecrementPointer();
            inputQueue.DecrementPointer();
            inputQueue.DecrementPointer();
            throw new FormatException("Input " + input + input2 + input3 + " is not of GNCard type");
        }
        private ClaferClassNode CheckCard(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Card);

            try
            {
                toReturn.Derivations.Add(CheckNCard(input));
            }
            catch (FormatException)
            {
                if (input.Equals("?") || input.Equals("+") || input.Equals("*")) toReturn.Derivations.Add(new 
                    ClaferClassNode(ClaferClasses.Symbol));
                else
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                    inputQueue.DecrementPointer();
                }
            }
            return toReturn;
        }
        private ClaferClassNode CheckName(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Name);

            try
            {
                toReturn.Derivations.Add(CheckListModId(input));
                toReturn.Derivations.Add(CheckIdent(inputQueue.Pop()));
            }
            catch (FormatException)
            {
                inputQueue.DecrementPointer();
                throw new FormatException("Input " + input + " and the following terms are not of type Name");
            }

            return toReturn;
        }
        private ClaferClassNode CheckGCard(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.GCard);

            if (input.Equals("xor") || input.Equals("or") || input.Equals("mux") || input.Equals("opt"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));
            }
            else
            {
                if (input.Equals("<"))
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    try
                    {
                        toReturn.Derivations.Add(CheckGNCard(inputQueue.Pop()));
                    }
                    catch (FormatException)
                    {
                        inputQueue.DecrementPointer();
                        throw new FormatException("Input " + input + " and following terms are not of GCard type");
                    }
                    if (inputQueue.Pop().Equals(">"))
                    {
                        toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    }
                    else
                    {
                        throw new FormatException("Input " + input + " and following terms are not of GCard type");
                    }
                }
                else
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                    inputQueue.DecrementPointer();
                }
            }
            return toReturn;
        }
        private ClaferClassNode CheckString(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].Equals('\"') && !input[i - 1].Equals('\\'))
                {
                    throw new FormatException("Input " + input + " is not of String type. " +
                        "No, seriously, it actually found a \" that was not preceded by a \\. I know, right?");
                }
            }
            return new ClaferClassNode(ClaferClasses.String);
        }
        private ClaferClassNode CheckStrExp(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.StrExp);

            try
            {
                toReturn.Derivations.Add(CheckString(input));
            }
            catch(FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of StrExp type");
            }

            string input2 = inputQueue.Pop();
            string input3 = inputQueue.Pop();

            if (input2.Equals("++"))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckStrExp(input3));
                }
                catch (FormatException)
                {
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckSExp(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.SExp);

            try
            {
                toReturn.Derivations.Add(CheckSExp1(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type SExp");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals("++"))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckSExp(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                }
            }
            else
            {
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckSExp1(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.SExp1);

            try
            {
                toReturn.Derivations.Add(CheckSExp2(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type SExp1");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals("&"))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckSExp1(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                }
            }
            else
            {
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckSExp2(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.SExp2);

            try
            {
                toReturn.Derivations.Add(CheckSExp3(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type SExp2");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals("<:"))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckSExp2(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                }
            }
            else
            {
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckSExp3(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.SExp3);

            try
            {
                toReturn.Derivations.Add(CheckSExp4(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type SExp3");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals(":>"))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckSExp3(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                }
            }
            else
            {
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckSExp4(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.SExp4);

            try
            {
                toReturn.Derivations.Add(CheckSExp5(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type SExp4");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals("."))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckSExp4(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                }
            }
            else
            {
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckSExp5(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.SExp5);

            if (input.Equals("("))
            {
                string input2 = inputQueue.Pop();
                
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckSExp(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + " and following terms are not of type SExp5");
                }

                string input3 = inputQueue.Pop();

                if (input3.Equals(")"))
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                }
                else
                {
                    throw new FormatException("Input " + input + input2 + " and following terms are not of type SExp5");
                }
            }

            else
            {
                try
                {
                    toReturn.Derivations.Add(CheckIdent(input));
                }
                catch (FormatException)
                {
                    throw new FormatException("Input " + input + " and following terms are not of type SExp5");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckDecl(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Decl);

            try
            {
                toReturn.Derivations.Add(CheckExQuant(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type Decl");
            }

            toReturn.Derivations.Add(CheckDisj(inputQueue.Pop()));

            string input2 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckListLocId(input2));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " up to " + input2 + 
                    " and following terms are not of type Decl");
            }

            if (inputQueue.Pop().Equals(":"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            }
            else
            {
                throw new FormatException("Input " + input + " and following terms are not of type Decl. Missing : symbol");
            }

            string input3 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckSExp(input3));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " up to " + input3 +
                    " and following terms are not of type Decl");
            }

            return toReturn;
        }
        private ClaferClassNode CheckAExp(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.AExp);

            try
            {
                toReturn.Derivations.Add(CheckAExp1(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type AExp");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals("-") || input2.Equals("+"))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckAExp(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                }
            }
            else
            {
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckAExp1(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.AExp1);

            try
            {
                toReturn.Derivations.Add(CheckAExp2(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of type AExp1");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals("*"))
            {
                try
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    toReturn.Derivations.Add(CheckAExp1(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                }
            }
            else
            {
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckAExp2(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.AExp2);

            if (input.Equals("#"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));

                string input2 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckSExp(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + " and following terms are not of AExp2 type");
                }
            }

            else if (input.Equals("("))
            {
                string input2 = inputQueue.Pop();
                
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));

                try
                {
                    toReturn.Derivations.Add(CheckAExp(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + " and following terms are not of AExp2 type");
                }

                if (inputQueue.Pop().Equals(")"))
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                }
                else
                {
                    throw new FormatException("Input " + input + input2 + " and following terms are not of AExp2 type");
                }
            }

            else
            {
                try
                {
                    toReturn.Derivations.Add(CheckInteger(input));
                }
                catch (FormatException)
                {
                    try
                    {
                        toReturn.Derivations.Add(CheckSExp(input));
                    }
                    catch (FormatException)
                    {
                        throw new FormatException("Input " + input + " is not of AExp2 type");
                    }
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckListDecl(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ListDecl);

            try
            {
                toReturn.Derivations.Add(CheckDecl(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of ListDecl type");
            }

            if (inputQueue.Pop().Equals(","))
            {
                string input2 = inputQueue.Pop();

                try
                {
                    CheckDecl(input2);
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    return toReturn;
                }

                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                toReturn.Derivations.Add(CheckListDecl(input2));
            }

            return toReturn;
        }
        private ClaferClassNode CheckSuper(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Super);

            string input2 = inputQueue.Pop();

            if (input.Equals(":"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));

                try
                {
                    toReturn.Derivations.Add(CheckName(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                }
            }

            else if (input.Equals("extends"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));

                try
                {
                    toReturn.Derivations.Add(CheckName(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    inputQueue.DecrementPointer();
                    toReturn.Derivations.RemoveAt(toReturn.Derivations.Count - 1);
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                }
            }

            else if (input.Equals("->"))
            {
                int pointerLocation = inputQueue.Pointer;

                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));

                try
                {
                    toReturn.Derivations.Add(CheckListModId(input2));
                    string input3 = inputQueue.Pop();
                    toReturn.Derivations.Add(CheckSExp(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointerLocation;
                    toReturn.Derivations.Clear();
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                }
            }

            else
            {
                inputQueue.DecrementPointer();
                inputQueue.DecrementPointer();
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
            }

            return toReturn;
        }
        private ClaferClassNode CheckExp(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Exp);

            try
            {
                toReturn.Derivations.Add(CheckAExp(input));
            }
            catch (FormatException)
            {
                try
                {
                    toReturn.Derivations.Add(CheckStrExp(input));
                }
                catch (FormatException)
                {
                    throw new FormatException("Input " + input + " is not of Exp type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckCmpExp(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.CmpExp);

            int pointer = inputQueue.Pointer;

            try
            {
                toReturn.Derivations.Add(CheckExp(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of CmpExp type");
            }

            string input2 = inputQueue.Pop();

            if (input2.Equals("<") || input2.Equals(">") || input2.Equals("=") || input2.Equals("==") ||
                input2.Equals("<=") || input2.Equals(">=") || input2.Equals("!=") || input2.Equals("/="))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
            }

            else if (input2.Equals("in") || input2.Equals("notin"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));
            }

            else
            {
                inputQueue.Pointer = pointer;
                throw new FormatException("Input " + input2 + " is not a valid symbol for CmpExp type");
            }

            string input3 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckExp(input3));
            }
            catch (FormatException)
            {
                inputQueue.Pointer = pointer;
                throw new FormatException("Input " + input3 + " did not suit the CmpExp type");
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp);

            int pointer = inputQueue.Pointer;
            
            try
            {
                toReturn.Derivations.Add(CheckLExp1(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of LExp type");
            }

            string input2 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckIff(input2));
            }
            catch (FormatException)
            {
                inputQueue.DecrementPointer();
            }

            if (toReturn.Derivations[toReturn.Derivations.Count - 1].Type.Equals(ClaferClasses.Iff))
            {
                string input3 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + input3 + " is not of LExp type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp1(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp1);

            int pointer = inputQueue.Pointer;
            
            try
            {
                toReturn.Derivations.Add(CheckLExp2(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of LExp1 type");
            }

            string input2 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckImplies(input2));
            }
            catch (FormatException)
            {
                inputQueue.DecrementPointer();
            }

            if (toReturn.Derivations[toReturn.Derivations.Count - 1].Type.Equals(ClaferClasses.Implies))
            {
                string input3 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp1Extension(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + input3 + " is not of LExp1 type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp1Extension(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp1);

            int pointer = inputQueue.Pointer;
            
            try
            {
                toReturn.Derivations.Add(CheckLExp2(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of LExp1 type");
            }

            string input2 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckImplies(input2));
            }
            catch (FormatException)
            {
                inputQueue.DecrementPointer();
            }

            if (toReturn.Derivations[toReturn.Derivations.Count - 1].Type.Equals(ClaferClasses.Implies))
            {
                string input3 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp1Extension(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + input3 + " is not of LExp1 type");
                }
            }

            if (inputQueue.Pop().Equals("else"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));

                string input3 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp1(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + input3 + " is not of LExp1 type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp2(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp2);

            int pointer = inputQueue.Pointer;

            try
            {
                toReturn.Derivations.Add(CheckLExp3(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of LExp2 type");
            }

            string input2 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckOr(input2));
            }
            catch (FormatException)
            {
                inputQueue.DecrementPointer();
            }

            if (toReturn.Derivations[toReturn.Derivations.Count - 1].Type.Equals(ClaferClasses.Or))
            {
                string input3 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp2(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + input3 + " is not of LExp2 type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp3(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp3);

            int pointer = inputQueue.Pointer;

            try
            {
                toReturn.Derivations.Add(CheckLExp4(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of LExp3 type");
            }

            string input2 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckXor(input2));
            }
            catch (FormatException)
            {
                inputQueue.DecrementPointer();
            }

            if (toReturn.Derivations[toReturn.Derivations.Count - 1].Type.Equals(ClaferClasses.Xor))
            {
                string input3 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp3(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + input3 + " is not of LExp3 type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp4(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp4);

            int pointer = inputQueue.Pointer;

            try
            {
                toReturn.Derivations.Add(CheckLExp5(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " is not of LExp4 type");
            }

            string input2 = inputQueue.Pop();

            try
            {
                toReturn.Derivations.Add(CheckAnd(input2));
            }
            catch (FormatException)
            {
                inputQueue.DecrementPointer();
            }

            if (toReturn.Derivations[toReturn.Derivations.Count - 1].Type.Equals(ClaferClasses.And))
            {
                string input3 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp4(input3));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + input3 + " is not of LExp4 type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp5(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp5);

            bool neg = false;

            try
            {
                toReturn.Derivations.Add(CheckNeg(input));
                neg = true;
            }
            catch (FormatException)
            {
                try
                {
                    toReturn.Derivations.Add(CheckLExp6(input));
                }
                catch (FormatException)
                {
                    throw new FormatException("Input " + input + " is not of LExp5 type");
                }
            }

            if (neg)
            {
                string input2 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckLExp6(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + " is not of LExp5 type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckLExp6(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.LExp6);

            int pointer = inputQueue.Pointer;

            try
            {
                toReturn.Derivations.Add(CheckTerm(input));
            }
            catch (FormatException)
            {
                if (input.Equals("("))
                {
                    string input2 = inputQueue.Pop();
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));

                    try
                    {
                        toReturn.Derivations.Add(CheckLExp(input2));
                    }
                    catch (FormatException)
                    {
                        inputQueue.DecrementPointer();
                        throw new FormatException("Input " + input + input2 + " is not of LExp6 type");
                    }
                    if (inputQueue.Pop().Equals(")"))
                    {
                        toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                    }
                    else
                    {
                        inputQueue.Pointer = pointer;
                        throw new FormatException("Input " + input + input2 + " is not followed by a valid LExp6 type symbol");
                    }
                }
                else
                {
                    throw new FormatException("Input " + input + " is not of LExp6 type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckTerm(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Term);

            bool done = false;
            int pointer = inputQueue.Pointer;

            //CmpExp part
            try
            {
                toReturn.Derivations.Add(CheckCmpExp(input));
                done = true;
            }
            catch (FormatException) { }

            //Quant SExp part
            if (!done)
            {
                try
                {
                    toReturn.Derivations.Add(CheckQuant(input));
                    done = true;
                }
                catch (FormatException) { }
                if (done)
                {
                    try
                    {
                        toReturn.Derivations.Add(CheckSExp(inputQueue.Pop()));
                    }
                    catch (FormatException)
                    {
                        inputQueue.DecrementPointer();
                        throw new FormatException("Input " + input + " and following terms are not of Term type");
                    }
                }
            }

            //ListDecl | LExp part
            if (!done)
            {
                try
                {
                    toReturn.Derivations.Add(CheckListDecl(input));
                    done = true;
                }
                catch (FormatException) { }

                if (done)
                {
                    if (inputQueue.Pop().Equals("|"))
                    {
                        toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));

                        try
                        {
                            toReturn.Derivations.Add(CheckLExp(inputQueue.Pop()));
                        }
                        catch (FormatException)
                        {
                            inputQueue.Pointer = pointer;
                            throw new FormatException("Input " + input + " and following terms are not of Term type");
                        }
                    }
                    else
                    {
                        inputQueue.Pointer = pointer;
                        throw new FormatException("Input " + input + " and following terms are not of Term type");
                    }
                }
            }

            //SExp part
            if (!done)
            {
                try
                {
                    toReturn.Derivations.Add(CheckSExp(input));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + " and following terms are not of Term type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckListLExp(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ListLExp);

            int pointer = inputQueue.Pointer;
            bool done = false;

            try
            {
                toReturn.Derivations.Add(CheckLExp(input));
                done = true;
            }
            catch (FormatException) 
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                inputQueue.DecrementPointer();
            }

            if (done)
            {
                try
                {
                    toReturn.Derivations.Add(CheckListLExp(inputQueue.Pop()));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + " and following terms are not of ListLExp type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckConstraint(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Constraint);

            int pointer = inputQueue.Pointer;

            if (input.Equals("["))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                string input2 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckListLExp(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + " is not of Constraint type");
                }

                if (inputQueue.Pop().Equals("]"))
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                }
                else
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + " and following terms are not of Constraint type");
                }
            }
            else
            {
                throw new FormatException("Input " + input + " is not of Constraint type");
            }

            return toReturn;
        }
        private ClaferClassNode CheckListElement(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ListElement);

            int pointer = inputQueue.Pointer;
            bool done = false;

            try
            {
                toReturn.Derivations.Add(CheckElement(input));
                done = true;
            }
            catch (FormatException)
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                inputQueue.DecrementPointer();
            }

            if (done)
            {
                try
                {
                    toReturn.Derivations.Add(CheckListElement(inputQueue.Pop()));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + " and following terms are not of ListElement type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckElement(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Element);

            int pointer = inputQueue.Pointer;
            
            // ' Name Card Elements part
            if (input.Equals("'"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                string input2 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckName(input2));
                    toReturn.Derivations.Add(CheckCard(inputQueue.Pop()));
                    toReturn.Derivations.Add(CheckElements(inputQueue.Pop()));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + input2 + " and following terms are not of Element type");
                }
            }

            else
            {
                try
                {
                    toReturn.Derivations.Add(CheckConstraint(input));
                }
                catch (FormatException)
                {
                    try
                    {
                        toReturn.Derivations.Add(CheckClafer(input));
                    }
                    catch (FormatException)
                    {
                        throw new FormatException("Input " + input + " and following terms are not of Element type");
                    }
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckListDeclaration(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.ListDeclaration);

            int pointer = inputQueue.Pointer;
            bool done = false;

            try
            {
                toReturn.Derivations.Add(CheckDeclaration(input));
                done = true;
            }
            catch (FormatException)
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                inputQueue.DecrementPointer();
            }

            if (done)
            {
                try
                {
                    toReturn.Derivations.Add(CheckListDeclaration(inputQueue.Pop()));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + " and following terms are not of ListDeclaration type");
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckDeclaration(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Declaration);

            int pointer = inputQueue.Pointer;

            if (input.Equals("enum"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.ReservedWord));

                try
                {
                    toReturn.Derivations.Add(CheckIdent(inputQueue.Pop()));
                }
                catch (FormatException)
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + " and following terms are not of Declaration type");
                }

                if (inputQueue.Pop().Equals("="))
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));

                    try
                    {
                        toReturn.Derivations.Add(CheckListEnumId(inputQueue.Pop()));
                    }
                    catch (FormatException)
                    {
                        inputQueue.Pointer = pointer;
                        throw new FormatException("Input " + input + " and following terms are not of Declaration type");
                    }
                }
            }

            else
            {
                try
                {
                    toReturn.Derivations.Add(CheckConstraint(input));
                }
                catch (FormatException)
                {
                    try
                    {
                        toReturn.Derivations.Add(CheckClafer(input));
                    }
                    catch (FormatException)
                    {
                        throw new FormatException("Input " + input + " and following terms are not of Declaration type");
                    }
                }
            }

            return toReturn;
        }
        private ClaferClassNode CheckElements(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Elements);

            int pointer = inputQueue.Pointer;

            if (input.Equals("{"))
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                string input2 = inputQueue.Pop();

                try
                {
                    toReturn.Derivations.Add(CheckListElement(input2));
                }
                catch (FormatException)
                {
                    inputQueue.DecrementPointer();
                    throw new FormatException("Input " + input + input2 + " is not of Elements type");
                }

                if (inputQueue.Pop().Equals("}"))
                {
                    toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Symbol));
                }
                else
                {
                    inputQueue.Pointer = pointer;
                    throw new FormatException("Input " + input + " and following terms are not of Elements type");
                }
            }
            else
            {
                toReturn.Derivations.Add(new ClaferClassNode(ClaferClasses.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }
        private ClaferClassNode CheckClafer(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Clafer);

            int pointer = inputQueue.Pointer;

            try
            {
                toReturn.Derivations.Add(CheckAbstract(input));
                toReturn.Derivations.Add(CheckGCard(inputQueue.Pop()));
                toReturn.Derivations.Add(CheckIdent(inputQueue.Pop()));
                toReturn.Derivations.Add(CheckSuper(inputQueue.Pop()));
                toReturn.Derivations.Add(CheckCard(inputQueue.Pop()));
                toReturn.Derivations.Add(CheckElements(inputQueue.Pop()));
            }
            catch (FormatException)
            {
                inputQueue.Pointer = pointer;
                throw new FormatException("Input " + input + " and following terms are not of Clafer type");
            }

            return toReturn;
        }
        private ClaferClassNode CheckModule(string input)
        {
            ClaferClassNode toReturn = new ClaferClassNode(ClaferClasses.Module);

            try
            {
                toReturn.Derivations.Add(CheckListDeclaration(input));
            }
            catch (FormatException)
            {
                throw new FormatException("Input " + input + " and following terms are not of Module type");
            }

            return toReturn;
        }
    }
}