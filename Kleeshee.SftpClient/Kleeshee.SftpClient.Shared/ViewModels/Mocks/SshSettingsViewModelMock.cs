using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kleeshee.SftpClient.ViewModels.Mocks
{
    public class SshSettingsViewModelMock : ISshSettingsViewModel
    {
        public SshSettingsViewModelMock()
        {
            this.Host = "192.168.0.1";
            this.Port = 22u;
            this.User = "username";
            this.KeyFilePath = @"c:\path\to\my\privatekey.key";
        }

        public bool ResetOnUnload { get { return true; } }

        public string Host { get; set; }

        public string User { get; set; }

        public uint Port { get; set; }

        public string KeyFilePath { get; set; }

        public void SetPassword(string password)
        {
        }

        public async Task SetKey()
        {
        }

        public async Task RemoveKey()
        {
        }
    }
}
