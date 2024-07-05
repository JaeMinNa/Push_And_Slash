using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;

public class AttackCollider : MonoBehaviour
{
    public enum Type
    {
        Player,
        Enemy,
    }

    public Type CharacterType;

    private GameObject _player;
    private EnemyController _enemyController;
    private PlayerCharacter _playerCharacter;
    private CameraShake _cameraShake;
    private ParticleSystem _attackParticleSystem;
    private EffectFixedPosition _effectFixedPosition;
    private PhotonView _photonView;
    private float _atk;

    private void Start()
    {
        _player = GameManager.I.PlayerManager.Player;
        _cameraShake = Camera.main.GetComponent<CameraShake>();

        if (CharacterType == Type.Player)
        {
            Transform topParent = transform;
            while (topParent.parent != null)
            {
                topParent = topParent.parent;
            }
            _playerCharacter = topParent.GetComponent<PlayerCharacter>();
            _attackParticleSystem = topParent.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
            _photonView = topParent.GetComponent<PhotonView>();
        }
        else if(CharacterType == Type.Enemy)
        {
            _enemyController = transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.GetComponent<EnemyController>();
            _attackParticleSystem = _enemyController.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        }

        _effectFixedPosition = _attackParticleSystem.GetComponent<EffectFixedPosition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CharacterType == Type.Player)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("적 공격 성공! " + other.name);
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                _effectFixedPosition.SetPosition(contactPoint);
                _attackParticleSystem.Play();
                other.GetComponent<EnemyController>().IsHit_attack = true;
            }
            else if (other.gameObject.CompareTag("Player") /*&& other.gameObject != transform.root.gameObject*/)
            {
                Debug.Log("적 공격 성공! " + other.name);
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                _effectFixedPosition.SetPosition(contactPoint);
                _attackParticleSystem.Play();

                if (!_photonView.IsMine)
                {
                    _atk = _playerCharacter.Atk;
                    other.GetComponent<PlayerCharacter>().
                        PhotonView.RPC("RPCPlayerNuckback", RpcTarget.AllViaServer, _playerCharacter.transform.position, _atk);
                }
            }
        }
        else if (CharacterType == Type.Enemy)
        {
            if (other.CompareTag("Player") && !_player.GetComponent<PlayerCharacter>().IsSkill)
            {
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                _effectFixedPosition.SetPosition(contactPoint);
                _attackParticleSystem.Play();

                _player.GetComponent<PlayerCharacter>().PlayerNuckback(transform.position, _enemyController.Atk);
            }
        }
    }
}
