using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class FileProcessor
{
    private static readonly HashSet<string> TextDataExtensions = new() { ".csv", ".json", ".txt", ".xml", ".tsv", ".ini" };
    private static readonly HashSet<string> UnityYamlExtensions = new() { ".unity", ".prefab", ".asset", ".mat", ".anim", ".controller", ".shader", ".yaml", ".yml" };

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

            // 대응 메타파일 경로
            string metaPath = filePath + ".meta";

            // === C# 코드: 그대로 복사 ===
            if (ext == ".cs")
            {
                destPath = Path.Combine(outputPath, fileName);
                File.Copy(filePath, destPath, overwrite: true);
            }
            // === 텍스트 파일: 그대로 복사 ===
            else if (TextDataExtensions.Contains(ext))
            {
                destPath = Path.Combine(outputPath, fileName);
                File.Copy(filePath, destPath, overwrite: true);
            }
            // === YAML 기반 유니티 파일: JSON으로 변환 ===
            else if (UnityYamlExtensions.Contains(ext))
            {
                string jsonText = YamlToJsonConverter.ConvertFile(filePath);
                File.WriteAllText(Path.Combine(outputPath, fileName + ".json"), jsonText);
            }
            // === 바이너리 파일은 무시 ===
            else
            {
                Debug.Log($"무시된 바이너리 파일: {fileName}");
            }

            // === 메타파일 처리 (내용 직접 저장) ===
            if (File.Exists(metaPath))
            {
                string jsonText = YamlToJsonConverter.ConvertFile(metaPath);
                metaResults[fileName + ".meta"] = jsonText;
            }
        }

        // 메타 결과 병합 저장 (문자열 수동 JSON 조립)
        string metaJson = "{\n";
        foreach (var kvp in metaResults)
        {
            metaJson += $"  \"{kvp.Key}\": {kvp.Value},\n";
        }
        metaJson = metaJson.TrimEnd(',', '\n') + "\n}";
        File.WriteAllText(Path.Combine(outputPath, "ProcessedMetas.json"), metaJson);
    }
}