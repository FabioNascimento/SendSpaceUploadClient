using RestHttpClient.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestHttpClient.Events
{
    public class EventProgressArgs
    {
        public bool Done { get; set; }
        public ProgressUpload ProgressInfo { get; set; }
    }
}
