   M S B A M L   `   `   `  ����W  SGeometryDebugger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=81cf2426fede7b0e?$clr-namespace:GeometryDebugger.UtilsGeometryDebugger.Utils  5      i eMicrosoft.VisualStudio.Shell.15.0, Version=17.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�Zclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0!Microsoft.VisualStudio.PlatformUI 5      4    .GeometryDebugger.UI.GeometryDebuggerToolWindow   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6local$clr-namespace:GeometryDebugger.Utils     5      ovs_shellZclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0     5      -   5          � 	Resources  5      2   ,GeometryDebugger.Utils.ColorToBrushConverter   ColorToBrushConverter �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ ../Style/Theme.xaml?�6%   6D   %   &	          5      6*   5      .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   6+    � RowDefinitions 5   
   �� 5       $
��Auto �6   6,   �� 5!      $��* �6   6)   �� 5"      $��5 �6   6)   �� 5#      $��* �6   6)   .+�5%   
   � $��0��6   $��0��6    5&      �� 5'      $
��Auto �6#   62   �� 5(      $��* �6#   6/   .+�5+      �� $��0��6    �� Orientation$ 
Horizontal=�6)   $
��Left=�6B   .+�5,      �� -   5        $��
btnAddMenu��5.      $��80��5-      $��45��6   ��5/      �� 50      $ 
Horizontal=�6%   .+�51      �� $ ��../Images/AddVariables.png��6$   $��40��6H   $��20��6S   �� 52      $	��Add��6(   $��Center=�63   6P   53      55      56      �� 57      $��1��6   $ 
Horizontal=�6)   $��Right=�6B   .+�58      �� -   5        $��	SelectBtn��58      $��45��6*   ��59      �� 5:      $ ��../Images/Select_Light.png��6    $��32��6D   $��32��6O   $
��Left=�6[   6x   5<      �� 5=      -   5        $��UnSelectBtn��5=      $��45��6,   ��5>      �� 5?      $"��../Images/Unselect_Light.png��6    $��32��6F   $��32��6Q   $
��Left=�6]   6z   5A      �� 5B      -   5        $��	DeleteBtn��5B      $��45��6+   ��5C      �� 5D      $ ��../Images/Delete_Light.png��6    $��32��6D   $��32��6O   $
��Left=�6[   6x   5F      �� 5G      -   5        $��	ReloadBtn��5G      $��45��6*   ��5H      �� 5I      $ ��../Images/Reload_Light.png��6    $��32��6D   $��32��6O   $
��Left=�6[   6x   5K      �� 5M      -   5        $��AddHistoryBtn��5M      $��45��6.   ��5N      �� 5O      $$��../Images/AddHistory_Light.png��6    $��32��6H   $��32��6S   $
��Left=�6_   6|   5Q      �� 5S      -   5        $��UpBtn��5S      $��45��6&   ��5T      �� 5U      $��../Images/Up_Light.png��6    $��32��6@   $��32��6K   $
��Left=�6W   6t   5W      �� 5Y      -	   5        $��DownBtn��5Y      $��45��6(   ��5Z      �� 5[      $��../Images/Down_Light.png��6    $��32��6B   $��32��6M   $
��Left=�6Y   6v   5]      �� 5_      -
   5        $��TopBtn��5_      $��45��6'   ��5`      �� 5a      $��../Images/Top_Light.png��6    $��32��6A   $��32��6L   $
��Left=�6X   6u   5c      �� 5e      -   5        $��	BottomBtn��5e      $��45��6*   ��5f      �� 5g      $ ��../Images/Bottom_Light.png��6    $��32��6D   $��32��6O   $
��Left=�6[   6x   5i      5k      5l      �� 5m   
   $��0��6   $��2��6'   $��5��64   $��Stretch=�6?   &   System.Windows.Controls.DataGrid  5n   
   -   5        $��	dgObjects��5n   0   $��1��6   $��0��6       HeadersVisibility$ Column=�6A      AutoGenerateColumns .     CanUserAddRows .  	   CanUserDeleteRows	 .  $
��Left=�5o   D   
   SelectionMode$
 Extended=�6_      RowDetailsVisibilityMode$ 	Collapsed=�5p   /    � ContextMenu 5q      �� 5r      .@�5s      m� -   5        $��Select��5s       m� Icon 5t      �� 5u      $ ��../Images/Select_Light.png��6$   $��16��6H   $��16��6S   $
��Left=�6_   6|   5w      m� 5x      -   5        $��Unselect��5x       5y      �� 5z      $"��../Images/Unselect_Light.png��6$   $��16��6J   $��16��6U   $
��Left=�6a   6~   5|      m� 5}      -   5        $��Delete��5}       5~      �� 5      $ ��../Images/Delete_Light.png��6$   $��16��6H   $��16��6S   $
��Left=�6_   6|   5�      m� 5�      -   5        $��Reload��5�       5�      �� 5�      $ ��../Images/Reload_Light.png��6$   $��16��6H   $��16��6S   $
��Left=�6_   6|   5�      m� 5�      -   5        $��
AddHistory��5�       5�      �� 5�      $$��../Images/AddHistory_Light.png��6$   $��16��6L   $��16��6W   $
��Left=�6c   6�   5�      m� 5�      -   5        $
��Down��5�       5�      �� 5�      $��../Images/Down_Light.png��6$   $��16��6F   $��16��6Q   $
��Left=�6]   6z   5�      m� 5�      -   5        $��Up��5�       5�      �� 5�      $��../Images/Up_Light.png��6$   $��16��6D   $��16��6O   $
��Left=�6[   6x   5�      m� 5�      -   5        $	��Top��5�       5�      �� 5�      $��../Images/Top_Light.png��6$   $��16��6E   $��16��6P   $
��Left=�6\   6y   5�      m� 5�      -   5        $��Bottom��5�       5�      �� 5�      $ ��../Images/Bottom_Light.png��6$   $��16��6H   $��16��6S   $
��Left=�6_   6|   5�      5�         Columns 5�      4  .System.Windows.Controls.DataGridCheckBoxColumn  5�      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6)      Header$ 
IsSelected��66   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6J   �� *m_B_IsSelected+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   4  .System.Windows.Controls.DataGridTemplateColumn  5�      $ 50 6)      MinWidth$ 50Q�64   $ Color��6B      CellTemplate 5�      �� 5�      .R�5�      ��  ��Name$ ColorDisplay��6%   -   5        $
��Auto��5�      $
��Auto��5�      ��5�      �� *	m_C_Color+ �� 	Converter# [  5�      5�      5�        5�      $ * 6)   $
 Name��63      SortMemberPath$ m_S_Name��6A    5�      �� 5�      .R�5�      �� $ 
Horizontal=�6)   .+�5�   "   �� $��ShowHistoryBtn��6)   -   5        
������ 
������ .��5�   &   ��  J� RenderTransformOrigin$ 0.5,0.5&�6,   $��10��6L   $��10��6W   $%��../Images/ShowHistory_Light.png��6c   5�   #   �� 5�   "   $��../Images/Cube.png��6(   $��32��6D   $��16��6O   �� 5�   "   ��6,   �� *
m_S_Name+5�      5�      5�      0  *System.Windows.Controls.DataGridTextColumn  5�      $ * 6%      
IsReadOnly . $
 Type��6A    6O   �� *
m_S_Type+6n     5�      $ * 6%    . $ Addres��6A    6Q   �� *
m_S_Addres+6r     5�      $
 Auto 6%    . $ Source��6D    6T   �� *
m_S_Source+6u      RowDetailsTemplate 5�      �� 5�      .R�5�      � $
��Auto��6   $��Stretch=�6(    5�      �� 5�      ��6/   �� *ActualWidth+ �� RelativeSource ��  �� AncestorType$ ScrollViewerL�6�   .+�5�        $��0��6#   $ Column=�6Z    .   .  	 .  $
��Left=�5�   N   $��5q�6i   ��5�   3   �� *m_OC_Childrens+ 5�      �� 5�   "   .@�5�   &   m� -   5        $��	SADASDASD��5�   /   5�   '   m� 5�   &   -   5        $��123123��5�   /   5�   '   5�   #    5�        5�   "   $ 
IsSelected��69   $
 Auto 6�    6M   �� *m_b_isSelected+$ TwoWay=�$ PropertyChanged=�6�     5�   "   $ 50 69   $ 50Q�6D   $ Color��6R    5�   &   �� 5�   *   .R�5�   .   �� $ ColorDisplayChildren��65   $
��Auto��5�   �   $
��Auto��6�   ��61   �� *	m_C_Color+# [  6�   5�   +   5�   #     5�   "   $
 Name��65   $ * 6`    6C   �� *
m_s_Name+6`     5�   "   $ Depth��65   $ * 6b    6D   �� *	m_i_Index+6b     5�   "    5�   &   �� 5�   *   .R�5�   .   �� $��DeleteFromHistory��65   -   5        $��Delete��5�   n   $��50��6   $��25��6�   5�   /   5�   +   5�   #   5�      5�      5�      5�      �� 5�   
   -   5        $��ControlHostElement��5�      $��0��6+   $��3��6;   6J   5�      5�      