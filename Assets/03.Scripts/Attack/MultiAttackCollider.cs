using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;

public class MultiAttackCollider : MonoBehaviour
{
    private CameraShake _cameraShake;
    public bool IsPhotonView;
    public float Atk;

    private void Awake()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    private void Start()
    {
        
    }

    public void SetInit(float atk, bool photon)
    {
        Atk = atk;
        IsPhotonView = photon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("적 공격 성공! " + other.name);
            StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
            //Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
            //_effectFixedPosition.SetPosition(contactPoint);
            //_attackParticleSystem.Play();
            other.GetComponent<EnemyController>().IsHit_attack = true;
        }
        else if (other.gameObject.CompareTag("Player")) /*&& other.GetComponent<PlayerCharacter>().PhotonView.IsMine != _photonView.IsMine*/
        {
            Debug.Log("적 공격 성공! " + other.name);
            StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
            //Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
            //_effectFixedPosition.SetPosition(contactPoint);
            //_attackParticleSystem.Play();

            if (IsPhotonView)
            {
                other.GetComponent<PlayerCharacter>().
                    PhotonView.RPC("RPCPlayerNuckback", RpcTarget.AllViaServer, transform.position, Atk);
            }
        }
    }
}
