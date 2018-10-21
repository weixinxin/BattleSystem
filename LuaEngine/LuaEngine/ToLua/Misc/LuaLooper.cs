/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
//using UnityEngine;
namespace LuaEngine
{
    public class LuaLooper //: MonoBehaviour 
    {
        public LuaBeatEvent UpdateEvent
        {
            get;
            private set;
        }

        public LuaBeatEvent LateUpdateEvent
        {
            get;
            private set;
        }

        public LuaBeatEvent FixedUpdateEvent
        {
            get;
            private set;
        }


        private LuaFunction UpdateFunc = null;
        private LuaFunction LateUpdateFunc = null;
        private LuaFunction FixedUpdateFunc = null;

        public LuaState luaState = null;

        internal void Start()
        {
            if (LuaConst.openToLuaLib)
            {
                try
                {
                    UpdateEvent = GetEvent("UpdateBeat");
                    LateUpdateEvent = GetEvent("LateUpdateBeat");
                    FixedUpdateEvent = GetEvent("FixedUpdateBeat");

                    UpdateFunc = luaState.GetFunction("Update");
                    LateUpdateFunc = luaState.GetFunction("LateUpdate");
                    FixedUpdateFunc = luaState.GetFunction("FixedUpdate");
                }
                catch (Exception e)
                {
                    //Destroy(this);
                    Destroy();
                }
            }
        }

        LuaBeatEvent GetEvent(string name)
        {
            LuaTable table = luaState.GetTable(name);

            if (table == null)
            {
                throw new LuaException(string.Format("Lua table {0} not exists", name));
            }

            LuaBeatEvent e = new LuaBeatEvent(table);
            table.Dispose();
            table = null;
            return e;
        }

        void ThrowException()
        {
            string error = luaState.LuaToString(-1);
            luaState.LuaPop(2);
            throw new LuaException(error, LuaException.GetLastError());
        }

        //    void Update()
        //    {
        //#if UNITY_EDITOR
        //        if (luaState == null)
        //        {
        //            return;
        //        }
        //#endif
        //        if (luaState.LuaUpdate(Time.deltaTime, Time.unscaledDeltaTime) != 0)
        //        {
        //            ThrowException();
        //        }

        //        luaState.LuaPop(1);
        //        luaState.Collect();
        //#if UNITY_EDITOR
        //        luaState.CheckTop();
        //#endif
        //    }
        internal void Update(float deltaTime, float unscaled)
        {
            //if (luaState.LuaUpdate(deltaTime, unscaled) != 0)
            //{
            //    ThrowException();
            //}

            UpdateFunc.BeginPCall();
            UpdateFunc.Push(deltaTime);
            UpdateFunc.Push(unscaled);
            UpdateFunc.PCall();
            UpdateFunc.EndPCall();
            luaState.LuaPop(1);
            luaState.Collect();
        }
        internal void LateUpdate()
        {
            //#if UNITY_EDITOR
            if (luaState == null)
            {
                return;
            }
            //#endif
            //if (luaState.LuaLateUpdate() != 0)
            //{
            //    ThrowException();
            //}

            LateUpdateFunc.BeginPCall();
            LateUpdateFunc.PCall();
            LateUpdateFunc.EndPCall();

            luaState.LuaPop(1);
        }

        //    void FixedUpdate()
        //    {
        //#if UNITY_EDITOR
        //        if (luaState == null)
        //        {
        //            return;
        //        }
        //#endif
        //        if (luaState.LuaFixedUpdate(Time.fixedDeltaTime) != 0)
        //        {
        //            ThrowException();
        //        }

        //        luaState.LuaPop(1);
        //    }
        internal void FixedUpdate(float fixedDeltaTime)
        {
            //if (luaState.LuaFixedUpdate(fixedDeltaTime) != 0)
            //{
            //    ThrowException();
            //}

            FixedUpdateFunc.BeginPCall();
            FixedUpdateFunc.Push(fixedDeltaTime);
            FixedUpdateFunc.PCall();
            FixedUpdateFunc.EndPCall();
            luaState.LuaPop(1);
        }

        public void Destroy()
        {
            if (luaState != null)
            {
                if (UpdateEvent != null)
                {
                    UpdateEvent.Dispose();
                    UpdateEvent = null;
                }

                if (LateUpdateEvent != null)
                {
                    LateUpdateEvent.Dispose();
                    LateUpdateEvent = null;
                }

                if (FixedUpdateEvent != null)
                {
                    FixedUpdateEvent.Dispose();
                    FixedUpdateEvent = null;
                }

                luaState = null;
            }
        }

        void OnDestroy()
        {
            if (luaState != null)
            {
                Destroy();
            }
        }
    }
}