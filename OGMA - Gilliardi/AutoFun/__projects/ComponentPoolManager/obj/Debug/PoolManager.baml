   M S B A M L   `   `   `  ����O  KComponentPoolManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:ComponentPoolManagerComponentPoolManager  5      &     ComponentPoolManager.PoolManager   )my"clr-namespace:ComponentPoolManager  \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5        ���	#FF212020.��5      �   � RowDefinitions  5	   
   �� 5
      $��24 �6   �� 5      $��30 �6   �� 5      $��* �6   �� 5      $��24 �6    � ColumnDefinitions 5   
   �� 5      $	��250 �6   $	��200��6+   �� 5      $��* �6   $	��200��6)   .+�5   
   o� $��0��6   
���**��$��2��6/   6E   �� 5   
   $��1��6   $��2��6#    �� Orientation$ 
Horizontal=�67   
�������.+�5      c� .@�5      �� -   5         ��Name$ buttonOpenConfig��5       � ToolTip$) #Loads pool configuration from file.��6R   .��5      �� $7��1/ComponentPoolManager;component/Images/folder.gif��6   5      �� 5      -   5        $ buttonSaveConfig��5      $' !Saves pool configuration to file.��6R   .��5      �� $5��//ComponentPoolManager;component/Images/save.gif��6   {�.  6U   5      �� 5      -   5        $ buttonLoadDanu��5      $: 4Import code information from an existing Bragi file.��61   .��5      �� $9��3/ComponentPoolManager;component/Images/arrow_up.gif��6   5      5      c� 5       .@�5!      �� -   5        $ buttonUpdateHandler��5!      $A ;Use selected code generator to setup initial code template.��66   .��5"      �� $7��1/ComponentPoolManager;component/Images/closed.gif��6   5#      5'      c� 5(      .@�5)      � $��2 0 5 0q�6   .+�5*      �� $
��Code��6    $��11(�6,   �� 5+      $��	Generator��6    $��0 10 0 0q�61   $��11(�6C   5,      �� 5-      -   5        $��comboBoxCodeHandlers��5-      $	��300��67   5.      c� 5/      .@�50      d� -   5        $��buttonInformation��50      .��51      �� $<��6/ComponentPoolManager;component/Images/information.gif��6   52      � 53      -   5         �Name$ popup��53       � VerticalOffset$ 0��54       � HorizontalOffset$ 5��6+   $��Right=�6@   $��Scroll=�6R    � AllowsTransparency . 	 � 	StaysOpen	 .  
 � PlacementTarget
 53   '   ��  �� ElementName$ buttonInformation����56      �� $ buttonInformation�� �� Path$ 	IsChecked� �� Mode$ TwoWay=�.��57      �� 
�������
���**��$��1q�6D    �� Padding$ 35q�6X   ._�58      �� $ Vertical=�6%   .+�59      �� $��3q�6(   
���**��$0��*1. Open a project or import a plugcd file;��6F   �� 5:      $��3q�6(   
���**��$5��/2. Add methods and parameters to each interface��6F   �� 5;      $��3q�6(   
���**��$"��3. Save it as *.plugcm file.��6F   5<      5=      5>      5?      5@      �� 5B   
   $��2��6   $��0��6$   $��0 0 1 0q�64   .��5C      �� -	   5        $��stackPanelComponents��5C   7   $��Stretch=�6   $��Stretch=�5D      
�������.+�5E      �� $��Import components from a��6    �� TextAlignment$ Center=�6<   $��0 15 0 0q�6S   
���iii�6e   �� 5F      $��SMarty UML diagram file��6   $ Center=�6;   
���iii�6R   �� 5G      $��or load an existing��6   $ Center=�67   
���iii�6N   �� 5H      $��component pool��6   $ Center=�62   
���iii�6I   �� 5I      $��configuration file.��6   $ Center=�67   
���iii�6N   5J      5K      �� 5L   
   $��0��6   $��2��6'   $��Stretch=�64   $��Right=�6P   $��1��5M      
���**��6!   �� 5N   
   $��1��6   $��2��6'   $��Stretch=�64   $
��Left=�6P   $��1��5O      
���**��6!   � 5Q   
   -
   5         �Name$ interfaceEditorContainer��5Q   ,   $��2��6   $��1��6   
�������$��1 0 0 0q�6a   �� 5R   
   -   5        $��	statusBar��5R      $��3��6%   
���**��
�������$��2��6X   @�5S      �� 5T      -   5        $��textBlockStatus��5T   <   $��Ready��6   
�������$��3 0 0 0q�6S   5V      5W      5X      