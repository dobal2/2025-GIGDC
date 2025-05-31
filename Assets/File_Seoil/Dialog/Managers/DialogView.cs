using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;

    private RectTransform rectTransform;
    private static readonly Vector2 baseDialogPosition = new Vector2(0, 2);
    private static readonly float dialogWritingDelay = 0.05f;

    private Dialog allocatedDialog = null;
    private Coroutine currentDialogCoroutine = null;

    private Dictionary<string, Action<string[]>> commandDatas = new();

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

            yield return new WaitForSeconds(dialogWritingDelay);
        }
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
            commandDatas[commandName].Invoke(commandParams
                .Split(',')
                .Select(item => item.Trim())
                .ToArray()
            );

            return true;
        }
        else return false;
    }
}
