   M S B A M L   `   `   `  ����M  IFeatureModelEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:FeatureModelEditorFeatureModelEditor  5   "   %    FeatureModelEditor.FeatureModel   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Ad3http://schemas.microsoft.c om/expression/blend/2008     5      /my clr-namespace:FeatureModelEditor     5   "   .��5      �   � RowDefinitions  5	   
   �� 5
      $��22 �6   �� 5      $��28 �6   �� 5      $��* �6   �� 5      $��22 �6    � ColumnDefinitions 5   
   �� 5      $��* �6   $��20��6)   �� 5      $	��320 �6   $��20��6+   .+�5   
   o� $��0��6   ���#73A3C4$��2��61   .@�5      m� 
�������$
��File��6*   .@�5      m� -   5        
���   �$
��_New��5   .    m� Icon 5      �� 5      $<��6/FeatureModelEditor;component/Images/document_file.gif��6    $��16��6`   $��16��6k   5      m� 5      -   5        
���   �$��_Open��5   .    5      �� 5      $5��//FeatureModelEditor;component/Images/folder.gif��6    $��16��6Y   $��16��6d   5      m� 5      -   5        
���   �$��_Save as...��5   .    5       �� 5!      $6��0/FeatureModelEditor;component/Images/save_as.gif��6    $��16��6Z   $��16��6e   5#      m� 5$      -   5        
���   �$��	_Print...��5$   .    5%      �� 5&      $6��0/FeatureModelEditor;component/Images/printer.gif��6    $��16��6Z   $��16��6e   5(      5)      5*      �� 5+   
    �� Orientation$ 
Horizontal=�6   
�������$��1��6A   $��2��6N   .+�5,      c� .@�5-      �� -   5        $��buttonClear��5-       � ToolTip$; 5Creates a new document. Unsaved changes will be lost.��5.      $��0q�6Y   .��5/      �� $<��6/FeatureModelEditor;component/Images/document_file.gif��6   50      �� 51      -   5        $��
buttonOpen��51      $% Load a feature model from file.��52      $��0q�6I   .��53      �� $5��//FeatureModelEditor;component/Images/folder.gif��6   54      �� 55      -   5        $��buttonSaveAs��55      $&  Save current feature model as...��56      $��0q�6E   .��57      �� $6��0/FeatureModelEditor;component/Images/save_as.gif��6   58      �� 59      -   5        $��
buttonSave��59      $) #Save current feature model to file.��5:      $��0q�6G   .��5;      �� $3��-/FeatureModelEditor;component/Images/save.png��6   5<      �� 5>      -	   5        $��buttonPrint��5>      $7 1Print current feature model in current grid mode.��5?      $��0q�6V   .��5@      �� $6��0/FeatureModelEditor;component/Images/printer.gif��6   5A      5B      c� 5C      .@�5D      d� -
   5        $��buttonAddFeature��5D      $ Adds a feature to model.��5E      $��0q�6B   .��5F      �� $2��,/FeatureModelEditor;component/Images/add.gif��6   5G      �� 5H      -   5        $��buttonRemoveFeature��5H      $# Removes a feature from model.��5I      $��0q�6B   .��5J      �� $5��//FeatureModelEditor;component/Images/eraser.gif��6   5K      �� 5L      -   5        $��buttonAddChildFeature��5L      $3 -Connect  a feature-child relation from model.��5M      $��0q�6R   .��5N      �� $3��-/FeatureModelEditor;component/Images/lock.gif��6   5O      �� 5P      -   5        $��buttonRemoveChildFeature��5P      $2 ,Removes a feature-child relation from model.��5Q      $��0q�6Q   .��5R      �� $5��//FeatureModelEditor;component/Images/unlock.gif��6   5S      �� 5T      	� 5U      -   5        $��radioButtonMandatory��5U   6    	� 	GroupName$ featureType��6   $- 'Select to set feature type to Mandatory��6y   .��5V      A� $��12��6   $��12��6*   
���   �
���   �$
��Left=�6Q   5W      	� 5X      -   5        $��radioButtonOptional��5X   6   $ featureType��6   $+ %Select to set feature type to Optinal��6w   .��5Y      A� $��12��6   $��12��6*   
�������
���   �$
��Left=�6Q   5Z      	� 5[      -   5        $��radionButtonOr��5[   6   $ featureType��6   $&  Select to set feature type to Or��6m   .��5\      �  � Points% �    �  �  � ��[  � 
���   �$��1��6J   
���   �6^   5]      	� 5^      -   5        $��radioButtonAlternative��5^   6   $ featureType��6   $/ )Select to set feature type to Alternative��6}   .��5_      � % �    �  �  � ��[  � 
���   �$��1��6J   
�������6^   5`      �� 5a      d� 5b      -   5        $��buttonAbstract��5b      $0 *Click to set selected feature to Abstract.��6R   .��5c      �� $��A��6    $��17��6)   $��17��64    �� TextAlignment$ Center=�6@   $��12(�6W   5d      �� 5e      -   5        $��textBoxSelected��5e   &   $��18��6   $	��120��6=   
���  ��$��5 0 0 0q�5f       �� 	MaxLength$ 25��6'   $&  Edit the selected feature's name��6X   5g      c� 5h      .@�5i      d� -   5        $��
buttonGrid��5i      $! Enables/Disabled grid mode.��61   	 d� 	IsChecked$
	 TrueY�6p   .��5j      �� $8��2/FeatureModelEditor;component/Images/options_2.gif��6   5k      d� 5l      -   5        $��buttonSnapToGrid��5l      $) #Enables/Disables snap-to-grid mode.��67   $
	 TrueY�6�   .��5m      �� $=��7/FeatureModelEditor;component/Images/sound_music-on.gif��6   5n      d� 5o      -   5        $��
buttonHelp��5o      $ 	Show help��61   .��5p      �� $:��4/FeatureModelEditor;component/Images/information.gif��6   5q      �� 5r      -   5        $��buttonPerformLayout��5r      $# Perform layout of the diagram��6V   .��5s      �� $5��//FeatureModelEditor;component/Images/wizard.gif��6   5t      5x      5y      �� 5{   
   $��2��6   $��Stretch=�6$   $��Stretch=�6@   $��Visible=�6^   $��Visible=�5|      $��0��6?   ���#eeeeee.��5}      � .+�5~      �� $��5 5 0 0q�6   $��10(�6-   $	��Top=�6;   $
��Left=�6S   $��0.12 Inches²��6n   �� 5      $��0 20 0 0q�6   $��1q�6+   
�������._�5�      � -   5        $��gridEditorContainer��5�      |�. 
 J� 	AllowDrop
 . ��5�      �� 5�       �� ImageSource$3 -/FeatureModelEditor;component/Images/grid.png��6)    j� Stretch$
 None=�6e    j� TileMode$
 Tile=�6t    j� Viewport$+ %0,0,24.0033497716744,24.0033497716744�6�    j� ViewportUnits$ Absolute=�6�   ��5�      �� 5�       �� 
TargetType" �6$   .�5�   "   ��  �� Property �@�FocusVisualStyle �� Value 6E   Z� 5�      .+�5�      �� -   5        $��rectanglePlaceholder��5�   P   $	��135��5�   $   $��35��60   
����  �
����d� J� Opacity$
 0.25Q�6`    J� IsHitTestVisible .  $	��Top=�5�      $
��Left=�65   6P   5�      5�      5�      5�      �� 5�   
   $��2��6   $��0��6$   $��1��64   ���#73A3C46>   �� 5�   
   $��2��6   $��1��6$   $��1��64   ���#73A3C4$
��Left=�6S   � 5�   
   -   5        $��gridContraintEditor��5�   ,   $��2��6   $��1��6   .+�5�      �� $$��No contraint editor available.��6   
�������$��Center=�5�      $��Center=�63   5�      � 5�   
   -   5         �Name$ popup��5�       � VerticalOffset$ 0��5�       � HorizontalOffset$ 5��6+   $��Right=�6@   $��Scroll=�6R    � AllowsTransparency .  � 	StaysOpen .   � PlacementTarget 5�      ��  �� ElementName$ 
buttonHelp����5�      �� $ 
buttonHelp�� �� Path$ 	IsChecked� �� Mode$ TwoWay=�.��5�      �� 
�������
����  �$��1q�6;    �� Padding$ 35q�6O   ._�5�      �� $ 
Horizontal=�6   .+�5�      �� $ Vertical=�6!   .+�5�      �� $��3q�6$   
����  �$ Center=�6A   $��Legend��6X   � 5�      $
��Left=�6   .+�5�      � $��12��6&   $��12��62   
����d�$
��Left=�6S   # �    �  �  �  � 6n   �� 5�      $��Concrete��6(   $��15 0 0 0q�68   $��10(�6J   $
��Left=�6X   5�      � 5�      $
��Left=�6   .+�5�      � $��12��6&   $��12��62   
�����_�$
��Left=�6N   # �    �  �  �  � 6i   �� 5�      $��Abstract��6(   $��15 0 0 0q�68   $��10(�6J   $
��Left=�6X   5�      � 5�      $
��Left=�6   .+�5�      A� $��12��6&   $��12��62   
���   �
���   �$
��Left=�6Y   �� 5�      $��	Mandatory��6(   $��15 0 0 0q�69   $��10(�6K   $
��Left=�6Y   5�      � 5�      $
��Left=�6   .+�5�      A� $��12��6&   $��12��62   
�������
���   �$
��Left=�6Y   �� 5�      $��Optional��6(   $��15 0 0 0q�68   $��10(�6K   $
��Left=�6Y   5�      � 5�      $
��Left=�6   .+�5�      � % �    �  �  � ��[  � 
���   �$��1��6R   
���   �6f   �� 5�      $��Or��6(   $��15 0 0 0q�62   $��10(�6E   $
��Left=�6S   5�      � 5�      $
��Left=�6   .+�5�      � % �    �  �  � ��[  � 
���   �$��1��6R   
�������6f   �� 5�      $��Alternative��6(   $��15 0 0 0q�6;   $��10(�6N   $
��Left=�6\   5�      5�      �� 5�      $ Vertical=�6!   $��20 0 0 0q�68   .+�5�      �� $��3q�6$   
����  �$ Center=�6A   $��Shortcut��6X   � 5�      $
��Left=�6   .+�5�      �� $��A��6(   $
��Left=�61   
����  �$��11(�6^   �� 5�      $��: adds feature to model��6(   $
��Left=�6G   $��11(�6b   $��10 0 0 0q�6p   5�      � 5�      $
��Left=�6   .+�5�      �� $��C��6(   $
��Left=�61   
����  �$��11(�6^   �� 5�      $��: connects two features��6(   $
��Left=�6G   $��11(�6b   $��10 0 0 0q�6p   5�      � 5�      $
��Left=�6   .+�5�      �� $	��DEL��6(   $
��Left=�63   
����  �$��11(�6`   �� 5�      $"��: removes feature from model��6(   $
��Left=�6L   $��11(�6g   $��22 0 0 0q�6u   5�      � 5�      $
��Left=�6   .+�5�      �� $��ARROWS��6(   $
��Left=�66   
����  �$��11(�6c   �� 5�      $��: moves feature around��6(   $
��Left=�6F   $��11(�6a   $��50 0 0 0q�6o   5�      5�      5�      5�      5�      �� 5�   
   -   5        $��	statusBar��5�      $��3��6%   ���#73A3C4
�������$��2��6Z   @�5�      �� 5�      -   5        $��textBlockStatus��5�   <   $��Ready��6   
�������$��3 0 0 0q�6S   5�      5�      5�      