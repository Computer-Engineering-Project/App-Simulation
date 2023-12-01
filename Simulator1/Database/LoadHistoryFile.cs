using Environment;
using Environment.Model.Module;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simulator1.Database
{
    public class LoadHistoryFile
    {
        string path = Env.path;
        public List<ModuleObject> listInModules = new List<ModuleObject>();
        public LoadHistoryFile()
        {
            
        }
        public bool ScanJsonFile()
        {
            
            var openFileDialog = new OpenFileDialog();
            bool? response = openFileDialog.ShowDialog();
            if(response == true)
            {
                path = openFileDialog.FileName;
                string json = File.ReadAllText(path);
                var nodeJSONList = JsonConvert.DeserializeObject<List<ModuleObject>>(json);
                if (nodeJSONList != null)
                {
                    listInModules = nodeJSONList;
                }
                return true;
            }
            return false;
        }
        public bool SaveJsonFile(string json)
        {
            var saveFileDialog = new SaveFileDialog();
            bool? response = saveFileDialog.ShowDialog();
            if(response == true)
            {
                path = saveFileDialog.FileName;
                File.WriteAllText(path, json);
                return true;
            }
            return false;
        }
    }
}
