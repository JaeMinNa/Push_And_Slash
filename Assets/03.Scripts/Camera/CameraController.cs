using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 OriginCameraRotation;
    public Vector3 Offset;
    private GameObject _player;

    private void Start()
    {
        CameraSetting();
    }

    void LateUpdate()
    {
        transform.position = _player.transform.position + Offset;
    }

    public void CameraSetting()
    {
        _player = GameManager.I.PlayerManager.Player;
    }
}
