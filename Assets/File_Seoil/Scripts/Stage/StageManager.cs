using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Animator viewAnimator;
    [SerializeField] private GameObject map;

    [SerializeField] private Effect boomEffectPrefab;

    [SerializeField] private SceneController.SceneType moveSceneType;

    private void Awake()
    {
        map.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Clear();
    }

    public void Clear()
    {
        viewAnimator.SetTrigger("OnClear");
    }

    public void Fail()
    {

    }


    [SerializeField]
    private void MoveScene()
    {
        SceneController.Instance.LoadScene(moveSceneType);
    }

    [SerializeField]
    private void ShowMap()
    {
        map.SetActive(true);
        Instantiate(boomEffectPrefab);
    }
}
