using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DG.Tweening;
using DG.Tweening.Plugins;
using NUnit.Framework.Internal.Commands;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogView : MonoBehaviour
{
    [HideInInspector] public bool SkipProcessingText = false;

    public bool IsCompleted { get; private set; }

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private RectTransform backGroundRect;

    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private RectTransform rectTransform;
    private static readonly Vector2 baseDialogPosition = new Vector2(0, 2);
    private static readonly float dialogWritingDelay = 0.05f;

    private Dialog allocatedDialog = null;
    private Coroutine currentDialogCoroutine = null;

    private float additionalDelay = 0;

    private List<SelectionData> selectionDatas = new();
    private int selectionIndex = 0;

    private IDialogGenerator dialogGenerator = null;

    private static Dictionary<string, MethodInfo> commandDatas = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeCommandDatas()
    {
        MethodInfo[] methods = typeof(DialogView).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach(MethodInfo method in methods)
        {
            DialogCommandAttribute attribute = method.GetCustomAttribute<DialogCommandAttribute>();

            if (attribute != null)
            {
                commandDatas.Add(attribute.CommandName, method);
            }
        }
    }
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        IsCompleted = false;
    }
    private void Update()
    {
        if (selectionDatas.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectionIndex <= 0)
                selectionIndex = selectionDatas.Count - 1;
            else selectionIndex--;

            PrintSelection();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectionIndex++;
            if (selectionIndex >= selectionDatas.Count)
                selectionIndex = 0;

            PrintSelection();
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            IsCompleted = true;
            dialogGenerator.SyncSelection(selectionDatas, selectionIndex);
        }
    }
    public void Dialog(Dialog dialog, Canvas dialogCanvas, IDialogGenerator dialogGenerator)
    {
        allocatedDialog = dialog;
        SetPosition(allocatedDialog, dialogCanvas);
        this.dialogGenerator = dialogGenerator;
        currentDialogCoroutine = StartCoroutine(ProcessText());
    }
    private void PrintSelection()
    {
        dialogText.text = selectionDatas[selectionIndex].Line;
        UpdateBackGround();
    }
    public void CompleteDialog()
    {
        if (selectionDatas.Count > 0)
            return;

        SkipProcessingText = true;
    }
    private IEnumerator ProcessText()
    {
        string line = allocatedDialog.Line;

        dialogText.text = "";        
        UpdateBackGround();

        int sizeTagNumber = 0;

        for (int index = 0; index < line.Length; index++)
        {
            string commandTag = InspectCommandTag(line, index);

            if (commandTag != null)
            {
                if (ProcessCommandTag(commandTag))
                {
                    line = line.Remove(index, commandTag.Length + 2);
                    index--;
                    continue;
                }
                else index += commandTag.Length + 1;
            }

            if (additionalDelay != 0)
            {
                if(!SkipProcessingText) yield return new WaitForSeconds(additionalDelay);
                additionalDelay = 0;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(line, 0, index + 1);

            if(commandTag != null)
            {
                if (commandTag.Contains("size="))
                    sizeTagNumber++;
                else if (commandTag.Contains("/size"))
                    sizeTagNumber--;
            }

            for (int i = 0; i < sizeTagNumber; i++)
                stringBuilder.Append("</size>");

            dialogText.text = stringBuilder.ToString();

            UpdateBackGround();

            if (!SkipProcessingText) yield return new WaitForSeconds(dialogWritingDelay);
        }

        if(selectionDatas.Count == 0) IsCompleted = true;
    }
    private void UpdateBackGround()
    {
        dialogText.ForceMeshUpdate();

        Vector2 newSize = new Vector2(dialogText.preferredWidth, backGroundRect.sizeDelta.y);
        backGroundRect.sizeDelta = newSize;

        ClampUIInsideParent(rectTransform.parent.GetComponent<RectTransform>(), rectTransform);
    }

    private void ClampUIInsideParent(RectTransform canvasRect, RectTransform uiRect)
    {
        Vector2 canvasSize = canvasRect.rect.size;

        Vector2 uiSize = uiRect.rect.size;
        uiSize.x += 75;
        Vector2 pivot = uiRect.pivot;

        Vector2 pos = uiRect.anchoredPosition;

        float left = pos.x - (uiSize.x * pivot.x);
        float right = pos.x + (uiSize.x * (1 - pivot.x));
        float bottom = pos.y - (uiSize.y * pivot.y);
        float top = pos.y + (uiSize.y * (1 - pivot.y));

        float canvasLeft = -canvasSize.x / 2f;
        float canvasRight = canvasSize.x / 2f;
        float canvasBottom = -canvasSize.y / 2f;
        float canvasTop = canvasSize.y / 2f;

        if (left < canvasLeft)
            pos.x += canvasLeft - left;
        else if (right > canvasRight)
            pos.x -= right - canvasRight;

        if (bottom < canvasBottom)
            pos.y += canvasBottom - bottom;
        else if (top > canvasTop)
            pos.y -= top - canvasTop;

        uiRect.anchoredPosition = pos;
    }


    private void SetPosition(Dialog dialog, Canvas dialogCanvas)
    {
        Vector3 worldPos = dialog.Target.Transform.position + baseDialogPosition.ToVector3();

        Vector2 screenPos = CameraUtility.TopCamera.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = dialogCanvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            dialogCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPoint
        );

        rectTransform.anchoredPosition = localPoint;
    }
    private string InspectCommandTag(string line, int index)
    {
        if(line[index] != '<') return null;

        int closeIndex = line.IndexOf('>', index);
        if (closeIndex == -1)
            return null;

        string commandTag = line.Substring(index + 1, closeIndex - index - 1);
        
        return commandTag;
    }
    private bool ProcessCommandTag(string commandTag)
    {
        int paramStartIndex = commandTag.IndexOf('(');
        int paramEndIndex = commandTag.IndexOf(')');

        if (paramStartIndex == -1) return false;
        if (paramEndIndex == -1) return false;

        string commandName = commandTag.Substring(0, paramStartIndex);
        string commandParams = commandTag.Substring(paramStartIndex + 1, paramEndIndex - paramStartIndex - 1);

        if (commandDatas.ContainsKey(commandName))
        {
            commandDatas[commandName].Invoke(this, new object[] { commandParams } );

            return true;
        }
        else return false;
    }

    public struct SelectionData
    {
        public string Line;
        public int Count;
    }

    //============================================================== Commands

    [DialogCommand("Selection")]
    private void Selection(string line)
    {
         string[] lines = line.Split('}');
        for(int index = 0; index < lines.Length - 1; index++)
            lines[index] = lines[index].Substring(lines[index].IndexOf('{') + 1, lines[index].Length - lines[index].IndexOf('{') - 1);

        foreach (var item in lines)
        {
            if(item.Length == 0) continue;

            string[] selectionLine = item.Split(',');
            SelectionData selection = new SelectionData();
            
            selection.Line = selectionLine[0];
            selection.Count = int.Parse(selectionLine[1]);

            selectionDatas.Add(selection);
        }

        leftArrow.color = Color.white;
        rightArrow.color = Color.white;

        if (currentDialogCoroutine != null) StopCoroutine(currentDialogCoroutine);
        PrintSelection();
    }

    [DialogCommand("StartBoss")]
    private void StartBoss(string line)
    {
        Boss.Instance.StartBattle();
    }

    [DialogCommand("MoveCharactor")]
    private void MoveCharactor(string line)
    {
        
    }

    [DialogCommand("AnimateCharactor")]
    private void AnimateCharactor(string line)
    {
        if(line == "Captain_Transform")
        {
            allocatedDialog.Target.Transform.GetComponentInParent<Animator>().SetBool("IsReaper", true);
        }
    }

    [DialogCommand("DelayWriting")]
    private void DelayWriting(string line)
    {

        try
        {
            float delayTime = Convert.ToSingle(line);
            additionalDelay += delayTime;
        }
        catch(FormatException)
        {
            Debug.LogWarning("Invalid DelayWriting Parameters: " + line);
            return;
        }
    }

    [DialogCommand("ShakeCamera")]
    private void ShakeCamera(string line)
    {
        float duration = 0;

        try
        {
            duration = Convert.ToSingle(line);
        }
        catch (FormatException)
        {
            Debug.LogWarning("Invalid ShakeCamera Parameters: " + line);
            return;
        }

        CameraUtility.TopCamera.transform
            .DOShakePosition(
                duration: duration, 
                strength: 0.5f
                );
    }

    [DialogCommand("ScaleCamera")]
    private void ScaleCamera(string line)
    {
        
    }

    [DialogCommand("FadeCameraToDark")]
    private void FadeCameraToDark(string line)
    {

    }

    [DialogCommand("FadeCameraToBright")]
    private void FadeCameraToBright(string line)
    {

    }

    [DialogCommand("ChangeScene")]
    private void ChangeScene(string line)
    {
        SceneController.Instance.LoadScene(line);
    }

    [DialogCommand("GiveItem")]
    private void GiveItem(string line)
    {
        switch(line)
        {
            case "Bow":
                WeaponDatabase.Instance.unlockedWeapons.Bow = true;
                break;
            case "Bomb":
                WeaponDatabase.Instance.unlockedWeapons.Bomb = true;
                break;
        }
    }

    [DialogCommand("PlaySound")]
    private void PlaySound(string line)
    {

    }

    [DialogCommand("MoveCamera")]
    private void MoveCamera(string line)
    {

    }

    [DialogCommand("SetTimeScale")]
    private void SetTimeScale(string line)
    {

    }

    [DialogCommand("Progress")]
    private void Progess()
    {
        Stage.Progress();
    }
}
