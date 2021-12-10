namespace EcuDox
{
    public class DataLogEntry
    {
        public DataLogEntry(string id, string name, string logData)
        {
            this.Id = id;
            this.DisplayName = name;
            this.LogData = logData;
        }

        public string Id;
        public string DisplayName;
        public string LogData;
    }
}
