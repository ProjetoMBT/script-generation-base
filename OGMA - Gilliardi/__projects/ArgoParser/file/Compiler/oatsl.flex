%%

%line
%char
%state JAVADOCSTATE STRINGSTATE IMPORTSTATE
%full
%byaccj

%{
	private Parser yyparser;

	public Yylex (java.io.Reader r, Parser yyparser)
	{
		this(r);
		this.yyparser = yyparser;
		yyline = 1;
	}

	public int getLine()
	{
		return yyline;
	}
%}

WHITE_SPACE=[\ \t\b\012]
NEWLINE=\r|\n|\r\n

%%

<YYINITIAL>import 		{ yybegin(IMPORTSTATE); return Parser.IMPORT; }
<YYINITIAL>public 		{ return Parser.PUBLIC; }
<YYINITIAL>class 		{ return Parser.CLASS; }
<YYINITIAL>extends		{ return Parser.EXTENDS; }
<YYINITIAL>"@ScriptService"	{ yybegin(IMPORTSTATE); return Parser.SCRIPTSERVICE; }
<YYINITIAL>void			{ return Parser.VOID; }
<YYINITIAL>throws		{ return Parser.THROWS; }
<YYINITIAL>think		{ return Parser.THINK; }
<YYINITIAL>beginStep		{ return Parser.BEGINSTEP; }
<YYINITIAL>endStep		{ return Parser.ENDSTEP; }


<YYINITIAL>"." |
<YYINITIAL>"," |
<YYINITIAL>";" |
<YYINITIAL>"{" |
<YYINITIAL>"}" |
<YYINITIAL>"(" |
<YYINITIAL>")" |
<YYINITIAL>"*" 			{ return (int) yycharat(0); }

<YYINITIAL>[0-9]+		{ return Parser.NUMBER; }
<YYINITIAL>[A-Za-z0-9]+ 	{ yyparser.yylval = new ParserVal(yytext());
				  return Parser.IDENTIFIER; }

<YYINITIAL>"/\*\*"		{ yybegin(JAVADOCSTATE); return Parser.JAVADOCSTART; }
<YYINITIAL>\"			{ yybegin(STRINGSTATE); return (int) yycharat(0); }

<YYINITIAL>{WHITE_SPACE}+	{ }
<YYINITIAL>{NEWLINE}		{ }
<YYINITIAL>.			{ return Parser.UNKNOWN; }



<JAVADOCSTATE>{WHITE_SPACE}+	{ }
<JAVADOCSTATE>{NEWLINE}		{ }
<JAVADOCSTATE>"\*/"		{ yybegin(YYINITIAL); return Parser.JAVADOCEND; }
<JAVADOCSTATE>.			{ yyparser.yylval = new ParserVal(yytext());
						  return Parser.ANY; }



<STRINGSTATE>{WHITE_SPACE}+	{ }
<STRINGSTATE>{NEWLINE}		{ }
<STRINGSTATE>\"			{ yybegin(YYINITIAL); return (int) yycharat(0); }
<STRINGSTATE>[^\"]*		{ yyparser.yylval = new ParserVal(yytext());
						  return Parser.ANY; }



<IMPORTSTATE>{WHITE_SPACE}+	{ }
<IMPORTSTATE>{NEWLINE}		{ }
<IMPORTSTATE>";"		{ yybegin(YYINITIAL); return (int) yycharat(0); }
<IMPORTSTATE>[^;\ \t\b\012\r\n]*	{ yyparser.yylval = new ParserVal(yytext());
								 	  return Parser.ANY; }
