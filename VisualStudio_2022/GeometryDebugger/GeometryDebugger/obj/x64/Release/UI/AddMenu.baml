   M S B A M L   `   `   `  ���� W  SGeometryDebugger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=81cf2426fede7b0e?$clr-namespace:GeometryDebugger.UtilsGeometryDebugger.Utils  i eMicrosoft.VisualStudio.Shell.15.0, Version=17.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�Zclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0!Microsoft.VisualStudio.PlatformUI !    GeometryDebugger.UI.AddMenu   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6local$clr-namespace:GeometryDebugger.Utils     ovs_shellZclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0       � 	Resources  2   ,GeometryDebugger.Utils.ColorToBrushConverter   ColorToBrushConverter �� MergedDictionaries �� Source��  �� $ ../Style/Theme.xaml?�%   &	          .���  � RowDefinitions �� $
��Auto ��� $��* � � ColumnDefinitions �� $��* �.+�� $��0��$��0��.+���  �� Orientation$ 
Horizontal=�$
��Left=�.+��� -   $��WL�� d� 	IsChecked$ FalseY��� -   $��CF��$ FalseY��� -   $��ButtonImport��$��50��$��50��.���� $��32��$��32��$ ��../Images/Export_Light.png���� $ 
Horizontal=�$��Right=�.+��� -   $ ��Add variables from TextBox��$��Right=��� -   $��MySelfAddedVariables��$	��100��$��Right=�$��20(�&   System.Windows.Controls.DataGrid  -   $��dgAddVariables��$��1��$��0��   HeadersVisibility$ Column=�$
��Left=�   AutoGenerateColumns .  	   CanUserAddRows	 .  
   CanUserDeleteRows
 .   � ContextMenu �� .@�m� -   $��Select�� m� Icon �� $ ��../Images/Select_Light.png��$��16��$��16��m� -   $��Unselect�� �� $"��../Images/Unselect_Light.png��$��16��$��16��m� -	   $��Delete�� �� $ ��../Images/Delete_Light.png��$��16��$��16��   Columns 4  .System.Windows.Controls.DataGridCheckBoxColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto    Header$ IsAdded��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *m_B_IsAdded+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�4  .System.Windows.Controls.DataGridTemplateColumn  $ 50    MinWidth$ 50Q�$ Color��   CellTemplate �� .R���  ��Name$ ColorDisplay��-
   $
��Auto��$
��Auto������ *	m_C_Color+ �� 	Converter# [  0  *System.Windows.Controls.DataGridTextColumn  $ *    
IsReadOnly . $
 Name�� �� *
m_S_Name+  $ *  . $
 Type�� �� *
m_S_Type+  $ *  . $ Addres�� �� *
m_S_Addres+  $
 Auto  . $ Source�� �� *
m_S_Source+