using System.Diagnostics;
using System.IO;

public static class PythonYamlConverter
{
    private static string GetScriptPath()
    {
#nullable enable
        string? codePath = new StackTrace(true).GetFrame(0)?.GetFileName();
#nullable restore
        if (string.IsNullOrEmpty(codePath))
        {
            UnityEngine.Debug.LogError("Python 스크립트 경로를 찾을 수 없습니다.");
            return "";
        }

        string directory = Path.GetDirectoryName(codePath) ?? "";
        string scriptPath = Path.Combine(directory, "unity_yaml_to_json.py");
        return scriptPath;
    }

    /// <summary>
    /// YAML 문자열을 Python 스크립트를 통해 JSON 문자열로 변환
    /// </summary>
    public static string ConvertYamlToJson(string yamlContent)
    {
        string scriptPath = GetScriptPath();
        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError("Python 스크립트가 존재하지 않습니다: " + scriptPath);
            return "";
        }

        ProcessStartInfo psi = new()
        {
            FileName = "python",
            Arguments = $"\"{scriptPath}\" --from-stdin",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using Process process = new();
        process.StartInfo = psi;

        try
        {
            process.Start();

            // YAML 문자열 전달
            using (StreamWriter writer = process.StandardInput)
            {
                writer.Write(yamlContent);
            }

            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(errors))
            {
                UnityEngine.Debug.LogError($"[YAML 변환 오류]\n{errors}");
                return "";
            }

            return output;
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[Python 실행 실패]\n{ex.Message}");
            return "";
        }
    }
}