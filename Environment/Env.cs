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
        public static readonly int CONFIG = 3;
        public static readonly int ACTIVE = 4;
    }
    public static class ERROR_TYPE
    {
        public static readonly int OUT_OF_RANGE = 0;
        public static readonly int PATH_LOSS = 1;
        public static readonly int COLLIDED = 2;
    }
}
