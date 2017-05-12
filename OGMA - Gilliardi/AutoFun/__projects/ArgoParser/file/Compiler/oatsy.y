%{
//v1.0.2

import java.io.*;
import java.util.*;

import Objects.*;
import Diagrams.*;

%}

%token IMPORT PUBLIC CLASS EXTENDS SCRIPTSERVICE VOID THROWS IDENTIFIER UNKNOWN JAVADOCSTART JAVADOCEND ANY NUMBER THINK BEGINSTEP ENDSTEP

%type <sval> element_details
%type <sval> element_name
%type <sval> protocol
%type <sval> object
%type <sval> ANY
%type <sval> identifier
%type <sval> identifier2
%type <sval> IDENTIFIER
%type <sval> action2
%type <sval> action3
%type <sval> action4
%type <sval> action5

%%

oats:	 		list_import class
					{
						CompleteDiagram();
					}
					| class
					{
						CompleteDiagram();
					}
					;

epsilon:		;

list_import: 		import list_import
				| epsilon
				;

import:			IMPORT ANY ';'
				;

identifier:		IDENTIFIER identifier2
					{
						$$ = $1 + $2;
					}
					;

identifier2:	'.' identifier
					{
						$$ = "." + $2;
					}
					| epsilon
					{
						$$ = "";
					}
					;

number:			NUMBER number2
				;

number2:		'.' NUMBER
				| epsilon
				;

class:				class_declaration '{' body '}'
						;

class_declaration:	PUBLIC CLASS IDENTIFIER EXTENDS IDENTIFIER
						;

body:				script_service_list methods
						| methods
						;

script_service_list:	script_service script_service_list
						| epsilon
						;

script_service:		SCRIPTSERVICE ANY ANY ';'
						;

methods:			javadoc methods2
						| methods2
						;

methods2:			method methods
						| epsilon
						;

any_javadoc:		ANY any_javadoc2
						;

any_javadoc2:		any_javadoc
						| epsilon
						;

javadoc:			JAVADOCSTART any_javadoc JAVADOCEND
						;

method:				method_declaration '{' method2
						;

method2:			block '}'
						| '}'
						;

method_declaration:	PUBLIC VOID IDENTIFIER '(' ')' method_declaration2
						;

method_declaration2:	THROWS IDENTIFIER
							| epsilon
							;

block:				script_element think block2
						| step block2
						;

block2:				block
						| epsilon
						;

think:				'{' think2 '}'
						| epsilon
						;
						
think2: 			THINK '(' number ')' ';' think2
						| epsilon
						;

think_script:		script_element
					| epsilon
					;

step:				begin_step step2
						;

step2:				'{' element_sequence '}' close_step
						| close_step
						;

begin_step:			BEGINSTEP '(' step_name ',' number ')' ';'
						;

step_name:			'"' step_name2
						;

step_name2:			ANY '"'
						{
							CurrentStep = $1;
							StepPosition = 1;
						}
						| '"'
						{
							CurrentStep = "NoStepName" + NoStepNameCount;
							NoStepNameCount++;
							StepPosition = 1;
						}
						;

close_step:			ENDSTEP '(' ')' ';'
						;

element_sequence:	script_element think element_sequence2
						;

element_sequence2:	element_sequence
						| epsilon
						;

element_name:		'"' ANY '"'
						{
							String[] aux = $2.split("\\.");
							tdProperties = aux[aux.length-2];
							aux = aux[aux.length-1].split("}");
							$$ = aux[0];
						}
						;

// ACTION BLOCK
// DOCUMENTATION: TODO

action:				action2
						{
							if($1.equals(""))
							{
								tdAction = "{" + disambiguityValue + "}";
							}
							else
							{
								tdAction = "{" + disambiguityValue + ";" + $1 + "}";
							}
						}
						;

action2:			action3 ')'
						{
							$$ = $1;
						}
						| ')'
						{
							$$ = "";
						}
						;

action3:			identifier action4
						{
							$$ = $1 + $2;
						}
						| '"' ANY '"' action5
						{
							$$ = '"' + $2 + '"' + $4;
						}
						| '"' '"' action5
						{
							$$ = "\"\"" + $3;
						}
						;

action4:			'(' action2
						{
							$$ = ';' + $2;
						}
						| epsilon
						{
							$$ = "";
						}
						;

action5:			',' action3
						{
							$$ = ";" + $2;
						}
						| epsilon
						{
							$$ = "";
						}
						;

// ACTION BLOCK

script_element:		protocol disambiguity script_element2 ';'
						;

script_element2:	object '.' disambiguity action
						{
							CreateActivity("{" + $1 + "}");
						}
						| action
						{
							CreateActivity("{" + tdProtocol + "}");
							tdProtocol = "None";
						}
						;
						

element_details:	element_name ')'
						{
							$$ = ";" + $1;
						}
						;

protocol:			IDENTIFIER '.'
						{
							tdProtocol = $1;
						}
						;

object:				',' element_details
						{
							$$ = disambiguityValue + $2;
						}
						;

// OBJECT/ACTION DISAMBIGUITY

disambiguity:		IDENTIFIER '(' disambiguity2
						{
							disambiguityValue = $1;
						}
						;

disambiguity2:		number
						| epsilon
						;

// OBJECT/ACTION DISAMBIGUITY

%%
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
