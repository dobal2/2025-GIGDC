using UnityEngine;

public class DialogGenerator : MonoBehaviour
{
    [field: SerializeField] public Chapter Chapter { get; set; }

    [SerializeField] private Canvas dialogCanvasPrefab;
    [SerializeField] private DialogView dialogViewPrefab;
    [SerializeField] private KeyData keyData;

    private int currentDialogIndex;
    private Dialog currentDialog;
    private DialogView currentDialogView;
    private Canvas dialogCanvas;

    private void Awake()
    {
        currentDialog = null;
        currentDialogIndex = -1;
        dialogCanvas = Instantiate(dialogCanvasPrefab);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if(Input.GetKeyDown(keyData.Dialog.ProcessKey))
            ProcessDialog();
    }

    private void ProcessDialog()
    {
        if (currentDialogIndex + 1 >= Chapter.Dialogs.Length)
            return;

        if(currentDialogView != null && !currentDialogView.IsCompleted)
        {
            currentDialogView.CompleteDialog();
        }
        else
        {
            if(currentDialogView != null) Destroy(currentDialogView.gameObject);

            currentDialog = Chapter.Dialogs[++currentDialogIndex];
            currentDialogView = Instantiate(dialogViewPrefab, dialogCanvas.transform);

            currentDialogView.Dialog(currentDialog, dialogCanvas);
        }
    }
}
