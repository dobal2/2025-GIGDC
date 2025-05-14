using UnityEngine;

public interface IVFXEffect
{
    void Initialize(GameObject prefab);
    void Play(Vector3 position, float intensity);
    void Stop();
}