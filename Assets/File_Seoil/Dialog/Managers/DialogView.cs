using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogView : MonoBehaviour
{
    [HideInInspector] public bool SkipProcessingText = false;

    public bool IsCompleted { get; private set; }
    public int IndexSkipCount { get; private set; } = 0;

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private RectTransform backGroundRect;

    private RectTransform rectTransform;
    private static readonly Vector2 baseDialogPosition = new Vector2(0, 2);
    private static readonly float dialogWritingDelay = 0.05f;

    private Dialog allocatedDialog = null;
    private Coroutine currentDialogCoroutine = null;

    private float additionalDelay = 0;

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
    public void Dialog(Dialog dialog, Canvas dialogCanvas)
    {
        allocatedDialog = dialog;
        SetPosition(allocatedDialog, dialogCanvas);
        currentDialogCoroutine = StartCoroutine(ProcessText());
    }
    public int CompleteDialog()
    {
        SkipProcessingText = true;
        return 0;
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

        IsCompleted = true;
    }
    private void UpdateBackGround()
    {
        dialogText.ForceMeshUpdate();

        Vector2 newSize = new Vector2(dialogText.preferredWidth, backGroundRect.sizeDelta.y);
        backGroundRect.sizeDelta = newSize;
    }
    private void SetPosition(Dialog dialog, Canvas dialogCanvas)
    {
        Vector3 worldPos = dialog.Target.Transform.position + baseDialogPosition.ToVector3();
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

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
            commandDatas[commandName].Invoke(this, new object[] { commandParams
                .Split(',')
                .Select(item => item.Trim())
                .ToArray() }
            );

            return true;
        }
        else return false;
    }

    //============================================================== Commands

    [DialogCommand("MoveCharactor")]
    private void MoveCharactor(string[] lines)
    {
        
    }

    [DialogCommand("AnimateCharactor")]
    private void AnimateCharactor(string[] lines)
    {

    }

    [DialogCommand("DelayWriting")]
    private void DelayWriting(string[] lines)
    {
        if (lines.Length != 1)
        {
            Debug.LogWarning("Invalid DelayWriting Parameters: " + lines);
            return;
        }

        try
        {
            float delayTime = Convert.ToSingle(lines[0]);
            additionalDelay += delayTime;
        }
        catch(FormatException)
        {
            Debug.LogWarning("Invalid DelayWriting Parameters: " + lines[0]);
            return;
        }
    }

    [DialogCommand("ShakeCamera")]
    private void ShakeCamera(string[] lines)
    {
        if(lines.Length != 1)
        {
            Debug.LogWarning("Invalid ShakeCamera Parameters: " + lines);
            return;
        }
        float duration = 0;

        try
        {
            duration = Convert.ToSingle(lines[0]);
        }
        catch (FormatException)
        {
            Debug.LogWarning("Invalid ShakeCamera Parameters: " + lines);
            return;
        }

        CameraUtility.TopCamera.transform
            .DOShakePosition(
                duration: duration, 
                strength: 0.5f
                );
    }

    [DialogCommand("ScaleCamera")]
    private void ScaleCamera(string[] lines)
    {
        
    }

    [DialogCommand("FadeCameraToDark")]
    private void FadeCameraToDark(string[] lines)
    {

    }

    [DialogCommand("FadeCameraToBright")]
    private void FadeCameraToBright(string[] lines)
    {

    }

    [DialogCommand("ChangeScene")]
    private void ChangeScene(string[] lines)
    {

    }

    [DialogCommand("GiveItem")]
    private void GiveItem(string[] lines)
    {

    }

    [DialogCommand("PlaySound")]
    private void PlaySound(string[] lines)
    {

    }

    [DialogCommand("MoveCamera")]
    private void MoveCamera(string[] lines)
    {

    }

    [DialogCommand("SetTimeScale")]
    private void SetTimeScale(string[] lines)
    {

    }
}
