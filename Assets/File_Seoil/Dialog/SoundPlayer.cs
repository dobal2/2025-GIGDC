using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
