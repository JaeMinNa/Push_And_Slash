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
    private EnemyController _enemyController;
    private PhotonView _photonView;
    private float _skillAtk;

    private void Start()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        _playerCharacter = transform.parent.GetComponent<PlayerCharacter>();
        _photonView = transform.parent.GetComponent<PhotonView>();

        if (transform.parent.CompareTag("Player"))
        {
            _playerCharacter = transform.parent.GetComponent<PlayerCharacter>();
        }
        else
        {
            _enemyController = transform.parent.GetComponent<EnemyController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(transform.parent.CompareTag("Player"))
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
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(_cameraShake.COShake(0.8f, 0.5f));

                other.GetComponent<PlayerCharacter>().PlayerNuckback(_enemyController.transform.position, _enemyController.EnemyData.MeleeSkillAtk);
            }
        }
    }
}
