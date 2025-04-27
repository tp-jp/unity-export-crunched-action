using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class BuildScript
    {
        [MenuItem("Tools/Export Crunched Textures")]
        public static void Build()
        {
            try
            {
                var cmdArgs = new CommandLineArgs(Environment.GetCommandLineArgs());
                var inputPath = cmdArgs.GetRequired<string>("inputPath");
                var outputPath = cmdArgs.GetRequired<string>("outputPath");
                var maxSize = cmdArgs.Get("maxSize", 2048);
                var compressionQuality = cmdArgs.Get("compressionQuality", 50);

                if (!Directory.Exists(inputPath))
                {
                    throw new Exception($"入力フォルダが存在しません: {inputPath}");
                }

                if (!Directory.Exists(outputPath))
                {
                    throw new Exception($"出力フォルダが存在しません: {outputPath}");
                }

                var pngFiles = Directory.GetFiles(inputPath, "*.png", SearchOption.AllDirectories);
                foreach (var filePath in pngFiles)
                {
                    var relativePath = filePath.Replace(Application.dataPath, "").Replace("\\", "/");
                    var importer = (TextureImporter)AssetImporter.GetAtPath(relativePath);
                    if (importer == null)
                    {
                        throw new Exception($"TextureImporter が取得できませんでした: {relativePath}");
                    }

                    importer.textureCompression = TextureImporterCompression.Compressed;
                    importer.crunchedCompression = true;
                    importer.compressionQuality = 50;
                    importer.textureType = TextureImporterType.Default;
                    importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                    {
                        name = "Standalone",
                        overridden = true,
                        maxTextureSize = maxSize,
                        format = TextureImporterFormat.DXT1Crunched,
                        compressionQuality = compressionQuality,
                    });

                    AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);

                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
                    if (tex == null)
                    {
                        throw new Exception($"Texture2D がロードできませんでした: {relativePath}");
                    }

                    var rawData = tex.GetRawTextureData();
                    var relativeToInput = Path.GetRelativePath(inputPath, filePath);
                    var outputFileName = Path.ChangeExtension(relativeToInput, ".crn");
                    var outputFilePath = Path.Combine(outputPath, outputFileName);

                    // 出力ディレクトリが存在しない場合は作成
                    var outputDirectory = Path.GetDirectoryName(outputFilePath);
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    File.WriteAllBytes(outputFilePath, rawData);
                    Debug.Log($"出力完了: {outputFilePath}");
                }

                Debug.Log("すべての処理が完了しました。");
                EditorApplication.Exit(0);
            }
            catch (ArgumentException ex)
            {
                // Debug.LogError(ex.Message);
                Console.Error.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }
    }
}