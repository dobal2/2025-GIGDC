using System;
using UnityEngine;
using UnityEngine.VFX;

public class Shield : MonoBehaviour {
    public VisualEffect shieldSpawn;
    public VisualEffect shieldBroke;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            shieldSpawn.gameObject.SetActive(true);
            shieldSpawn.Play();
        }

        if (Input.GetKeyUp(KeyCode.Tab)) {
            shieldBroke.Play();
            shieldSpawn.gameObject.SetActive(false);
        }
    }
}
