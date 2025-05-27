using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TargetSelector))]
public class TargetSelectorCustemEditor : Editor
{
    private TargetSelector targetSelector;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        targetSelector = (TargetSelector)target;
        
        if (targetSelector.ScriptData == null) return;


        int chapterIndex = ShowChapter();
        AllocateChapter(chapterIndex);

        int targetIndex = ShowTarget();
        AllocateTarget(targetIndex);
    }

    private void AllocateChapter(int selectedIndex)
    {
        if(selectedIndex < 0)
        {
            targetSelector.AllocatedChapterName = null;
            return;
        }

        if(targetSelector.AllocatedChapterName != targetSelector.ScriptData.Chapters[selectedIndex].Name)
        {
            InitializeTarget();
        }
        targetSelector.AllocatedChapterName = targetSelector.ScriptData.Chapters[selectedIndex].Name;
    }

    private void AllocateTarget(int selectedIndex)
    {
        if(selectedIndex < 0)
        {
            targetSelector.AllocatedTargetName = null;
            return;
        }

        foreach(Chapter chapter in targetSelector.ScriptData.Chapters)
        {
            if(chapter.Name == targetSelector.AllocatedChapterName)
            {
                targetSelector.AllocatedTargetName = chapter.DialogTargets[selectedIndex].Name;
                return;
            }
        }
    }

    private int ShowChapter()
    {
        int selectedIndex = 0;

        List<string> chapters = targetSelector.ScriptData.Chapters.Select(chapter => chapter.Name).ToList();
        chapters.Insert(0, "None");

        if(targetSelector.AllocatedChapterName != null)
        {
            selectedIndex = chapters.IndexOf(targetSelector.AllocatedChapterName);
        }

        selectedIndex = EditorGUILayout.Popup(
            label: "Chapter",
            selectedIndex: selectedIndex,
            displayedOptions: chapters.ToArray()
            );

        return --selectedIndex;
    }

    private int ShowTarget()
    {
        if (targetSelector.AllocatedChapterName == null) return -1;

        int selectedIndex = 0;

        List<string> targets = new();
        foreach (Chapter chapter in targetSelector.ScriptData.Chapters)
        {
            if (chapter.Name == targetSelector.AllocatedChapterName)
            {
                targets = chapter.DialogTargets.Select(target => target.Name).ToList();
                break;
            }
        }

        targets.Insert(0, "None");

        if(targetSelector.AllocatedTargetName != null)
        {
            selectedIndex = targets.IndexOf(targetSelector.AllocatedTargetName);
        }

        selectedIndex = EditorGUILayout.Popup(
            label: "Target",
            selectedIndex: selectedIndex,
            displayedOptions: targets.ToArray()
            );

        return --selectedIndex;
    }

    private void InitializeTarget()
    {
        targetSelector.AllocatedTargetName = null;
    }
}