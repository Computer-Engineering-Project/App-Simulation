using Environment.Model.Module;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.VMParameter
{
    public class ModuleParameterVM
    {
        public string horizontal_x;

        public string vertical_y;

        public List<string> listPort;

        public string port;

        public string id;

        public string isUpdate;

        public string moduleType;
        public string kindOfModule;

        public ModuleObject tmp_moduleObject;

        public bool isEnableSave = false;

        public bool isEnableDelete = false;

        public bool isEnablePortSelect = false;
    }
}
