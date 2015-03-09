using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Kleeshee.SftpClient.Common
{
    public static class NotificationHelper
    {
        public static ToastNotification Factory(string title, string text, string image)
        {
            if (image == null)
            {
                return NotificationHelper.Factory(title, text);
            }

            var toastTemplate = ToastTemplateType.ToastImageAndText02;
            var toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            var textElement1 = toastXml.SelectSingleNode("//text[@id='1']");
            textElement1.AppendChild(toastXml.CreateTextNode(title));
            var textElement2 = toastXml.SelectSingleNode("//text[@id='2']");
            textElement2.AppendChild(toastXml.CreateTextNode(text));
            var imageElement = toastXml.SelectSingleNode("//image");
            ((XmlElement)imageElement).SetAttribute("src", image);
            return new ToastNotification(toastXml);
        }

        public static ToastNotification Factory(string title, string text)
        {
            var toastTemplate = ToastTemplateType.ToastText02;
            var toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            var textElement1 = toastXml.SelectSingleNode("//text[@id='1']");
            textElement1.AppendChild(toastXml.CreateTextNode(title));
            var textElement2 = toastXml.SelectSingleNode("//text[@id='2']");
            textElement2.AppendChild(toastXml.CreateTextNode(text));
            return new ToastNotification(toastXml);
        }
    }
}
