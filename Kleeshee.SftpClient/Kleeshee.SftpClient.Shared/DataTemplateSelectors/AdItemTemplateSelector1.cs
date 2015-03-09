using Kleeshee.SftpClient.DataModels;
using Kleeshee.SftpClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kleeshee.SftpClient.DataTemplateSelectors
{
    public class AdItemTemplateSelector1 : AdItemTemplateSelectorBase
    {
        public AdItemTemplateSelector1()
            : base(1)
        {
        }

        public DataTemplate AdTemplate1
        {
            get { return this.adTemplates[0]; }
            set { this.adTemplates[0] = value; }
        }
    }
}
