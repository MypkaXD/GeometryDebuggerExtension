   M S B A M L   `   `   `  ����i  eMicrosoft.VisualStudio.Shell.15.0, Version=17.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�Zclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0!Microsoft.VisualStudio.PlatformUI  5      W SGeometryDebugger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=81cf2426fede7b0e9!clr-namespace:GeometryDebugger.UIGeometryDebugger.UI 5      %   GeometryDebugger.UI.ColorPicker   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ovs_shellZclr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0     5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      1local!clr-namespace:GeometryDebugger.UI    5      /muxc using:Microsoft.UI.Xaml.Controls    5      -   5        $	��305��5	      $	��275��6,     � 	Resources  5       �� MergedDictionaries �� Source �� 
TargetType �� Property9   3Microsoft.VisualStudio.PlatformUI.EnvironmentColors#   ToolWindowBackgroundBrushKey �� Value   ToolWindowTextBrushKey   CommandBarHoverBrushKey�� 5   
    5      �� 5      $ ../Style/Theme.xaml?�6%   6D   %  '��       '�� F     '�� �     �� 5      " ��6   .�5      ��  � ��# �  6/   5      �� 5      " ��6   .�5      ��  � ��# �  6/   5      �� 5      " ��6   .�5      ��  � ��# �  6/   �� 5       � ��# �  6/   5      5      .��5       �� .+�5!   
   �� 	 �� Orientation$	 
Horizontal=�6   .+�5"      � $��10q�6   .+�5#      �� -   5        
 ��Name$
 ColorCanvas��5#      $	��200��6.   $	��200��6:   $
��Left=�6G   $	��Top=�6b   
�������}�. .+�5'      �� -   5         ��Name$ 	ColorRect��5'       $	��200��63   $	��200��6?   ��5(      �� 5)       �� EndPoint$	 1,0&�62   .��5*   "   �  � Color$	 Red��6/    � Offset$ 0Q�6;   � 5+   "   $ Yellow��6/   $
 0.17Q�6>   � 5,   "   $
 Lime��6/   $
 0.33Q�6<   � 5-   "   $
 Cyan��6/   $	 0.5Q�6<   � 5.   "   $
 Blue��6/   $
 0.67Q�6<   � 5/   "   $ Magenta��6/   $
 0.83Q�6?   � 50   "   $	 Red��6/   $ 1Q�6;   51      53      �� 54      -   5        $ RectangleGray��54       $	��200��67   $	��200��6C   ��55      �� 56       �� 
StartPoint$	 0,0&�62   $	 0,1&�6C   .��57   "   � $ Transparent��6/   $ 0Q�6C   � 58   "   $
 Gray��6/   $ 1Q�6<   59      5;      �� 5<      
���   �$��1q�61   $	��200��6E   $	��200��6Q   A� 5>      -   5         A�Name$ SelectorEllipse��5>      $��10��67   $��10��6B   
���   �
������  �� Left$	 100��5@      
 �� Top$	 100��6,   5A      5B      �� 5C      .+�5D      �� -   5        $ LightnessRect��5D      $��30��63   $	��200��6>   $��Center=�6K   $	��Top=�6h   $��8,10,0,0q�6�   �� 5E      -   5         ��Name$ LightnessSlider��5E       �� Orientation$ Vertical=�62   $��31��6I   $	��190��6T    � Minimum$ 0Q�6a    � Maximum$ 1Q�6m    � Value$	 0.5Q�6y   $��Center=�6�   $	��Top=�6�    K� ZIndex$ 1��6�   $ 13��6�   $ 15��6  �� 5F      
���   �$��1q�6-   $��30��6A   $	��200��6L   $��8,10,0,0q�6Y   5G      5H      �� 5I   
   $	 
Horizontal=�6   .+�5J      �� -   5        $
 CanvasOfColor��5J      $��75��6,   $��75��67   
�������}�. $��10,0,0,0q�6j   $
��Left=�6|   .+�5K      �� -	   5        $ ColorPreview��5K      $��75��62   $��75��6>   �� 5L      
���   �$��1q�6-   $��75��6A   $��75��6L   $��0,0,0,0q�6X   5M      �� 5N      $	 Vertical=�6   $��10,0,0,0q�60   .+�5O      �� $	 
Horizontal=�6   .+�5P      �� $��R:��6    $
��Bold"�6*   �� 5Q      -
   5         ��Name$ RedValue��5Q      $��50��60   $	��255��6;   $
��Auto��6F   $��10,0,0,0q�6T    �� TextAlignment$ Center=�6f   6}   5R      �� 5S      $	 
Horizontal=�6   $��0,10,0,0q�66   .+�5T      �� $��G:��6    $
��Bold"�6*   �� 5U      -   5        $ 
GreenValue��5U      $��50��62   $	��255��6=   $��10,0,0,0q�6H   $ Center=�6Z   6q   5V      �� 5W      $	 
Horizontal=�6   $��0,10,0,0q�66   .+�5X      �� $��B:��6    $
��Bold"�6*   �� 5Y      -   5        $ 	BlueValue��5Y      $��50��61   $	��255��6<   $��10,0,0,0q�6G   $ Center=�6Y   6p   5Z      5[      �� 5\      $	 Vertical=�6   $��10,0,0,0q�60   .+�5]      �� $	 
Horizontal=�6   .+�5^      �� $��H(°):��6    $
��Bold"�6-   �� 5_      -   5        $ HueValue��5_      $��50��60   $	��180��6;   $
��Auto��6F   $��13,0,0,0q�6T   $ Center=�6f   6}   5`      �� 5a      $	 
Horizontal=�6   $��0,10,0,0q�66   .+�5b      �� $��S(%):��6    $
��Bold"�6-   �� 5c      -   5        $ SaturationValue��5c      $��50��67   $��50��6B   $��10,0,0,0q�6L   $ Center=�6^   6u   5d      �� 5e      $	 
Horizontal=�6   $��0,10,0,0q�66   .+�5f      �� $��L(%):��6    $
��Bold"�6-   �� 5g      -   5        $ LightnessValue��5g      $��50��66   $��50��6A   $��10,0,0,0q�6K   $ Center=�6]   6t   5h      5i      5j      5k      5l      