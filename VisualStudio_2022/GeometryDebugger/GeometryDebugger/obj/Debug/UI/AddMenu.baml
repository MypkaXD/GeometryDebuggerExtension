   M S B A M L   `   `   `  ����W  SGeometryDebugger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=81cf2426fede7b0e?$clr-namespace:GeometryDebugger.UtilsGeometryDebugger.Utils  5      i eMicrosoft.VisualStudio.Shell.15.0, Version=17.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�Zclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0!Microsoft.VisualStudio.PlatformUI 5      !    GeometryDebugger.UI.AddMenu   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6local$clr-namespace:GeometryDebugger.Utils     5      ovs_shellZclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0     5        � 	Resources  5      2   ,GeometryDebugger.Utils.ColorToBrushConverter   ColorToBrushConverter �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ ../Style/Theme.xaml?�6%   6D   %   &	          5      6*   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)    � ColumnDefinitions 5   
   �� 5      $��* �6   6+   .+�5   
   � $��0��6   $��0��6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $
��Left=�62   .+�5       �� -   5        $��WL��5        d� 	IsChecked$ FalseY�6C   5!      �� 5"      -   5        $��CF��5"      $ FalseY�6K   5#      �� 5$      -   5        $��ButtonImport��5$   K   $��50��64   $��50��6?   .��5%      �� $��32��6   $��32��6'   $ ��../Images/Export_Light.png��63   5&      5'      �� 5(      $ 
Horizontal=�6   $��Right=�62   .+�5)      �� -   5        $ ��Add variables from TextBox��5)      $��Right=�6^   5*      �� 5+      -   5        $��MySelfAddedVariables��5+      $	��100��66   $��Right=�6B   $��20(�6^   6n   5,      5-      &   System.Windows.Controls.DataGrid  5.   
   -   5        $��dgAddVariables��5.   0   $��1��6   $��0��6       HeadersVisibility$ Column=�6G   $
��Left=�6b      AutoGenerateColumns .  	   CanUserAddRows	 .  
   CanUserDeleteRows
 .   � ContextMenu 5/      �� 50      .@�51      m� -   5        $��Select��51       m� Icon 52      �� 53      $ ��../Images/Select_Light.png��6$   $��16��6H   $��16��6S   6a   55      m� 56      -   5        $��Unselect��56       57      �� 58      $"��../Images/Unselect_Light.png��6$   $��16��6J   $��16��6U   6c   5:      m� 5;      -	   5        $��Delete��5;       5<      �� 5=      $ ��../Images/Delete_Light.png��6$   $��16��6H   $��16��6S   6a   5?      5@         Columns 5B      4  .System.Windows.Controls.DataGridCheckBoxColumn  5C      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6)      Header$ IsAdded��66   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6G   �� *m_B_IsAdded+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   4  .System.Windows.Controls.DataGridTemplateColumn  5D      $ 50 6)      MinWidth$ 50Q�64   $ Color��6B      CellTemplate 5E      �� 5F      .R�5G      ��  ��Name$ ColorDisplay��6%   -
   5        $
��Auto��5I      $
��Auto��5J      ��5H      �� *	m_C_Color+ �� 	Converter# [  5K      5L      5N      0  *System.Windows.Controls.DataGridTextColumn  5O      $ * 6%      
IsReadOnly . $
 Name��6A    6O   �� *
m_S_Name+6n     5P      $ * 6%    . $
 Type��6A    6O   �� *
m_S_Type+6n     5Q      $ * 6%    . $ Addres��6A    6Q   �� *
m_S_Addres+6r     5R      $
 Auto 6%    . $ Source��6D    6T   �� *
m_S_Source+6u   5T      5U      5V      