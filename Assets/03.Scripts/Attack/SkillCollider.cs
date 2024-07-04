using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;

public class SkillCollider : MonoBehaviour
{
    private CameraShake _cameraShake;
    private PlayerCharacter _playerCharacter;
    private PhotonView _photonView;
    private float _skillAtk;

    private void Start()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        _playerCharacter = transform.parent.GetComponent<PlayerCharacter>();
        _photonView = transform.parent.GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(_cameraShake.COShake(0.8f, 0.5f));
            other.GetComponent<EnemyController>().IsHit_skill = true;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(_cameraShake.COShake(0.8f, 0.5f));

            if (!_photonView.IsMine)
            {
                _skillAtk = _playerCharacter.SkillAtk;
                other.GetComponent<PlayerCharacter>().
                        PhotonView.RPC("RPCPlayerNuckback", RpcTarget.AllViaServer, _playerCharacter.transform.position, _skillAtk);
            }
        }
    }
}
