using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class DialogCameraEventReciever : DialogEventRecieverBase
{
    [SerializeField] private FadeToDarkView fadeToDarkViewPrefab;
    [SerializeField] private FadeToBrightView fadeToBrightViewPrefab;

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
        try
        {
            if(line == "true")
            {
                FindAnyObjectByType<CinemachineCamera>().Lens.OrthographicSize = 4f;
            }
            else if(line == "false")
            {
                FindAnyObjectByType<CinemachineCamera>().Lens.OrthographicSize = 4.74f;
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private void FadeCameraToDark(string line)
    {
        try
        {
            Instantiate(fadeToDarkViewPrefab).SetDarkView(float.Parse(line));
        }
        catch (Exception)
        {
            return;
        }
    }

    private void FadeCameraToBright(string line)
    {
        try
        {
            Instantiate(fadeToBrightViewPrefab).SetBrightView(float.Parse(line));
        }
        catch (Exception)
        {
            return;
        }
    }

    private void MoveCamera(string line)
    {
        //throw new NotImplementedException();
    }
}
