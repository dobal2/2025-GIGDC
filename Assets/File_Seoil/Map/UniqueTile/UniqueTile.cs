using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UniqueTile : MonoBehaviour
{
    [SerializeField] private List<TileMoveTarget> tileMoveTargets;
    [SerializeField] private TMP_Text countView;

    [Serializable]
    public class TileMoveTarget
    {
        public int RequireCount;
        public Transform Target;
    }

    private void Start()
    {
        countView.text = tileMoveTargets[0].RequireCount.ToString();
    }

    public void OnEnemyKilled(int count)
    {
        if(tileMoveTargets.Count <= 0)
            return;

        var tileMoveTarget = tileMoveTargets[0];
        tileMoveTarget.RequireCount -= count;

        if(tileMoveTarget.RequireCount <= 0)
        {
            tileMoveTargets.RemoveAt(0);
            transform.position = tileMoveTarget.Target.position;
        }

        if(tileMoveTargets.Count > 0) 
            countView.text = tileMoveTargets[0].RequireCount.ToString();
        else
            Destroy(countView.gameObject);
    }
}
