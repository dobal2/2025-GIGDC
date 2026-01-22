using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class DialogGenerator : MonoBehaviour
{
    [field: SerializeField] public Chapter Chapter { get; set; }

    [SerializeField] private Canvas dialogCanvasPrefab;
    [SerializeField] private DialogView dialogViewPrefab;
    [SerializeField] private DialogEventReciever dialogEventRecieverPrefab;
    [SerializeField] private KeyData keyData;

    private int dialogIndex;
    private Dialog dialog;
    private DialogView dialogView;
    private DialogEventReciever dialogEventReciever;
    private Canvas dialogCanvas;

    private int processingSelectionIndex = 0;
    private int selectionSkipIndex = 0;

    private InputManager.InputContext pastInputContext;

    public void GenerateDialog()
    {
        dialog = null;
        dialogIndex = -1;

        dialogCanvas = Instantiate(dialogCanvasPrefab);
        
        dialogEventReciever = Instantiate(dialogEventRecieverPrefab);
        dialogEventReciever.Initialize(dialog);

        SetDialog();
        ProcessDialog();
    }

    public void SetDialog()
    {
        InputManager.Instance.RegisterDialog(this);
        pastInputContext = InputManager.Instance.CurrentContext;
        InputManager.Instance.CurrentContext = InputManager.InputContext.Dialog;
    }

    public void ProcessDialog()
    {
        if (dialogIndex + 1 >= Chapter.Dialogs.Length && dialogView.IsCompleted)
        {
            if(dialogView != null) Destroy(dialogView.gameObject);
            if(InputManager.Instance.CurrentContext == InputManager.InputContext.Dialog) InputManager.Instance.CurrentContext = pastInputContext;
            return;
        }

        if(dialogView != null && !dialogView.IsCompleted)
        {
            dialogView.CompleteDialog();
        }
        else
        {
            if (dialogView != null)
            {
                if (processingSelectionIndex <= 0)
                {
                    dialogIndex += selectionSkipIndex;
                    selectionSkipIndex = 0;
                }
                else processingSelectionIndex--;

                Destroy(dialogView.gameObject);
            }

            dialog = Chapter.Dialogs[++dialogIndex];
            dialogView = Instantiate(dialogViewPrefab, dialogCanvas.transform);

            dialogView.Dialog(dialog, dialogCanvas, dialogEventReciever);
            dialogView.OnSelect += SyncSelection;
        }
    }

    public void SyncSelection(List<DialogView.SelectionData> selectionDatas, int selectedIndex)
    {
        int skipIndex = 0;
        for(int index = 0; index < selectedIndex; index++)
        {
            skipIndex += selectionDatas[index].Count;
        }

        dialogIndex += skipIndex;

        processingSelectionIndex += selectionDatas[selectedIndex].Count;

        for(int index = selectedIndex + 1; index < selectionDatas.Count; index++)
        {
            selectionSkipIndex += selectionDatas[index].Count;
        }

        ProcessDialog();
    }

    private void OnDestroy()
    {
        if(dialogEventReciever != null)
            Destroy(dialogEventReciever.gameObject);
    }
}
