#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using UnityEngine;
using System.Collections.Generic;
[DisallowMultipleComponent, ExecuteInEditMode]
public class PrefabLightmapData : MonoBehaviour
{
    [System.Serializable]
    struct RendererInfo
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }

    [SerializeField]
    RendererInfo[] m_RendererInfo;
    [SerializeField]
    Texture2D[] m_Lightmaps;
    [SerializeField]
    Texture2D[] m_Lightmaps2;
    [SerializeField]
    Texture2D[] m_Lightmaps3;

    const string LIGHTMAP_RESOURCE_PATH = "Assets/Resources/Lightmaps/";

    [System.Serializable]
    struct Texture2D_Remap
    {
        public int originalLightmapIndex;
        public Texture2D originalLightmap;
        public Texture2D lightmap0;
        public Texture2D lightmap1;
        public Texture2D lightmap2;
    }

    static List<Texture2D_Remap> sceneLightmaps = new List<Texture2D_Remap>();

    void Awake()
    {
        ApplyLightmaps(m_RendererInfo, m_Lightmaps, m_Lightmaps2, m_Lightmaps3);
    }

    static void ApplyLightmaps(RendererInfo[] rendererInfo, Texture2D[] lightmaps, Texture2D[] lightmaps2, Texture2D[] lightmaps3)
    {
        bool existsAlready = false;
        int counter = 0;
        int[] lightmapArrayOffsetIndex;

        if (rendererInfo == null || rendererInfo.Length == 0)
            return;

        var settingslightmaps = LightmapSettings.lightmaps;
        var combinedLightmaps = new List<LightmapData>();
        lightmapArrayOffsetIndex = new int[lightmaps.Length];

        for (int i = 0; i < lightmaps.Length; i++)
        {
            existsAlready = false;
            for (int j = 0; j < settingslightmaps.Length; j++)
            {
                if (lightmaps[i] == settingslightmaps[j].lightmapColor)
                {
                    lightmapArrayOffsetIndex[i] = j;
                    existsAlready = true;
                }
            }

            if (!existsAlready)
            {
                lightmapArrayOffsetIndex[i] = counter + settingslightmaps.Length;
                var newLightmapData = new LightmapData();
                newLightmapData.lightmapColor = lightmaps[i];
                newLightmapData.lightmapDir = lightmaps2[i];
                newLightmapData.shadowMask = lightmaps3[i];
                combinedLightmaps.Add(newLightmapData);
                ++counter;
            }
        }

        var combinedLightmaps2 = new LightmapData[settingslightmaps.Length + counter];
        settingslightmaps.CopyTo(combinedLightmaps2, 0);

        if (counter > 0)
        {
            for (int i = 0; i < combinedLightmaps.Count; i++)
            {
                combinedLightmaps2[i + settingslightmaps.Length] = new LightmapData();
                combinedLightmaps2[i + settingslightmaps.Length].lightmapColor = combinedLightmaps[i].lightmapColor;
                combinedLightmaps2[i + settingslightmaps.Length].lightmapDir = combinedLightmaps[i].lightmapDir;
                combinedLightmaps2[i + settingslightmaps.Length].shadowMask = combinedLightmaps[i].shadowMask;
            }
        }

        ApplyRendererInfo(rendererInfo, lightmapArrayOffsetIndex);

        LightmapSettings.lightmaps = combinedLightmaps2;
    }

    static void ApplyRendererInfo(RendererInfo[] infos, int[] arrayOffsetIndex)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            info.renderer.lightmapIndex = arrayOffsetIndex[info.lightmapIndex];
            info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Lightmaps/Update Scene with Prefab Lightmaps")]
    static void UpdateLightmaps()
    {
        PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

        foreach (var instance in prefabs)
        {
            ApplyLightmaps(instance.m_RendererInfo, instance.m_Lightmaps, instance.m_Lightmaps2, instance.m_Lightmaps3);
        }

        Debug.Log("Prefab lightmaps updated");
    }

    [MenuItem("Lightmaps/Bake Prefab Lightmaps")]
    static void GenerateLightmapInfo()
    {
        Debug.ClearDeveloperConsole();

        if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
            return;
        }

        Lightmapping.Bake();

        string lightMapPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), LIGHTMAP_RESOURCE_PATH);

        if (!Directory.Exists(lightMapPath))
            Directory.CreateDirectory(lightMapPath);

        sceneLightmaps = new List<Texture2D_Remap>();

        //var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var sceneName = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
        var resourcePath = LIGHTMAP_RESOURCE_PATH + sceneName;
        var scenePath = System.IO.Path.GetDirectoryName(EditorApplication.currentScene) + "/" + sceneName + "/";

        PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

        foreach (var instance in prefabs)
        {
            var gameObject = instance.gameObject;
            var rendererInfos = new List<RendererInfo>();
            var lightmaps = new List<Texture2D>();
            var lightmaps2 = new List<Texture2D>();
            var lightmaps3 = new List<Texture2D>();

            GenerateLightmapInfo(scenePath, resourcePath, gameObject, rendererInfos, lightmaps, lightmaps2, lightmaps3);

            instance.m_RendererInfo = rendererInfos.ToArray();
            instance.m_Lightmaps = lightmaps.ToArray();
            instance.m_Lightmaps2 = lightmaps2.ToArray();
            instance.m_Lightmaps3 = lightmaps3.ToArray();

            var targetPrefab = PrefabUtility.GetPrefabParent(gameObject) as GameObject;
            if (targetPrefab != null)
            {
                //Prefab
                PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
            }

            ApplyLightmaps(instance.m_RendererInfo, instance.m_Lightmaps, instance.m_Lightmaps2, instance.m_Lightmaps3);
        }

        Debug.Log("Update to prefab lightmaps finished");
    }

    static void GenerateLightmapInfo(string scenePath, string resourcePath, GameObject root, List<RendererInfo> rendererInfos, List<Texture2D> lightmaps, List<Texture2D> lightmaps2, List<Texture2D> lightmaps3)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.lightmapIndex != -1)
            {
                RendererInfo info = new RendererInfo();
                info.renderer = renderer;
                info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
                Texture2D lightmap2 = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapDir;
                Texture2D lightmap3 = LightmapSettings.lightmaps[renderer.lightmapIndex].shadowMask;
                int sceneLightmapIndex = AddLightmap(scenePath, resourcePath, renderer.lightmapIndex, lightmap, lightmap2, lightmap3);

                info.lightmapIndex = lightmaps.IndexOf(sceneLightmaps[sceneLightmapIndex].lightmap0);
                if (info.lightmapIndex == -1)
                {
                    info.lightmapIndex = lightmaps.Count;
                    lightmaps.Add(sceneLightmaps[sceneLightmapIndex].lightmap0);
                    lightmaps2.Add(sceneLightmaps[sceneLightmapIndex].lightmap1);
                    lightmaps3.Add(sceneLightmaps[sceneLightmapIndex].lightmap2);
                }

                rendererInfos.Add(info);
            }
        }
    }

    static int AddLightmap(string scenePath, string resourcePath, int originalLightmapIndex, Texture2D lightmap, Texture2D lightmap2, Texture2D lightmap3)
    {
        int newIndex = -1;

        for (int i = 0; i < sceneLightmaps.Count; i++)
        {
            if (sceneLightmaps[i].originalLightmapIndex == originalLightmapIndex)
            {
                return i;
            }
        }

        if (newIndex == -1)
        {
            var lightmap_Remap = new Texture2D_Remap();
            lightmap_Remap.originalLightmapIndex = originalLightmapIndex;
            lightmap_Remap.originalLightmap = lightmap;

            var filename = scenePath + "Lightmap-" + originalLightmapIndex;

            lightmap_Remap.lightmap0 = GetLightmapAsset(filename + "_comp_light.exr", resourcePath + "_light", originalLightmapIndex, lightmap);
            if (lightmap2 != null)
            {
                lightmap_Remap.lightmap1 = GetLightmapAsset(filename + "_comp_dir.png", resourcePath + "_dir", originalLightmapIndex, lightmap2);
            }

            if (lightmap3 != null)
            {
                lightmap_Remap.lightmap2 = GetLightmapAsset(filename + "_comp_shadowmask.png", resourcePath + "_shadowmask", originalLightmapIndex, lightmap3);
            }

            sceneLightmaps.Add(lightmap_Remap);
            newIndex = sceneLightmaps.Count - 1;
        }

        return newIndex;
    }

    static Texture2D GetLightmapAsset(string filename, string resourcePath, int originalLightmapIndex, Texture2D lightmap)
    {
        var assetLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(filename);
        return assetLightmap;

        //AssetDatabase.ImportAsset(filename, ImportAssetOptions.ForceUpdate);
        //var importer = AssetImporter.GetAtPath(filename) as TextureImporter;
        //importer.isReadable = true;
        //AssetDatabase.ImportAsset(filename, ImportAssetOptions.ForceUpdate);

        //var assetLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(filename);

        //var assetPath = resourcePath + "-" + originalLightmapIndex + ".png";
        //var newLightmap = Instantiate<Texture2D>(assetLightmap);

        //AssetDatabase.CreateAsset(newLightmap, assetPath);

        //newLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        //importer.isReadable = false;
        //AssetDatabase.ImportAsset(filename, ImportAssetOptions.ForceUpdate);

        //return newLightmap;
    }

#endif

}