using System.IO;

namespace EcuDoxAPI
{
    public class AG6INIT
    {
        public AG6INIT()
        {

        }

        private void InitDirectories()
        {
            if (!Directory.Exists("./AG6_DATA"))
                Directory.CreateDirectory("AG6_DATA");

            if (!Directory.Exists("./AG6_DATA/StoredMaps"))
                Directory.CreateDirectory("./AG6_DATA/StoredMaps");

            if (!Directory.Exists("./AG6_DATA/ReadMaps"))
                Directory.CreateDirectory("./AG6_DATA/ReadMaps");

            if (!Directory.Exists("./AG6_DATA/Logs"))
                Directory.CreateDirectory("./AG6_DATA/Logs");

            if (!Directory.Exists("./AG6_DATA/RaceROM"))
                Directory.CreateDirectory("./AG6_DATA/RaceROM");

            if (!Directory.Exists("./AG6_DATA/RaceROM/Cache"))
                Directory.CreateDirectory("./AG6_DATA/RaceROM/Cache");
        }

        public bool Init()
        {
            InitDirectories();

            return true;
        }
    }
}
