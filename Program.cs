using ElectronCgi.DotNet;
using System.Threading;
using EcuDox;

namespace EcuDoxAPI
{
    public class Program
    {
        static Connection JsConnection = null;

        static void Main(string[] args)
        {
            AG6INIT AG6 = new AG6INIT();
            if (!AG6.Init())
                return;

            JsConnection = new ConnectionBuilder()
                .WithLogging()
                .Build();

            Thread serialThread = new Thread(AG6Main.SerialThread);
            serialThread.IsBackground = true;
            serialThread.Start(JsConnection);

            JsConnection.Listen();
        }
    }
}
