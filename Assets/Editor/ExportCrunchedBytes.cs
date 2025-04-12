using System;
using UnityEditor;
using UnityEngine;
using System.IO;

public class ExportCrunchedTextures
{
    [MenuItem("Tools/Export Crunched Textures")]
    public static void Export()
    {
        var inputPath = Environment.GetEnvironmentVariable("INPUT_PATH");
        var outputPath = Environment.GetEnvironmentVariable("OUTPUT_PATH");

        if (string.IsNullOrEmpty(inputPath) || string.IsNullOrEmpty(outputPath))
        {
            Debug.LogError("INPUT_PATH または OUTPUT_PATH が設定されていません。");
            return;
        }
        if (!Directory.Exists(inputPath))
        {
            Debug.LogError($"入力フォルダが存在しません: {inputPath}");
            return;
        }
        if (!Directory.Exists(outputPath))
        {
            Debug.LogError($"出力フォルダが存在しません: {outputPath}");
            return;
        }

        var pngFiles = Directory.GetFiles(inputPath, "*.png", SearchOption.TopDirectoryOnly);
        foreach (var filePath in pngFiles)
        {
            var relativePath = "Assets" + filePath.Replace(Application.dataPath, "").Replace("\\", "/");
            var importer = (TextureImporter)AssetImporter.GetAtPath(relativePath);
            if (importer == null)
            {
                Debug.LogWarning($"TextureImporter が取得できませんでした: {relativePath}");
                continue;
            }

            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.crunchedCompression = true;
            importer.compressionQuality = 50;
            importer.textureType = TextureImporterType.Default;
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                name = "Standalone",
                overridden = true,
                maxTextureSize = 2048,
                format = TextureImporterFormat.DXT1Crunched,
                compressionQuality = 50
            });

            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);

            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
            if (tex == null)
            {
                Debug.LogWarning($"Texture2D がロードできませんでした: {relativePath}");
                continue;
            }

            var rawData = tex.GetRawTextureData();
            var outputFileName = Path.GetFileNameWithoutExtension(filePath) + ".crn";
            var outputFilePath = Path.Combine(outputPath, outputFileName);

            File.WriteAllBytes(outputPath, rawData);
            Debug.Log($"出力完了: {outputFilePath}");
        }
        
        Debug.Log("すべての処理が完了しました。");
    }
}
