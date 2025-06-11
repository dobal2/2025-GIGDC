using DG.Tweening;
using UnityEngine;

public static class CameraUtility
{
    public static Camera GetTopCamera()
    {
        Camera[] cams = Camera.allCameras;
        Camera topCamera = null;
        float topDepth = float.NegativeInfinity;

        foreach (Camera cam in cams)
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
            {
                if (cam.depth > topDepth)
                {
                    topDepth = cam.depth;
                    topCamera = cam;
                }
            }
        }

        return topCamera;
    }

    public static void ShakeCamera(
        float duration = 0.3f,
        float strength = 1,
        int vibrato = 10,
        int randomness = 90,
        bool fadeOut = true
        )
    {
        Camera topCamera = GetTopCamera();
        topCamera.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
    }
}
