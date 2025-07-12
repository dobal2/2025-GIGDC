using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LobbySceneMover : MonoBehaviour
{
    [Header("Fixed Data")]
    [SerializeField] private SpriteRenderer moveContextPrefab;
    [SerializeField] private KeyData keyData;

    private SpriteRenderer currentMoveContext = null;
    private bool isEntered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        if (currentMoveContext != null) Destroy(currentMoveContext.gameObject);

        currentMoveContext = Instantiate(moveContextPrefab);
        currentMoveContext.transform.position = transform.position + new Vector3(0, 2, 0);

        isEntered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        if (currentMoveContext != null) Destroy(currentMoveContext.gameObject);

        isEntered = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyData.Player.InteractionKey) && isEntered)
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        PlayerHealth.CurrentHealth = 5;
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
            default:
                SceneController.Instance.LoadScene(SceneController.SceneType.Stage1_1);
                break;
        }
    }
}
