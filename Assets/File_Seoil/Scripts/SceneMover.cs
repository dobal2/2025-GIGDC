using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneMover : MonoBehaviour
{
    [Header("Fixed Data")]
    [SerializeField] private SpriteRenderer moveContextPrefab;
    [SerializeField] private KeyData keyData;

    [Space]
    [SerializeField] private SceneController.SceneType sceneType;
    [SerializeField] private bool isProgress;
    [SerializeField] private bool isImmediatelyMove;

    private SpriteRenderer currentMoveContext = null;
    private bool isEntered = false;

    private void Awake()
    {
        if (!isImmediatelyMove) return;

        LoadScene();
    }

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
        if(Input.GetKeyDown(keyData.Player.InteractionKey) && isEntered)
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (isProgress)
            Stage.Progress();
        SceneController.Instance.LoadScene(sceneType);
    }
}
