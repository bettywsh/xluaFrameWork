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
    public class ResManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ResManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 6, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CreateResLoader", _m_CreateResLoader);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnLoadAsset", _m_OnLoadAsset);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadAssetAsync", _m_LoadAssetAsync);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddReloader", _m_AddReloader);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UnLoadAssetBundle", _m_UnLoadAssetBundle);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DestroyCache", _m_DestroyCache);
			
			
			
			
			
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
					
					var gen_ret = new ResManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ResManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateResLoader(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ResManager gen_to_be_invoked = (ResManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.CreateResLoader(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnLoadAsset(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ResManager gen_to_be_invoked = (ResManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _resName = LuaAPI.lua_tostring(L, 2);
                    string _relativePath = LuaAPI.lua_tostring(L, 3);
                    ResType _resType;translator.Get(L, 4, out _resType);
                    
                        var gen_ret = gen_to_be_invoked.OnLoadAsset( _resName, _relativePath, _resType );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetAsync(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ResManager gen_to_be_invoked = (ResManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 6&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ResType>(L, 4)&& translator.Assignable<System.Action<UnityEngine.Object>>(L, 5)&& (LuaAPI.lua_isnil(L, 6) || LuaAPI.lua_type(L, 6) == LuaTypes.LUA_TFUNCTION)) 
                {
                    string _resName = LuaAPI.lua_tostring(L, 2);
                    string _relativePath = LuaAPI.lua_tostring(L, 3);
                    ResType _resType;translator.Get(L, 4, out _resType);
                    System.Action<UnityEngine.Object> _sharpFunc = translator.GetDelegate<System.Action<UnityEngine.Object>>(L, 5);
                    XLua.LuaFunction _luaFunc = (XLua.LuaFunction)translator.GetObject(L, 6, typeof(XLua.LuaFunction));
                    
                    gen_to_be_invoked.LoadAssetAsync( _resName, _relativePath, _resType, _sharpFunc, _luaFunc );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 5&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ResType>(L, 4)&& translator.Assignable<System.Action<UnityEngine.Object>>(L, 5)) 
                {
                    string _resName = LuaAPI.lua_tostring(L, 2);
                    string _relativePath = LuaAPI.lua_tostring(L, 3);
                    ResType _resType;translator.Get(L, 4, out _resType);
                    System.Action<UnityEngine.Object> _sharpFunc = translator.GetDelegate<System.Action<UnityEngine.Object>>(L, 5);
                    
                    gen_to_be_invoked.LoadAssetAsync( _resName, _relativePath, _resType, _sharpFunc );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ResType>(L, 4)) 
                {
                    string _resName = LuaAPI.lua_tostring(L, 2);
                    string _relativePath = LuaAPI.lua_tostring(L, 3);
                    ResType _resType;translator.Get(L, 4, out _resType);
                    
                    gen_to_be_invoked.LoadAssetAsync( _resName, _relativePath, _resType );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 6&& translator.Assignable<ResLoader>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ResType>(L, 4)&& translator.Assignable<System.Action<UnityEngine.Object>>(L, 5)&& (LuaAPI.lua_isnil(L, 6) || LuaAPI.lua_type(L, 6) == LuaTypes.LUA_TFUNCTION)) 
                {
                    ResLoader _resLoader = (ResLoader)translator.GetObject(L, 2, typeof(ResLoader));
                    string _relativePath = LuaAPI.lua_tostring(L, 3);
                    ResType _resType;translator.Get(L, 4, out _resType);
                    System.Action<UnityEngine.Object> _sharpFunc = translator.GetDelegate<System.Action<UnityEngine.Object>>(L, 5);
                    XLua.LuaFunction _luaFunc = (XLua.LuaFunction)translator.GetObject(L, 6, typeof(XLua.LuaFunction));
                    
                    gen_to_be_invoked.LoadAssetAsync( _resLoader, _relativePath, _resType, _sharpFunc, _luaFunc );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 5&& translator.Assignable<ResLoader>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ResType>(L, 4)&& translator.Assignable<System.Action<UnityEngine.Object>>(L, 5)) 
                {
                    ResLoader _resLoader = (ResLoader)translator.GetObject(L, 2, typeof(ResLoader));
                    string _relativePath = LuaAPI.lua_tostring(L, 3);
                    ResType _resType;translator.Get(L, 4, out _resType);
                    System.Action<UnityEngine.Object> _sharpFunc = translator.GetDelegate<System.Action<UnityEngine.Object>>(L, 5);
                    
                    gen_to_be_invoked.LoadAssetAsync( _resLoader, _relativePath, _resType, _sharpFunc );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& translator.Assignable<ResLoader>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ResType>(L, 4)) 
                {
                    ResLoader _resLoader = (ResLoader)translator.GetObject(L, 2, typeof(ResLoader));
                    string _relativePath = LuaAPI.lua_tostring(L, 3);
                    ResType _resType;translator.Get(L, 4, out _resType);
                    
                    gen_to_be_invoked.LoadAssetAsync( _resLoader, _relativePath, _resType );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ResManager.LoadAssetAsync!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddReloader(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ResManager gen_to_be_invoked = (ResManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _resName = LuaAPI.lua_tostring(L, 2);
                    string _abName = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.AddReloader( _resName, _abName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnLoadAssetBundle(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ResManager gen_to_be_invoked = (ResManager)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _resLoaderName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.UnLoadAssetBundle( _resLoaderName );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ResType>(L, 3)) 
                {
                    string _relativePath = LuaAPI.lua_tostring(L, 2);
                    ResType _resType;translator.Get(L, 3, out _resType);
                    
                    gen_to_be_invoked.UnLoadAssetBundle( _relativePath, _resType );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ResManager.UnLoadAssetBundle!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DestroyCache(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ResManager gen_to_be_invoked = (ResManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.DestroyCache(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
