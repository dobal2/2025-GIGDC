using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Lobby_DialogSelector : MonoBehaviour
{
    [SerializeField] private DialogGenerator firstChapter;
    [SerializeField] private DialogGenerator first_secondChapter;
    [SerializeField] private DialogGenerator thirdChapter;
    [SerializeField] private DialogGenerator fifthChapter;
    [SerializeField] private DialogGenerator seventhChapter;

    private bool isPlayed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayed) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            switch(Stage.Data)
            {
                case StageDataType.Start:
                    firstChapter.GenerateDialog();
                    break;
                case StageDataType.Tutorial:
                    first_secondChapter.GenerateDialog();
                    break;
                case StageDataType.Stage1:
                    thirdChapter.GenerateDialog();
                    break;
                case StageDataType.Stage2:
                    fifthChapter.GenerateDialog();
                    break;
                case StageDataType.Stage3:
                    seventhChapter.GenerateDialog();
                    break;
            }

            isPlayed = true;
        }
    }
}