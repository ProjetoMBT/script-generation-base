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

import java.io.*;
import java.util.*;

import Objects.*;
import Diagrams.*;

//#line 25 "Parser.java"




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
public final static short WEB=273;
public final static short BROWSER=274;
public final static short WINDOW=275;
public final static short TEXTBOX=276;
public final static short BUTTON=277;
public final static short IMAGE=278;
public final static short ALERTDIALOG=279;
public final static short LINK=280;
public final static short ELEMENT=281;
public final static short LAUNCH=282;
public final static short YYERRCODE=256;
final static short yylhs[] = {                           -1,
    0,    0,   10,    8,    8,   11,    3,    4,    4,   12,
   13,   13,    9,   14,   15,   15,   16,   16,   18,   17,
   17,   20,   20,   22,   23,   23,   19,   21,   25,   25,
   24,   27,   27,   26,   26,   30,   30,   29,   29,   31,
   33,   33,   32,   36,   37,   37,   35,   34,   38,   38,
    2,   39,    5,    5,    6,    6,    7,    7,   28,   28,
    1,   40,   40,   40,   40,   40,   40,   40,   42,   43,
   44,   45,   46,   47,   48,   41,   49,
};
final static short yylen[] = {                            2,
    2,    1,    0,    2,    1,    3,    2,    2,    1,    2,
    2,    1,    4,    5,    2,    1,    2,    1,    4,    2,
    1,    2,    1,    2,    1,    1,    3,    3,    2,    1,
    6,    2,    1,    3,    2,    1,    1,    7,    1,    2,
    4,    1,    7,    2,    2,    1,    4,    3,    1,    1,
    3,    3,    2,    1,    2,    3,    2,    1,    3,    3,
    7,    1,    1,    1,    1,    1,    1,    1,    3,    3,
    3,    3,    3,    3,    3,    1,    4,
};
final static short yydefred[] = {                         0,
    0,    0,    0,    0,    2,    5,    0,    0,    0,    0,
    1,    4,    0,    6,    0,    0,    0,    0,   18,    0,
    0,   16,    0,    0,   21,    0,    0,    0,    0,    0,
    0,    0,   13,   23,   15,   18,   17,   20,   22,    0,
   14,    0,    0,   26,   25,   24,   27,    0,    0,    0,
   30,   28,    0,    0,    0,    0,    0,   19,    0,    0,
    0,   29,    0,   39,    0,   37,   36,   35,    0,    0,
   40,   42,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   59,   62,   63,   64,   65,   66,   67,   68,
    0,   60,   76,    0,   34,    0,    0,    0,    0,   33,
   31,    0,   46,   44,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   32,   45,
    0,    0,    0,   69,   70,   71,   72,   73,   74,   75,
    0,    0,   47,   50,   49,   48,   41,    0,   12,   10,
    0,    0,   77,    0,   11,   43,    0,    0,    0,    0,
    0,   38,   51,    0,    0,    0,   61,    0,    7,    9,
    0,    8,   54,    0,    0,   52,    0,    0,    0,   55,
   58,   53,   56,   57,
};
final static short yydgoto[] = {                          3,
  107,  148,  165,  159,  166,  167,  170,    4,    5,   34,
    7,  122,  140,    8,   20,   21,   22,   23,   24,   25,
   26,   32,   46,   27,   52,   67,  101,   54,   65,   68,
   55,   56,   71,   98,   72,   75,  104,  136,  157,   83,
   92,   84,   85,   86,   87,   88,   89,   90,   93,
};
final static short yysindex[] = {                      -212,
 -262, -239,    0, -217,    0,    0, -204,  -69,   -4, -206,
    0,    0, -244,    0, -201, -202, -207, -205,    0,  -63,
 -242,    0, -197, -193,    0, -242,  -57, -196, -195, -198,
 -205, -194,    0,    0,    0,    0,    0,    0,    0, -122,
    0,   31,   13,    0,    0,    0,    0,   34,   30,   32,
    0,    0,  -48,  -44, -231, -112,   39,    0,   47, -249,
 -200,    0, -187,    0, -231,    0,    0,    0,   44, -225,
    0,    0, -178,  -34,   42,   48,   48,   48,   48,   48,
   48,   48,    0,    0,    0,    0,    0,    0,    0,    0,
   49,    0,    0,   50,    0,   46,  -44,  -32, -173,    0,
    0,   58,    0,    0, -175, -175,   36,   37,   38,   41,
   43,   45,   51,   57, -175,   52, -225, -171,    0,    0,
   59,   62,   63,    0,    0,    0,    0,    0,    0,    0,
   53,   65,    0,    0,    0,    0,    0, -161,    0,    0,
   54,   75,    0,   55,    0,    0, -153,   76,   -9,   84,
   74,    0,    0, -143,   78,   82,    0, -143,    0,    0,
  -33,    0,    0, -142,   85,    0,   86,   94,  -33,    0,
    0,    0,    0,    0,
};
final static short yyrindex[] = {                         0,
    0,    0,    0,    0,    0,    0, -129,    0,    0,    0,
    0,    0,    5,    0,    0,    0,    0,    0,    0,    0,
    5,    0, -121,    5,    0,    5,    0,    0,    0,    0,
 -136,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, -118,    5,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    5,    0,    0,    0,    0,    0,
    0,    0,    9,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, -115,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    5,    0,    0,    0,
    3,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   10,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   92,    0,    0,    0,    0,    0,
    0,    0,    0,    0,
};
final static short yygindex[] = {                         0,
  -43,    0, -135,    0,  -35,    0,    0,  128,  132,    2,
    0,  -94,    0,    0,    0,  115,   -8,    0,    0,  116,
    0,  108,    0,    0,    0,  101,    0,  -65,   64,   77,
    0,    0,    0,   26,   28,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,
};
final static int YYTABLESIZE=234;
static short yytable[];
static { yytable();}
static void yytable(){
yytable = new short[]{                        103,
  164,    6,   51,    3,   97,    9,    3,  163,    6,    3,
   70,  123,   35,   16,   19,   16,   17,   39,  156,   10,
  132,   18,  162,   18,   36,   76,   77,   78,   79,   80,
   81,   82,   44,  108,  109,  110,  111,  112,  113,   48,
    2,   49,   50,    3,    1,    2,    3,   49,   50,    3,
    3,   97,    1,   13,   14,   64,   66,   15,   28,   29,
   30,   33,   31,   17,   16,   40,   66,   41,   42,   43,
   57,   58,   47,   59,  100,   60,   62,   61,   63,   73,
   74,   91,   94,   96,   99,  105,  116,  106,  114,  115,
  119,  120,  118,  121,  124,  125,  126,  131,   64,  127,
   69,  128,  141,  129,  138,  144,  142,  145,  147,  130,
  133,  143,  146,  149,  150,  152,  151,  153,  134,  154,
  155,  161,  139,  158,  169,  168,  172,  173,    3,    3,
    3,    3,    3,  174,   12,   11,    3,   37,   45,   38,
   53,   95,  135,    0,    3,  137,    0,    0,   48,    0,
   49,   50,    3,    0,    3,    3,  160,    3,    3,   69,
  117,    0,    0,    0,    0,    0,  171,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  155,    0,    0,  102,
};
}
static short yycheck[];
static { yycheck(); }
static void yycheck() {
yycheck = new short[] {                         34,
   34,    0,  125,  125,   70,  268,  125,   41,    7,  125,
  123,  106,   21,  258,   13,  258,  261,   26,  154,  259,
  115,  266,  158,  266,   23,  275,  276,  277,  278,  279,
  280,  281,   31,   77,   78,   79,   80,   81,   82,  271,
  258,  273,  274,   41,  257,  258,   44,  273,  274,   40,
   41,  117,  257,  123,   59,   54,   55,  264,  260,  262,
  268,  125,  268,  261,  258,  123,   65,  264,  264,  268,
   40,   59,  267,   40,   73,   46,  125,   46,  123,   41,
   34,  282,  270,   40,  263,   44,   41,   40,   40,   40,
  264,   34,  125,  269,   59,   59,   59,   41,   97,   59,
  272,   59,   41,   59,   46,   41,   44,  269,   34,   59,
   59,   59,   59,   59,  268,  125,   41,   34,  117,   46,
  264,   40,  121,   46,   40,  268,   41,   34,  258,  125,
  267,  123,   41,  169,    7,    4,  258,   23,   31,   24,
   40,   65,  117,   -1,  266,  118,   -1,   -1,  271,   -1,
  273,  274,  271,   -1,  273,  274,  155,  273,  274,  272,
   97,   -1,   -1,   -1,   -1,   -1,  165,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  264,   -1,   -1,  268,
};
}
final static short YYFINAL=3;
final static short YYMAXTOKEN=282;
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
"JAVADOCEND","ANY","NUMBER","THINK","BEGINSTEP","ENDSTEP","WEB","BROWSER",
"WINDOW","TEXTBOX","BUTTON","IMAGE","ALERTDIALOG","LINK","ELEMENT","LAUNCH",
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
"think : '{' THINK '(' number ')' ';' '}'",
"think : epsilon",
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
"action : identifier '(' action2",
"action2 : action3 ')'",
"action2 : ')'",
"action3 : identifier action4",
"action3 : '\"' ANY '\"'",
"action4 : '(' action2",
"action4 : epsilon",
"script_element : WEB '.' web",
"script_element : BROWSER '.' browser",
"element_details : '(' number ',' element_name ')' '.' action",
"web : wwindow",
"web : wtextbox",
"web : wbutton",
"web : wimage",
"web : walertdialog",
"web : wlink",
"web : welement",
"wwindow : WINDOW element_details ';'",
"wtextbox : TEXTBOX element_details ';'",
"wbutton : BUTTON element_details ';'",
"wimage : IMAGE element_details ';'",
"walertdialog : ALERTDIALOG element_details ';'",
"wlink : LINK element_details ';'",
"welement : ELEMENT element_details ';'",
"browser : blaunch",
"blaunch : LAUNCH '(' ')' ';'",
};

//#line 288 "oatsy.y"
	private static Yylex lexer;
	private static UmlModel model;
	private static UmlActivityDiagram acdiagram;
	private static UmlTransition lastAssociation;
	private static UmlActionState lastActivity;
	private static String CurrentStep;
	private static String tdAction;
	private static String currentObject;
	private static int StepPosition;
	private static int NoStepNameCount;

	public static void Initialize()
	{
		NoStepNameCount = 0;
		currentObject = " ";

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
		if(tagValue.equals(currentObject))
		{
			String currentAction = lastAssociation.getTaggedValues().get("TDACTION");
			lastAssociation.addTag("TDACTION", currentAction + ',' + tdAction);
		}
		else
		{
			currentObject = tagValue;
			lastAssociation = new UmlTransition();
			lastAssociation.setSource(lastActivity);

			UmlActionState newActivity = new UmlActionState();
			newActivity.setName(CurrentStep + StepPosition);
			StepPosition++;
			acdiagram.addUmlObject(newActivity);

			lastAssociation.setTarget(newActivity);
			lastAssociation.addTag("TDOBJECT", tagValue);
			lastAssociation.addTag("TDACTION", tdAction);
			acdiagram.addUmlObject(lastAssociation);

			lastActivity = newActivity;
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

		//TestMethod();

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

						System.err.println(tdObjectP + " " + tdActionP + "\n\n");
					}
				}
			}
		}
	}
//#line 551 "Parser.java"
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
//#line 26 "oatsy.y"
{
						CompleteDiagram();
					}
break;
case 2:
//#line 30 "oatsy.y"
{
						CompleteDiagram();
					}
break;
case 7:
//#line 45 "oatsy.y"
{
						yyval.sval = val_peek(1).sval + val_peek(0).sval;
					}
break;
case 8:
//#line 51 "oatsy.y"
{
						yyval.sval = "." + val_peek(0).sval;
					}
break;
case 9:
//#line 55 "oatsy.y"
{
						yyval.sval = "";
					}
break;
case 45:
//#line 142 "oatsy.y"
{
							CurrentStep = val_peek(1).sval;
							StepPosition = 1;
						}
break;
case 46:
//#line 147 "oatsy.y"
{
							CurrentStep = "NoStepName" + NoStepNameCount;
							NoStepNameCount++;
							StepPosition = 1;
						}
break;
case 51:
//#line 165 "oatsy.y"
{
							String[] aux = val_peek(1).sval.split("\\.");
							aux = aux[aux.length-1].split("}");
							yyval.sval = aux[0];
						}
break;
case 52:
//#line 176 "oatsy.y"
{
							if(val_peek(0).sval.equals(""))
							{
								tdAction = "{" + val_peek(2).sval + "}";
							}
							else
							{
								tdAction = "{" + val_peek(2).sval + ";" + val_peek(0).sval + "}";
							}
						}
break;
case 53:
//#line 189 "oatsy.y"
{
							yyval.sval = val_peek(1).sval;
						}
break;
case 54:
//#line 193 "oatsy.y"
{
							yyval.sval = "";
						}
break;
case 55:
//#line 199 "oatsy.y"
{
							yyval.sval = val_peek(1).sval + val_peek(0).sval;
						}
break;
case 56:
//#line 203 "oatsy.y"
{
							yyval.sval = '"' + val_peek(1).sval + '"';
						}
break;
case 57:
//#line 209 "oatsy.y"
{
							yyval.sval = ';' + val_peek(0).sval;
						}
break;
case 58:
//#line 213 "oatsy.y"
{
							yyval.sval = "";
						}
break;
case 61:
//#line 225 "oatsy.y"
{
							yyval.sval = val_peek(3).sval;
						}
break;
case 69:
//#line 240 "oatsy.y"
{
							CreateActivity("{window;" + val_peek(1).sval + "}");
						}
break;
case 70:
//#line 246 "oatsy.y"
{
							CreateActivity("{textbox;" + val_peek(1).sval + "}");
						}
break;
case 71:
//#line 252 "oatsy.y"
{
							CreateActivity("{button;" + val_peek(1).sval + "}");
						}
break;
case 72:
//#line 258 "oatsy.y"
{
							CreateActivity("{image;" + val_peek(1).sval + "}");
						}
break;
case 73:
//#line 264 "oatsy.y"
{
							CreateActivity("{alertdialog;" + val_peek(1).sval + "}");
						}
break;
case 74:
//#line 270 "oatsy.y"
{
							CreateActivity("{link;" + val_peek(1).sval + "}");
						}
break;
case 75:
//#line 276 "oatsy.y"
{
							CreateActivity("{element;" + val_peek(1).sval + "}");
						}
break;
//#line 850 "Parser.java"
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
