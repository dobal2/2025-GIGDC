using Unity.Mathematics;
using UnityEngine;

public class GlassWall : Monster
{
    [SerializeField] private GameObject glassEffectPrefab;
    private AudioSource glassSound;

    protected override void Start()
    {
        Destroy(gameObject,20);
    }

    protected override void Attack()
    {
        
    }

    void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
        transform.Translate(new Vector3(speed,0,0) * Time.deltaTime);
    }

    public override void TakeDamage(float damage)
    {
        hp -= damage;
    }
    

    protected override void Die()
    {
        ParticleSystem newGlassEffect = Instantiate(glassEffectPrefab, transform.position, Quaternion.Euler(-180f, 90f, 0f)).GetComponent<ParticleSystem>();
        newGlassEffect.Play();
        Destroy(newGlassEffect.gameObject, 3f);

        if (glassSound == null)
            glassSound = GetComponent<AudioSource>();

        if (glassSound != null && glassSound.clip != null)
        {
            GameObject audioObj = new GameObject("TempGlassBreakSound");
            audioObj.transform.position = transform.position;

            AudioSource tempAudio = audioObj.AddComponent<AudioSource>();
            tempAudio.clip = glassSound.clip;
            tempAudio.volume = glassSound.volume;
            tempAudio.pitch = glassSound.pitch;
            tempAudio.spatialBlend = glassSound.spatialBlend;
            tempAudio.outputAudioMixerGroup = glassSound.outputAudioMixerGroup;

            tempAudio.Play();

            Destroy(audioObj, tempAudio.clip.length);
        }
        else
        {
            Debug.LogWarning("GlassWall: AudioSource or clip is missing.");
        }

        Destroy(gameObject);
    }
    
}
