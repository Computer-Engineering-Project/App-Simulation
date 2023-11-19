using Environment.Model.Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Database
{
    public class LoadParameter
    {
        string path = "C:\\code\\DA\\APP\\App-Simulation\\Simulator1\\model.json";
        public List<ModuleObject> listInModules = new List<ModuleObject>();
        public LoadParameter()
        {
            ScanJsonFile();
        }
        public void ScanJsonFile()
        {
            string json = File.ReadAllText(path);
            var nodeJSONList = JsonConvert.DeserializeObject<List<ModuleObject>>(json);
            if (nodeJSONList != null)
            {
                listInModules = nodeJSONList;
            }
        }
    }
}
