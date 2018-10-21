using LuaEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ToluaDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClass();
            Console.Read();
        }
        public static void TestClass()
        {
            TestLogger logger = new TestLogger();
            InitConfig cfg = new InitConfig()
            {
                logger = logger,
                luaDir = LuaConst.luaDir,
                toluaDir = LuaConst.toluaDir,
                osDir = LuaConst.osDir,
                luaResDir = LuaConst.luaResDir,
                zbsDir = LuaConst.zbsDir,
                openToLuaLib = true,//LuaConst.openToLuaLib,
                openLuaSocket = LuaConst.openLuaSocket,
                openLuaDebugger = LuaConst.openLuaDebugger
            }  ;
            LuaInterface.Init(cfg);
            LuaClient client = LuaInterface.CreateLuaClient();
            //client.Init();

            LuaState lua = client.GetLuaState();
            string hello = @"coroutine.start(function() 
                    for i = 1, 10 do
                        coroutine.wait(1);
                        print(i);
                    end
                end);
            ";

            lua.DoString(hello, "Program.cs");
            //lua.CheckTop();

            while(true)
            {
                Thread.Sleep(50);
                client.Update(0.05f);
                client.FixedUpdate(0.05f);
                client.LateUpdate();
            }
            //lua.Dispose();
            //lua = null;
        }
        public static void TestHelloWorld()
        {

            LuaState lua = new LuaState();
            lua.Start();
            string hello = @"local a = 10; print(a); local b = a + 10;print(b)";

            lua.DoString(hello, "Program.cs");
            lua.CheckTop();
            lua.Dispose();
            lua = null;
        }


        public static string script =
            @"  function luaFunc(num)                        
                return num + 1
            end

            test = {}
            test.luaFunc = luaFunc
        ";

        public static LuaFunction luaFunc = null;
        public static LuaState lua = null;
        public static string tips = null;
        public static void TestCallLuaFunction()
        {
            lua = new LuaState();
            lua.Start();
            DelegateFactory.Init();
            lua.DoString(script);

            //Get the function object
            luaFunc = lua.GetFunction("test.luaFunc");

            if (luaFunc != null)
            {
                int num = luaFunc.Invoke<int, int>(123456);
                Console.WriteLine("generic call return: {0}", num);

                num = CallFunc();
                Console.WriteLine("expansion call return: {0}", num);

                Func<int, int> Func = luaFunc.ToDelegate<Func<int, int>>();
                num = Func(123456);
                Console.WriteLine("Delegate call return: {0}", num);

                num = lua.Invoke<int, int>("test.luaFunc", 123456, true);
                Console.WriteLine("luastate call return: {0}", num);
            }

            lua.CheckTop();
            if (luaFunc != null)
            {
                luaFunc.Dispose();
                luaFunc = null;
            }

            lua.Dispose();
            lua = null;
        }

        public static int CallFunc()
        {
            luaFunc.BeginPCall();
            luaFunc.Push(123456);
            luaFunc.PCall();
            int num = (int)luaFunc.CheckNumber();
            luaFunc.EndPCall();
            return num;
        }
    }
}
