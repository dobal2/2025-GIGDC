using UnityEngine;

public class CaptainController : MonoBehaviour
{
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();

    public void Captain_Walk() => animator.Play("Captain_Walk");

    public void Captain_Sitdown() => animator.Play("Captain_Sitdown");

    public void Captain_Transform() => animator.Play("Captain_Transform");

    public void Death_Walk() => animator.Play("Death_Walk");

    public void Death_Greet() => animator.Play("Death_Greet");
}
