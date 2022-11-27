#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class NetManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(NetManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 15, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Init", _m_Init);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Update", _m_Update);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetConnectInfo", _m_GetConnectInfo);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DisConnect", _m_DisConnect);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsConnected", _m_IsConnected);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnConnected", _m_OnConnected);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnLostConnect", _m_OnLostConnect);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnDisconnected", _m_OnDisconnected);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnReceiveData", _m_OnReceiveData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ConnectTo", _m_ConnectTo);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DisConnectFrom", _m_DisConnectFrom);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DisConnectAll", _m_DisConnectAll);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsConnectedTo", _m_IsConnectedTo);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendTo", _m_SendTo);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnApplicationQuit", _m_OnApplicationQuit);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new NetManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to NetManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Init(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Init(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Update(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Update(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetConnectInfo(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _targetName = LuaAPI.lua_tostring(L, 2);
                    string _ip;
                    int _port;
                    
                        var gen_ret = gen_to_be_invoked.GetConnectInfo( _targetName, out _ip, out _port );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    LuaAPI.lua_pushstring(L, _ip);
                        
                    LuaAPI.xlua_pushinteger(L, _port);
                        
                    
                    
                    
                    return 3;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DisConnect(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.DisConnect(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsConnected(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.IsConnected(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnConnected(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    IConnect _connect = (IConnect)translator.GetObject(L, 2, typeof(IConnect));
                    
                    gen_to_be_invoked.OnConnected( _connect );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnLostConnect(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    IConnect _connect = (IConnect)translator.GetObject(L, 2, typeof(IConnect));
                    
                    gen_to_be_invoked.OnLostConnect( _connect );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnDisconnected(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    IConnect _connect = (IConnect)translator.GetObject(L, 2, typeof(IConnect));
                    
                    gen_to_be_invoked.OnDisconnected( _connect );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnReceiveData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    IConnect _connect = (IConnect)translator.GetObject(L, 2, typeof(IConnect));
                    int _id = LuaAPI.xlua_tointeger(L, 3);
                    object _obj = translator.GetObject(L, 4, typeof(object));
                    
                    gen_to_be_invoked.OnReceiveData( _connect, _id, _obj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ConnectTo(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _targetName = LuaAPI.lua_tostring(L, 2);
                    string _targetIP = LuaAPI.lua_tostring(L, 3);
                    int _targetPort = LuaAPI.xlua_tointeger(L, 4);
                    
                    gen_to_be_invoked.ConnectTo( _targetName, _targetIP, _targetPort );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DisConnectFrom(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _targetName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.DisConnectFrom( _targetName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DisConnectAll(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.DisConnectAll(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsConnectedTo(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _targetName = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.IsConnectedTo( _targetName );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendTo(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<object>(L, 4)) 
                {
                    string _targetName = LuaAPI.lua_tostring(L, 2);
                    int _packetId = LuaAPI.xlua_tointeger(L, 3);
                    object _data = translator.GetObject(L, 4, typeof(object));
                    
                    gen_to_be_invoked.SendTo( _targetName, _packetId, _data );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& (LuaAPI.lua_isnil(L, 4) || LuaAPI.lua_type(L, 4) == LuaTypes.LUA_TSTRING)) 
                {
                    string _targetName = LuaAPI.lua_tostring(L, 2);
                    int _packetId = LuaAPI.xlua_tointeger(L, 3);
                    byte[] _data = LuaAPI.lua_tobytes(L, 4);
                    
                    gen_to_be_invoked.SendTo( _targetName, _packetId, _data );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to NetManager.SendTo!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnApplicationQuit(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetManager gen_to_be_invoked = (NetManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.OnApplicationQuit(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
