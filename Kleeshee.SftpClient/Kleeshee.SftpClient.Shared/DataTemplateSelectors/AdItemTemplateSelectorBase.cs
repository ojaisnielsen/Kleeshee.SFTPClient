using Kleeshee.SftpClient.DataModels;
using Kleeshee.SftpClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kleeshee.SftpClient.DataTemplateSelectors
{
    public abstract class AdItemTemplateSelectorBase : DataTemplateSelector
    {
        protected readonly DataTemplate[] adTemplates;

        public AdItemTemplateSelectorBase(int n)
        {
            this.adTemplates = new DataTemplate[n];
        }

        public DataTemplate ItemTemplate { get; set; }

        public DataTemplate NonServingAdTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ISftpItem)
            {
                return this.ItemTemplate;
            }

            var ad = item as AdItem;
            if (item != null)
            {
                return ad.Serving ? this.adTemplates[ad.Id % this.adTemplates.Length] : this.NonServingAdTemplate;
            }

            return null;
        }
    }
}
