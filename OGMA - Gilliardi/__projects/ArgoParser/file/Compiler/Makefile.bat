echo: CREATING COMPILER
	java -jar JFlex.jar oatsl.flex
	yacc -tv -J oatsy.y
	javac Parser.java
echo: CLEAN
	del *.class y.output *~
