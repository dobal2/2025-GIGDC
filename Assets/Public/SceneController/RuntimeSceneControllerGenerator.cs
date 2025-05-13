using UnityEngine;

public static class RuntimeSceneControllerGenerator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnGameStart()
    {
        GameObject sceneControllerObject = new GameObject(typeof(SceneController).Name);
        sceneControllerObject.AddComponent<SceneController>();
    }
}
