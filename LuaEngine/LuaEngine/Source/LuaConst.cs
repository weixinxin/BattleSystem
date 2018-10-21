//using UnityEngine;

public static class LuaConst
{
    public static string luaDir = "../Lua"; //Application.dataPath + "/Lua";                //lua逻辑代码目录
    public static string toluaDir = "../ToLua/Lua"; //Application.dataPath + "/ToLua/Lua";        //tolua lua文件目录

    public static string osDir = "Windows"; 

    public static string luaResDir = string.Format("{0}/Lua", System.IO.Directory.GetCurrentDirectory());      //手机运行时lua文件下载目录 

    public static string zbsDir = luaResDir + "/mobdebug/";  //ZeroBraneStudio目录

    public static bool openToLuaLib = false;            //是否打开toLua库

    public static bool openLuaSocket = true;            //是否打开Lua Socket库
    public static bool openLuaDebugger = false;         //是否连接lua调试器
}