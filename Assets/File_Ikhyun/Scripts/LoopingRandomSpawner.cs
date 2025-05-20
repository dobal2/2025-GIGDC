using UnityEngine;

public class LoopingRandomSpawner : MonoBehaviour
{
    public ParticleSystem ps;
    public float minInterval = 0.5f;
    public float maxInterval = 1.2f;

    private float nextEmitTime;

    void Start()
    {
        ScheduleNext();
    }

    void Update()
    {
        if (Time.time >= nextEmitTime)
        {
            ps.Emit(1); // 한 개씩 스폰
            ScheduleNext();
        }
    }

    void ScheduleNext()
    {
        nextEmitTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}