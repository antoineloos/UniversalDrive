﻿#pragma checksum "C:\Users\Antoine\Desktop\UniversalDrive\OneDriveSimpleSample.Univ\Views\OneDriveFilePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9C3561D300791F4443D4C3F615E78C25"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneDriveSimpleSample.Views
{
    partial class OneDriveFilePage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.mainframe = (global::Windows.UI.Xaml.Controls.Page)(target);
                    #line 10 "..\..\..\Views\OneDriveFilePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Page)this.mainframe).Loaded += this.Mainframe_Loaded;
                    #line default
                }
                break;
            case 2:
                {
                    this.PleaseWaitCache = (global::Windows.UI.Xaml.Shapes.Rectangle)(target);
                }
                break;
            case 3:
                {
                    this.Progress = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 4:
                {
                    global::Windows.UI.Xaml.Controls.Button element4 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 46 "..\..\..\Views\OneDriveFilePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element4).Click += this.Button_Click;
                    #line default
                }
                break;
            case 5:
                {
                    global::Windows.UI.Xaml.Controls.Button element5 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 61 "..\..\..\Views\OneDriveFilePage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element5).Click += this.BackButton_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.icon_noun_57650_cc = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

