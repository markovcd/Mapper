﻿using System.Xml.Serialization;

namespace Mapper
{
    public class ContentMapping : Mapping
    {
        [XmlAttribute]
        public string Content { get; set; }

        public string GetValue()
        {
            return Content;
        }
    }
}
