   M S B A M L   `   `   `  ����  ProductInstantiator&     ProductInstantiator.Instantiator   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5        .��5      �   � RowDefinitions  5	   
   �� 5
      $��24 �6   �� 5      $��* �6   .+�5   
   o� $��0��6   
��� ���62   � 5   
   $��1��6   .+�5   
   �� .+�5      �� $$��CSharp Code Compiler Extension��6   $��	10 10 0 0q�6>   �� 5      -   5        $��checkBoxOptmize��5   Q   $"��Optimize code with /optimize��6   $��	10 10 0 0q�6>   6Q   �� 5      -   5        $��checkBoxLaunch��5   ?   $��Launch instance��6   $
��10 0q�61   6?   �� 5      $!��Create installation package��6   $
��10 0q�6=   {�.  6K   �� 5      $��Delete all but executable��6   $
��10 0q�6;   {�.  6I   �� 5      $
��10 0q�6    �� Orientation$ 
Horizontal=�6'   {�.  .+�5      �� $��Warning level:��6   	� 5      $��1��6   $	��5 0q�6*    	� 	GroupName$ warning_group��67   	� 5      $��2��6   $	��5 0q�6*   $ warning_group��67   	� 5      $��3��6   $	��5 0q�6*   $ warning_group��67   	� 5      $��4��6   $	��5 0q�6*   $ warning_group��67   	� 5      $��5��6   $	��5 0q�6*   $ warning_group��67   5      5      �� 5   
   $��10,105,10,125q�6   .��5      �� -   5        $��
textBoxLog��5      $��Stretch=�6(   $��Stretch=�6D    �� TextWrapping$
 Wrap=�5        �� AcceptsReturn . 
���   �
�������$�� ��5!       �� 
IsReadOnly . 6   5"      �� 5#   
   -   5        $��Bottom=�5#      $
��Left=�6,   $��	10,0,0,50q�6G   $	��100��6Z   $��42��6{   .��5$      �� $��Stretch=�6   $��Stretch=�65   .+�5%      �� $��Load component��6   $��10(�62   $	��Top=�6@   $��0q�6X   $
��Left=�6c   �� 5&      $��metadata��6   $��10(�6,   $��Bottom=�6:   $��0q�6U   5'      5(      �� 5)   
   $��Compiler path��6   $��	10,0,0,98q�6)   $��79��6<   $
��Left=�6G   $��22��6b   $��Bottom=�6n   �� 5*   
   -   5        $��textBoxCompilerPath��5*   A   $��Bottom=�6   $��
90,0,12,98q�6-   6A   �� 5+   
   -   5        $��Bottom=�5+      $
��Left=�6,   $��10,0,0,5q�6G   $	��100��6Y   $��40��6|   .��5,      �� $��Stretch=�6   $��Stretch=�65   .+�5-      �� $��Load product��6   $��10(�60   $	��Top=�6>   $��0q�6V   $
��Left=�6a   �� 5.      $��configuration��6   $��10(�61   $��Bottom=�6?   $��0q�6Z   5/      50      �� 51   
   -   5        $��
textBoxRed��52   /   
���  ��$��No file loaded.��51   %   $��Bottom=�6<   $��
120 0 0 53q�6W   $
��Left=�52      6/   �� 53   
   -	   5        $��textBoxGreen��54   /   
��� � �$��No file loaded.��53   '   $��Bottom=�6>   $��	120,0,0,8q�6Y   $
��Left=�54      6/   �� 55   
   -
   5        $��buttonCompile��55      $��Bottom=�6&   $��Right=�6A   $��Compile��6]   $
��10 5q�56      $	��100��6    6,   57      58      59      