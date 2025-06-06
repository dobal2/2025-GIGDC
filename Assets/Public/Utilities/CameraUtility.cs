using UnityEngine;

public static class CameraUtility
{
    public static Camera FindTopMostCamera()
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
}
