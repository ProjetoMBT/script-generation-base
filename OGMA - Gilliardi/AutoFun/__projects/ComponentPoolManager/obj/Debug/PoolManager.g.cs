﻿#pragma checksum "..\..\PoolManager.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "29BA46531884E8A861A9619711AA5A16"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ComponentPoolManager;
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


namespace ComponentPoolManager {
    
    
    /// <summary>
    /// PoolManager
    /// </summary>
    public partial class PoolManager : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonOpenConfig;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonSaveConfig;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonLoadDanu;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonUpdateHandler;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxCodeHandlers;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton buttonInformation;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup popup;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel stackPanelComponents;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid interfaceEditorContainer;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBar statusBar;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\PoolManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlockStatus;
        
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
            System.Uri resourceLocater = new System.Uri("/ComponentPoolManager;component/poolmanager.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PoolManager.xaml"
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
            
            #line 7 "..\..\PoolManager.xaml"
            ((ComponentPoolManager.PoolManager)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.buttonOpenConfig = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\PoolManager.xaml"
            this.buttonOpenConfig.Click += new System.Windows.RoutedEventHandler(this.buttonOpenConfig_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.buttonSaveConfig = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\PoolManager.xaml"
            this.buttonSaveConfig.Click += new System.Windows.RoutedEventHandler(this.buttonSaveConfig_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.buttonLoadDanu = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\PoolManager.xaml"
            this.buttonLoadDanu.Click += new System.Windows.RoutedEventHandler(this.buttonLoadDanu_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.buttonUpdateHandler = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\PoolManager.xaml"
            this.buttonUpdateHandler.Click += new System.Windows.RoutedEventHandler(this.buttonUpdateHandler_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.comboBoxCodeHandlers = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.buttonInformation = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 8:
            this.popup = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            case 9:
            this.stackPanelComponents = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 10:
            this.interfaceEditorContainer = ((System.Windows.Controls.Grid)(target));
            return;
            case 11:
            this.statusBar = ((System.Windows.Controls.Primitives.StatusBar)(target));
            return;
            case 12:
            this.textBlockStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

