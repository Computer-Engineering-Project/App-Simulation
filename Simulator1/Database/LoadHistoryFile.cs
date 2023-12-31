﻿using Environment;
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
        public ModuleObject choosenModule = new ModuleObject();
        public LoadHistoryFile()
        {

        }
        public bool readConfigFromFile(string id, string type)
        {
            var openFileDialog = new OpenFileDialog();
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                var pathFile = openFileDialog.FileName;
                if (!pathFile.Contains(".json"))
                {
                    throw new Exception("File open is not right format, please choose again (.json)");
                }
                path = pathFile;
                string json = File.ReadAllText(path);
                var nodeJSONList = JsonConvert.DeserializeObject<List<ModuleObject>>(json);
                if (nodeJSONList != null)
                {
                    choosenModule = nodeJSONList.FirstOrDefault(x => x.type == type && x.id == id);
                }
                return true;
            }
            return false;
        }
        public bool OpenJsonFile()
        {
            var openFileDialog = new OpenFileDialog();
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                var pathFile = openFileDialog.FileName;
                if (!pathFile.Contains(".json"))
                {
                    throw new Exception("File open is not right format, please choose again (.json)");
                }
                path = pathFile;
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
                    var pathFile = saveFileDialog.FileName;
                    if (!pathFile.Contains(".json"))
                    {
                        throw new Exception("File is not right format, please save file with right format (.json)");
                    }
                    path = pathFile;
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
