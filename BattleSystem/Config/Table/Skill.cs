using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    [XmlRoot("root")]
    public class skill
    {
        public class Row
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }

            [XmlAttribute("Desc")]
            public string Desc { get; set; }

            [XmlAttribute("Cost")]
            public int Cost { get; set; }

            [XmlAttribute("CD")]
            public float CD { get; set; }

            [XmlAttribute("Paragraph")]
            public int Paragraph { get; set; }


            [XmlAttribute("Duration")]
            public string _Duration { get; set; }
            private XMLValueArray<float> __Duration__;
            public XMLValueArray<float> Duration
            {
                get
                {
                    if (__Duration__ == null)
                    {
                        __Duration__ = _Duration;
                    }
                    return __Duration__;
                }
            }

            [XmlAttribute("AutoCast")]
            public bool AutoCast { get; set; }

            
        }

        [XmlElement("Skill")]
        public Row[] Rows;
        public skill.Row getRow(int key)
        {
            for (int i = 0; i < Rows.Length; ++i)
            {
                var row = Rows[i];
                if (row.ID == key)
                {
                    return row;
                }
            }
            return null;
        }
    }
}
