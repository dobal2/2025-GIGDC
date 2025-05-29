using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AnalyzerWindow : EditorWindow
{
    private string inputPath = "";
    private string outputPath = "";
    private Vector2 scrollPos;
    private FileNode rootNode;
    private Dictionary<string, bool> fileSelections = new();

    [MenuItem("Tools/코드 내보내기")]
    public static void ShowWindow()
    {
        GetWindow<AnalyzerWindow>("코드 내보내기");
    }

    private void OnEnable()
    {
        // 바탕화면 기본 경로 설정
        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "Exported");
        }

        if (string.IsNullOrEmpty(inputPath))
        {
            inputPath = Application.dataPath;
            LoadTree();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("분석 대상 폴더 선택", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        inputPath = EditorGUILayout.TextField("대상 폴더", inputPath);
        if (GUILayout.Button("찾기", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("대상 폴더 선택", "", "");
            if (!string.IsNullOrEmpty(selected))
            {
                inputPath = selected;
                LoadTree();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("분석할 파일 선택", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
        if (rootNode != null)
            DrawTree(rootNode);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("출력 폴더 설정", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        outputPath = EditorGUILayout.TextField("출력 폴더", outputPath);
        if (GUILayout.Button("찾기", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("출력 폴더 선택", outputPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
                outputPath = selected;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("전처리 및 출력 시작", GUILayout.Height(40)))
        {
            Debug.Log("선택된 파일 수: " + GetSelectedFiles().Count);
            Debug.Log("출력 폴더: " + outputPath);

            List<string> selected = GetSelectedFiles();
            FileProcessor.ProcessFiles(selected, outputPath);

            OpenDirectory(outputPath);
        }
    }

    public static void OpenDirectory(string path)
    {
#if UNITY_EDITOR_WIN
        System.Diagnostics.Process.Start("explorer.exe", path.Replace("/", "\\"));
#elif UNITY_EDITOR_OSX
        System.Diagnostics.Process.Start("open", path);
#else
        Debug.LogWarning("이 OS에서는 자동 폴더 열기 미지원");
#endif
    }

    // ---------------- 트리 구조 ----------------

    class FileNode
    {
        public string Path;
        public string Name;
        public bool IsFolder;
        public List<FileNode> Children = new();
        public bool IsExpanded = true;
    }

    private void LoadTree()
    {
        fileSelections.Clear();
        if (Directory.Exists(inputPath))
            rootNode = BuildTreeRecursive(inputPath);
    }

    private FileNode BuildTreeRecursive(string path)
    {
        FileNode node = new FileNode
        {
            Path = path,
            Name = System.IO.Path.GetFileName(path),
            IsFolder = Directory.Exists(path),
            IsExpanded = false
        };

        if (node.IsFolder)
        {
            foreach (var dir in Directory.GetDirectories(path))
                node.Children.Add(BuildTreeRecursive(dir));

            foreach (var file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".meta")) continue;

                node.Children.Add(new FileNode
                {
                    Path = file,
                    Name = System.IO.Path.GetFileName(file),
                    IsFolder = false
                });

                if (!fileSelections.ContainsKey(file))
                    fileSelections[file] = false;
            }
        }

        return node;
    }

    private void DrawTree(FileNode node, int indent = 0)
    {
        EditorGUI.indentLevel = indent;

        if (node.IsFolder)
        {
            node.IsExpanded = EditorGUILayout.Foldout(node.IsExpanded, node.Name);
            if (node.IsExpanded)
            {
                foreach (var child in node.Children)
                    DrawTree(child, indent + 1);
            }
        }
        else
        {
            fileSelections[node.Path] = EditorGUILayout.ToggleLeft(node.Name, fileSelections[node.Path]);
        }
    }

    private List<string> GetSelectedFiles()
    {
        List<string> selected = new();
        foreach (var kvp in fileSelections)
            if (kvp.Value)
                selected.Add(kvp.Key);
        return selected;
    }
}