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
    public class MessageConstWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(MessageConst);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 28, 1, 1);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgConnected", MessageConst.MsgConnected);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgLostConnect", MessageConst.MsgLostConnect);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgDisconnected", MessageConst.MsgDisconnected);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DefaultConnectName", MessageConst.DefaultConnectName);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "LoginSeverConnect", MessageConst.LoginSeverConnect);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgNetData", MessageConst.MsgNetData);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgNetCmd", MessageConst.MsgNetCmd);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgNetMsg", MessageConst.MsgNetMsg);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateLostConnect", MessageConst.MsgUpdateLostConnect);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateSmallVersion", MessageConst.MsgUpdateSmallVersion);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateBigVersion", MessageConst.MsgUpdateBigVersion);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateNo", MessageConst.MsgUpdateNo);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateYes", MessageConst.MsgUpdateYes);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateFristCopy", MessageConst.MsgUpdateFristCopy);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateFristProgress", MessageConst.MsgUpdateFristProgress);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateDownLoad", MessageConst.MsgUpdateDownLoad);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateDownLoadUpdate", MessageConst.MsgUpdateDownLoadUpdate);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateDownLoadComplete", MessageConst.MsgUpdateDownLoadComplete);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgUpdateDownLoadError", MessageConst.MsgUpdateDownLoadError);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EventApplicationPause", MessageConst.EventApplicationPause);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgMainClick", MessageConst.MsgMainClick);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgRaceTrigger", MessageConst.MsgRaceTrigger);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgRaceSprint", MessageConst.MsgRaceSprint);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgRaceComplete", MessageConst.MsgRaceComplete);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgGuideClickComplete", MessageConst.MsgGuideClickComplete);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgOnExceptionLogEvent", MessageConst.MsgOnExceptionLogEvent);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MsgOnApplicationQuit", MessageConst.MsgOnApplicationQuit);
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "DebugNetworkIO", _g_get_DebugNetworkIO);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "DebugNetworkIO", _s_set_DebugNetworkIO);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new MessageConst();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to MessageConst constructor!");
            
        }
        
		
        
		
        
        
        
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_DebugNetworkIO(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushboolean(L, MessageConst.DebugNetworkIO);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_DebugNetworkIO(RealStatePtr L)
        {
		    try {
                
			    MessageConst.DebugNetworkIO = LuaAPI.lua_toboolean(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
