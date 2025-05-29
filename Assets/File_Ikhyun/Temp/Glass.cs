using System;
using UnityEngine;

public class Glass : MonoBehaviour {
    private SpriteRenderer sr;

    [SerializeField] private ParticleSystem ps;

    private bool isBroken = false;
    
    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) {
            Broke();
        }
    }

    public void Broke() {
        if (isBroken) return;
        isBroken = true;

        sr.enabled = false;
        ps.Play();
        StartCoroutine(WaitAndDestroy());
    }

    private System.Collections.IEnumerator WaitAndDestroy() {
        yield return new WaitUntil(() => !ps.IsAlive(true));
        Destroy(gameObject);
    }

}
