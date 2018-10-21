using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaEngine
{
    public struct InitConfig
    {
        /// <summary>
        /// lua逻辑代码目录
        /// </summary>
        public string luaDir;

        /// <summary>
        /// tolua lua文件目录
        /// </summary>
        public string toluaDir;

        /// <summary>
        /// 平台目录
        /// </summary>
        public string osDir;

        /// <summary>
        /// 手机运行时lua文件下载目录
        /// </summary>
        public string luaResDir;  

        /// <summary>
        /// ZeroBraneStudio目录
        /// </summary>
        public string zbsDir;

        /// <summary>
        /// 是否打开toLua库
        /// </summary>
        public bool openToLuaLib;

        /// <summary>
        /// 是否打开Lua Socket库
        /// </summary>
        public bool openLuaSocket;

        /// <summary>
        /// 是否连接lua调试器
        /// </summary>
        public bool openLuaDebugger;

        public ILogger logger;
    }
    public static class LuaInterface
    {
        private static bool mInited = false;

        private static List<LuaClient> mList = new List<LuaClient>();
        public static void Init(InitConfig cfg)
        {
            LuaConst.luaDir = cfg.luaDir;
            LuaConst.toluaDir = cfg.toluaDir;
            LuaConst.osDir = cfg.osDir;
            LuaConst.luaResDir = cfg.luaResDir;
            LuaConst.zbsDir = cfg.zbsDir;
            LuaConst.openToLuaLib = cfg.openToLuaLib;
            LuaConst.openLuaSocket = cfg.openLuaSocket;
            LuaConst.openLuaDebugger = cfg.openLuaDebugger;
            Debug.InitLogger(cfg.logger);
            mInited = true;
        }
        public static void UnInit()
        {
            for (var i = 0; i < mList.Count; ++i)
            {
                mList[i].Destroy();
            }
            mList.Clear();
            mInited = false;
        }
        public static LuaClient CreateLuaClient()
        {
            if(!mInited)
            {
                throw new Exception("you should call LuaInterface.Init first!");
            }
            var client = new LuaClient();
            mList.Add(client);
            return client;
        }
        public static void DestroyLuaClient(LuaClient client)
        {
            if(mList.Contains(client))
            {
                client.Destroy();
                mList.Remove(client);
            }
            else
            {
                throw new Exception("do not find LuaClient!");
            }
        }
        public static void Update(float deltaTime)
        {
            for(var i = 0; i < mList.Count;++i)
            {
                mList[i].Update(deltaTime);
            }
        }
        public static void LateUpdate()
        {
            for (var i = 0; i < mList.Count; ++i)
            {
                mList[i].LateUpdate();
            }
        }
        public static void FixedUpdate(float fixedDeltaTime)
        {
            for (var i = 0; i < mList.Count; ++i)
            {
                mList[i].Update(fixedDeltaTime);
            }
        }
    }
}
