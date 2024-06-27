using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CameraController _cameraController;

    private void Start()
    {
        _cameraController = GetComponent<CameraController>();
    }

    public IEnumerator COShake(float shakeAmount, float shakeTime)
    {
        float timer = 0;
        while (timer <= shakeTime)
        {
            Camera.main.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation + (Vector3)UnityEngine.Random.insideUnitCircle * shakeAmount);
            timer += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation);
    }
}
