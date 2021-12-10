using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.IO;
using ElectronCgi.DotNet;
using System.Threading;

namespace EcuDox
{
    public class MapHandler
    {
        private class MapHandlerRequest
        {
            public MapHandlerRequest(AvailableMap map, bool requestSent = false)
            {
                this.Map = map;
                this.RequestSent = requestSent;
            }

            public AvailableMap Map;
            public bool RequestSent;
        }

        private Connection _js;
        private Queue<MapHandlerRequest> _queue;

        public bool QueueEmpty { get { return _queue.Count <= 0; } }

        public MapHandler(Connection con)
        {
            this._js = con;
            this._queue = new Queue<MapHandlerRequest>();
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

                MapHandlerRequest cur = _queue.Peek();

                if (cur.RequestSent == false)
                {
                    _js.Send("MapFileNameRequest", cur.Map.Id);
                    cur.RequestSent = true;
                }

                Thread.Sleep(1000);
            }
        }

        public void RegisterQueueEvents()
        {
            _js.On<string>("MapFileNameResponse", name =>
            {
                MapHandlerRequest req = _queue.Dequeue();
                req.Map.DisplayName = string.IsNullOrWhiteSpace(name) ? req.Map.Id : name;

                if (File.Exists("./AG6_DATA/StoredMaps.json"))
                {
                    try
                    {
                        List<AvailableMap> existing = JsonConvert.DeserializeObject<List<AvailableMap>>(File.ReadAllText("./AG6_DATA/StoredMaps.json"));

                        existing.Add(req.Map);

                        File.WriteAllText("./AG6_DATA/StoredMaps.json",
                            JsonConvert.SerializeObject(existing));
                    }
                    catch (Exception ex)
                    {
                        _js.Send("VehicleInfoMessage", "Error updating logfile: " + ex.Message);
                    }
                }
                else
                {
                    string mapJson = JsonConvert.SerializeObject(new List<AvailableMap>() { req.Map });

                    try
                    {
                        File.WriteAllText("./AG6_DATA/StoredMaps.json", mapJson);
                    }
                    catch (Exception ex)
                    {
                        _js.Send("VehicleInfoMessage", "Error creating logfile: " + ex.Message);
                    }
                }

                _js.Send("MapsUpdated", "f");
            });

            Thread mapHandler = new Thread(HandleQueue);
            mapHandler.Start();
        }

        public void QueueNameRequest(AvailableMap map) =>
            _queue.Enqueue(new MapHandlerRequest(map));
    }
}
