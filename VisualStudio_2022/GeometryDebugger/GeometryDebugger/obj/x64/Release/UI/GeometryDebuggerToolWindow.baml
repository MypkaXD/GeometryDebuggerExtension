   M S B A M L   `   `   `  ���� W  SGeometryDebugger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=81cf2426fede7b0e?$clr-namespace:GeometryDebugger.UtilsGeometryDebugger.Utils  i eMicrosoft.VisualStudio.Shell.15.0, Version=17.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�Zclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0!Microsoft.VisualStudio.PlatformUI 4    .GeometryDebugger.UI.GeometryDebuggerToolWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6local$clr-namespace:GeometryDebugger.Utils     ovs_shellZclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0     -     � 	Resources  2   ,GeometryDebugger.Utils.ColorToBrushConverter   ColorToBrushConverter �� MergedDictionaries �� Source��  �� $ ../Style/Theme.xaml?�%   &	          .��� $��10q� � ColumnDefinitions �� $��* � � RowDefinitions �� $
��Auto ��� $��* ��� $��5 ��� $��* �.+�� $��0��$��0�� �� $
��Auto ��� $��* �.+��� $��0�� �� Orientation$ 
Horizontal=�$
��Left=�.+��� -   $��
btnAddMenu��$��80��$��45������ $ 
Horizontal=�.+��� $ ��../Images/AddVariables.png��$��40��$��20���� $	��Add��$��Center=��� $��1��$ 
Horizontal=�$��Right=�.+��� -   $��	SelectBtn��$��45������ $ ��../Images/Select_Light.png��$��32��$��32��$
��Left=��� -   $��UnSelectBtn��$��45������ $"��../Images/Unselect_Light.png��$��32��$��32��$
��Left=��� -   $��	DeleteBtn��$��45������ $ ��../Images/Delete_Light.png��$��32��$��32��$
��Left=��� -   $��	ReloadBtn��$��45������ $ ��../Images/Reload_Light.png��$��32��$��32��$
��Left=��� -   $��AddHistoryBtn��$��45������ $$��../Images/AddHistory_Light.png��$��32��$��32��$
��Left=��� -   $��UpBtn��$��45������ $��../Images/Up_Light.png��$��32��$��32��$
��Left=��� -	   $��DownBtn��$��45������ $��../Images/Down_Light.png��$��32��$��32��$
��Left=��� -
   $��TopBtn��$��45������ $��../Images/Top_Light.png��$��32��$��32��$
��Left=��� -   $��	BottomBtn��$��45������ $ ��../Images/Bottom_Light.png��$��32��$��32��$
��Left=��� $��0��$��2��$��5��$��Stretch=�&   System.Windows.Controls.DataGrid  -   $��	dgObjects��$��1��$��0��   HeadersVisibility$ Column=�   AutoGenerateColumns .     CanUserAddRows .  	   CanUserDeleteRows	 .  $
��Left=�
   SelectionMode$
 Extended=�   RowDetailsVisibilityMode$ 	Collapsed=� � ContextMenu �� .@�m� -   $��Select�� m� Icon �� $ ��../Images/Select_Light.png��$��16��$��16��$
��Left=�m� -   $��Unselect�� �� $"��../Images/Unselect_Light.png��$��16��$��16��$
��Left=�m� -   $��Delete�� �� $ ��../Images/Delete_Light.png��$��16��$��16��$
��Left=�m� -   $��Reload�� �� $ ��../Images/Reload_Light.png��$��16��$��16��$
��Left=�m� -   $��
AddHistory�� �� $$��../Images/AddHistory_Light.png��$��16��$��16��$
��Left=�m� -   $
��Down�� �� $��../Images/Down_Light.png��$��16��$��16��$
��Left=�m� -   $��Up�� �� $��../Images/Up_Light.png��$��16��$��16��$
��Left=�m� -   $	��Top�� �� $��../Images/Top_Light.png��$��16��$��16��$
��Left=�m� -   $��Bottom�� �� $ ��../Images/Bottom_Light.png��$��16��$��16��$
��Left=�   Columns 4  .System.Windows.Controls.DataGridCheckBoxColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto    Header$ 
IsSelected��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *m_B_IsSelected+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�4  .System.Windows.Controls.DataGridTemplateColumn  $ 50    MinWidth$ 50Q�$ Color��   CellTemplate �� .R���  ��Name$ ColorDisplay��-   $
��Auto��$
��Auto������ *	m_C_Color+ �� 	Converter# [    $ * $
 Name��   SortMemberPath$ m_S_Name�� �� .R��� $ 
Horizontal=�.+��� $��ShowHistoryBtn��-   
������ 
������ .����  J� RenderTransformOrigin$ 0.5,0.5&�$��10��$��10��$%��../Images/ShowHistory_Light.png���� $��../Images/Cube.png��$��32��$��16���� ���� *
m_S_Name+0  *System.Windows.Controls.DataGridTextColumn  $ *    
IsReadOnly . $
 Type�� �� *
m_S_Type+  $ *  . $ Addres�� �� *
m_S_Addres+  $
 Auto  . $ Source�� �� *
m_S_Source+   RowDetailsTemplate �� .R�� $
��Auto��$��Stretch=� �� ���� *ActualWidth+ �� RelativeSource ��  �� AncestorType$ ScrollViewerL�.+�  $��0��$ Column=� .   .  	 .  $
��Left=�$��5q����� *m_OC_Childrens+ �� .@�m� -   $��	SADASDASD��m� -   $��123123��   $ 
IsSelected��$
 Auto  �� *m_b_isSelected+$ TwoWay=�$ PropertyChanged=�  $ 50 $ 50Q�$ Color�� �� .R��� $ ColorDisplayChildren��$
��Auto��$
��Auto������ *	m_C_Color+# [    $
 Name��$ *  �� *
m_s_Name+  $ Depth��$ *  �� *	m_i_Index+   �� .R��� $��DeleteFromHistory��-   $��Delete��$��50��$��25���� -   $��ControlHostElement��$��0��$��3��