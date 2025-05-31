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
            string destPath = Path.Combine(outputPath, fileName);
            string metaPath = filePath + ".meta";

            // === 텍스트 및 코드 파일 복사 ===
            if (ext == ".cs" || TextDataExtensions.Contains(ext))
            {
                File.Copy(filePath, destPath, overwrite: true);
            }
            // === Unity YAML 변환 ===
            else if (UnityYamlExtensions.Contains(ext) || IsUnityYamlFile(filePath))
            {
                string yamlText = File.ReadAllText(filePath);
                string jsonResult = PythonYamlConverter.ConvertYamlToJson(yamlText);

                if (!string.IsNullOrWhiteSpace(jsonResult))
                {
                    string outputJsonPath = Path.Combine(outputPath, fileName + ".json");
                    File.WriteAllText(outputJsonPath, jsonResult);
                    Debug.Log($"[변환 완료] {outputJsonPath}");
                }
                else
                {
                    Debug.LogWarning($"[YAML 변환 실패 또는 누락] {fileName}");
                }
            }
            else
            {
                Debug.Log($"무시된 파일: {fileName} (확장자: {ext})");
            }

            // === 메타 파일 처리 ===
            if (File.Exists(metaPath))
            {
                string metaJsonPath = Path.Combine(outputPath, fileName + ".meta.json");
                string metaYaml = File.ReadAllText(metaPath);
                string metaJson = PythonYamlConverter.ConvertYamlToJson(metaYaml);

                if (!string.IsNullOrWhiteSpace(metaJson))
                {
                    metaResults[fileName + ".meta"] = metaJson;
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

        int count = 0;
        foreach (var kvp in metaResults)
        {
            writer.WriteLine($"  \"{kvp.Key}\": {kvp.Value}{(count++ < metaResults.Count - 1 ? "," : "")}");
        }

        writer.WriteLine("}");
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