using System;
using UnityEngine;

public class PlayerParticleController : MonoBehaviour {
    
    [SerializeField] private ParticleSystem movementParticle;

    [Range(0, 10)] 
    [SerializeField] private float occurAfterVelocity;

    [Range(0, .2f)] 
    [SerializeField] private float dustFromationPeriod;

    [SerializeField] private Rigidbody2D playerRb;
    
    private float counter;
    private bool isOnGround;

    [SerializeField] private ParticleSystem landParticle;
    
    private void Update() {
        
        counter += Time.deltaTime;

        if (isOnGround && Mathf.Abs(playerRb.linearVelocityX) > occurAfterVelocity) {
            if (counter > dustFromationPeriod) {
                movementParticle.Play();
                counter = 0;
            }
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Ground")) {
            landParticle.Play();
            isOnGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Ground")) {
            landParticle.Play();
            isOnGround = false;
        }
    }
}
