using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFixedPosition : MonoBehaviour
{
    private Vector3 _position;
    private Vector3 _rotation;

    private void Start()
    {
        _position = transform.position;
        _rotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        transform.position = _position;
        transform.rotation = Quaternion.Euler(_rotation);
    }

    public void SetPosition(Vector3 position)
    {
        _position = position;
    }
}
