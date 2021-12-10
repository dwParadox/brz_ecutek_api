using System;
using System.Collections.Generic;

using ElectronCgi.DotNet;

using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace EcuDox
{
    public class LogHandler
    {
        private class LogHandlerRequest
        {
            public LogHandlerRequest(DataLogEntry log, bool requestSent = false)
            {
                this.Log = log;
                this.RequestSent = requestSent;
            }

            public DataLogEntry Log;
            public bool RequestSent;
        }

        private Connection _js;
        private Queue<LogHandlerRequest> _queue;

        public bool QueueEmpty { get { return _queue.Count <= 0; } }

        public LogHandler(Connection con)
        {
            this._js = con;
            this._queue = new Queue<LogHandlerRequest>();
        }

        public void HandleQueue()
        {
            while (true)
            {
                if (_queue.Count <= 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                LogHandlerRequest cur = _queue.Peek();

                if (cur.RequestSent == false)
                {
                    _js.Send("LogFileNameRequest", cur.Log.Id);
                    cur.RequestSent = true;
                }

                Thread.Sleep(1000);
            }
        }

        public void RegisterQueueEvents()
        {
            _js.On<string>("LogFileNameResponse", name =>
            {
                LogHandlerRequest req = _queue.Dequeue();
                req.Log.DisplayName = string.IsNullOrWhiteSpace(name) ? req.Log.Id : name;

                if (File.Exists("./AG6_DATA/LogFiles.json"))
                {
                    try
                    {
                        List<DataLogEntry> existing = JsonConvert.DeserializeObject<List<DataLogEntry>>(File.ReadAllText("./AG6_DATA/LogFiles.json"));

                        existing.Add(req.Log);

                        File.WriteAllText("./AG6_DATA/LogFiles.json",
                            JsonConvert.SerializeObject(existing));
                    }
                    catch (Exception ex)
                    {
                        _js.Send("VehicleInfoMessage", "Error updating logfile: " + ex.Message);
                    }
                }
                else
                {
                    string logJson = JsonConvert.SerializeObject(new List<DataLogEntry>() { req.Log });

                    try
                    {
                        File.WriteAllText("./AG6_DATA/LogFiles.json", logJson);
                    }
                    catch (Exception ex)
                    {
                        _js.Send("VehicleInfoMessage", "Error creating logfile: " + ex.Message);
                    }
                }

                _js.Send("LogsUpdated", "f");
            });

            Thread logHandler = new Thread(HandleQueue);
            logHandler.Start();
        }

        public void QueueNameRequest(DataLogEntry log) => 
            _queue.Enqueue(new LogHandlerRequest(log));
    }
}
