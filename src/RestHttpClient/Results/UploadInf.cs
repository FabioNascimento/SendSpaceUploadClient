using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestHttpClient.Results
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ResultUpload
    {

        private string urlField;

        private string progress_urlField;

        private string max_file_sizeField;

        private string upload_identifierField;

        private string extra_infoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string progress_url
        {
            get
            {
                return this.progress_urlField;
            }
            set
            {
                this.progress_urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string max_file_size
        {
            get
            {
                return this.max_file_sizeField;
            }
            set
            {
                this.max_file_sizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string upload_identifier
        {
            get
            {
                return this.upload_identifierField;
            }
            set
            {
                this.upload_identifierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extra_info
        {
            get
            {
                return this.extra_infoField;
            }
            set
            {
                this.extra_infoField = value;
            }
        }
    }

}
