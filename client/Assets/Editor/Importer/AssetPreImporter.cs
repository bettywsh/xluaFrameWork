using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AssetPreImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;
        TexturePreImporter.ProcTexture(assetPath, ref importer);
    }

    //void OnPreprocessModel()
    //{
    //    var importer = assetImporter as ModelImporter;
    //    ModelPostImporter.PreProcModel(assetPath, ref importer);
    //}

    //void OnPostprocessModel(GameObject model)
    //{
    //    var importer = assetImporter as ModelImporter;
    //    ModelPostImporter.PostProcModel(assetPath, model);
    //}

    void OnPreprocessAudio()
    {
        var importer = assetImporter as AudioImporter;
        AudioPreImporter.ProcAudio(assetPath, ref importer);
    }


}
