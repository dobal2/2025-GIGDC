using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LobbySceneMover : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            LoadScene();
    }

    private void LoadScene()
    {
        switch(Stage.Data)
        {
            case StageDataType.Tutorial:
                SceneController.Instance.LoadScene(SceneController.SceneType.Stage1_1);
                break;
            case StageDataType.Stage1:
                SceneController.Instance.LoadScene(SceneController.SceneType.Stage2_1);
                break;
            case StageDataType.Stage2:
                SceneController.Instance.LoadScene(SceneController.SceneType.Stage3_1);
                break;
            case StageDataType.Stage3:
                SceneController.Instance.LoadScene(SceneController.SceneType.Stage4_1);
                break;
            case StageDataType.Stage4:
                break;
        }
    }
}
