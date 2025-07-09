using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneMover : MonoBehaviour
{
    [SerializeField] private SceneController.SceneType sceneType;
    [SerializeField] private bool isProgress;
    [SerializeField] private bool isImmediatelyMove;

    private void Awake()
    {
        if (!isImmediatelyMove) return;

        LoadScene();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            LoadScene();
    }

    private void LoadScene()
    {
        if (isProgress)
            Stage.Progress();
        SceneController.Instance.LoadScene(sceneType);
    }
}
