using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.IO;
using ElectronCgi.DotNet;
using System.Linq;

namespace EcuDox
{
    public class MapSwitching
    {
        private MapHandler _handler;
        private Connection _js;

        public MapSwitching(Connection con)
        {
            this._js = con;
            this._handler = new MapHandler(this._js);

            this._handler.RegisterQueueEvents();
        }

        public AvailableMap GetActiveMap() =>
            JsonConvert.DeserializeObject<List<AvailableMap>>
                (File.ReadAllText("./AG6_DATA/StoredMaps.json")).Where(m => m.Active).FirstOrDefault();

        public void SetActiveMap(string mapId)
        {
            var maps = JsonConvert.DeserializeObject<List<AvailableMap>>
                (File.ReadAllText("./AG6_DATA/StoredMaps.json"));

            foreach (var map in maps)
                map.Active = (map.Id.Equals(mapId)) ? true : false;

            File.WriteAllText("./AG6_DATA/StoredMaps.json", JsonConvert.SerializeObject(maps));
        }

        private List<AvailableMap> ReadMapDirectory()
        {
            var mapList = new List<AvailableMap>();

            string[] mapFiles = Directory.GetFiles("./AG6_DATA/StoredMaps/", "*.bin");

            foreach (string file in mapFiles)
            {
                string mapId = Path.GetFileNameWithoutExtension(file);
                mapList.Add(new AvailableMap(mapId, "", false));
            }

            return mapList;
        }

        private string CreateMapBase()
        {
            var maps = ReadMapDirectory();

            if (maps.Count <= 0)
                return "";

            maps.ForEach(x => _handler.QueueNameRequest(x));

            return "";
        }

        private string UpdateMapBase()
        {
            string curFileMaps = File.ReadAllText("./AG6_DATA/StoredMaps.json");

            if (_handler.QueueEmpty)
            {
                var mapFolder = ReadMapDirectory();
                var mapFile = JsonConvert.DeserializeObject<List<AvailableMap>>(curFileMaps);

                mapFolder
                    .Where(m => !mapFile.Any(m2 => m2.Id == m.Id))
                    .ToList()
                    .ForEach(x => _handler.QueueNameRequest(x));
            }

            return curFileMaps;
        }

        public string GetMapsSerialized()
        {
            try
            {
                if (!File.Exists("./AG6_DATA/StoredMaps.json"))
                    return CreateMapBase();

                return UpdateMapBase();
            }
            catch (Exception ex)
            {
                _js.Send("APIException", ex.Message);
                return "";
            }
        }
    }
}
