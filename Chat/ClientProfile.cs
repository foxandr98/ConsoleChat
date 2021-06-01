using System;
using System.Collections.Generic;
using System.Text;

namespace Chat
{
    class ClientProfile
    {
        public string UserName { get; set; }

        public ClientProfile(string userName)
        {
            UserName = userName;
        }

    }
}
