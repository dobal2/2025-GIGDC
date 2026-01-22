using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneNameGeneratorSettings", menuName = "Scriptable Objects/SceneNameGeneratorSettings")]
public sealed class SceneNameGeneratorSettings : ScriptableObject
{
    [Header("Output")]
    [SerializeField] private DefaultAsset generatedFolder;

    [SerializeField] private string enumFileName = "SceneType.cs";
    [SerializeField] private string mapFileName = "SceneTypeMap.cs";

    [Header("Code")]
    [SerializeField] private bool useNamespace; 
    [SerializeField] private string namespaceName;

    [Header("Options")]
    [SerializeField] private bool generateMap = true;

    public string GeneratedFolder => AssetDatabase.GetAssetPath(generatedFolder);
    public string EnumFileName => enumFileName;
    public string MapFileName => mapFileName;
    public bool UseNamespace => useNamespace;
    public string NamespaceName => namespaceName;
    public bool GenerateMap => generateMap;
}