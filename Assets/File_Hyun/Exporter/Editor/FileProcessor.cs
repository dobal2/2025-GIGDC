using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileProcessor
{
    private static readonly HashSet<string> TextDataExtensions = new()
    {
        ".csv", ".json", ".txt", ".xml", ".tsv", ".ini"
    };

    private static readonly HashSet<string> UnityYamlExtensions = new()
    {
        ".unity", ".prefab", ".asset", ".mat", ".anim", ".controller", ".overrideController",
        ".mask", ".lighting", ".physicsMaterial", ".physicMaterial", ".physicsMaterial2D",
        ".terrainlayer", ".spriteatlas", ".timeline", ".signal", ".renderTexture",
        ".shaderGraph", ".vfx", ".preset", ".yaml", ".yml",
        ".brush", ".fln", ".signalEmitter", ".volumeProfile", ".sceneTemplate",
        ".customEditorExtension", ".variant", ".visualeffect"
    };

    public static void ProcessFiles(List<string> selectedFiles, string outputPath)
    {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        Dictionary<string, string> metaResults = new();

        foreach (string filePath in selectedFiles)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            string fileName = Path.GetFileName(filePath);
            string destPath;
            string metaPath = filePath + ".meta";

            // === C# 및 텍스트 파일 ===
            if (ext == ".cs" || TextDataExtensions.Contains(ext))
            {
                destPath = Path.Combine(outputPath, fileName);
                File.Copy(filePath, destPath, overwrite: true);
            }
            // === Unity YAML 기반 에셋 처리 ===
            else if (UnityYamlExtensions.Contains(ext) || IsUnityYamlFile(filePath))
            {
                string outputJsonPath = Path.Combine(outputPath, fileName + ".json");
                PythonYamlConverter.RunYamlToJson(filePath, outputJsonPath);

                if (!File.Exists(outputJsonPath))
                    Debug.LogWarning($"[YAML 변환 실패 또는 누락] {outputJsonPath}");
            }
            // === 바이너리 파일 ===
            else
            {
                Debug.Log($"무시된 파일: {fileName} (확장자: {ext})");
            }

            // === 메타 파일 처리 ===
            if (File.Exists(metaPath))
            {
                string metaJsonPath = Path.Combine(outputPath, fileName + ".meta.json");
                PythonYamlConverter.RunYamlToJson(metaPath, metaJsonPath);

                if (File.Exists(metaJsonPath))
                {
                    string metaJsonText = File.ReadAllText(metaJsonPath);
                    metaResults[fileName + ".meta"] = metaJsonText;
                }
                else
                {
                    Debug.LogWarning($"[메타 변환 실패 또는 누락] {metaJsonPath}");
                }
            }
        }

        // === 메타 병합 저장 ===
        string metaOutputPath = Path.Combine(outputPath, "ProcessedMetas.json");
        using StreamWriter writer = new(metaOutputPath, false);
        writer.WriteLine("{");
        foreach (var kvp in metaResults)
        {
            writer.WriteLine($"  \"{kvp.Key}\": {kvp.Value},");
        }

        if (metaResults.Count > 0 && writer.BaseStream.Length >= 2)
        {
            writer.BaseStream.SetLength(writer.BaseStream.Length - 2); // 마지막 , 제거
            writer.WriteLine("\n}");
        }
        else
        {
            writer.WriteLine("}");
        }
    }

    private static bool IsUnityYamlFile(string filePath)
    {
#nullable enable
        try
        {
            using var reader = new StreamReader(filePath);
            for (int i = 0; i < 5; i++)
            {
                string? line = reader.ReadLine();
                if (line == null) break;

                if (line.StartsWith("%YAML") || line.StartsWith("%TAG") || line.StartsWith("--- !u!"))
                    return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"YAML 판별 실패: {filePath}\n{e.Message}");
        }
        return false;
#nullable restore
    }
}