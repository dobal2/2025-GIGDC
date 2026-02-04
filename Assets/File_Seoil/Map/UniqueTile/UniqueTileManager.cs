using System.Collections.Generic;
using UnityEngine;

public class UniqueTileManager : MonoBehaviour
{
    public static UniqueTileManager Instance {  get; private set; }

    [SerializeField] private List<UniqueTile> tiles;

    private void Awake()
    {
        Instance = this;
        StageManager.OnObjectKilled += OnEnemyKilled;
    }

    public void OnEnemyKilled(int count)
    {
        foreach (var tile in tiles)
        {
            tile.OnEnemyKilled(count);
        }
    }

    private void OnDisable()
    {
        StageManager.OnObjectKilled -= OnEnemyKilled;
    }
}
