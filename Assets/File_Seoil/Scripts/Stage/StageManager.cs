using System;
using UnityEngine;

public static class Stage
{
    private static readonly string DATA_SAVE_NAME = "StageData";
    private static StageDataType data;
    public static StageDataType Data
    {
        get => data;
        set
        {
            data = value;
            PlayerPrefs.SetInt(DATA_SAVE_NAME, (int)data);
        }
    }
    public static void LoadData()
    {
        data = (StageDataType)PlayerPrefs.GetInt(DATA_SAVE_NAME);
    }
    public static void Progress()
    {
        switch (Stage.Data)
        {
            case StageDataType.Start:
                Stage.Data = StageDataType.Tutorial;
                break;
            case StageDataType.Tutorial:
                Stage.Data = StageDataType.Stage1;
                break;
            case StageDataType.Stage1:
                Stage.Data = StageDataType.Stage2;
                break;
            case StageDataType.Stage2:
                Stage.Data = StageDataType.Stage3;
                break;
            case StageDataType.Stage3:
                Stage.Data = StageDataType.Stage4;
                break;
            case StageDataType.Stage4:
                break;
        }
    }
}

[Serializable]
public enum StageDataType
{
    Start, Tutorial, Stage1, Stage2, Stage3, Stage4
}

public class StageManager : MonoBehaviour
{
    [SerializeField] private Animator viewAnimator;
    [SerializeField] private GameObject map;

    [SerializeField] private Effect boomEffectPrefab;

    [SerializeField] private bool enableLoadScene = true;
    [SerializeField] private SceneType moveSceneType;

    [SerializeField] private SoundPlayer shutterSoundPrefab;

    [Header("Progress")]
    [SerializeField] private bool isProgress;
    [SerializeField] private StageDataType stageType;

    [Header("Move Type")]
    [SerializeField] private SceneLoader.SceneChangeAnimation sceneChangeAnimation;

    public static StageManager Instance { get; private set; }
    public static event Action<int> OnObjectKilled;
    public static event Action OnAllObjectKilled;

    private static int objects = 0;

    public static int Objects
    {
        get => objects;
        set
        {
            if(value - objects < 0)
                OnObjectKilled?.Invoke(value - objects);

            objects = value;
            Debug.Log("Left Objects : " + objects);
            if (objects <= 0)
            {
                objects = 0;
                OnAllObjectKilled?.Invoke();
                if(PlayerController.Instance.CurrentStateType != PlayerStateType.Death) Instance.Clear();
            }

        }
    }

    private void Awake()
    {
        Instance = this;
        map.SetActive(false);
    }

    public static void SetObjects(int _objects) =>
        objects = _objects;

    public void Clear()
    {
        if (isProgress)
        {
            Stage.Data = stageType;
            MoveScene();
        }
        else viewAnimator.SetTrigger("OnClear");
    }

    public static void Fail() => Instance.FailByInstance();

    public void FailByInstance()
    {
        SetObjects(0);
        PlayerHealth.CurrentHealth = PlayerHealth.Instance.MaxHealth;

        switch (Stage.Data)
        {
            case StageDataType.Tutorial:
                SceneLoader.Instance.LoadScene(SceneType.Stage1_1);
                break;
            case StageDataType.Stage1:
                SceneLoader.Instance.LoadScene(SceneType.Stage2_1);
                break;
            case StageDataType.Stage2:
                SceneLoader.Instance.LoadScene(SceneType.Stage3_1);
                break;
            case StageDataType.Stage3:
                SceneLoader.Instance.LoadScene(SceneType.Stage4_1);
                break;
            case StageDataType.Stage4:
                break;
            default:
                SceneLoader.Instance.LoadScene(SceneType.Lobby_Over);
                break;
        }
    }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Boss.Instance != null)
            Boss.Instance.TakeDamage(200);

        if (Input.GetKeyDown(KeyCode.W) && PlayerHealth.Instance != null)
            PlayerHealth.CurrentHealth = PlayerHealth.Instance.MaxHealth;

        if (Input.GetKeyDown(KeyCode.E))
            Objects = 0;
    }
#endif

    public void MoveScene()
    {
        SceneLoader.Instance.LoadScene(moveSceneType, sceneChangeAnimation);
    }

    public void ShowMap()
    {
        Instantiate(shutterSoundPrefab);
        map.SetActive(true);
        Instantiate(boomEffectPrefab);
    }
}
