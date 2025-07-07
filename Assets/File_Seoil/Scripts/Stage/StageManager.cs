using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Animator viewAnimator;
    [SerializeField] private GameObject map;

    [SerializeField] private Effect boomEffectPrefab;

    [SerializeField] private SceneController.SceneType moveSceneType;

    public static StageManager Instance { get; private set; }

    private static int objects = 0;

    public static int Objects
    {
        get => objects;
        set
        {
            objects = value;
            if (objects == 0) Instance.Clear();
        }
    }

    private void Awake()
    {
        Instance = this;
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
