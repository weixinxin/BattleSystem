using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleSystem.ObjectModule;
using BattleSystem.SpaceModule;
using BattleSystem.SkillModule;
using System.Diagnostics;
using LuaEngine;
using System.Threading;
namespace BattleSystem
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Test1();
            //Test2();
            //TestSkill();
            TestLuaEngine();
            Console.Read();
        }
        public static void TestLuaEngine()
        {
            var logger = new ToluaDemo.TestLogger();
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
            };
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

            while (true)
            {
                Thread.Sleep(50);
                LuaInterface.Update(0.05f);
                LuaInterface.FixedUpdate(0.05f);
                LuaInterface.LateUpdate();
            }
            //lua.Dispose();
            //lua = null;
        }
        static void TestSkill()
        {
            Debug.InitLogger(new TestLogger());
            Skill skill = new Skill(0, 0, 1);
            skill.Cast();
            while (true)
            {
                skill.Update(0.1f);
            }
        }
        /*
        static void Test2()
        {
            float w_width = 1000;
            float w_height = 1000;
            float w_grid = 10;
            float testCount = 100;
            float searchCount = 20;
            WorldSpace world = new WorldSpace();
            world.Init(w_width, w_height, w_grid,1000);
            List<BaseObject> all = new List<BaseObject>();
            for (int x = 0; x < (int)(w_width / w_grid); ++x)
            {
                for (int y = 0; y < (int)(w_height / w_grid); ++y)
                {
                    BaseObject obj = new BaseObject(world);
                    obj.UpdatePosition(x * w_grid + w_grid * 0.5f, y * w_grid + w_grid * 0.5f, 0);
                    all.Add(obj);
                    //Console.WriteLine(obj.position.ToString());
                }
            }
            bool succeed = true;
            Stopwatch stopwatch1 = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();

            Random rnd = new Random();
            for (var i = 0; i < testCount; ++i)
            {

                float mx = rnd.Next((int)w_width);
                float my = rnd.Next((int)w_height);
                //float mRadius = rnd.Next((int)(w_width * 0.25f));
                //float mx = 500;
                //float my = 500;
                float mRadius = 100;
                List<BaseObject> res = new List<BaseObject>(16);
                stopwatch1.Start();
                float sqr_raduis = mRadius * mRadius;
                foreach (BaseObject obj in all)
                {
                    int a = 0;
                    for (var xx = 0; xx < searchCount; xx++)
                    {
                        a += xx;
                    }
                    var dis = (obj.position.x - mx) * (obj.position.x - mx) + (obj.position.y - my) * (obj.position.y - my);
                    if (dis <= sqr_raduis)
                    {
                        res.Add(obj);
                    }
                }
                stopwatch1.Stop();
                List<BaseObject> ls = new List<BaseObject>(4);
                stopwatch2.Start();
                world.Select(mx, my, mRadius, ls, (obj) =>
                {
                    int a = 0;
                    for (var xx = 0; xx < searchCount; xx++)
                    {
                        a += xx;
                    }
                    return true;
                });
                stopwatch2.Stop();
                //Console.WriteLine("=========================");

                //Console.WriteLine(string.Format("x = {0} y = {1} r = {2} ", mx, my, mRadius));
                foreach (BaseObject obj in res)
                {

                    if (!ls.Contains(obj))
                    {
                        Console.WriteLine("No find " + obj.position.ToString());
                        succeed = false;
                    }
                }
                if (ls.Count != res.Count)
                    succeed = false;
            }
            Console.WriteLine(succeed ? "Test2 pass" : "Test2 error");
            TimeSpan timespan1 = stopwatch1.Elapsed;
            TimeSpan timespan2 = stopwatch2.Elapsed;
            Console.WriteLine(string.Format(" normal = {0} code = {1} ", timespan1.TotalMilliseconds, timespan2.TotalMilliseconds));
        }
        static void Test1()
        {
            float w_width = 1000;
            float w_height = 1000;
            float w_grid = 10;
            float testCount = 100;
            float searchCount = 20;
            WorldSpace world = new WorldSpace();
            world.Init(w_width, w_height, w_grid,1000);
            List<BaseObject> all = new List<BaseObject>();
            for (int x = 0; x < (int)(w_width / w_grid); ++x)
            {
                for (int y = 0; y < (int)(w_height / w_grid); ++y)
                {
                    BaseObject obj = new BaseObject(world);
                    obj.UpdatePosition(x * w_grid + w_grid * 0.5f, y * w_grid + w_grid * 0.5f, 0);
                    all.Add(obj);
                    //Console.WriteLine(obj.position.ToString());
                }
            }
            bool succeed = true;
            Stopwatch stopwatch1 = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();

            Random rnd = new Random();
            for (var i = 0; i < testCount; ++i)
            {

                float mx = rnd.Next((int)w_width);
                float my = rnd.Next((int)w_height);
                //float mwidth =  rnd.Next((int)(w_width * 0.5f));
                //float mheight =  rnd.Next((int)(w_height * 0.5f));
                //float mx = -100;
                //float my = -100;
                float mwidth = 200;
                float mheight = 200;
                List<BaseObject> res = new List<BaseObject>(16);
                stopwatch1.Start();
                foreach (BaseObject obj in all)
                {
                    int a = 0;
                    for (var xx = 0; xx < searchCount; xx++)
                    {
                        a += xx;
                    }
                    if (obj.position.x >= mx && obj.position.x <= mx + mwidth && obj.position.y >= my && obj.position.y <= my + mheight)
                    {
                        res.Add(obj);
                    }
                }
                stopwatch1.Stop();
                List<BaseObject> ls = new List<BaseObject>(4);
                stopwatch2.Start();
                world.Select(mx, my, mwidth, mheight, ls, (obj) =>
                {
                    int a = 0;
                    for (var xx = 0; xx < searchCount; xx++)
                    {
                        a += xx;
                    }
                    return true;
                });
                stopwatch2.Stop();
                
                //Console.WriteLine("=========================");

                //Console.WriteLine(string.Format("x = {0} y = {1} r = {2} t = {3}", mx, my, mx + mwidth, my + mheight));
                foreach (BaseObject obj in res)
                {

                    if (!ls.Contains(obj))
                    {
                        Console.WriteLine("No find " + obj.position.ToString());
                        succeed = false;
                    }
                }
                 
            }
            Console.WriteLine(succeed ? "Test1 pass" : "Test1 error");
            TimeSpan timespan1 = stopwatch1.Elapsed;
            TimeSpan timespan2 = stopwatch2.Elapsed;
            Console.WriteLine(string.Format(" normal = {0} code = {1}",stopwatch1.Elapsed.TotalMilliseconds,stopwatch2.Elapsed.TotalMilliseconds));
        }
         */
    }

}