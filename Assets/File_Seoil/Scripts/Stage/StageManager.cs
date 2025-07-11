using System;
using Unity.VisualScripting;
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

    [SerializeField] private SceneController.SceneType moveSceneType;

    [Header("Progress")]
    [SerializeField] private bool isProgress;
    [SerializeField] private StageDataType stageType;

    [Header("Move Type")]
    [SerializeField] private SceneController.SceneChangeAnimation sceneChangeAnimation;

    private static StageManager Instance { get; set; }

    private static int objects = 0;

    public static int Objects
    {
        get => objects;
        set
        {
            objects = value;
            Debug.Log("Left Objects : " + objects);
            if (objects <= 0)
            {
                objects = 0;
                Instance.Clear();
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
        

        switch (Stage.Data)
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
                SceneController.Instance.LoadScene(SceneController.SceneType.Lobby_Over);
                break;
        }
    }


    [SerializeField]
    private void MoveScene()
    {
        SceneController.Instance.LoadScene(moveSceneType, sceneChangeAnimation);
    }

    [SerializeField]
    private void ShowMap()
    {
        map.SetActive(true);
        Instantiate(boomEffectPrefab);
    }
}
