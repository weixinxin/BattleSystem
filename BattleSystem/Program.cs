using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleSystem.ObjectModule;
using BattleSystem.SpaceModule;
using BattleSystem.SkillModule;
using System.Diagnostics;
using System.Threading;
using BattleSystem.Config;
using LuaEngine;
namespace BattleSystem
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Test1();
            //Test2();
            //TestSkill();
            //TestLuaEngine();

            //Console.WriteLine(IsInRect(475, 995, 608, 982, -205, 268, 200, 200));
            //Test3();
            //TestGetNearestAlly();
            //TestSelectSector();
            //TestSelectRect();
            TestAI();
            Console.Read();
        }
        public static void TestAI()
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
            Debug.InitLogger(new TestLogger());
            ConfigManager.Init();
            new BattleInterface();
            BattleInterface.Instance.InitBattle(2);
            var unit1 = BattleInterface.Instance.AddUnit(100005, 0, 1);
            var unit2 = BattleInterface.Instance.AddUnit(100002, 1, 1);
            var unit3 = BattleInterface.Instance.AddUnit(100004, 1, 1);
            unit1.position = new Vector3(0, 16);
            unit2.position = new Vector3(-1, 16);
            unit3.position = new Vector3(1, 16);
            float deltaTime = 0.05f;
            while (true)
            {
                
                Thread.Sleep(50);
                BattleInterface.Instance.Update(deltaTime);
                Debug.Log("time = " + BattleInterface.Instance.GameTimeElapsed);
                LuaEngine.LuaInterface.Update(deltaTime);
                LuaInterface.FixedUpdate(deltaTime);
                LuaInterface.LateUpdate();
                Debug.Log("");
            }
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
            LuaTable buff = lua.Require<LuaTable>("buff/buff001");
            LuaFunction luaFunc = buff["new"] as LuaFunction;
            luaFunc.BeginPCall();
            luaFunc.PCall();
            LuaTable buff1 = (LuaTable)luaFunc.CheckLuaTable();
            luaFunc.EndPCall();
            luaFunc.BeginPCall();
            luaFunc.PCall();
            LuaTable buff2 = (LuaTable)luaFunc.CheckLuaTable();
            luaFunc.EndPCall();
            buff1["id"] = 10;
            buff2["id"] = 20;
            Console.WriteLine(buff1["id"]);
            Console.WriteLine(buff2["id"]);
            string hello = @"coroutine.start(function() 
                    for i = 1, 10 do
                        coroutine.wait(1);
                        print(i);
                    end
                end);
                local test = {};
                test.name = 'testClass';
                function test:func()
                    print('1000'..self.name);
                end
                return test;
            ";

            LuaTable res = lua.DoString<LuaTable>(hello, "Program.cs");
            //lua.CheckTop();
            Console.WriteLine(res["name"]);
            LuaFunction lf = res["func"] as LuaFunction;
            lf.BeginPCall();
            lf.Push(res);
            lf.PCall();
            lf.EndPCall();
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
            SkillModule.Skill skill = new SkillModule.Skill(null, 0, 1);
            skill.Cast();
            while (true)
            {
                skill.Update(0.1f);
            }
        }
        static void TestSelectSector()
        {
            int col = 20;
            int row = 20;
            float w_grid = 10;

            float w_width = col * w_grid;
            float w_height = row * w_grid;
            float testCount = 100000;
            float searchCount = 10;
            Random rnd = new Random();
            WorldSpace world = new WorldSpace();
            world.Init(w_width, w_height, w_grid, 1000);
            List<UnitBase> all = new List<UnitBase>();

            int start = 1,end = 200;
            for(int nn = start; nn <= end;nn+= 5)
            {

                float unitCount = nn;

                for (int x = 0; x < unitCount; ++x)
                {
                    UnitBase obj = new UnitBase(world, 0, 0, 0);
                    obj.position = new Vector3(rnd.Next((int)w_width), rnd.Next((int)w_height), 0);
                    all.Add(obj);
                }
                Stopwatch stopwatch1 = new Stopwatch();
                Stopwatch stopwatch2 = new Stopwatch();
                int count = 0;
                for (var i = 0; i < testCount; ++i)
                {
                    bool succeed = true;
                    float mx = rnd.Next((int)w_width);
                    float my = rnd.Next((int)w_height);
                    float ux = rnd.Next((int)w_width);

                    float uy = rnd.Next((int)w_height);
                    
                    //float mRadius = rnd.Next((int)(w_width * 0.25f));
                    //float mx = 500;
                    //float my = 500;
                    float mRadius = 20;
                    float mTheta = (float)(0.25f * Math.PI);
                    float cosht = (float)Math.Cos(0.5f * mTheta);
                    List<UnitBase> res = new List<UnitBase>(16);
                    stopwatch1.Start();
                    float sqr_raduis = mRadius * mRadius;
                    foreach (UnitBase obj in all)
                    {
                        if (IsPointInCircularSector(mx, my, ux, uy, sqr_raduis, cosht, obj.position.x, obj.position.y))
                        {
                            int a = 0;
                            for (var xx = 0; xx < searchCount; xx++)
                            {
                                a += xx;
                            }
                            res.Add(obj);
                        }
                    }
                    stopwatch1.Stop();
                    List<UnitBase> ls = new List<UnitBase>(4);
                    stopwatch2.Start();
                    world.SelectSector(mx, my, ux, uy, mRadius, mTheta, ls, (obj) =>
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

                    //Console.WriteLine(string.Format("x = {0} y = {1} ux = {2} uy = {3} r = {4} theta = {5} ", mx, my, ux, uy, mRadius, mTheta));
                    foreach (UnitBase obj in res)
                    {

                        if (!ls.Contains(obj))
                        {
                            //Console.WriteLine("No find " + obj.position.ToString());
                            succeed = false;
                        }
                    }
                    if (ls.Count != res.Count)
                        succeed = false;
                    if (succeed) count++;
                }
                Console.WriteLine("============ "+ nn +" =============");
                Console.WriteLine(string.Format("total = {0} pass = {1} pecent = {2} %", testCount, count, count * 100 / testCount));
                Console.WriteLine(string.Format(" normal = {0} code = {1} ", stopwatch1.Elapsed.TotalMilliseconds, stopwatch2.Elapsed.TotalMilliseconds));
            }


        }
        static void TestGetNearestAlly()
        {

            float w_width = 1000;
            float w_height = 1000;
            float w_grid = 10;
            float testCount = 100;
            Stopwatch stopwatch2 = new Stopwatch();
            for (var i = 0; i < testCount; ++i)
            {
                WorldSpace world = new WorldSpace();
                world.Init(w_width, w_height, w_grid, 1000);

                new BattleInterface().InitBattle(2, world);
                Random rnd = new Random();
                for (int x = 0; x < (int)(w_width / w_grid); ++x)
                {
                    for (int y = 0; y < (int)(w_height / w_grid); ++y)
                    {
                        var unit = BattleInterface.Instance.AddUnit(0, rnd.Next(0, 1), 0);
                        unit.position = new Vector3(x * w_grid + w_grid * 0.5f, y * w_grid + w_grid * 0.5f, 0);
                    }
                }

                var unit1 = BattleInterface.Instance.AddUnit(0, 1, 0);
                unit1.position = new Vector3(100f, 100f, 0);

                var unit2 = BattleInterface.Instance.AddUnit(0, 1, 0);
                unit2.position = new Vector3(200f, 100f, 0);

                //Stopwatch stopwatch1 = new Stopwatch();

                //stopwatch1.Start();
                //var res = world.GetNearestAlly(unit2.position.x, unit2.position.y, unit2.ID, unit2.CampID);
                //stopwatch1.Stop();
                //Console.WriteLine(res == unit1);
                stopwatch2.Start();
                var res2 = BattleInterface.Instance.GetNearestAlly(unit2.position.x, unit2.position.y, unit2.ID, unit2.CampID);
                stopwatch2.Stop();
            }
            Console.WriteLine(stopwatch2.Elapsed.TotalMilliseconds);
            //Console.WriteLine(string.Format(" code = {0} normal = {1}", stopwatch1.Elapsed.TotalMilliseconds, stopwatch2.Elapsed.TotalMilliseconds));
        }
        
        static void Test2()
        {
            float w_width = 1000;
            float w_height = 1000;
            float w_grid = 10;
            float testCount = 100;
            float searchCount = 20;
            WorldSpace world = new WorldSpace();
            world.Init(w_width, w_height, w_grid,1000);
            List<UnitBase> all = new List<UnitBase>();
            for (int x = 0; x < (int)(w_width / w_grid); ++x)
            {
                for (int y = 0; y < (int)(w_height / w_grid); ++y)
                {
                    UnitBase obj = new UnitBase(world, 0, 0, 0);
                    obj.position = new Vector3(x * w_grid + w_grid * 0.5f, y * w_grid + w_grid * 0.5f, 0);
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
                List<UnitBase> res = new List<UnitBase>(16);
                stopwatch1.Start();
                float sqr_raduis = mRadius * mRadius;
                foreach (UnitBase obj in all)
                {
                    var dis = (obj.position.x - mx) * (obj.position.x - mx) + (obj.position.y - my) * (obj.position.y - my);
                    if (dis <= sqr_raduis)
                    {
                        int a = 0;
                        for (var xx = 0; xx < searchCount; xx++)
                        {
                            a += xx;
                        }
                        res.Add(obj);
                    }
                }
                stopwatch1.Stop();
                List<UnitBase> ls = new List<UnitBase>(4);
                stopwatch2.Start();
                world.SelectCircle(mx, my, mRadius, ls, (obj) =>
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
                foreach (UnitBase obj in res)
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
            List<UnitBase> all = new List<UnitBase>();
            for (int x = 0; x < (int)(w_width / w_grid); ++x)
            {
                for (int y = 0; y < (int)(w_height / w_grid); ++y)
                {
                    UnitBase obj = new UnitBase(world, 0, 0, 0);
                    obj.position = new Vector3(x * w_grid + w_grid * 0.5f, y * w_grid + w_grid * 0.5f, 0);
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
                List<UnitBase> res = new List<UnitBase>(16);
                stopwatch1.Start();
                foreach (UnitBase obj in all)
                {
                    if (obj.position.x >= mx && obj.position.x <= mx + mwidth && obj.position.y >= my && obj.position.y <= my + mheight)
                    {
                        int a = 0;
                        for (var xx = 0; xx < searchCount; xx++)
                        {
                            a += xx;
                        }
                        res.Add(obj);
                    }
                }
                stopwatch1.Stop();
                List<UnitBase> ls = new List<UnitBase>(4);
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
                foreach (UnitBase obj in res)
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
         
        static void TestSelectRect()
        {


            int col = 20;
            int row = 20;
            float w_grid = 10;

            float w_width = col * w_grid;
            float w_height = row * w_grid;
            float testCount = 100000;
            float searchCount = 10;
            Random rnd = new Random();
            WorldSpace world = new WorldSpace();
            world.Init(w_width, w_height, w_grid, 1000);
            List<UnitBase> all = new List<UnitBase>();

            int start = 1, end = 200;
            for (int nn = start; nn <= end; nn += 5)
            {

                float unitCount = nn;

                for (int x = 0; x < unitCount; ++x)
                {
                    UnitBase obj = new UnitBase(world, 0, 0, 0);
                    obj.position = new Vector3(rnd.Next((int)w_width), rnd.Next((int)w_height), 0);
                    all.Add(obj);
                }
                Stopwatch stopwatch1 = new Stopwatch();
                Stopwatch stopwatch2 = new Stopwatch();
                int count = 0;
                for (var i = 0; i < testCount; ++i)
                {
                    bool succeed = true;
                    float mx = rnd.Next((int)w_width);
                    float my = rnd.Next((int)w_height);
                    float ux = rnd.Next(1,(int)w_width);

                    float uy = rnd.Next(1,(int)w_height);

                    //float mRadius = rnd.Next((int)(w_width * 0.25f));
                    //float mx = 500;
                    //float my = 500;
                    float mwidth = 50;
                    float mheight = 50;
                    List<UnitBase> res = new List<UnitBase>(16);
                    stopwatch1.Start();
                    foreach (UnitBase obj in all)
                    {
                        if (IsInRect(obj.position.x, obj.position.y, mx, my, ux, uy, mwidth, mheight))
                        {
                            int a = 0;
                            for (var xx = 0; xx < searchCount; xx++)
                            {
                                a += xx;
                            }
                            res.Add(obj);
                        }
                    }
                    stopwatch1.Stop();
                    List<UnitBase> ls = new List<UnitBase>(4);
                    stopwatch2.Start();
                    world.SelectRect(mx, my, ux, uy, mwidth, mheight, ls, (obj) =>
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

                    //Console.WriteLine(string.Format("x = {0} y = {1} ux = {2} uy = {3} r = {4} theta = {5} ", mx, my, ux, uy, mRadius, mTheta));
                    foreach (UnitBase obj in res)
                    {

                        if (!ls.Contains(obj))
                        {
                            //Console.WriteLine("No find " + obj.position.ToString());
                            succeed = false;
                        }
                    }
                    if (ls.Count != res.Count)
                        succeed = false;
                    if (succeed) count++;
                }
                Console.WriteLine("============ " + nn + " =============");
                Console.WriteLine(string.Format("total = {0} pass = {1} pecent = {2} %", testCount, count, count * 100 / testCount));
                Console.WriteLine(string.Format(" normal = {0} code = {1} ", stopwatch1.Elapsed.TotalMilliseconds, stopwatch2.Elapsed.TotalMilliseconds));
            }
        }


        static float TriangleArea(float v0x, float v0y, float v1x, float v1y, float v2x, float v2y)
        {
            //float v = (v0x * v1y + v1x * v2y + v2x * v0y
            //    - v1x * v0y - v2x * v1y - v0x * v2y) * 0.5f;
            var a = (decimal)(v0x * v1y);
            var b = (decimal)(v1x * v2y);
            var c = (decimal)(v2x * v0y);
            var d = (decimal)(v1x * v0y);
            var e = (decimal)(v2x * v1y);
            var f = (decimal)(v0x * v2y);
            var g = a + b + c - d - e - f;
            var v = decimal.ToSingle(g) * 0.5f;
            return Mathf.Abs(v);
        }
        public static bool IsInTriangle(float x, float y, float v0x, float v0y, float v1x, float v1y, float v2x, float v2y)
        {
            float t = TriangleArea(v0x, v0y, v1x, v1y, v2x, v2y);
            float a = TriangleArea(v0x, v0y, v1x, v1y, x, y) + TriangleArea(v0x, v0y, x, y, v2x, v2y) + TriangleArea(x, y, v1x, v1y, v2x, v2y);

            if (Mathf.Abs(t - a) <= 0.01f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public static bool IsInRect(float px, float py,float x,float y,float dx,float dy,float width,float height)
        {

            var l_d = 1 / Math.Sqrt(dx * dx + dy * dy);
            var n_dx = dx * l_d;
            var n_dy = dy * l_d;

            var a_x = x - n_dy * 0.5f * width;
            var a_y = y + n_dx * 0.5f * width;

            var b_x = a_x + n_dy * width;
            var b_y = a_y - n_dx * width;

            var c_x = a_x + n_dx * height;
            var c_y = a_y + n_dy * height;

            var d_x = c_x + n_dy * width;
            var d_y = c_y - n_dx * width;

            if (IsInTriangle(px, py, (float)a_x, (float)a_y, (float)b_x, (float)b_y, (float)c_x, (float)c_y) || IsInTriangle(px, py, (float)d_x, (float)d_y, (float)b_x, (float)b_y, (float)c_x, (float)c_y))
            {
                return true;
            }
            return false;
        }
        static bool IsPointInCircularSector(float cx, float cy, float ux, float uy, float squaredR, float cosTheta,float px, float py)
        {
            float u_l = (float)Math.Sqrt(ux * ux + uy * uy);
            ux /= u_l;
            uy /= u_l;
            float dx = px - cx;
            float dy = py - cy;
            // |D|^2 = (dx^2 + dy^2)
            float squaredLength = dx * dx + dy * dy;

            // |D|^2 > r^2
            if (squaredLength > squaredR)

                return false;

            // D dot U

            float DdotU = dx * ux + dy * uy;



            // D dot U > |D| cos(theta)

            // <=>

            // (D dot U)^2 > |D|^2 (cos(theta))^2 if D dot U >= 0 and cos(theta) >= 0

            // (D dot U)^2 < |D|^2 (cos(theta))^2 if D dot U <  0 and cos(theta) <  0

            // true                               if D dot U >= 0 and cos(theta) <  0

            // false                              if D dot U <  0 and cos(theta) >= 0

            if (DdotU >= 0 && cosTheta >= 0)
                return DdotU * DdotU > squaredLength * cosTheta * cosTheta;
            else if (DdotU < 0 && cosTheta < 0)
                return DdotU * DdotU < squaredLength * cosTheta * cosTheta;
            else
                return DdotU >= 0;

        }

    }

}