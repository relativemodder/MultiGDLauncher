using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiGDLauncher
{
    public class InstanceInfo
    {
        public string Name = "Instance";
        public string GameVersion = "Unknown";
        public string LocalIconName = "0.png";
        public string GameExecutable = "GeometryDash.exe";

        public InstanceInfo(string name, string version, string localIconName, string gameExecutable = "GeometryDash.exe")
        {
            Name = name;
            GameVersion = version;
            LocalIconName = localIconName;
            GameExecutable = gameExecutable;
        }
    }
}
