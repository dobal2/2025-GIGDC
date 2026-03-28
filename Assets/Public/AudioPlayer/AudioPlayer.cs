using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod]
    public static void GenerateAudioPlayer()
    {
        var audioPlayer = new GameObject(nameof(AudioPlayer));
        audioPlayer.AddComponent<AudioPlayer>();
    }
#endif

    [field: SerializeField] public AudioRegistry AudioRegistry { get; private set; }

    [SerializeField] private AudioSource sfxPrefab;

    private List<AudioSource> sfxSourcePool = new();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        AudioSource audioSource = GetAvailableSFXSource();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (var sfxSource in sfxSourcePool)
        {
            if (!sfxSource.isPlaying)
                return sfxSource;
        }

        AudioSource newSource = Instantiate(sfxPrefab, transform);
        sfxSourcePool.Add(newSource);

        return newSource;
    }
}
