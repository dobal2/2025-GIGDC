using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class PythonYamlConverter
{
    private static string GetScriptPath()
    {
        string codePath = new System.Diagnostics.StackTrace(true).GetFrame(0)?.GetFileName();
        if (string.IsNullOrEmpty(codePath))
        {
            UnityEngine.Debug.LogError("Python 경로를 찾을 수 없습니다.");
            return "";
        }

        string directory = Path.GetDirectoryName(codePath) ?? "";
        string scriptPath = Path.Combine(directory, "unity_yaml_to_json.py");
        return scriptPath;
    }

    public static void RunYamlToJson(string yamlPath, string outputJsonPath)
    {
        string scriptPath = GetScriptPath();
        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError("Python 스크립트 파일이 존재하지 않습니다: " + scriptPath);
            return;
        }

        ProcessStartInfo psi = new()
        {
            FileName = "python",
            Arguments = $"\"{scriptPath}\" \"{yamlPath}\" \"{outputJsonPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using Process process = Process.Start(psi);
        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrWhiteSpace(errors))
            UnityEngine.Debug.LogError($"[YAML 변환 오류] {Path.GetFileName(yamlPath)}\n{errors}");
        else
            UnityEngine.Debug.Log($"[YAML 변환 성공] {Path.GetFileName(outputJsonPath)}");
    }
}
