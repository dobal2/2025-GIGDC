using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(DialogGenerator))]
public class DialogSelector : MonoBehaviour
{
    private bool isPlayed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayed) return;

        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GetComponent<DialogGenerator>().GenerateDialog();
            isPlayed = true;
        }
    }
}
