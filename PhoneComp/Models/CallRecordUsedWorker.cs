using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneComp.Models
{
    public class CallRecordUsedWorker
    {
        public int CallRecordUsedWorkerID { get; set; }

        public virtual CallRecord CallRecord { get; set; }
        public virtual UsedWorker UsedWorker { get; set; }
    }
}