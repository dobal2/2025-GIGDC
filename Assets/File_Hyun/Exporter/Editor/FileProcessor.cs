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
                string jsonText = YamlToJsonConverter.ConvertFile(filePath);
                File.WriteAllText(Path.Combine(outputPath, fileName + ".json"), jsonText);
            }
            // === 바이너리 파일 ===
            else
            {
                Debug.Log($"무시된 파일: {fileName} (확장자: {ext})");
            }

            // === 메타 파일 처리 ===
            if (File.Exists(metaPath))
            {
                string jsonText = YamlToJsonConverter.ConvertFile(metaPath);
                metaResults[fileName + ".meta"] = jsonText;
            }
        }

        // === 메타 병합 저장 ===
        string metaJson = "{\n";
        foreach (var kvp in metaResults)
            metaJson += $"  \"{kvp.Key}\": {kvp.Value},\n";
        metaJson = metaJson.TrimEnd(',', '\n') + "\n}";
        File.WriteAllText(Path.Combine(outputPath, "ProcessedMetas.json"), metaJson);
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