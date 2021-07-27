using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseHandheld.Helpers
{
    public interface ICrashReporting
    {
        bool CrashReportingInit();
        bool CrashReportingMisc();
        void ForceCrash();
    }
}
