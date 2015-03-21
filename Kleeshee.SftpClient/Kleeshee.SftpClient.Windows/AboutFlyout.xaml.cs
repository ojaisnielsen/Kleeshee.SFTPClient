using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Kleeshee.SftpClient
{
    public sealed partial class AboutFlyout : SettingsFlyout
    {
        public AboutFlyout()
        {
            this.InitializeComponent();
            this.NameAndVersion.Text = string.Format("{0} v{1}.{2}.{3}", 
                Package.Current.DisplayName,
                Package.Current.Id.Version.Major, 
                Package.Current.Id.Version.Minor, 
                Package.Current.Id.Version.Revision);
            this.Copyright.Text = this.GetType().GetTypeInfo().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        }
    }
}
