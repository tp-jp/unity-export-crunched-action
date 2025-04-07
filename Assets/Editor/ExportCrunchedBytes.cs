using UnityEditor;
using UnityEngine;
using System.IO;

public class ExportCrunchedBytes
{
    [MenuItem("Tools/Export Crunched Textures")]
    public static void Export()
    {
        var inputPath = "Assets/Textures/001.png";
        var outputPath = "Assets/Output/001.bytes";

        var importer = (TextureImporter)AssetImporter.GetAtPath(inputPath);
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.crunchedCompression = true;
        importer.compressionQuality = 50;
        importer.textureType = TextureImporterType.Default;
        importer.textureFormat = TextureImporterFormat.DXT1Crunched;

        AssetDatabase.ImportAsset(inputPath, ImportAssetOptions.ForceUpdate);

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(inputPath);
        var rawData = tex.GetRawTextureData();
        File.WriteAllBytes(outputPath, rawData);

        Debug.Log($"Exported crunched bytes to: {outputPath}");
    }
}
