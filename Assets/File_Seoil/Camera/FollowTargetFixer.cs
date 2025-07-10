using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class FollowTargetFixer : MonoBehaviour
{
    private void Awake() =>
        GetComponent<CinemachineCamera>().Target.TrackingTarget = PlayerController.Instance.IsUnityNull() ? PlayerController.Instance.transform : LobbyPlayerController.Instance.transform;
}
