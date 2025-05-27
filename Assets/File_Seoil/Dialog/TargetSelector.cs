using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public ScriptData ScriptData;
    [HideInInspector] public string AllocatedChapterName;
    [HideInInspector] public string AllocatedTargetName;

    private void Awake()
    {
        foreach(Chapter chapter in ScriptData.Chapters)
        {
            if(chapter.Name == AllocatedChapterName)
            {
                foreach(Dialog.TargetData targetData in chapter.DialogTargets)
                {
                    if (targetData.Name == AllocatedTargetName)
                    {
                        targetData.Transform = transform;
                        return;
                    }
                }
            }
        }
    }
}
