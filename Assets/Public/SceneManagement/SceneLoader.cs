using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader, GlobalScope>
{
    public static SceneType CurrentScene => GetSceneType(SceneManager.GetActiveScene().name);

    [SerializeField] private FadeView fadeView;
    [SerializeField] private InkView inkView;

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeSceneLink()
    {
        sceneLinks ??=
            ((SceneType[])Enum.GetValues(typeof(SceneType)))
            .Select(stage => new SceneLink(stage, stage.ToString()))
            .ToArray();
    }

    public void LoadScene(SceneType sceneType, SceneChangeAnimation changeAnimation = SceneChangeAnimation.Fade)
    {
        Debug.Log("LoadScene =" + changeAnimation);
        Debug.Log(sceneType.ToString());
        LoadScene(GetSceneName(sceneType), changeAnimation);
    }

    public void LoadScene(string sceneName, SceneChangeAnimation changeAnimation = SceneChangeAnimation.Fade)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("씬 이름이 비어있습니다.");
            return;
        }

        switch (changeAnimation)
        {
            case SceneChangeAnimation.None:
                StartCoroutine(LoadSceneAsync(sceneName));
                break;
            case SceneChangeAnimation.Fade:
                StartCoroutine(LoadSceneByFade(sceneName));
                break;
            case SceneChangeAnimation.Ink:
                StartCoroutine(LoadSceneByInk(sceneName));
                break;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;
    }

    private IEnumerator LoadSceneByFade(string sceneName)
    {
        Instantiate(fadeView);
        yield return new WaitForSeconds(0.5f);
        yield return LoadSceneAsync(sceneName);
    }

    private IEnumerator LoadSceneByInk(string sceneName)
    {
        Instantiate(inkView);
        yield return new WaitForSeconds(6f);
        yield return LoadSceneAsync(sceneName);
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
        foreach (SceneLink linkData in sceneLinks)
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

    public enum SceneChangeAnimation
    {
        None, Fade, Ink
    }
}