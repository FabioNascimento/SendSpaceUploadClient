using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestHttpClient.Results
{
    
    public class LoginResult
    {
        [XmlElementAttribute("session_key")]
        public string SessionKey { get; set; }

        [XmlElementAttribute("email")]
        public string Email { get; set; }

        [XmlElementAttribute("membership_type")]
        public string MembershipType { get; set; }

        [XmlElementAttribute("capable_upload")]
        public string CapableUpload { get; set; }

        [XmlElementAttribute("capable_download")]
        public string CapableDownload { get; set; }

        [XmlElementAttribute("capable_folders")]
        public string CapableFolder { get; set; }

        [XmlElementAttribute("capable_files")]
        public string CapableFiles { get; set; }
        [XmlElementAttribute("bandwidth_left")]
        public int BandWidthLeft { get; set; }

        [XmlElementAttribute("diskspace_left")]
        public int DiskspaceLeft { get; set; }

        [XmlElementAttribute("diskspace_used")]
        public string DiskspaceUsed { get; set; }

        [XmlElementAttribute("points")]
        public int Points { get; set; }

    }
}
