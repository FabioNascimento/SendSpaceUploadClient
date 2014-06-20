using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestHttpClient.Results
{
    public class ProgressUpload
    {
        [XmlElementAttribute("status")]
        public string Status { get; set; }

        [XmlElementAttribute("eta")]
        public string Eta { get; set; }

        [XmlElementAttribute("speed")]
        public decimal Speed { get; set; }

        [XmlElementAttribute("uploaded_bytes")]
        public double UploadedBytes { get; set; }

        [XmlElementAttribute("total_size")]
        public double TotalSize { get; set; }

        [XmlElementAttribute("elapsed")]
        public DateTime Elapsed { get; set; }

        [XmlElementAttribute("meter")]
        public double Meter { get; set; }
    }
}
