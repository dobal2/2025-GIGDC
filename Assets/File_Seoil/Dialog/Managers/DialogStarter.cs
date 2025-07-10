using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(DialogGenerator))]
public class DialogStarter : MonoBehaviour
{
    private bool isEntered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && !isEntered)
        {
            isEntered = true;
            GetComponent<DialogGenerator>().GenerateDialog();
        }
    }
}
