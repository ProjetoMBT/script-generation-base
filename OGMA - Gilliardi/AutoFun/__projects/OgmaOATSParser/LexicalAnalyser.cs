using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace OgmaOATSParser
{
    /*
     * As explained in the Ogma class header, LexicalAnalyser is responsible for both the Lexical and 
     * Syntactic analysis. It is recommended that a certain order be followed in reading this class,
     * as certain concepts are only introduced once, in the first rule where they appear. The order is
     * as follows:
     * 
     * 1º: OATS
     *      Explanation of several basic concepts.
     * 2º: LIST_IMPORT
     *      Explanation of how to deal with Epsilon tokens.
     * 3º: IMPORT
     *      Explanation of how to deal with composite tokens (tokens that are not divided by white
     *      space, e.g. System.Text is a composite token made up of "System", "." and "Text". Also
     *      has an explanation of how to deal with reserved words. Also has an explanation of how
     *      to represent optional symbols.
     * 4º: IDENTIFIER
     *      Explanation of how to deal with Terminal Symbols. Also has a basic explanation of some 
     *      Regular Expression mechanisms in C#.
     *      
     * 5º: IDENTIFIER3
     *      Explanation of how to deal with Symbols with multiple derivation options.
     *      
     * 6º: CLASS_DECLARATION
     *      Explanation of when and how to use Regular Expressions in Non-Terminal Symbols.
     *      
     * 7º: ANY2
     *      This is a particularly unique rule that merits reading.
     *      
     * 8º: SCRIPT_ELEMENT
     *      This rule is implemented in a slightly different way from the rest of this class, and should
     *      be of some interest.
     */
    internal class LexicalAnalyser
    {
        private List<string> reservedWords;
        private InputQueue inputQueue;

        internal LexicalAnalyser()
        {
            /*
             * The "reserved words" represent all Terminal constant tokens.
             */
            reservedWords = new List<string>();

            reservedWords.Add("import");
        }

        /*
         * This is the starting point of the Lexicon construction process, making a call to the OATS 
         * function, representing the first rule of the CFG.
         */
        internal Tuple<ClassNode, InputQueue> BuildLexicon(InputQueue inputQueue)
        {
            this.inputQueue = inputQueue;
            return new Tuple<ClassNode, InputQueue>(OATS(this.inputQueue.Pop()), this.inputQueue);
        }

        /*
         * OATS is the first rule of the CFG. Given any Oracle ATS script, the syntactic structure
         * can always be reduced to a single OATS token that represents the entire script.
         * 
         * We apologize for this particular function seeming so "polluted" with comments, but we
         * hope that by explaining as many implementation concepts as possible here, we will be able
         * keep the rest of the class cleaner.
         * 
         * OATS -> LIST_IMPORT CLASS
         */
        private ClassNode OATS(string input)
        {
#if DEBUG
            DateTime startTime = DateTime.Now;
#endif

            /*
             * At this point in any function of the Lexical Analyser, a ClassNode object is created
             * with its type assigned as the equivalent type of this function. There is a reason this
             * is done at the beginning of the function, rather than after the rule has been validated.
             * 
             * Should a specific rule fail in its validation, it will invariably throw an exception,
             * which the previous function call in the recursion stack will be prepared to deal with.
             * Either it will attempt validating another rule, in the case of more than one option of 
             * derivation being available, or it will forward the exception further down the recursion 
             * stack.
             */
            ClassNode toReturn = new ClassNode(Classes.Oats);
            /*
             * This variable serves as a backup of the current state of the input script at
             * the time this rule validation began. Should a rule validation fail, the input script
             * state is reset to this point for the checking of further derivation options.
             */
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            /* 
             * A single "try" block always represents the validation of a rule. The typical contents of
             * this block are a sequence of function calls for each rule comprising one derivation option.
             */
            try
            {
                toReturn.Derivations.Add(LIST_IMPORT(input));
                toReturn.Derivations.Add(CLASS(inputQueue.Pop()));
            }
            /*
             * A "catch" block represents the failure of a derivation. Within it, there are typically two
             * options of content: Either another derivation rule will be attempted with a "try" block,
             * or an exception will be thrown declaring that the token being analysed is not of this
             * function's equivalent type. Additionally, should the rule being validated be optional,
             * that is, should it have the possibility of deriving Epsilon, then the last "catch" block
             * should be the Epsilon treatment.
             */
            catch (FormatException e)
            {
                /*
                 * Before attempting the derivation of another rule option, or returning the exception
                 * that signals the failure of a derivation, the input state is returned to the point
                 * recorded in the backup state.
                 */
#if DEBUG
                inputQueue.PrintTokenList("TokenizationListCriticalError.txt");
#endif

                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of OATS type; " + e.Message);
            }

#if DEBUG
            inputQueue.PrintTokenList("TokenizationTestEnd.txt");
            toReturn.PrintSyntaxTree("SyntaxTree.txt", inputQueue);
            DateTime currentTime = DateTime.Now;
            using (StreamWriter writer = new StreamWriter("SyntaxTime.txt"))
            {
                writer.WriteLine(currentTime - startTime);
            }
#endif
            return toReturn;
        }

        /*
         * LIST_IMPORT represents a list of "import" statements.
         * 
         * LIST_IMPORT -> IMPORT LIST_IMPORT
         *              | Epsilon
         */
        private ClassNode LIST_IMPORT(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.List_Import);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                toReturn.Derivations.Add(IMPORT(input));
                toReturn.Derivations.Add(LIST_IMPORT(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                /*
                 * This is a case where a "catch" block is being used to validate an "Epsilon" derivation.
                 * First, the inputQueue is reset to the backup state. Due to its failure, none of the 
                 * alterations made in the "try" block should be taken into consideration. Then, the
                 * ClassNode object that represents the result of this validation has its derivations
                 * wiped, as the only derivation it should have is Epsilon itself. Finally, after adding
                 * Epsilon to the derivations, the input queue pointer should be decremented. Since Epsilon
                 * represents the absence of a symbol, no tokens should be consumed by it.
                 */
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * IMPORT represents a single "import" statement. The derivation rule for this Non-Terminal symbol,
         * IMPORT -> 'import' IDENTIFIER IDENTIFIER2 ';', is implemented in this manner to facilitate the
         * treatment of optional symbols.
         * 
         * This rule could have been represented as follows:
         * IMPORT -> 'import' IDENTIFIER ('.' IDENTIFIER)* ';'
         *         | 'import' IDENTIFIER ';'
         *         
         * Implementing this, however, would be far more complicated than we would like. To simplify it,
         * a common GLC technique is applied where the optional part is transformed into a new rule.
         * 
         * IMPORT -> 'import' IDENTIFIER IDENTIFIER2 ';'
         * 
         * The new rule is comprised exactly of the optional symbols it has replaced, or Epsilon. If, on
         * top of being optional, the symbols are also marked with the '*', meaning "0 or more", then the
         * new rule is comprised of the optional symbols, followed by the rule itself, or Epsilon. Since
         * that is the case with IDENTIFIER2, the new rule is:
         * 
         * IDENTIFIER2 -> '.' IDENTIFIER IDENTIFIER2
         *              | Epsilon
         * 
         * 
         * IMPORT -> 'import' IDENTIFIER IDENTIFIER2 ';'
         * Has split.
         */
        private ClassNode IMPORT(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Import);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                /*
                 * An "if-else" block structured in this way represents an attempt to identify a reserved
                 * word token. Since the constant 'import' is mandatory to this rule, entering the "else"
                 * statement implies a failed validation, causing an exception to be thrown.
                 */
                if (input.Equals("import"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                /*
                 * It is important to note that whenever a terminal symbol is added to the syntax tree,
                 * the related input should not be analysed again within the scope of that function call or
                 * derived function calls.
                 * 
                 * The IDENTIFIER rule has the particularity of having tokens that are not divided by white
                 * spaces, and are therefore not caught by the initial tokenization of the input script.
                 * This is not unique to IDENTIFIER.
                 * 
                 * The InputQueue class has methods available to deal with this situation. When called more
                 * than once, such as is necessary here, it should be called in the inverse order of appearance
                 * of the characters that serve as borders. For example, since an IMPORT statement can have
                 * several periods through it, and only one semicolon at the end, we first split at semicolon
                 * and then at period. The InputQueue will rearrange itself during these calls and can continue
                 * to be used as normal.
                 */
                inputQueue.SplitItem(";");
                inputQueue.SplitItem(".");

                toReturn.Derivations.Add(IDENTIFIER(inputQueue.Pop()));
                toReturn.Derivations.Add(IDENTIFIER2(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of IMPORT type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * An IDENTIFIER is defined as any string of interspaced letters or numbers. It is a Terminal symbol.
         * 
         * IDENTIFIER -> [A-za-z0-9]+
         */
        private ClassNode IDENTIFIER(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Identifier);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                /*
                 * Because IDENTIFIER is a Terminal Symbol, it must read the value of input itself, rather
                 * than attempt to apply further derivation rules. To do this, we have used Regular Expressions.
                 * A Regex is defined in the form (@"^string$").
                 *      
                 *      The @ symbol means "Read this string without applying any interpretations". That means 
                 *          the string is read without requiring to use backslash for special characters.
                 *      The ^ symbol means "The beginning of the string must match this Regex exactly".
                 *      The $ symbol means "The end of the string must match this Regex exactly".
                 *      
                 * If a match is not found, then the Terminal Symbol validation has failed and an Exception is
                 * thrown.
                 */
                Regex toCheck = new Regex(@"^[A-za-z0-9]+$");
                if (toCheck.Match(input).Success)
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of IDENTIFIER type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * IDENTIFIER2 is an auxiliary rule to derive identifiers interspaced with periods.
         * 
         * IDENTIFIER2 -> '.' IDENTIFIER IDENTIFIER3
         *              | Epsilon
         */
        private ClassNode IDENTIFIER2(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Identifier2);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("."))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(IDENTIFIER(inputQueue.Pop()));
                toReturn.Derivations.Add(IDENTIFIER3(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * IDENTIFIER3 is an auxiliary rule to derive identifiers interspaced with periods and ending in *.
         * 
         * IDENTIFIER3 -> '.' '*'
		 *  		    | IDENTIFIER2
         */
        private ClassNode IDENTIFIER3(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Identifier3);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("."))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("*"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException)
            {
                /*
                 * This is a case where the "catch" block is being used to validate a second derivation
                 * option. Because the previous derivation failed, the inputQueue is reset to the backup
                 * state and the return node's derivations list is reset, akin to the Epsilon treatment.
                 * However, unlike the Epsilon treatment, the pointer is not decremented, given that the
                 * same input will be tested on the next rule. After this, a try-catch block is made for
                 * the next rule in the same way as a normal try-catch block.
                 */
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();

                try
                {
                    toReturn.Derivations.Add(IDENTIFIER2(input));
                }
                catch (FormatException e)
                {
                    inputQueue = backupInput;
                    throw new FormatException("Input " + input + " and following terms are not of IDENTIFIER3 type; " + e.Message);
                }
            }

            return toReturn;
        }

        /*
         * CLASS represents an entire class declaration.
         * 
         * CLASS ->	CLASS_DECLARATION '{' BODY '}'
         */
        private ClassNode CLASS(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Class);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                toReturn.Derivations.Add(CLASS_DECLARATION(input));
                if (inputQueue.Pop().Equals("{"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
                toReturn.Derivations.Add(BODY(inputQueue.Pop()));
                if (inputQueue.Pop().Equals("}"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of CLASS type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * CLASS_DECLARATION represents the reserved words, name and parameters of a class.
         * 
         * CLASS_DECLARATION -> 'public' 'class' [A-Za-z][A-Za-z0-9_]* 'extends' 'IteratingVUserScript'
         */
        private ClassNode CLASS_DECLARATION(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Class_Declaration);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("public"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("class"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                /*
                 * The reason why we have placed a regular expression within a Non-Terminal Symbol is 
                 * that the value of this regular expression is irrelevant to the Semantic Analysis. 
                 * Should this value become relevant in future implementations, a Terminal Symbol should 
                 * be created to represent it. Otherwise, it can be regarded in the same way as a Reserved 
                 * Word.
                 */
                Regex toCheck = new Regex(@"^[A-Za-z][A-Za-z0-9_]*$");
                if (toCheck.Match(inputQueue.Pop()).Success)
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("extends"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("IteratingVUserScript"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of CLASS_DECLARATION type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * BODY represents the body of statements, attributes and methods of a class.
         * 
         * BODY -> SCRIPT_SERVICE METHODS
         */
        private ClassNode BODY(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Body);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
#if DEBUG
                inputQueue.PrintTokenList("TokenizationListAtStartOfBody.txt");
#endif
                toReturn.Derivations.Add(SCRIPT_SERVICE(input));
                toReturn.Derivations.Add(METHODS(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of BODY type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * SCRIPT_SERVICE represents the script service declarations at the beginning of a class.
         * 
         * SCRIPT_SERVICE -> '@ScriptService' IDENTIFIER IDENTIFIER2 IDENTIFIER ';' SCRIPT_SERVICE
         *                 | Epsilon
         * Has split.
         */
        private ClassNode SCRIPT_SERVICE(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Script_Service);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("@ScriptService"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem(".");

                toReturn.Derivations.Add(IDENTIFIER(inputQueue.Pop()));
                toReturn.Derivations.Add(IDENTIFIER2(inputQueue.Pop()));

                inputQueue.SplitItem(";");

                toReturn.Derivations.Add(IDENTIFIER(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(SCRIPT_SERVICE(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * METHODS represents the list of methods contained in a class.
         * 
         * METHODS -> JAVADOC METHODS2
         */
        private ClassNode METHODS(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Methods);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                toReturn.Derivations.Add(JAVADOC(input));
                toReturn.Derivations.Add(METHODS2(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of METHODS type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * METHODS2 is an auxiliary rule to derive the list of methods contained in a class.
         * 
         * METHODS2 -> METHOD METHODS
         *           | Epsilon
         */
        private ClassNode METHODS2(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Methods2);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                toReturn.Derivations.Add(METHOD(input));
                toReturn.Derivations.Add(METHODS(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * ANY is a Terminal symbol used VERY sparingly to represent certain tokens that
         * can be represented by a large number of possibilities.
         * 
         * ANY -> [A-Za-z0-9,\(\)\[\]/\*&_#\- ""]*
         */
        private ClassNode ANY(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Any);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                Regex toCheck = new Regex(@"^[A-Za-z0-9,\(\)\[\]/\*&_#\- ""]*$");
                if (!toCheck.Match(input).Success)
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of ANY type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * ANY2 is an auxiliary symbol to ANY. It is an unique rule in that it is specialized 
         * to predict a specific symbol and stop at that point. This kind of specialization is
         * not recommended, and was used here as a last resort to simplify what could have been
         * a very long series of unimportant rules.
         * 
         * ANY2 -> ANY_JAVADOC ANY2
         *       | Epsilon
         */
        private ClassNode ANY2(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Any2);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                /*
                 * This is the specialization mentioned above. Once the input reaches this specific
                 * token, the sequence ends.
                 */
                Regex toCheck = new Regex(@"^[\*]*/$");
                if (toCheck.Match(input).Success)
                    throw new FormatException();

                toReturn.Derivations.Add(ANY_JAVADOC(input));
                toReturn.Derivations.Add(ANY2(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * ANY3 is an auxiliary symbol to ANY, and has a functionality akin to ANY2, but 
         * with a different special rule to read strings.
         * 
         * ANY3 -> ANY_JAVADOC ANY3
         *       | Epsilon
         */
        private ClassNode ANY3(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Any3);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                inputQueue.SplitItem("\"");

                Regex toCheck = new Regex("^\"$");
                if (toCheck.Match(input).Success)
                    throw new FormatException();

                toReturn.Derivations.Add(ANY_JAVADOC(input));
                toReturn.Derivations.Add(ANY3(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * ANY_JAVADOC is a variant of ANY used to evaluate text within comments.
         * 
         * ANY_JAVADOC -> [A-Za-z0-9,.:\(\)\[\]/\*&_#\-=+ ""]*
         */
        private ClassNode ANY_JAVADOC(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Any_Javadoc);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                Regex toCheck = new Regex(@"^[A-Za-z0-9,.:\(\)\[\]/\*&_#\-=+ ""]*$");
                if (!toCheck.Match(input).Success)
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of ANY type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * JAVADOC represents any type of comment structured in the standard Javadoc format.
         * 
         * JAVADOC -> /\*\* ANY2 [\*]* /
         *          | Epsilon
         */
        private ClassNode JAVADOC(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Javadoc);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                Regex toCheck = new Regex(@"^/\*\*$");
                if (toCheck.Match(input).Success)
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ANY2(inputQueue.Pop()));

                toCheck = new Regex(@"^[\*]*/$");
                if (toCheck.Match(inputQueue.Pop()).Success)
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * METHOD represents a single class method.
         * 
         * METHOD -> METHOD_DECLARATION '{' BLOCK '}'
         */
        private ClassNode METHOD(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Method);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                toReturn.Derivations.Add(METHOD_DECLARATION(input));

                if (inputQueue.Pop().Equals("{"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestBeforeMethodBody.txt");
#endif

                toReturn.Derivations.Add(BLOCK(inputQueue.Pop()));

                if (inputQueue.Pop().Equals("}"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of METHOD type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * METHOD_DECLARATION represents the signature of a method.
         * 
         * METHOD_DECLARATION -> 'public' 'void' ([A-Za-z][A-Za-z0-9_]*)\(\) METHOD_DECLARATION2
         */
        private ClassNode METHOD_DECLARATION(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Method_Declaration);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("public"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("void"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                Regex toCheck = new Regex(@"^([A-Za-z][A-Za-z0-9_]*)\(\)$");
                if (toCheck.Match(inputQueue.Pop()).Success)
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(METHOD_DECLARATION2(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of METHOD_DECLARATION type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * METHOD_DECLARATION2 is an auxiliary rule for the derivation of METHOD_DECLARATION.
         * 
         * METHOD_DECLARATION2 -> 'throws' 'Exception'
         *                      | Epsilon
         */
        private ClassNode METHOD_DECLARATION2(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Method_Declaration2);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("throws"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("Exception"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * BLOCK represents the statements of a method.
         * 
         * BLOCK -> SCRIPT_ELEMENT THINK BLOCK
         *        | STEP BLOCK
         *        | Epsilon
         * Has split.
         */
        private ClassNode BLOCK(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Block);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                toReturn.Derivations.Add(SCRIPT_ELEMENT(input));
                toReturn.Derivations.Add(THINK(inputQueue.Pop()));
                toReturn.Derivations.Add(BLOCK(inputQueue.Pop()));
            }
            catch (FormatException)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                try
                {
                    toReturn.Derivations.Add(STEP(input));
                    toReturn.Derivations.Add(BLOCK(inputQueue.Pop()));
                }
                catch (FormatException e)
                {
                    inputQueue = backupInput;
                    toReturn.Derivations = new List<ClassNode>();
                    toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                    inputQueue.DecrementPointer();
                }
            }

            return toReturn;
        }

        /*
         * THINK represents the "think" statement that is specific to certain script elements.
         * 
         * THINK ->	'{' 'think' '(' NUMBER ')' ';' '}'
         *        | Epsilon
         */
        private ClassNode THINK(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Think);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                inputQueue.SplitItem(";");
                inputQueue.SplitItem(")");
                inputQueue.SplitItem(".");
                inputQueue.SplitItem("(");

                if (input.Equals("{"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("think"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("("))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(NUMBER(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(")"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("}"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * NUMBER represents any real value.
         * 
         * NUMBER -> [0-9]+ NUMBER2
         */
        private ClassNode NUMBER(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Number);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                Regex toCheck = new Regex(@"^[0-9]+$");
                if (toCheck.Match(input).Success)
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(NUMBER2(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of NUMBER type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * NUMBER2 is an auxiliary rule to represent the floating point value of a number.
         * 
         * NUMBER2 -> '.' [0-9]+
         *          | Epsilon
         */
        private ClassNode NUMBER2(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Number2);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("."))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                Regex toCheck = new Regex(@"^[0-9]+$");
                if (toCheck.Match(inputQueue.Pop()).Success)
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * STEP represents a "step" block of the script.
         * 
         * STEP -> BEGIN_STEP '{' ELEMENT_SEQUENCE '}' CLOSE_STEP
         */
        private ClassNode STEP(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Step);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                inputQueue.DecrementPointer();

                inputQueue.SplitItem("\"");
                inputQueue.SplitItem("(");

                toReturn.Derivations.Add(BEGIN_STEP(inputQueue.Pop()));

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterFirstBeginStepRule.txt");
#endif

                if (inputQueue.Pop().Equals("{"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_SEQUENCE(inputQueue.Pop()));

                if (inputQueue.Pop().Equals("}"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(CLOSE_STEP(inputQueue.Pop()));

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterFirstStepRule.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of STEP type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * BEGIN_STEP represents the entry point of a step block.
         * 
         * BEGIN_STEP -> 'beginStep' '(' STEP_NAME ',' NUMBER ')' ';'
         */
        private ClassNode BEGIN_STEP(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Begin_Step);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("beginStep"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("("))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(STEP_NAME(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(","))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem(")");

                toReturn.Derivations.Add(NUMBER(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(")"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of BEGIN_STEP type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * STEP_NAME represents the name value of a step.
         * 
         * STEP_NAME ->	'"' ANY3 '"'
         */
        private ClassNode STEP_NAME(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Step_Name);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("\""))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ANY3(inputQueue.Pop()));

                if (inputQueue.Pop().Equals("\""))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of STEP_NAME type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * CLOSE_STEP represents the end statement of a step block.
         * 
         * CLOSE_STEP -> 'endStep();'
         */
        private ClassNode CLOSE_STEP(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Close_Step);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("endStep();"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of CLOSE_STEP type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * ELEMENT_SEQUENCE represents a sequence of script elements within a step block.
         * 
         * ELEMENT_SEQUENCE -> SCRIPT_ELEMENT THINK ELEMENT_SEQUENCE
         *                   | Epsilon
         */
        private ClassNode ELEMENT_SEQUENCE(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Element_Sequence);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                toReturn.Derivations.Add(SCRIPT_ELEMENT(input));
                toReturn.Derivations.Add(THINK(inputQueue.Pop()));
                toReturn.Derivations.Add(ELEMENT_SEQUENCE(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                inputQueue.DecrementPointer();
            }

            return toReturn;
        }

        /*
         * ELEMENT_NAME represents the string with the name tag of a script element.
         * 
         * ELEMENT_NAME -> '"' '{' '{' ANY '.' ANY '.' ANY '}' '}' '"'
         */
        private ClassNode ELEMENT_NAME(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Element_Name);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("\""))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem("}");
                inputQueue.SplitItem(".");
                inputQueue.SplitItem("{");

                if (inputQueue.Pop().Equals("{"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("{"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ANY(inputQueue.Pop()));

                if (inputQueue.Pop().Equals("."))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ANY(inputQueue.Pop()));

                if (inputQueue.Pop().Equals("."))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ANY(inputQueue.Pop()));

                if (inputQueue.Pop().Equals("}"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("}"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("\""))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of ELEMENT_NAME type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * ACTION represents the action tag value of a specific script element.
         * 
         * ACTION -> ANY '(' ACTION2 ')'
         */
        private ClassNode ACTION(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Action);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
#if DEBUG
                if (input.Equals("setPassword"))
                    inputQueue.PrintTokenList("TokenizationTestBeforeSetPassword.txt");
#endif

                toReturn.Derivations.Add(ANY(input));

                if (inputQueue.Pop().Equals("("))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem(")");

                toReturn.Derivations.Add(ACTION2(inputQueue.Pop()));

                inputQueue.SplitItem(")");

                if (inputQueue.Pop().Equals(")"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of ACTION type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * ACTION2 is an auxiliary rule to ACTION, allowing for nested action symbols.
         * 
         * ACTION2 -> ACTION
		 *			| ANY
         *			| '"' ANY3 '"'
		 *		    | Epsilon
         */
        private ClassNode ACTION2(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Action2);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

#if DEBUG
            inputQueue.PrintTokenList("TokenizationTestBeforeAction2.txt");
#endif

            try
            {
                toReturn.Derivations.Add(ACTION(input));
            }
            catch (FormatException)
            {
                inputQueue = backupInput;
                toReturn.Derivations = new List<ClassNode>();
                try
                {
                    if (input.Equals(")"))
                        throw new FormatException();
                    else if (input.StartsWith("\""))
                    {
                        inputQueue.DecrementPointer();
                        inputQueue.SplitItem("\"");

                        if (inputQueue.Pop().Equals("\""))
                            toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                        else
                            throw new FormatException();

                        toReturn.Derivations.Add(ANY3(inputQueue.Pop()));

                        if (inputQueue.Pop().Equals("\""))
                            toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                        else
                            throw new FormatException();
                    }
                    else
                        toReturn.Derivations.Add(ANY(input));
                }
                catch (FormatException)
                {
                    inputQueue = backupInput;
                    toReturn.Derivations = new List<ClassNode>();
                    toReturn.Derivations.Add(new ClassNode(Classes.Epsilon));
                    inputQueue.DecrementPointer();
                }
            }

            return toReturn;
        }

        /*
         * SCRIPT_ELEMENT is a generic placeholder symbol for any specific script element.
         * This rule in particular was made in a predictive way, rather than recursive like the
         * rest of the Analyser, because of the large number of the derivations it has and the 
         * ease of identifying which one to use.
         * 
         * SCRIPT_ELEMENT -> WEB_WINDOW
		 *			       | WEB_TEXTBOX
		 *			       | WEB_BUTTON
		 *			       | WEB_IMAGE
		 *			       | WEB_ALERT_DIALOG
		 *			       | WEB_LINK
		 *			       | BROWSER_LAUNCH
		 *                 | WEB_ELEMENT
         */
        private ClassNode SCRIPT_ELEMENT(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Script_Element);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                inputQueue.DecrementPointer();

                inputQueue.SplitItem(";");
                inputQueue.SplitItem(")");
                inputQueue.SplitItem("(");

                string toCheck = inputQueue.Pop();

                switch (toCheck)
                {
                    case "web.window":
                        toReturn.Derivations.Add(WEB_WINDOW(toCheck));
                        break;
                    case "web.textBox":
                        toReturn.Derivations.Add(WEB_TEXTBOX(toCheck));
                        break;
                    case "web.button":
                        toReturn.Derivations.Add(WEB_BUTTON(toCheck));
                        break;
                    case "web.image":
                        toReturn.Derivations.Add(WEB_IMAGE(toCheck));
                        break;
                    case "web.alertDialog":
                        toReturn.Derivations.Add(WEB_ALERT_DIALOG(toCheck));
                        break;
                    case "web.link":
                        toReturn.Derivations.Add(WEB_LINK(toCheck));
                        break;
                    case "web.element":
                        toReturn.Derivations.Add(WEB_ELEMENT(toCheck));
                        break;
                    case "web.selectBox":
                        toReturn.Derivations.Add(WEB_SELECT_BOX(toCheck));
                        break;
                    case "web.dialog":
                        toReturn.Derivations.Add(WEB_DIALOG(toCheck));
                        break;
                    case "web.radioButton":
                        toReturn.Derivations.Add(WEB_RADIO_BUTTON(toCheck));
                        break;
                    case "web.checkBox":
                        toReturn.Derivations.Add(WEB_CHECK_BOX(toCheck));
                        break;
                    case "web.textArea":
                        toReturn.Derivations.Add(WEB_TEXT_AREA(toCheck));
                        break;
                    case "browser.launch":
                        toReturn.Derivations.Add(BROWSER_LAUNCH(toCheck));
                        break;
                    default:
                        throw new FormatException();
                }
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of SCRIPT_ELEMENT type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * ELEMENT_DETAILS represents the values of a script element that are of interest
         * to our purpose.
         * 
         * ELEMENT_DETAILS -> '(' NUMBER ',' ELEMENT_NAME ')' '.' ACTION
         */
        private ClassNode ELEMENT_DETAILS(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Element_Details);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("("))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem(",");

                toReturn.Derivations.Add(NUMBER(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(","))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem("\"");

                toReturn.Derivations.Add(ELEMENT_NAME(inputQueue.Pop()));

                inputQueue.SplitItem(".");

                if (inputQueue.Pop().Equals(")"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem(".");

                if (inputQueue.Pop().Equals("."))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                inputQueue.SplitItem(")");
                inputQueue.SplitItem("(");

                toReturn.Derivations.Add(ACTION(inputQueue.Pop()));
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of ELEMENT_DETAILS type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * WEB_WINDOW is one of the script elements covered by this analyser.
         * 
         * WEB_WINDOW -> 'web.window' ELEMENT_DETAILS ';'
         */
        private ClassNode WEB_WINDOW(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Window);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("web.window"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizerTestAfterWebWindow.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_WINDOW type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * WEB_TEXTBOX is one of the script elements covered by this analyser.
         * 
         * WEB_TEXTBOX -> 'web.textBox' ELEMENT_DETAILS ';'
         */
        private ClassNode WEB_TEXTBOX(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Textbox);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("web.textBox"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterTextBox.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_TEXTBOX type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * WEB_BUTTON is one of the script elements covered by this analyser.
         * 
         * WEB_BUTTON -> 'web.button' ELEMENT_DETAILS ';'
         */
        private ClassNode WEB_BUTTON(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Button);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("web.button"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterButton.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_BUTTON type; " + e.Message);
            }

            return toReturn;
        }

        private ClassNode WEB_IMAGE(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Image);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.image"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterImage.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_IMAGE type; " + e.Message);
            }

            return toReturn;
        }

        private ClassNode WEB_ALERT_DIALOG(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Alert_Dialog);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.alertDialog"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterAlertDialog.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_ALERT_DIALOG type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * WEB_LINK is one of the script elements covered by this analyser.
         * 
         * WEB_LINK -> 'web.link' ELEMENT_DETAILS ';'
         */
        private ClassNode WEB_LINK(string input)
        {
#if DEBUG
            inputQueue.PrintTokenList("TokenizerTestWebWindow.txt");
#endif

            ClassNode toReturn = new ClassNode(Classes.Web_Link);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("web.link"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_LINK type; " + e.Message);
            }

            return toReturn;
        }

        private ClassNode WEB_ELEMENT(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Element);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.element"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterElement.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_ELEMENT type; " + e.Message);
            }

            return toReturn;
        }

        private ClassNode WEB_SELECT_BOX(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Select_Box);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.selectBox"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterSelectBox.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_SELECT_BOX type; " + e.Message);
            }

            return toReturn;
        }

        //TODO - Test (Edemar)
        private ClassNode WEB_DIALOG(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Dialog);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.dialog"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterDialog.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_DIALOG type; " + e.Message);
            }

            return toReturn;
        }

        //TODO - Test (Edemar)
        private ClassNode WEB_RADIO_BUTTON(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Radio_Button);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.radioButton"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterRadioButton.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_RADIO_BUTTON type; " + e.Message);
            }

            return toReturn;
        }

        //TODO - Test (Edemar)
        private ClassNode WEB_CHECK_BOX(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Check_Box);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.checkBox"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterCheckBox.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_CHECK_BOX type; " + e.Message);
            }

            return toReturn;
        }

        //TODO - Test (Edemar)
        private ClassNode WEB_TEXT_AREA(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Web_Text_Area);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                //RuleChecks
                if (input.Equals("web.textArea"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                toReturn.Derivations.Add(ELEMENT_DETAILS(inputQueue.Pop()));

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

#if DEBUG
                inputQueue.PrintTokenList("TokenizationTestAfterTextArea.txt");
#endif
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of WEB_TEXT_AREA type; " + e.Message);
            }

            return toReturn;
        }

        /*
         * BROWSER_LAUNCH represents the browser.launch script element. It is written in a
         * slightly different way from the rest of the script elements due to its different
         * structure.
         * 
         * BROWSER_LAUNCH -> 'browser.launch' '(' ')' ';'
         */
        private ClassNode BROWSER_LAUNCH(string input)
        {
            ClassNode toReturn = new ClassNode(Classes.Browser_Launch);
            InputQueue backupInput = (InputQueue)inputQueue.Clone();

            try
            {
                if (input.Equals("browser.launch"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals("("))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals(")"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();

                if (inputQueue.Pop().Equals(";"))
                    toReturn.Derivations.Add(new ClassNode(Classes.Reserved_Word));
                else
                    throw new FormatException();
            }
            catch (FormatException e)
            {
                inputQueue = backupInput;
                throw new FormatException("Input " + input + " and following terms are not of BROWSER_LAUNCH type; " + e.Message);
            }

            return toReturn;
        }
    }
}