using System;
using System.IO;
using UnityEngine;

public static class YamlToJsonConverter
{
    public static string ConvertFile(string yamlFilePath)
    {
        try
        {
            string yamlText = File.ReadAllText(yamlFilePath);

            // 파서가 못 읽는 태그 제거 or 치환
            yamlText = RemoveUnityTags(yamlText);

            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                .Build();

            var yamlObject = deserializer.Deserialize(new StringReader(yamlText));

            var serializer = new YamlDotNet.Serialization.SerializerBuilder()
                .JsonCompatible()
                .Build();

            string jsonText = serializer.Serialize(yamlObject);
            return jsonText;
        }
        catch (Exception e)
        {
            Debug.LogError($"YAML → JSON 변환 실패: {yamlFilePath}\n{e.Message}");
            return "{}";
        }
    }

    // Unity 태그 제거 함수
    private static string RemoveUnityTags(string yamlText)
    {
        // !u!xxx &id 형태 제거
        yamlText = System.Text.RegularExpressions.Regex.Replace(yamlText, @"^--- !u!\d+ &\d+\s*", "---", System.Text.RegularExpressions.RegexOptions.Multiline);
        // %TAG !u! tag:unity3d.com,2011: 제거
        yamlText = System.Text.RegularExpressions.Regex.Replace(yamlText, @"^%TAG.*", "", System.Text.RegularExpressions.RegexOptions.Multiline);
        // %YAML 1.1 제거
        yamlText = System.Text.RegularExpressions.Regex.Replace(yamlText, @"^%YAML.*", "", System.Text.RegularExpressions.RegexOptions.Multiline);

        return yamlText.Trim();
    }
}