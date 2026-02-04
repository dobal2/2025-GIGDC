using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance {  get; private set; }

    

    private void OnEndTutorial()
    {
        Stage.Progress();
    }
}
