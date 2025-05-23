using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class BuildScript
    {
        const string FileExtension = ".crn";

        [MenuItem("Tools/Export Crunched Textures")]
        public static void Build()
        {
            try
            {
                var cmdArgs = new CommandLineArgs(Environment.GetCommandLineArgs());
                var inputPath = cmdArgs.GetRequired<string>("inputPath");
                var outputPath = cmdArgs.GetRequired<string>("outputPath");
                var mipmapEnabled = cmdArgs.Get("mipmapEnabled", false);
                var maxSize = cmdArgs.Get("maxSize", 2048);
                var resizeAlgorithm = cmdArgs.Get("resizeAlgorithm", TextureResizeAlgorithm.Mitchell);
                var compressionQuality = cmdArgs.Get("compressionQuality", 50);


                if (!Directory.Exists(inputPath))
                {
                    throw new Exception($"入力フォルダが存在しません: {inputPath}");
                }

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                var absoluteInputPath = Path.GetFullPath(inputPath);
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
                    importer.compressionQuality = compressionQuality;
                    importer.mipmapEnabled = mipmapEnabled;
                    importer.textureType = TextureImporterType.Default;
                    importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                    {
                        name = "Standalone",
                        overridden = true,
                        maxTextureSize = maxSize,
                        resizeAlgorithm = resizeAlgorithm,
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
                    var relativeToInput = Path.GetRelativePath(absoluteInputPath, filePath);
                    
                    // 解像度を付与したファイル名に変更
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(relativeToInput);
                    var fileNameWithSize = $"{fileNameWithoutExt}_{tex.width}x{tex.height}{FileExtension}";
                    var relativeDir = Path.GetDirectoryName(relativeToInput);
                    var outputFilePath = Path.Combine(outputPath, relativeDir ?? string.Empty, fileNameWithSize);
                    Debug.Log($"出力ファイルパス: {outputFilePath}");

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
                Console.Error.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }
    }
}