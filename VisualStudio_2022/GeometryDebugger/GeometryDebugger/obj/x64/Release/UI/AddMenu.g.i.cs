﻿#pragma checksum "..\..\..\..\UI\AddMenu.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E2BAE37E9C3FAE11B16198B8C000A8828BAFBFE1F4C17AD669A5924BC1080A62"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GeometryDebugger.Utils;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace GeometryDebugger.UI {
    
    
    /// <summary>
    /// AddMenu
    /// </summary>
    public partial class AddMenu : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 32 "..\..\..\..\UI\AddMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox WL;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\UI\AddMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CF;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\UI\AddMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonImport;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\UI\AddMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MySelfAddedVariables;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\UI\AddMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgAddVariables;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GeometryDebugger;component/ui/addmenu.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UI\AddMenu.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.WL = ((System.Windows.Controls.CheckBox)(target));
            
            #line 32 "..\..\..\..\UI\AddMenu.xaml"
            this.WL.Click += new System.Windows.RoutedEventHandler(this.ButtonWatchList_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CF = ((System.Windows.Controls.CheckBox)(target));
            
            #line 34 "..\..\..\..\UI\AddMenu.xaml"
            this.CF.Click += new System.Windows.RoutedEventHandler(this.ButtonCurrentStackFrame_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ButtonImport = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\..\UI\AddMenu.xaml"
            this.ButtonImport.Click += new System.Windows.RoutedEventHandler(this.ButtonImport_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 41 "..\..\..\..\UI\AddMenu.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonMyselfAdded_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MySelfAddedVariables = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.dgAddVariables = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            
            #line 49 "..\..\..\..\UI\AddMenu.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItemAddForImport_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 54 "..\..\..\..\UI\AddMenu.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItemAddForIsntImport_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 59 "..\..\..\..\UI\AddMenu.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItemDelete_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 10:
            
            #line 75 "..\..\..\..\UI\AddMenu.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ColorDisplay_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

