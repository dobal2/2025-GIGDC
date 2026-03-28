using UnityEngine;

[CreateAssetMenu(fileName = "AudioRegistry", menuName = "Audio/Audio Registry")]
public class AudioRegistry : ScriptableObject
{
    [field: SerializeField] public AudioClip Interaction { get; private set; }
}
