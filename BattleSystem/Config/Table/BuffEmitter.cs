using System;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    [XmlRoot("root")]
    public class BuffEmitter
    {

        public class Row
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }
            
        }

        [XmlElement("BuffEmitter")]
        public Row[] Rows;
        public BuffEmitter.Row getRow(int key)
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
