   M S B A M L   `   `   `  ����   TestingTool#    Coc.Apps.PLeTs.MainWindowOATS   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  	PLeTsFunc��$	��419��$	��800�� � Icon$9 3/TestingTool;component/Images/pie-chart_diagram.ico�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ CanMinimize=�.��]� .+�o� $	��Top=����#eee.@�m� $��_File��.@�m� -   $��buttonClose��$��_Close��m� $��_Configuration��.@�m� -   $��buttonOption��$��_Options...��m� $
��_Log��.@�m� -   $��buttonSaveLog��$��_Save log to file...��m� -   $��buttonClearLog��$��_Clear��m� $��_Help��.@�m� -   $��
buttonHelp��$��_Help��`� $	��Top=�.�c� .@��� -   $��btnOpenOATS�� � ToolTip$ 	Open OATS��.����  �� Orientation$ 
Horizontal=�.+��� $ ��/Images/OpenScript_401.ico��$��16��$��16���� �� -   $��btnOpenAstah��$ 
Open Astah��.���� $ 
Horizontal=�.+��� $��/Images/astah.ico��$��16��$��16���� �� -   $��btnOpenExcel��$ 
Open Excel��.���� $ 
Horizontal=�.+��� $��/Images/excel.jpg��$��16��$��16���� � $
��Left=�$	��773��$	��330�� � RowDefinitions �� $��* ��� $
��329* �.+��� -	   $��btnImportJava��$ Import java��$��23��$
��Left=�$��8,7,0,0q�$	��Top=�$��75��$��1��.���� $ 
Horizontal=�.+��� -
   $��/Images/cup--pencil.png���� �� -   $��btnImportAstahXmi��$ Import Astah XMI��$��23��$
��Left=�$��89,7,0,0q�$	��Top=�$��75��$��1��$y�Hidden=�.���� $ 
Horizontal=�.+��� $��/Images/astah-doc.ico���� �� -   $��buttonGenerateTestCases��$��Generate Test Cases��$0 *Generate test cases from loaded test data.��{�.  $
��Left=�$	��Top=�$��
20,233,0,0q�$��35��$	��175��$
��Left=��� -   $��cmbExportType��$��1��$��23��$
��Left=�$��
536,10,0,0q�$	��Top=�$	��150��$y�Hidden=�.@��� .���� $ 
Horizontal=�$
��Left=�.+��� $��/Images/document-code.png��$��16��$��16���� Export to XMI�� .���� $ 
Horizontal=�.+��� $��/Images/table-excel.png��$��16��$��16���� Export to Excel�� .���� $ 
Horizontal=�.+��� $ ��/Images/OpenScript_401.ico��$��16��$��16���� XMI to OATS�� -   $��label1��$��Export type��$��1��$��26��$
��Left=�$��	460,9,0,0q�$	��Top=�$y�Hidden=��� -   $��	btnExport��$��Export��$��1��$��23��$
��Left=�$��
692,10,0,0q�$	��Top=�$��75��$y�Hidden=��� $��1��$��	8,94,6,40q�.@��� $��Status��.���� -   $��textBlockLogContainer��$��5q� �� 
IsReadOnly . $��Consolas)�" �� VerticalScrollBarVisibility$
 Auto=�$	��159��$	��736��	 � 	Resources	 
 �� 
TargetType �� Property �� Value��%3   'J�       �� "
 J�.���  �@��Margin$ 0q��� ���#eee$��	0,307,0,0q�$��1��.@��� .��� -   $��progressStatus��$	��100��$��16��{�.  �� .���� -   $��textBlockStatusContainer��$\��VThis is a status caption. Use MainWindowOATS.SetStatus(String p) to update this value.���� -   $��btnImportAstahXMI��$��1��$��23��$
��Left=�$��	101,7,0,0q�$	��Top=�$	��100��.���� $ 
Horizontal=�.+��� $ ��/Images/OpenScript_401.ico��$��16��$��16���� XMI to OATS