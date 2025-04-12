using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ExportCrunchedTextures
    {
        [MenuItem("Tools/Export Crunched Textures")]
        public static void Build()
        {
            var args = Environment.GetCommandLineArgs();
            var inputPath = GetArgument(args, "inputPath");
            var outputPath = GetArgument(args, "outputPath");

            if (string.IsNullOrEmpty(inputPath))
            {
                Debug.LogError("INPUT_PATH が設定されていません。");
                EditorApplication.Exit(-1);
                return;
            }
            if (string.IsNullOrEmpty(outputPath))
            {
                Debug.LogError("OUTPUT_PATH が設定されていません。");
                EditorApplication.Exit(-1);
                return;
            }
            if (!Directory.Exists(inputPath))
            {
                Debug.LogError($"入力フォルダが存在しません: {inputPath}");
                EditorApplication.Exit(-1);
                return;
            }
            if (!Directory.Exists(outputPath))
            {
                Debug.LogError($"出力フォルダが存在しません: {outputPath}");
                EditorApplication.Exit(-1);
                return;
            }

            var pngFiles = Directory.GetFiles(inputPath, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var filePath in pngFiles)
            {
                var relativePath = filePath.Replace(Application.dataPath, "").Replace("\\", "/");
                var importer = (TextureImporter)AssetImporter.GetAtPath(relativePath);
                if (importer == null)
                {
                    Debug.LogWarning($"TextureImporter が取得できませんでした: {relativePath}");
                    EditorApplication.Exit(-1);
                    return;
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

                File.WriteAllBytes(outputFilePath, rawData);
                Debug.Log($"出力完了: {outputFilePath}");
            }
        
            Debug.Log("すべての処理が完了しました。");
            EditorApplication.Exit(0);
        }

        static string GetArgument(string[] args, string name)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == $"-{name}" && i + 1 < args.Length)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
    }
}
