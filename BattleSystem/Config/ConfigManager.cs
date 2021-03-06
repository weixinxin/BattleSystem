﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.Config
{
    public class ConfigManager
    {

        private static bool mInited = false;
        public static Animators Animator = null;
        public static bullet Bullet = null;
        public static buffEmitter BuffEmitter = null;
        public static unit Unit = null;
        public static buff Buff = null;
        public static buffEffect BuffEffect = null;
        public static skill Skill = null;
        
        public static bool Init()
        {
            if (mInited)
                return true;
            Debug.Log("ConfigManager start init ...");
            Animator = GetXMLConfig<Animators>("config/animator.xml");
            Bullet = GetXMLConfig <bullet>("config/bullet.xml");
            BuffEmitter = GetXMLConfig<buffEmitter>("config/buffemitter.xml");
            Unit = GetXMLConfig<unit>("config/unit.xml");
            Buff = GetXMLConfig<buff>("config/buff.xml");
            BuffEffect = GetXMLConfig<buffEffect>("config/buffeffect.xml");
            Skill = GetXMLConfig<skill>("config/skill.xml");
            
            mInited = true;
            Debug.Log("ConfigManager init succeed ！");
            return true;
        }
        private static T GetXMLConfig<T>(string path)
        {

            try
            {
                T ret = Util.Utils.SerializeFromString<T>(Util.Utils.LoadFile(path));
                return ret;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("failed to load xml {0}\n{1}",path,e.ToString());
                return default(T);
            }
        }
    }

    public class XMLValueArray<T>
    {
        private T[] _List;
        public XMLValueArray()
        {

        }
        public XMLValueArray(T[] list)
        {
            _List = list;
        }
        private bool ParseFromString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            Type type = typeof(T);
            string[] sublist = text.Split('|');
            if (sublist.Length > 0)
            {
                _List = new T[sublist.Length];
                for (int i = 0; i < sublist.Length; ++i)
                {
                    string s = sublist[i];
                    try
                    {
                        _List[i] = (T)Convert.ChangeType(s, type);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(string.Format("XMLValueArray.ParseFromString: failed parse {0} to type {1}, text = {2}", s, type, text));
                    }
                }
            }
            return true;
        }
        private string ConvertToString()
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < _List.Length; ++i)
            {
                if (i > 0)
                    text.Append("|");
                text.Append(_List[i]);
            }
            return text.ToString();
        }
        public T this[int index]
        {
            get
            {
                return _List[index];
            }
            set
            {
                _List[index] = value;
            }
        }
        public int Length
        {
            get
            {
                return _List.Length;
            }
        }
        public override string ToString()
        {
            return ConvertToString();
        }

        public static implicit operator string(XMLValueArray<T> array)
        {
            if (array == null)
                return null;
            return array.ConvertToString();
        }
        public static implicit operator XMLValueArray<T>(string text)
        {
            XMLValueArray<T> array = new XMLValueArray<T>();
            if (array.ParseFromString(text))
            {
                return array;
            }
            return null;
        }
    }
}
