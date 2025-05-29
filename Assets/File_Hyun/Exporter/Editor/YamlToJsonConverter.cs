using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class YamlToJsonConverter
{
    public static string ConvertFile(string yamlFilePath)
    {
        try
        {
            string yamlText = File.ReadAllText(yamlFilePath);
            yamlText = RemoveUnityTags(yamlText);

            string[] yamlDocs = Regex.Split(yamlText, @"^---\s*", RegexOptions.Multiline);

            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var resultList = new List<object>();
            foreach (var doc in yamlDocs)
            {
                if (string.IsNullOrWhiteSpace(doc)) continue;
                var reader = new StringReader(doc);

                try
                {
                    object parsed = ParseByUnityType(deserializer, doc, reader);
                    resultList.Add(parsed);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[YAML 파싱 실패] fallback 처리됨: {ex.Message}");
                    reader = new StringReader(doc);
                    var fallback = deserializer.Deserialize(reader);
                    resultList.Add(fallback);
                }
            }

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            return serializer.Serialize(resultList);
        }
        catch (Exception e)
        {
            Debug.LogError($"YAML → JSON 변환 실패: {yamlFilePath}\n{e}");
            return "[]";
        }
    }

    private static object ParseByUnityType(IDeserializer deserializer, string doc, StringReader reader)
    {
        if (doc.StartsWith("!u!1")) return deserializer.Deserialize<GameObjectData>(reader);
        if (doc.StartsWith("!u!4")) return deserializer.Deserialize<TransformData>(reader);
        if (doc.StartsWith("!u!212")) return deserializer.Deserialize<SpriteRendererData>(reader);
        if (doc.StartsWith("!u!50")) return deserializer.Deserialize<Rigidbody2DData>(reader);
        if (doc.StartsWith("!u!58")) return deserializer.Deserialize<BoxCollider2DData>(reader);
        if (doc.StartsWith("!u!114")) return deserializer.Deserialize<MonoBehaviourData>(reader);
        if (doc.StartsWith("!u!224")) return deserializer.Deserialize<RectTransformData>(reader);
        return deserializer.Deserialize(reader); // fallback
    }

    private static string RemoveUnityTags(string yamlText)
    {
        yamlText = Regex.Replace(yamlText, @"^%TAG.*", "", RegexOptions.Multiline);
        yamlText = Regex.Replace(yamlText, @"^%YAML.*", "", RegexOptions.Multiline);
        yamlText = Regex.Replace(yamlText, @"^--- !u!\d+ &-?\d+", "---", RegexOptions.Multiline);
        return yamlText.Trim();
    }
}