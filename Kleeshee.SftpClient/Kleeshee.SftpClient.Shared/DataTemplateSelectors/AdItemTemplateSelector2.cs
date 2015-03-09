using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace Kleeshee.SftpClient.DataTemplateSelectors
{
    public class AdItemTemplateSelector2 : AdItemTemplateSelectorBase
    {
        public AdItemTemplateSelector2()
            : base(2)
        {
        }

        public DataTemplate AdTemplate1
        {
            get { return this.adTemplates[0]; }
            set { this.adTemplates[0] = value; }
        }

        public DataTemplate AdTemplate2
        {
            get { return this.adTemplates[1]; }
            set { this.adTemplates[1] = value; }
        }
    }
}
