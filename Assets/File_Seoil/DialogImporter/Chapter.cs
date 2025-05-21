using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "Scriptable Objects/Chapter")]
public class Chapter : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Dialog.TargetData[] DialogTargets { get; set; }
    [field: SerializeField] public Dialog[] Dialogs { get; private set; }

    public Dialog.TargetData GetTargetByName(string targetName)
    {
        foreach(Dialog.TargetData target in DialogTargets)
        {
            if (target.Name == targetName)
                return target;
        }

        Debug.LogWarning($"TargetName is invalid: {targetName}");
        return null;
    }

    public void SetUnallocatedDialogTargets(List<string> targetNames)
    {
        List<Dialog.TargetData> targetDatas = new();
        foreach(string targetName in targetNames)
        {
            Dialog.TargetData targetData = new();
            targetData.Name = targetName;
            targetData.Transform = null;

            targetDatas.Add(targetData);
        }

        DialogTargets = targetDatas.ToArray();
    }

    public void SetDialogs(Dialog[] dialogs) =>
        Dialogs = dialogs;

    public void SetChapterName(string chapterName) =>
        Name = chapterName;
}
