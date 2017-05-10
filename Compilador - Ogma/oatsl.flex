%{
#include "y.tab.h"
#include <stdio.h>
#include <string.h>
%}

%s JAVADOCSTATE STRINGSTATE IMPORTSTATE

%%

<INITIAL>import 		{ BEGIN IMPORTSTATE; return IMPORT; }
<INITIAL>public 		{ return PUBLIC; }
<INITIAL>class 			{ return CLASS; }
<INITIAL>extends		{ return EXTENDS; }
<INITIAL>"@ScriptService"	{ BEGIN IMPORTSTATE; return SCRIPTSERVICE; }
<INITIAL>void			{ return VOID; }
<INITIAL>throws			{ return THROWS; }
<INITIAL>think			{ return THINK; }
<INITIAL>beginStep		{ return BEGINSTEP; }
<INITIAL>endStep		{ return ENDSTEP; }
<INITIAL>web			{ return WEB; }
<INITIAL>browser		{ return BROWSER; }
<INITIAL>window			{ return WINDOW; }
<INITIAL>textBox		{ return TEXTBOX; }
<INITIAL>button			{ return BUTTON; }
<INITIAL>image			{ return IMAGE; }
<INITIAL>alertDialog		{ return ALERTDIALOG; }
<INITIAL>link			{ return LINK; }
<INITIAL>element		{ return ELEMENT; }
<INITIAL>launch			{ return LAUNCH; }


<INITIAL>"." |
<INITIAL>"," |
<INITIAL>";" |
<INITIAL>"{" |
<INITIAL>"}" |
<INITIAL>"(" |
<INITIAL>")" |
<INITIAL>"*" 			{ return (int) yytext[0]; }

<INITIAL>[0-9]+			{ return NUMBER; }
<INITIAL>[A-Za-z0-9]+ 		{ return IDENTIFIER; }

<INITIAL>"/\*\*"		{ BEGIN JAVADOCSTATE; return JAVADOCSTART; }
<INITIAL>\"			{ BEGIN STRINGSTATE; return (int) yytext[0]; }

<INITIAL>[ \t\n\r]+		;
<INITIAL>.			{ return UNKNOWN; }



<JAVADOCSTATE>[ \t\n\r]+	;
<JAVADOCSTATE>"\*/"		{ BEGIN INITIAL; return JAVADOCEND; }
<JAVADOCSTATE>.			{ return ANY; }



<STRINGSTATE>[ \t\n\r]+		;
<STRINGSTATE>\"			{ BEGIN INITIAL; return (int) yytext[0]; }
<STRINGSTATE>[^\"]*		{ return ANY; }



<IMPORTSTATE>[ \t\n\r]+		;
<IMPORTSTATE>";"		{ BEGIN INITIAL; return (int) yytext[0]; }
<IMPORTSTATE>[^; \t\n\r]*	{ return ANY; }

%%
