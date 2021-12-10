using System;
using System.Collections.Generic;

using ElectronCgi.DotNet;

using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace EcuDox
{

    public class DataLogging
    {
        private Connection _js;
        private LogHandler _handler;

        public DataLogging(Connection con)
        {
            this._js = con;
            this._handler = new LogHandler(this._js);

            this._handler.RegisterQueueEvents();
        }

        public string GetLogData(string id)
        {
            string filePath = "./AG6_DATA/Logs/" + id + ".csv";
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);

            else
                new AG6Exception(_js, "LogFile(" + id + ") could not be loaded.");

            return "";
        }

        public void CreateNewLogFile(string id, string name)
        {
            if (!File.Exists("./AG6_DATA/LogFiles.json"))
                File.WriteAllText("./AG6_DATA/LogFiles.json", JsonConvert.SerializeObject(new List<DataLogEntry>() { new DataLogEntry(id, name, "PENDING") }));
            else
            {
                var logFile = JsonConvert.DeserializeObject<List<DataLogEntry>>(File.ReadAllText("./AG6_DATA/LogFiles.json"));
                logFile.Add(new DataLogEntry(id, name, "PENDING"));

                File.WriteAllText("./AG6_DATA/LogFiles.json", JsonConvert.SerializeObject(logFile));
            }

            File.WriteAllText("./AG6_DATA/Logs/" + id + ".csv", "-");
        }

        private List<DataLogEntry> ReadLogDirectory()
        {
            var logList = new List<DataLogEntry>();

            string[] logFiles = Directory.GetFiles("./AG6_DATA/Logs/", "*.csv");

            foreach (string file in logFiles)
            {
                string logId = Path.GetFileNameWithoutExtension(file);
                logList.Add(new DataLogEntry(logId, "", "STORED"));
            }

            return logList;
        }

        private string CreateLogBase()
        {
            var logs = ReadLogDirectory();

            if (logs.Count <= 0)
                return "";

            logs.ForEach(x => _handler.QueueNameRequest(x));

            return "";
        }

        private string UpdateLogBase()
        {
            string curFileLogs = File.ReadAllText("./AG6_DATA/LogFiles.json");

            if (_handler.QueueEmpty)
            {
                var logFolder = ReadLogDirectory();
                var logFile = JsonConvert.DeserializeObject<List<DataLogEntry>>(curFileLogs);

                logFolder
                    .Where(l => !logFile.Any(l2 => l2.Id == l.Id))
                    .ToList()
                    .ForEach(x => _handler.QueueNameRequest(x));
            }

            return curFileLogs;
        }

        public string GetLogsSerialized()
        {
            try
            {
                if (!File.Exists("./AG6_DATA/LogFiles.json"))
                    return CreateLogBase();

                return UpdateLogBase();
            }
            catch (Exception ex)
            {
                _js.Send("APIException", ex.Message);
                return "";
            }
        }
    }
}
