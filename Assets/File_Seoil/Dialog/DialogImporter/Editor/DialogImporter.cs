using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
public class DialogImporter : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach(string assetFilePath in importedAssets)
        {
            if  (
                !assetFilePath.EndsWith(".tsv") ||
                File.ReadAllLines(assetFilePath)[0].Split("\t").Length != 2
                )
                continue;
            
            Chapter chapter = GenerateChapter(assetFilePath);
            AllocateChapter(assetFilePath, chapter);

            AssetDatabase.SaveAssets();

            AssetDatabase.DeleteAsset(assetFilePath);
        }
    }

    private static Chapter GenerateChapter(string filePath)
    {
        string originalDirectory = Path.GetDirectoryName(filePath);
        string chapterName = Path.GetFileNameWithoutExtension(filePath);

        string chapterFolder = Path.Combine(originalDirectory, chapterName);
        if (!Directory.Exists(chapterFolder))
            Directory.CreateDirectory(chapterFolder);

        string assetFolderPath = chapterFolder.Replace("\\", "/");

        string fullAssetPath = Path.Combine(assetFolderPath, chapterName + ".asset").Replace("\\", "/");

        Chapter chapter = ScriptableObject.CreateInstance<Chapter>();

        if (File.Exists(fullAssetPath))
            AssetDatabase.DeleteAsset(fullAssetPath);

        AssetDatabase.CreateAsset(chapter, fullAssetPath);

        return chapter;
    }


    private static void AllocateChapter(string filePath, Chapter chapter)
    {
        chapter.SetChapterName(Path.GetFileNameWithoutExtension(filePath));

        string[] textlines = File.ReadAllLines(filePath);

        List<string> targets = new();
        
        foreach (string textLine in textlines)
        {
            string[] texts = textLine.Split('\t');

            if(texts.Length != 2)
            {
                Debug.LogWarning($"Dialog line is invalid(Text Length : {texts.Length}): {textLine}");
                continue;
            }

            string targetName = texts[0];

            bool isSameTargetNameExist = false;
            foreach (string _targetName in targets)
            {
                if (_targetName == targetName)
                {
                    isSameTargetNameExist = true;
                    continue;
                }
            }

            if(!isSameTargetNameExist) targets.Add(targetName);
        }

        chapter.SetUnallocatedDialogTargets(targets);

        List<Dialog> dialogs = new();

        foreach (string textLine in textlines)
        {
            string[] texts = textLine.Split('\t');

            if (texts.Length != 2)
            {
                Debug.LogWarning($"Dialog line is invalid(Text Length : {texts.Length}): {textLine}");
                continue;
            }

            string targetName = texts[0];
            string line = texts[1];
            
            TargetData targetData = chapter.GetTargetByName(targetName);
            Dialog dialog = new Dialog(targetData, line);

            dialogs.Add(dialog);
        }

        chapter.SetDialogs(dialogs.ToArray());

        EditorUtility.SetDirty(chapter);
    }
}
