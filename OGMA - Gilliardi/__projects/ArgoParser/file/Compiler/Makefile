all:
	java -jar JFlex.jar oatsl.flex
	byaccj -tv -J oatsy.y
	sudo javac Parser.java
clean:
	rm -f *.class *.s *.o *~ Parser.java ParserVal.java Yylex.java y.output
