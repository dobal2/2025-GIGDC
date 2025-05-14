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

    public void LoadScene(SceneType sceneType)
    {
        LoadScene(GetSceneName(sceneType));
    }

    private void LoadScene(string sceneName)
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

    private static SceneLink[] sceneLinks =
    {
        new SceneLink(SceneType.Stage1_1, "Stage1_1"),
        new SceneLink(SceneType.Stage1_2, "Stage1_2"),
        new SceneLink(SceneType.Stage1_3, "Stage1_3"),
        new SceneLink(SceneType.Stage1_4, "Stage1_4"),
        new SceneLink(SceneType.Stage1_5, "Stage1_5"),
        new SceneLink(SceneType.Stage1_6, "Stage1_6"),
        new SceneLink(SceneType.Stage1_7, "Stage1_7")
    };

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
        Stage1_1,
        Stage1_2, 
        Stage1_3, 
        Stage1_4, 
        Stage1_5, 
        Stage1_6, 
        Stage1_7
    }
}
