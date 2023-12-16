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
        string path = "";
        public List<ModuleObject> listInModules = new List<ModuleObject>();
        public LoadHistoryFile()
        {

        }
        public bool OpenJsonFile()
        {
            var openFileDialog = new OpenFileDialog();
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                path = openFileDialog.FileName;
                if (!path.Contains(".json"))
                {
                    throw new Exception("File open is not right format, please choose again (.json)");
                }
                string json = File.ReadAllText(path);
                var nodeJSONList = JsonConvert.DeserializeObject<List<ModuleObject>>(json);
                if (nodeJSONList != null)
                {
                    listInModules = nodeJSONList.Select(m => new ModuleObject()
                    {
                        id = m.id,
                        x = m.x,
                        y = m.y,
                        transformX = m.x - 70,
                        transformY = m.y - 72,
                        parameters = m.parameters,
                        coveringAreaRange = m.coveringAreaRange,
                        type = m.type,
                    }).ToList();
                }
                return true;
            }
            return false;
        }
        public bool SaveJsonFile(string json, string saveType)
        {
            if (string.IsNullOrEmpty(path) || saveType == "saveas")
            {
                var saveFileDialog = new SaveFileDialog();
                bool? response = saveFileDialog.ShowDialog();
                if (response == true)
                {
                    path = saveFileDialog.FileName;
                    if (!path.Contains(".json"))
                    {
                        throw new Exception("File is not right format, please save file with right format (.json)");
                    }
                    File.WriteAllText(path, json);
                    return true;
                }
                return false;
            }
            else
            {
                File.WriteAllText(path, json);
                return true;
            }
        }
    }
}
