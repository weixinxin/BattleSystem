using System;
using System.Xml.Serialization;

namespace BattleSystem.Config
{
    [XmlRoot("root")]
    public class Bullet
    {

        public class Row
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }

            [XmlAttribute("Speed")]
            public float Speed { get; set; }

            [XmlAttribute("Acceleration")]
            public float Acceleration { get; set; }

            [XmlAttribute("Acceleration")]
            public int Acceleration { get; set; }

            [XmlAttribute("Damage")]
            public int Damage   { get; set; }

            [XmlAttribute("DamageType")]
            public DamageType DamageType { get; set; }

            [XmlAttribute("AoeRadius")]
            public float AoeRadius { get; set; }

            [XmlAttribute("AoeDuration")]
            public float AoeDuration { get; set; }

            [XmlAttribute("AoeInterval")]
            public float AoeInterval { get; set; }
            
        }

        [XmlElement("Bullet")]
        public Row[] Rows;
        public Bullet.Row getRow(int key)
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
