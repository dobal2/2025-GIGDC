using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public interface IDialogGenerator
{
    public void SyncSelection(List<DialogView.SelectionData> selectionDatas, int selectedIndex);
}

public class DialogGenerator : MonoBehaviour, IDialogGenerator
{
    [field: SerializeField] public Chapter Chapter { get; set; }

    [SerializeField] private Canvas dialogCanvasPrefab;
    [SerializeField] private DialogView dialogViewPrefab;
    [SerializeField] private KeyData keyData;

    private int currentDialogIndex;
    private Dialog currentDialog;
    private DialogView currentDialogView;
    private Canvas dialogCanvas;

    private int processingSelectionIndex = 0;
    private int selectionSkipIndex = 0;

    private void Awake()
    {
        currentDialog = null;
        currentDialogIndex = -1;
        dialogCanvas = Instantiate(dialogCanvasPrefab);
    }

    private void Start()
    {
        SetDialog();
    }

    public void SetDialog()
    {
        InputManager.Instance.RegisterDialog(this);
        InputManager.Instance.currentContext = InputManager.InputContext.Dialog;
    }

    public void ProcessDialog()
    {
        if (currentDialogIndex + 1 >= Chapter.Dialogs.Length)
            return;

        if(currentDialogView != null && !currentDialogView.IsCompleted)
        {
            currentDialogView.CompleteDialog();
        }
        else
        {
            if (currentDialogView != null)
            {
                if (processingSelectionIndex <= 0)
                {
                    currentDialogIndex += selectionSkipIndex;
                    selectionSkipIndex = 0;
                }
                else processingSelectionIndex--;

                Destroy(currentDialogView.gameObject);
            }

            currentDialog = Chapter.Dialogs[++currentDialogIndex];
            currentDialogView = Instantiate(dialogViewPrefab, dialogCanvas.transform);

            currentDialogView.Dialog(currentDialog, dialogCanvas, this);
        }
    }

    public void SyncSelection(List<DialogView.SelectionData> selectionDatas, int selectedIndex)
    {
        int skipIndex = 0;
        for(int index = 0; index < selectedIndex; index++)
        {
            skipIndex += selectionDatas[index].Count;
        }

        currentDialogIndex += skipIndex;

        processingSelectionIndex += selectionDatas[selectedIndex].Count;

        for(int index = selectedIndex + 1; index < selectionDatas.Count; index++)
        {
            selectionSkipIndex += selectionDatas[index].Count;
        }

        ProcessDialog();
    }
}
