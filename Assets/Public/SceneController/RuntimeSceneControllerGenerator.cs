using UnityEngine;

public static class RuntimeSceneControllerGenerator
{
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnGameStart()
    {
        GameObject sceneControllerObject = new GameObject(typeof(SceneController).Name);
        sceneControllerObject.AddComponent<SceneController>();
    }
#endif
}
