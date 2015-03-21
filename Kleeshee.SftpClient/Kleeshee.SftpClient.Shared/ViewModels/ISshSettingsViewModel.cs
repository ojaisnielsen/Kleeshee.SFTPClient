using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kleeshee.SftpClient.ViewModels
{
    public interface ISshSettingsViewModel
    {
        string Host { get; set; }

        string User { get; set; }

        uint Port { get; set; }

        string KeyFilePath { get; set; }

        bool ResetOnUnload { get; }

        void SetPassword(string password);

        Task SetKey();

        Task RemoveKey();
    }
}
