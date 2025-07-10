using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneController;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    public static SceneType CurrentScene => GetSceneType(SceneManager.GetActiveScene().name);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [InitializeOnLoadMethod]
    private static void InitializeSceneLink()
    {
        sceneLinks ??=
            ((SceneType[])Enum.GetValues(typeof(SceneType)))
            .Select(stage => new SceneLink(stage, stage.ToString()))
            .ToArray();
    }

    public void LoadScene(SceneType sceneType)
    {
        LoadScene(GetSceneName(sceneType));
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static string GetSceneName(SceneType sceneType)
    {
        foreach (SceneLink linkData in sceneLinks)
        {
            if (linkData.Type == sceneType) return linkData.Name;
        }

        throw new System.ArgumentOutOfRangeException();
    }

    public static SceneType GetSceneType(string sceneName)
    {
        foreach(SceneLink linkData in sceneLinks)
        {
            if (linkData.Name == sceneName) return linkData.Type;
        }

        throw new System.ArgumentOutOfRangeException();
    }

    private static SceneLink[] sceneLinks;

    private class SceneLink
    {
        public SceneLink(SceneType type, string name)
        {
            Type = type;
            Name = name;
        }

        public SceneType Type;
        public string Name;
    }

    public enum SceneType
    {
        TutorialScene,
        Lobby_Over,
        Lobby_Under,
        Stage1_1,
        Stage1_2, 
        Stage1_3, 
        Stage1_4, 
        Stage1_5, 
        Stage1_6, 
        Stage1_7,
        Stage1_8,
        Stage1_9,
        Stage1_10,
        Stage1_11,
        Stage2_1,
        Stage2_2,
        Stage2_3,
        Stage2_4,
        Stage2_5,
        Stage2_6,
        Stage2_7,
        Stage2_8,
        Stage2_9,
        Stage2_10,
        Stage3_1,
        Stage3_2,
        Stage3_3,
        Stage3_4,
        Stage3_5,
        Stage3_6,
        Stage3_7,
        Stage3_8,
        Stage3_9,
        Stage3_10,
        Stage3_11,
        Stage3_12,
        Stage3_13,
        Stage3_14,
        Stage3_15,
        Stage4_1,
        Stage4_2,
        Stage4_3,
        Stage4_4,
        Stage4_5,
        Stage4_6,
        Stage4_7,
        Stage4_8,
        Stage4_9,
        Stage4_10,
        Stage4_11,
        EndingScene_1
    }
}