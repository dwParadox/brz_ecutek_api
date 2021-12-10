using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EcuDox.XmlData
{
    public class XmlLogParamsFile
    {
        [XmlArrayItem(typeof(XmlAnalogLogParam))]
        public XmlAnalogLogParam[] XmlLogParams { get; set; }
    }
}
