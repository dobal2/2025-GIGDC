using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogView : MonoBehaviour
{
    [HideInInspector] public bool SkipProcessingText = false;

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private RectTransform backGroundRect;

    private RectTransform rectTransform;
    private static readonly Vector2 baseDialogPosition = new Vector2(0, 2);
    private static readonly float dialogWritingDelay = 0.05f;

    private Dialog allocatedDialog = null;
    private Coroutine currentDialogCoroutine = null;

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
    }
    public void Dialog(Dialog dialog, Canvas dialogCanvas)
    {
        allocatedDialog = dialog;
        SetPosition(allocatedDialog, dialogCanvas);
        currentDialogCoroutine = StartCoroutine(ProcessText());
    }
    private IEnumerator ProcessText()
    {
        string line = allocatedDialog.Line;

        dialogText.text = "";
        UpdateBackGround();
        
        for (int index = 0; index < line.Length; index++)
        {
            string commandTag = InspectCommandTag(line, index);
            if(commandTag != null)
            {
                if(ProcessCommandTag(commandTag))
                {
                    line = line.Remove(index, commandTag.Length + 2);
                    index--;
                    continue;
                }
            }

            dialogText.text = line.Substring(0, index + 1) ?? "";
            UpdateBackGround();

            if (!SkipProcessingText) yield return new WaitForSeconds(dialogWritingDelay);
        }

        if(SkipProcessingText) SkipProcessingText = false;
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
        string commandParams = commandTag.Substring(paramStartIndex, paramEndIndex - paramStartIndex);

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

    [DialogCommand("ChangeWritingScale")]
    private void ChangeWritingScale(string[] lines)
    {

    }

    [DialogCommand("DelayWriting")]
    private void DelayWriting(string[] lines)
    {

    }

    [DialogCommand("ShakeCamera")]
    private void ShakeCamera(string[] lines)
    {

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
