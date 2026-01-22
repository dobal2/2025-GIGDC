using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DialogCameraEventReciever : DialogEventRecieverBase
{
    protected override void RegisterCommands()
    {
        OnEvent.Add("ShakeCamera", ShakeCamera);
        OnEvent.Add("ScaleCamera", ScaleCamera);
        OnEvent.Add("FadeCameraToDark", FadeCameraToDark);
        OnEvent.Add("FadeCameraToBright", FadeCameraToBright);
        OnEvent.Add("MoveCamera", MoveCamera);
    }

    private void ShakeCamera(string line)
    {
        float duration = 0;

        try
        {
            duration = Convert.ToSingle(line);
        }
        catch (FormatException)
        {
            Debug.LogWarning("Invalid ShakeCamera Parameters: " + line);
            return;
        }

        CameraUtility.TopCamera.transform
            .DOShakePosition(
                duration: duration,
                strength: 0.5f
                );
    }

    private void ScaleCamera(string line)
    {
        throw new NotImplementedException();
    }

    private void FadeCameraToDark(string line)
    {
        throw new NotImplementedException();
    }

    private void FadeCameraToBright(string line)
    {
        throw new NotImplementedException();
    }

    private void MoveCamera(string line)
    {
        throw new NotImplementedException();
    }
}
