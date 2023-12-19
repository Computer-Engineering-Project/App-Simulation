using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment
{
    public static class PROGRAM_STATUS
    {
        public static readonly int IDLE = 0;
        public static readonly int RUN = 1;
        public static readonly int PAUSE = 2;
    }
    public static class MODE_MODULE
    {
        public static readonly int ACTIVE = 0;
        public static readonly int READ_CONFIG = 1;
        public static readonly int CONFIG = 2;
        public static readonly int CHANGE_MODE = 3;
        public static readonly int SEND_DATA = 4;
    }
    public static class ERROR_TYPE
    {
        public static readonly int OUT_OF_RANGE = 1;
        public static readonly int PATH_LOSS = 2;
        public static readonly int COLLIDED = 3;
    }
    
    public static class EnvState
    {
        public static int PreProgramStatus;
        public static int ProgramStatus;
        public static int ModeModule;
        public static int ErrorType;
    }
}
