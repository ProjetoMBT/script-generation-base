   M S B A M L   `   `   `  ����   TestingTool    Coc.Apps.PLeTs.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  	PLeTsFunc��$	��419��$	��800�� � Icon$9 3/TestingTool;component/Images/pie-chart_diagram.ico�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ CanMinimize=�.��� $	��373�� � DataContext ��  � RowDefinitions �� $��24 ��� $��0 ��� $
��394* ��� $��0 �.+�o� ���#eee.@�m� $��_File��.@�m� -   $��buttonParseLRtoXMI��$$��Parse LoadRunner Script to XMI��m� -   $��buttonXmiExport��$��
Export Xml��m� -   $��buttonClose��$��_Close��m� $��_Configuration��.@�m� -   $��buttonConfigure��$��_Edit Configuration File��m� -   $��buttonOption��$��_Options��m� $
��_Log��.@�m� -   $��buttonSaveLog��$��_Save log to file...��m� -   $��buttonClearLog��$��_Clear��m� $��_Help��.@�m� -	   $��
buttonHelp��$��_Help���� $��2��$��213,6,12,12q�.@��� $��Status��.���� -
   $��textBlockLogContainer��$��5q� �� 
IsReadOnly . $��Consolas)�" �� VerticalScrollBarVisibility$
 Auto=�$	��299��$	��546�� � 	Resources 	 �� 
TargetType
 �� Property �� Value��%3   'J�       �� "	 J�.��� 
 �@��Margin$ 0q��� $��3�����#eee.@��� .��� -   $��progressStatus��$	��100��$��16��{�.  �� .���� -   $��textBlockStatusContainer��$X��RThis is a status caption. Use MainWindow.SetStatus(String p) to update this value.���� -   $��SequenceGeneratorType��$��2��$��35��$
��Left=�$��	20,41,0,0q� � ToolTip$ Choose the method��$	��Top=�$	��175��$��Center=����� .@��� $��Choose the method�� �� 
IsSelected . $
��Left=�$��Center=��� -   $��Parser��$��2��$��35��$
��Left=�$��	20,96,0,0q�$ Choose the type of import��$	��Top=�$	��175��{�.  $��Center=�.@��� $��Choose the type of import�� . $
��Left=�$��Center=��� -   $��GenerateFile��$��2��$��35��$
��Left=�$��
20,252,0,0q�$' !choose the file generation method��$	��Top=�$	��175�� �� Text$ Generate��$��Center=�$ �� IsSynchronizedWithCurrentItem Z�  ��  �� ElementName$ GenerateFile�� �� StringFormat$ Generate File��.@��� $��Select type to export�� . $
��Left=�$��Center=��� -   $��buttonLoadData��$��	Load File��$ Load file to be parsed...��$��2��$��35��$
��Left=�$��
20,147,0,0q�$	��Top=�$	��175��$
��Left=��� -   $��buttonGenerateTestCases��$��Generate Test Cases��$0 *Generate test cases from loaded test data.��{�.  $
��Left=�$	��Top=�$��
20,199,0,0q�$��2��$��35��$	��175��$
��Left=�