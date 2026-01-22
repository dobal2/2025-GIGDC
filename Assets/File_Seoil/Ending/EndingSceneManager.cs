using UnityEngine;

public class EndingSceneManager : MonoBehaviour
{
    public void End()
    {
        Stage.Data = StageDataType.Start;
        SceneLoader.Instance.LoadScene(SceneType.TitleScene);
    }
}
