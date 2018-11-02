using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.Config
{
    public class ConfigMnager
    {

        private static bool mInited = false;

        public static Bullet Bullet = null;
        public static bool Init()
        {
            if (mInited)
                return true;
            Bullet = GetXMLConfig <Bullet>("config/bullet.xml");
            mInited = true;
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
                Debug.LogErrorFormat("failed to load xml {0}",path);
                return default(T);
            }
        }
    }
}
