﻿

#pragma checksum "c:\users\srujan\documents\visual studio 2015\Projects\Hikeathon\Hikeathon\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "86FA74C9AB93848C248D6A9301AE50FC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hikeathon
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 17 "..\..\MainPage.xaml"
                ((global::Facebook.Client.Controls.LoginButton)(target)).SessionStateChanged += this.OnSessionStateChanged;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


