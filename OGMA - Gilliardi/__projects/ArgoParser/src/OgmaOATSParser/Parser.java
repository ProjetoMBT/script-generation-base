package OgmaOATSParser;

//### This file created by BYACC 1.8(/Java extension  1.15)
//### Java capabilities added 7 Jan 97, Bob Jamison
//### Updated : 27 Nov 97  -- Bob Jamison, Joe Nieten
//###           01 Jan 98  -- Bob Jamison -- fixed generic semantic constructor
//###           01 Jun 99  -- Bob Jamison -- added Runnable support
//###           06 Aug 00  -- Bob Jamison -- made state variables class-global
//###           03 Jan 01  -- Bob Jamison -- improved flags, tracing
//###           16 May 01  -- Bob Jamison -- added custom stack sizing
//###           04 Mar 02  -- Yuval Oren  -- improved java performance, added options
//###           14 Mar 02  -- Tomas Hurka -- -d support, static initializer workaround
//### Please send bug reports to tom@hukatronic.cz
//### static char yysccsid[] = "@(#)yaccpar	1.8 (Berkeley) 01/20/90";






//#line 2 "oatsy.y"
/*v1.0.2*/

import java.io.*;
import java.util.*;

import Objects.*;
import Diagrams.*;

//#line 26 "Parser.java"




public class Parser
{

boolean yydebug;        //do I want debug output?
int yynerrs;            //number of errors so far
int yyerrflag;          //was there an error?
int yychar;             //the current working character

//########## MESSAGES ##########
//###############################################################
// method: debug
//###############################################################
void debug(String msg)
{
  if (yydebug)
    System.out.println(msg);
}

//########## STATE STACK ##########
final static int YYSTACKSIZE = 500;  //maximum stack size
int statestk[] = new int[YYSTACKSIZE]; //state stack
int stateptr;
int stateptrmax;                     //highest index of stackptr
int statemax;                        //state when highest index reached
//###############################################################
// methods: state stack push,pop,drop,peek
//###############################################################
final void state_push(int state)
{
  try {
		stateptr++;
		statestk[stateptr]=state;
	 }
	 catch (ArrayIndexOutOfBoundsException e) {
     int oldsize = statestk.length;
     int newsize = oldsize * 2;
     int[] newstack = new int[newsize];
     System.arraycopy(statestk,0,newstack,0,oldsize);
     statestk = newstack;
     statestk[stateptr]=state;
  }
}
final int state_pop()
{
  return statestk[stateptr--];
}
final void state_drop(int cnt)
{
  stateptr -= cnt; 
}
final int state_peek(int relative)
{
  return statestk[stateptr-relative];
}
//###############################################################
// method: init_stacks : allocate and prepare stacks
//###############################################################
final boolean init_stacks()
{
  stateptr = -1;
  val_init();
  return true;
}
//###############################################################
// method: dump_stacks : show n levels of the stacks
//###############################################################
void dump_stacks(int count)
{
int i;
  System.out.println("=index==state====value=     s:"+stateptr+"  v:"+valptr);
  for (i=0;i<count;i++)
    System.out.println(" "+i+"    "+statestk[i]+"      "+valstk[i]);
  System.out.println("======================");
}


//########## SEMANTIC VALUES ##########
//public class ParserVal is defined in ParserVal.java


String   yytext;//user variable to return contextual strings
ParserVal yyval; //used to return semantic vals from action routines
ParserVal yylval;//the 'lval' (result) I got from yylex()
ParserVal valstk[];
int valptr;
//###############################################################
// methods: value stack push,pop,drop,peek.
//###############################################################
void val_init()
{
  valstk=new ParserVal[YYSTACKSIZE];
  yyval=new ParserVal();
  yylval=new ParserVal();
  valptr=-1;
}
void val_push(ParserVal val)
{
  if (valptr>=YYSTACKSIZE)
    return;
  valstk[++valptr]=val;
}
ParserVal val_pop()
{
  if (valptr<0)
    return new ParserVal();
  return valstk[valptr--];
}
void val_drop(int cnt)
{
int ptr;
  ptr=valptr-cnt;
  if (ptr<0)
    return;
  valptr = ptr;
}
ParserVal val_peek(int relative)
{
int ptr;
  ptr=valptr-relative;
  if (ptr<0)
    return new ParserVal();
  return valstk[ptr];
}
final ParserVal dup_yyval(ParserVal val)
{
  ParserVal dup = new ParserVal();
  dup.ival = val.ival;
  dup.dval = val.dval;
  dup.sval = val.sval;
  dup.obj = val.obj;
  return dup;
}
//#### end semantic value section ####
public final static short IMPORT=257;
public final static short PUBLIC=258;
public final static short CLASS=259;
public final static short EXTENDS=260;
public final static short SCRIPTSERVICE=261;
public final static short VOID=262;
public final static short THROWS=263;
public final static short IDENTIFIER=264;
public final static short UNKNOWN=265;
public final static short JAVADOCSTART=266;
public final static short JAVADOCEND=267;
public final static short ANY=268;
public final static short NUMBER=269;
public final static short THINK=270;
public final static short BEGINSTEP=271;
public final static short ENDSTEP=272;
public final static short YYERRCODE=256;
final static short yylhs[] = {                           -1,
    0,    0,   13,   11,   11,   14,    5,    6,    6,   15,
   16,   16,   12,   17,   18,   18,   19,   19,   21,   20,
   20,   23,   23,   25,   26,   26,   22,   24,   28,   28,
   27,   30,   30,   29,   29,   33,   33,   32,   32,   35,
   35,   36,   36,   34,   38,   38,   37,   41,   42,   42,
   40,   39,   43,   43,    2,   44,    7,    7,    8,    8,
    8,    9,    9,   10,   10,   31,   46,   46,    1,    3,
    4,   45,   47,   47,
};
final static short yylen[] = {                            2,
    2,    1,    0,    2,    1,    3,    2,    2,    1,    2,
    2,    1,    4,    5,    2,    1,    2,    1,    4,    2,
    1,    2,    1,    2,    1,    1,    3,    3,    2,    1,
    6,    2,    1,    3,    2,    1,    1,    3,    1,    6,
    1,    1,    1,    2,    4,    1,    7,    2,    2,    1,
    4,    3,    1,    1,    3,    1,    2,    1,    2,    4,
    3,    2,    1,    2,    1,    4,    4,    1,    2,    2,
    2,    3,    1,    1,
};
final static short yydefred[] = {                         0,
    0,    0,    0,    0,    2,    5,    0,    0,    0,    0,
    1,    4,    0,    6,    0,    0,    0,    0,   18,    0,
    0,   16,    0,    0,   21,    0,    0,    0,    0,    0,
    0,    0,   13,   23,   15,   18,   17,   20,   22,    0,
   14,    0,    0,   26,   25,   24,   27,    0,    0,   30,
    0,   28,    0,    0,    0,    0,    0,   19,   70,    0,
    0,    0,   29,    0,   39,    0,   37,   36,   35,    0,
    0,   44,   46,    0,    0,    0,    0,    0,   58,    0,
    0,    0,    0,   56,    0,   68,    0,    0,   41,    0,
   34,    0,    0,    0,    0,   33,   31,    0,   50,   48,
    0,    0,   74,   73,   72,    0,    7,    9,    0,   71,
    0,    0,    0,    0,    0,   59,   63,   57,   66,    0,
   38,    0,    0,    0,   32,   49,    0,    0,   12,   10,
    8,    0,   69,    0,    0,   61,   65,    0,   62,    0,
   51,   54,   53,   52,   45,    0,   11,   55,   60,   64,
   67,    0,   47,    0,   40,
};
final static short yydgoto[] = {                          3,
  110,  111,   51,   82,   83,  107,   84,   85,  116,  136,
    4,    5,   34,    7,  104,  130,    8,   20,   21,   22,
   23,   24,   25,   26,   32,   46,   27,   52,   68,   97,
   54,   66,   69,   55,   90,    0,   56,   72,   94,   73,
   76,  100,  144,   86,   62,   87,  105,
};
final static short yysindex[] = {                      -224,
 -242, -228,    0, -219,    0,    0, -217,  -81,  -18, -221,
    0,    0, -237,    0, -216, -215, -223, -222,    0,  -77,
 -236,    0, -212, -208,    0, -236,  -72, -211, -210, -213,
 -222, -207,    0,    0,    0,    0,    0,    0,    0, -109,
    0,   12,   -3,    0,    0,    0,    0,   11,   21,    0,
 -202,    0,  -62,  -59, -239, -118,   24,    0,    0,   32,
   29,  -32,    0, -199,    0, -239,    0,    0,    0,   33,
 -192,    0,    0, -189,  -34,   31, -193,   34,    0,   43,
  -33,   37,   39,    0,   44,    0,   25,   46,    0,  -37,
    0,   48,  -59,  -35, -173,    0,    0,   58,    0,    0,
 -193,   47,    0,    0,    0, -170,    0,    0, -172,    0,
   54,   64,   55, -202,  -27,    0,    0,    0,    0, -193,
    0,   41, -192, -171,    0,    0,   61, -166,    0,    0,
    0,   70,    0,   55,  -28,    0,    0,  -27,    0,   66,
    0,    0,    0,    0,    0,   49,    0,    0,    0,    0,
    0,   50,    0, -199,    0,
};
final static short yyrindex[] = {                         0,
    0,    0,    0,    0,    0,    0, -153,    0,    0,    0,
    0,    0,  -15,    0,    0,    0,    0,    0,    0,    0,
  -15,    0, -117,  -15,    0,  -15,    0,    0,    0,    0,
 -156,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, -107,  -15,    0,    0,    0,    0,    0,
    0,    0,    0,  -15,    0,  -15,    0,    0,    0,    0,
    0,    0,    0,  -11,    0,    0,  -31,   -4,    0,    0,
    0,    0,   72,    0,    0,    0,    0,    0,    0,    0,
    0,    0, -105,    0,    0,    0,    0,    0,    0,    0,
    0,  -31,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   72,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  -15,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   72,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -15,    0,
};
final static short yygindex[] = {                         0,
    0,    0,    0,    0,    8,    0,    1,  -20,    0,  -16,
  112,  116,    4,    0,  -82,    0,    0,    0,   98,    2,
    0,    0,   99,    0,   91,    0,    0,    0,   84,    0,
  -56,   35,   59,    0,  -25,    0,    0,    0,    3,    6,
    0,    0,    0,   -7,   18,    0,    0,
};
final static int YYTABLESIZE=237;
static short yytable[];
static { yytable();}
static void yytable(){
yytable = new short[]{                         99,
  113,   81,    3,    6,   71,   81,   81,    3,   79,    3,
    6,   80,    3,   79,   93,   50,   19,    3,  127,    3,
   16,   16,   35,   17,   48,    9,   36,   39,   18,   18,
   10,   49,    1,    2,   44,    3,    3,  140,    2,    1,
   14,   13,   15,   28,   30,   31,   29,   33,   17,   16,
   40,   57,   41,   42,   43,   58,   59,   65,   67,   47,
   60,   61,   63,   64,   74,   75,   93,   89,   77,   67,
   88,   48,   92,   95,  101,  102,  109,   96,  115,  106,
  103,  108,  114,  119,  118,  120,  117,  121,  122,  124,
  125,  126,  128,   78,  133,  132,   65,  134,  135,  141,
   70,  146,  147,  148,    3,  129,  152,  153,  154,    3,
    3,    3,    3,  131,  150,  139,  137,  149,   12,   11,
   37,   45,   38,   53,   91,  143,  142,  123,  155,  145,
  151,  138,    0,    0,    0,    0,    0,  137,    0,    0,
    3,    0,    0,    0,    0,    0,    0,    0,    3,    0,
    0,    0,    0,   70,   48,    0,    3,   89,    3,    0,
    0,   49,    0,    3,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   78,    3,   98,  112,   78,   78,
};
}
static short yycheck[];
static { yycheck(); }
static void yycheck() {
yycheck = new short[] {                         34,
   34,   34,   34,    0,  123,   34,   34,  125,   41,   41,
    7,   44,   44,   41,   71,  125,   13,  125,  101,  125,
  258,  258,   21,  261,  264,  268,   23,   26,  266,  266,
  259,  271,  257,  258,   31,   40,   41,  120,  258,  257,
   59,  123,  264,  260,  268,  268,  262,  125,  261,  258,
  123,   40,  264,  264,  268,   59,   46,   54,   55,  267,
   40,  264,  125,  123,   41,   34,  123,   64,   40,   66,
  270,  264,   40,  263,   44,  269,   34,   74,   40,   46,
   77,   78,   46,   59,   41,   40,   83,  125,   41,  125,
  264,   34,   46,  264,   41,  268,   93,   34,   44,   59,
  272,   41,  269,   34,  258,  102,   41,   59,   59,  125,
  267,  123,   41,  106,  135,  115,  113,  134,    7,    4,
   23,   31,   24,   40,   66,  123,  123,   93,  154,  124,
  138,  114,   -1,   -1,   -1,   -1,   -1,  134,   -1,   -1,
  258,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  266,   -1,
   -1,   -1,   -1,  272,  264,   -1,  264,  154,  264,   -1,
   -1,  271,   -1,  271,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,  264,  268,  268,  264,  264,
};
}
final static short YYFINAL=3;
final static short YYMAXTOKEN=272;
final static String yyname[] = {
"end-of-file",null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,"'\"'",null,null,null,null,null,"'('","')'",null,null,"','",
null,"'.'",null,null,null,null,null,null,null,null,null,null,null,null,"';'",
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
"'{'",null,"'}'",null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,
null,null,null,null,null,null,null,"IMPORT","PUBLIC","CLASS","EXTENDS",
"SCRIPTSERVICE","VOID","THROWS","IDENTIFIER","UNKNOWN","JAVADOCSTART",
"JAVADOCEND","ANY","NUMBER","THINK","BEGINSTEP","ENDSTEP",
};
final static String yyrule[] = {
"$accept : oats",
"oats : list_import class",
"oats : class",
"epsilon :",
"list_import : import list_import",
"list_import : epsilon",
"import : IMPORT ANY ';'",
"identifier : IDENTIFIER identifier2",
"identifier2 : '.' identifier",
"identifier2 : epsilon",
"number : NUMBER number2",
"number2 : '.' NUMBER",
"number2 : epsilon",
"class : class_declaration '{' body '}'",
"class_declaration : PUBLIC CLASS IDENTIFIER EXTENDS IDENTIFIER",
"body : script_service_list methods",
"body : methods",
"script_service_list : script_service script_service_list",
"script_service_list : epsilon",
"script_service : SCRIPTSERVICE ANY ANY ';'",
"methods : javadoc methods2",
"methods : methods2",
"methods2 : method methods",
"methods2 : epsilon",
"any_javadoc : ANY any_javadoc2",
"any_javadoc2 : any_javadoc",
"any_javadoc2 : epsilon",
"javadoc : JAVADOCSTART any_javadoc JAVADOCEND",
"method : method_declaration '{' method2",
"method2 : block '}'",
"method2 : '}'",
"method_declaration : PUBLIC VOID IDENTIFIER '(' ')' method_declaration2",
"method_declaration2 : THROWS IDENTIFIER",
"method_declaration2 : epsilon",
"block : script_element think block2",
"block : step block2",
"block2 : block",
"block2 : epsilon",
"think : '{' think2 '}'",
"think : epsilon",
"think2 : THINK '(' number ')' ';' think2",
"think2 : epsilon",
"think_script : script_element",
"think_script : epsilon",
"step : begin_step step2",
"step2 : '{' element_sequence '}' close_step",
"step2 : close_step",
"begin_step : BEGINSTEP '(' step_name ',' number ')' ';'",
"step_name : '\"' step_name2",
"step_name2 : ANY '\"'",
"step_name2 : '\"'",
"close_step : ENDSTEP '(' ')' ';'",
"element_sequence : script_element think element_sequence2",
"element_sequence2 : element_sequence",
"element_sequence2 : epsilon",
"element_name : '\"' ANY '\"'",
"action : action2",
"action2 : action3 ')'",
"action2 : ')'",
"action3 : identifier action4",
"action3 : '\"' ANY '\"' action5",
"action3 : '\"' '\"' action5",
"action4 : '(' action2",
"action4 : epsilon",
"action5 : ',' action3",
"action5 : epsilon",
"script_element : protocol disambiguity script_element2 ';'",
"script_element2 : object '.' disambiguity action",
"script_element2 : action",
"element_details : element_name ')'",
"protocol : IDENTIFIER '.'",
"object : ',' element_details",
"disambiguity : IDENTIFIER '(' disambiguity2",
"disambiguity2 : number",
"disambiguity2 : epsilon",
};

//#line 295 "oatsy.y"
	private static Yylex lexer;
	private static UmlModel model;
	private static UmlActivityDiagram acdiagram;
	private static UmlTransition lastAssociation;
	private static UmlActionState lastActivity;
	private static String CurrentStep;
	private static String tdAction;
	private static String tdProtocol;
	private static String currentProtocol;
	private static String currentObject;
	private static String disambiguityValue;
	private static String tdProperties;
	private static int StepPosition;
	private static int NoStepNameCount;

	public static void Initialize()
	{
		NoStepNameCount = 0;
		currentObject = " ";
		currentProtocol = " ";
		tdProperties = " ";

		model = new UmlModel();
		model.setName("script");

		GenerateUseCaseDiagram();

		acdiagram = new UmlActivityDiagram();
		acdiagram.setName("UseCase0");
		UmlInitialState initial = new UmlInitialState();
		initial.setName("InitialNode");
		lastActivity = initial;
		acdiagram.addUmlObject(initial);

		model.addDiagram(acdiagram);
	}

	private static void GenerateUseCaseDiagram()
	{
		UmlUseCaseDiagram ucdiagram = new UmlUseCaseDiagram();
		ucdiagram.setName("UseCase Diagram0");
		UmlActor actor = new UmlActor();
		actor.setName("Actor0");
		actor.addTag("TDHOST", "0");
		UmlUseCase uc = new UmlUseCase();
		uc.setName("UseCase0");
		UmlAssociation assoc = new UmlAssociation();
		assoc.setEnd1(actor);
		assoc.setEnd2(uc);
		ucdiagram.addUmlObject(actor);
		ucdiagram.addUmlObject(uc);
		ucdiagram.addUmlObject(assoc);
		model.addDiagram(ucdiagram);
	}

	private static void CreateActivity(String tagValue)
	{
		if(tagValue.equals(currentObject) && tdProtocol.equals(currentProtocol))
		{
			String currentAction = lastAssociation.getTaggedValues().get("TDACTION");
			lastAssociation.addTag("TDACTION", currentAction + ',' + tdAction);
		}
		else
		{
			currentObject = tagValue;
			currentProtocol = tdProtocol;
			lastAssociation = new UmlTransition();
			lastAssociation.setSource(lastActivity);

			UmlActionState newActivity = new UmlActionState();
			newActivity.setName((CurrentStep==null ? "Step" : CurrentStep) + StepPosition);
			StepPosition++;
			acdiagram.addUmlObject(newActivity);

			lastAssociation.setTarget(newActivity);
			
			lastAssociation.addTag("TDITERATIONS", "0");
			lastAssociation.addTag("TDLOOPCONDITION", "");
			lastAssociation.addTag("TDWAIT", "0");
			lastAssociation.addTag("TDVERIFY", "false");
			lastAssociation.addTag("TDASSERT", "");
			
			lastAssociation.addTag("TDPROTOCOL", tdProtocol);
			lastAssociation.addTag("TDPROPERTIES", tdProperties);
			lastAssociation.addTag("TDOBJECT", tagValue);
			lastAssociation.addTag("TDACTION", tdAction);
			
			acdiagram.addUmlObject(lastAssociation);

			lastActivity = newActivity;
			tdProperties = " ";
		}
	}

	private static void CompleteDiagram()
	{
		lastAssociation = new UmlTransition();
		lastAssociation.setSource(lastActivity);

		UmlFinalState finalState = new UmlFinalState();
		finalState.setName("FinalNode");
		acdiagram.addUmlObject(finalState);
		lastAssociation.setTarget(finalState);
		acdiagram.addUmlObject(lastAssociation);
	}

	private int yylex()
	{
		int retVal = -1;
		
		try 
		{
			yylval = new ParserVal(0); //resets value of token
			retVal = lexer.yylex(); //reads entry and returns token
		} 
		catch (IOException e) 
		{
			System.err.println("IO Error:" + e);
		}
		return retVal; //returns token to Parser
	}

	public void yyerror (String error) 
	{
		System.err.println("\n\nError : " + error + " in line " + lexer.getLine()+"\n\n");
	}

	public Parser (Reader r) 
	{
		lexer = new Yylex(r, this);
	}

	public UmlModel ReadScript(String inputFile) throws IOException
	{
		Parser yyparser;
		Initialize();

		yyparser = new Parser(new FileReader(inputFile));
		yyparser.yyparse();

		return model;
	}

	public static void main(String args[]) throws IOException 
	{
		Parser yyparser;
		Initialize();

	    if ( args.length > 0 ) 
		{
      		// parse a file
      		yyparser = new Parser(new FileReader(args[0]));
    	}
    	else 
		{
      		// interactive mode
      		System.out.println("[Quit with CTRL-D]");
      		System.out.print("Input file:\n");
		  	yyparser = new Parser(new InputStreamReader(System.in));
    	}

    	yyparser.yyparse();

		TestMethod();

		System.out.print("\n\nFeito!\n");
  	}

	public static void TestMethod()
	{
		for(UmlDiagram diagram : model.getDiagrams())
		{
			if(diagram instanceof UmlActivityDiagram)
			{
				for(UmlBase umlObject : diagram.getUmlObjects())
				{
					if(umlObject instanceof UmlTransition)
					{
						HashMap<String, String> tags = umlObject.getTaggedValues();
						String tdObjectP = tags.get("TDOBJECT");
						String tdActionP = tags.get("TDACTION");
						String tdPropertiesP = tags.get("TDPROPERTIES");
						String tdProtocolP = tags.get("TDPROTOCOL");

						System.out.println(tdObjectP + " " + tdActionP + "\n");
						System.out.println(tdProtocolP + " " + tdPropertiesP + "\n\n");
					}
				}
			}
		}
	}
//#line 554 "Parser.java"
//###############################################################
// method: yylexdebug : check lexer state
//###############################################################
void yylexdebug(int state,int ch)
{
String s=null;
  if (ch < 0) ch=0;
  if (ch <= YYMAXTOKEN) //check index bounds
     s = yyname[ch];    //now get it
  if (s==null)
    s = "illegal-symbol";
  debug("state "+state+", reading "+ch+" ("+s+")");
}





//The following are now global, to aid in error reporting
int yyn;       //next next thing to do
int yym;       //
int yystate;   //current parsing state from state table
String yys;    //current token string


//###############################################################
// method: yyparse : parse input and execute indicated items
//###############################################################
int yyparse()
{
boolean doaction;
  init_stacks();
  yynerrs = 0;
  yyerrflag = 0;
  yychar = -1;          //impossible char forces a read
  yystate=0;            //initial state
  state_push(yystate);  //save it
  val_push(yylval);     //save empty value
  while (true) //until parsing is done, either correctly, or w/error
    {
    doaction=true;
    if (yydebug) debug("loop"); 
    //#### NEXT ACTION (from reduction table)
    for (yyn=yydefred[yystate];yyn==0;yyn=yydefred[yystate])
      {
      if (yydebug) debug("yyn:"+yyn+"  state:"+yystate+"  yychar:"+yychar);
      if (yychar < 0)      //we want a char?
        {
        yychar = yylex();  //get next token
        if (yydebug) debug(" next yychar:"+yychar);
        //#### ERROR CHECK ####
        if (yychar < 0)    //it it didn't work/error
          {
          yychar = 0;      //change it to default string (no -1!)
          if (yydebug)
            yylexdebug(yystate,yychar);
          }
        }//yychar<0
      yyn = yysindex[yystate];  //get amount to shift by (shift index)
      if ((yyn != 0) && (yyn += yychar) >= 0 &&
          yyn <= YYTABLESIZE && yycheck[yyn] == yychar)
        {
        if (yydebug)
          debug("state "+yystate+", shifting to state "+yytable[yyn]);
        //#### NEXT STATE ####
        yystate = yytable[yyn];//we are in a new state
        state_push(yystate);   //save it
        val_push(yylval);      //push our lval as the input for next rule
        yychar = -1;           //since we have 'eaten' a token, say we need another
        if (yyerrflag > 0)     //have we recovered an error?
           --yyerrflag;        //give ourselves credit
        doaction=false;        //but don't process yet
        break;   //quit the yyn=0 loop
        }

    yyn = yyrindex[yystate];  //reduce
    if ((yyn !=0 ) && (yyn += yychar) >= 0 &&
            yyn <= YYTABLESIZE && yycheck[yyn] == yychar)
      {   //we reduced!
      if (yydebug) debug("reduce");
      yyn = yytable[yyn];
      doaction=true; //get ready to execute
      break;         //drop down to actions
      }
    else //ERROR RECOVERY
      {
      if (yyerrflag==0)
        {
        yyerror("syntax error");
        yynerrs++;
        }
      if (yyerrflag < 3) //low error count?
        {
        yyerrflag = 3;
        while (true)   //do until break
          {
          if (stateptr<0)   //check for under & overflow here
            {
            yyerror("stack underflow. aborting...");  //note lower case 's'
            return 1;
            }
          yyn = yysindex[state_peek(0)];
          if ((yyn != 0) && (yyn += YYERRCODE) >= 0 &&
                    yyn <= YYTABLESIZE && yycheck[yyn] == YYERRCODE)
            {
            if (yydebug)
              debug("state "+state_peek(0)+", error recovery shifting to state "+yytable[yyn]+" ");
            yystate = yytable[yyn];
            state_push(yystate);
            val_push(yylval);
            doaction=false;
            break;
            }
          else
            {
            if (yydebug)
              debug("error recovery discarding state "+state_peek(0)+" ");
            if (stateptr<0)   //check for under & overflow here
              {
              yyerror("Stack underflow. aborting...");  //capital 'S'
              return 1;
              }
            state_pop();
            val_pop();
            }
          }
        }
      else            //discard this token
        {
        if (yychar == 0)
          return 1; //yyabort
        if (yydebug)
          {
          yys = null;
          if (yychar <= YYMAXTOKEN) yys = yyname[yychar];
          if (yys == null) yys = "illegal-symbol";
          debug("state "+yystate+", error recovery discards token "+yychar+" ("+yys+")");
          }
        yychar = -1;  //read another
        }
      }//end error recovery
    }//yyn=0 loop
    if (!doaction)   //any reason not to proceed?
      continue;      //skip action
    yym = yylen[yyn];          //get count of terminals on rhs
    if (yydebug)
      debug("state "+yystate+", reducing "+yym+" by rule "+yyn+" ("+yyrule[yyn]+")");
    if (yym>0)                 //if count of rhs not 'nil'
      yyval = val_peek(yym-1); //get current semantic value
    yyval = dup_yyval(yyval); //duplicate yyval if ParserVal is used as semantic value
    switch(yyn)
      {
//########## USER-SUPPLIED ACTIONS ##########
case 1:
//#line 30 "oatsy.y"
{
						CompleteDiagram();
					}
break;
case 2:
//#line 34 "oatsy.y"
{
						CompleteDiagram();
					}
break;
case 7:
//#line 49 "oatsy.y"
{
						yyval.sval = val_peek(1).sval + val_peek(0).sval;
					}
break;
case 8:
//#line 55 "oatsy.y"
{
						yyval.sval = "." + val_peek(0).sval;
					}
break;
case 9:
//#line 59 "oatsy.y"
{
						yyval.sval = "";
					}
break;
case 49:
//#line 154 "oatsy.y"
{
							CurrentStep = val_peek(1).sval;
							StepPosition = 1;
						}
break;
case 50:
//#line 159 "oatsy.y"
{
							CurrentStep = "NoStepName" + NoStepNameCount;
							NoStepNameCount++;
							StepPosition = 1;
						}
break;
case 55:
//#line 177 "oatsy.y"
{
							String[] aux = val_peek(1).sval.split("\\.");
							tdProperties = aux[aux.length-2];
							aux = aux[aux.length-1].split("}");
							yyval.sval = aux[0];
						}
break;
case 56:
//#line 189 "oatsy.y"
{
							if(val_peek(0).sval.equals(""))
							{
								tdAction = "{" + disambiguityValue + "}";
							}
							else
							{
								tdAction = "{" + disambiguityValue + ";" + val_peek(0).sval + "}";
							}
						}
break;
case 57:
//#line 202 "oatsy.y"
{
							yyval.sval = val_peek(1).sval;
						}
break;
case 58:
//#line 206 "oatsy.y"
{
							yyval.sval = "";
						}
break;
case 59:
//#line 212 "oatsy.y"
{
							yyval.sval = val_peek(1).sval + val_peek(0).sval;
						}
break;
case 60:
//#line 216 "oatsy.y"
{
							yyval.sval = '"' + val_peek(2).sval + '"' + val_peek(0).sval;
						}
break;
case 61:
//#line 220 "oatsy.y"
{
							yyval.sval = "\"\"" + val_peek(0).sval;
						}
break;
case 62:
//#line 226 "oatsy.y"
{
							yyval.sval = ';' + val_peek(0).sval;
						}
break;
case 63:
//#line 230 "oatsy.y"
{
							yyval.sval = "";
						}
break;
case 64:
//#line 236 "oatsy.y"
{
							yyval.sval = ";" + val_peek(0).sval;
						}
break;
case 65:
//#line 240 "oatsy.y"
{
							yyval.sval = "";
						}
break;
case 67:
//#line 251 "oatsy.y"
{
							CreateActivity("{" + val_peek(3).sval + "}");
						}
break;
case 68:
//#line 255 "oatsy.y"
{
							CreateActivity("{" + tdProtocol + "}");
							tdProtocol = "None";
						}
break;
case 69:
//#line 263 "oatsy.y"
{
							yyval.sval = ";" + val_peek(1).sval;
						}
break;
case 70:
//#line 269 "oatsy.y"
{
							tdProtocol = val_peek(1).sval;
						}
break;
case 71:
//#line 275 "oatsy.y"
{
							yyval.sval = disambiguityValue + val_peek(0).sval;
						}
break;
case 72:
//#line 283 "oatsy.y"
{
							disambiguityValue = val_peek(2).sval;
						}
break;
//#line 861 "Parser.java"
//########## END OF USER-SUPPLIED ACTIONS ##########
    }//switch
    //#### Now let's reduce... ####
    if (yydebug) debug("reduce");
    state_drop(yym);             //we just reduced yylen states
    yystate = state_peek(0);     //get new state
    val_drop(yym);               //corresponding value drop
    yym = yylhs[yyn];            //select next TERMINAL(on lhs)
    if (yystate == 0 && yym == 0)//done? 'rest' state and at first TERMINAL
      {
      if (yydebug) debug("After reduction, shifting from state 0 to state "+YYFINAL+"");
      yystate = YYFINAL;         //explicitly say we're done
      state_push(YYFINAL);       //and save it
      val_push(yyval);           //also save the semantic value of parsing
      if (yychar < 0)            //we want another character?
        {
        yychar = yylex();        //get next character
        if (yychar<0) yychar=0;  //clean, if necessary
        if (yydebug)
          yylexdebug(yystate,yychar);
        }
      if (yychar == 0)          //Good exit (if lex returns 0 ;-)
         break;                 //quit the loop--all DONE
      }//if yystate
    else                        //else not done yet
      {                         //get next state and push, for next yydefred[]
      yyn = yygindex[yym];      //find out where to go
      if ((yyn != 0) && (yyn += yystate) >= 0 &&
            yyn <= YYTABLESIZE && yycheck[yyn] == yystate)
        yystate = yytable[yyn]; //get new state
      else
        yystate = yydgoto[yym]; //else go to new defred
      if (yydebug) debug("after reduction, shifting from state "+state_peek(0)+" to state "+yystate+"");
      state_push(yystate);     //going again, so push state & val...
      val_push(yyval);         //for next action
      }
    }//main loop
  return 0;//yyaccept!!
}
//## end of method parse() ######################################



//## run() --- for Thread #######################################
/**
 * A default run method, used for operating this parser
 * object in the background.  It is intended for extending Thread
 * or implementing Runnable.  Turn off with -Jnorun .
 */
public void run()
{
  yyparse();
}
//## end of method run() ########################################



//## Constructors ###############################################
/**
 * Default constructor.  Turn off with -Jnoconstruct .

 */
public Parser()
{
  //nothing to do
}


/**
 * Create a parser, setting the debug to true or false.
 * @param debugMe true for debugging, false for no debug.
 */
public Parser(boolean debugMe)
{
  yydebug=debugMe;
}
//###############################################################



}
//################### END OF CLASS ##############################
