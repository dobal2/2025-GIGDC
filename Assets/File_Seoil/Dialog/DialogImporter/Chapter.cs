using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "Scriptable Objects/Chapter")]
public class Chapter : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public TargetData[] DialogTargets { get; set; }
    [field: SerializeField] public Dialog[] Dialogs { get; private set; }

    public TargetData GetTargetByName(string targetName)
    {
        foreach (TargetData target in DialogTargets)
        {
            if (target.Name == targetName)
                return target;
        }

        Debug.LogWarning($"TargetName is invalid: {targetName}");
        return null;
    }

#if UNITY_EDITOR
    public void SetUnallocatedDialogTargets(List<string> targetNames)
    {
        List<TargetData> targetDatas = new();

        string chapterPath = AssetDatabase.GetAssetPath(this);
        string chapterDir = Path.GetDirectoryName(chapterPath);
        string targetDir = Path.Combine(chapterDir, "TargetData");

        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);

        foreach (string targetName in targetNames)
        {
            string fileName = $"{targetName}.asset";
            string assetPath = Path.Combine(targetDir, fileName).Replace("\\", "/");

            TargetData targetData = AssetDatabase.LoadAssetAtPath<TargetData>(assetPath);
            if (targetData == null)
            {
                targetData = CreateInstance<TargetData>();
                targetData.Name = targetName;
                targetData.Transform = null;

                AssetDatabase.CreateAsset(targetData, assetPath);
            }

            targetDatas.Add(targetData);
        }

        DialogTargets = targetDatas.ToArray();
    }
#endif

    public void SetDialogs(Dialog[] dialogs) =>
        Dialogs = dialogs;

    public void SetChapterName(string chapterName) =>
        Name = chapterName;
}
