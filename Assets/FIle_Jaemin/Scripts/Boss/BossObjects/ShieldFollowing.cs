using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldFollowing : MonoBehaviour
{
    [SerializeField] private GameObject shieldBrokeEffect;
    private GameObject player;
    private AudioSource shieldBrokeSound;
    
    void Start()
    {
        StartCoroutine(Break(3));
        player = GameObject.FindGameObjectWithTag("Player");
        shieldBrokeSound = GetComponent<AudioSource>();
        if (player == null)
        {
            Debug.LogWarning("No Player");
            return;
        }
        
    }

    IEnumerator Break(float delay)
    {
        yield return new WaitForSeconds(delay);

        // VFX 처리
        VisualEffect newShieldBroke = Instantiate(shieldBrokeEffect, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
        newShieldBroke.Play();
        Destroy(newShieldBroke.gameObject, 2f);

        // SFX 처리 - AudioSource가 유효할 때만 실행
        if (shieldBrokeSound != null)
        {
            GameObject audioObj = new GameObject("TempShieldBreakSound");
            audioObj.transform.position = transform.position;

            AudioSource tempAudio = audioObj.AddComponent<AudioSource>();
            tempAudio.clip = shieldBrokeSound.clip;
            tempAudio.volume = shieldBrokeSound.volume;
            tempAudio.pitch = shieldBrokeSound.pitch;
            tempAudio.spatialBlend = shieldBrokeSound.spatialBlend;
            tempAudio.outputAudioMixerGroup = shieldBrokeSound.outputAudioMixerGroup;
            tempAudio.Play();

            Destroy(audioObj, tempAudio.clip.length);
        }
        else
        {
            Debug.LogWarning("Shield break sound is missing!");
        }

        // 본체 제거
        Destroy(gameObject);
    }


    
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + new Vector3(0,player.transform.localScale.y/2,0);
        }
    }
}
