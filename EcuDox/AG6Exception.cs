using ElectronCgi.DotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox
{
    class AG6Exception
    {
        public AG6Exception(Connection js, string message)
        {
            js.Send("APIException", message);
            throw new Exception(message);
        }
    }
}
