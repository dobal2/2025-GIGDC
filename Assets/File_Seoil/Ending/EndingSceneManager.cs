using UnityEngine;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField]
    private void End()
    {
        Stage.Data = StageDataType.Start;
        SceneController.Instance.LoadScene("TitleScene");
    }
}
