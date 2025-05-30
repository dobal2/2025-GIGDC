using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ToVector3(this Vector2 target, float z = 0) =>
        new Vector3(target.x, target.y, z);

    public static Vector3 SetX(this Vector3 target, float x) =>
        new Vector3(x, target.y, target.z);

    public static Vector3 SetY(this Vector3 target, float y) =>
        new Vector3(target.x, y, target.z);

    public static Vector3 SetZ(this Vector3 target, float z) =>
        new Vector3(target.x, target.y, z);

    public static Vector2 ToVector2(this Vector3 target) =>
        new Vector2(target.x, target.y);

    public static Vector2 SetX(this Vector2 target, float x) =>
        new Vector2(x, target.y);

    public static Vector2 SetY(this Vector2 target, float y) =>
        new Vector2(target.x, y);
}
