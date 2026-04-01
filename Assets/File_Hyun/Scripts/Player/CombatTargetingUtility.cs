using System.Collections.Generic;
using UnityEngine;

public static class CombatTargetingUtility
{
    public static bool TryFindNearestEnemyPoint(Vector2 origin, float range, LayerMask enemyMask, out Vector2 targetPoint)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, enemyMask);
        HashSet<Monster> monsters = new();
        float bestSqrDistance = float.MaxValue;
        targetPoint = default;
        bool found = false;

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            if (monster == null)
                monster = hit.GetComponentInParent<Monster>();
            if (monster == null || !monsters.Add(monster))
                continue;

            Vector2 closestPoint = hit.ClosestPoint(origin);
            float sqrDistance = (closestPoint - origin).sqrMagnitude;
            if (sqrDistance >= bestSqrDistance)
                continue;

            bestSqrDistance = sqrDistance;
            targetPoint = hit.bounds.center;
            found = true;
        }

        return found;
    }

    public static bool TryProjectPointToGround(Vector2 point, Vector2 boxSize, LayerMask groundMask, float castStartHeight, float castDistance, out Vector2 groundPoint)
    {
        Vector2 origin = point + Vector2.up * castStartHeight;
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, castDistance, groundMask);

        if (hit.collider == null)
        {
            groundPoint = default;
            return false;
        }

        groundPoint = hit.point;
        return true;
    }

    public static bool TryGetBallisticVelocity(Vector2 startPoint, Vector2 targetPoint, float gravityMagnitude, float extraArcHeight, out Vector2 velocity)
    {
        if (gravityMagnitude <= 0f)
        {
            velocity = default;
            return false;
        }

        float apexY = Mathf.Max(startPoint.y, targetPoint.y) + Mathf.Max(0.1f, extraArcHeight);
        float riseHeight = apexY - startPoint.y;
        float fallHeight = apexY - targetPoint.y;

        float verticalSpeed = Mathf.Sqrt(2f * gravityMagnitude * riseHeight);
        float timeUp = verticalSpeed / gravityMagnitude;
        float timeDown = Mathf.Sqrt(2f * fallHeight / gravityMagnitude);
        float totalTime = timeUp + timeDown;

        if (totalTime <= 0f)
        {
            velocity = default;
            return false;
        }

        float horizontalSpeed = (targetPoint.x - startPoint.x) / totalTime;
        velocity = new Vector2(horizontalSpeed, verticalSpeed);
        return true;
    }
}