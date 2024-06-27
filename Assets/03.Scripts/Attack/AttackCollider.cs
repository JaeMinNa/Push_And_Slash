using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

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
    private CharacterData _playerCharacterData;
    private float _atk;

    public TMP_Text text;

    private void Start()
    {
        _player = GameManager.I.PlayerManager.Player;
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        _playerCharacterData = GameManager.I.DataManager.PlayerData;

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

            if(!_photonView.IsMine) _atk = _playerCharacter.Atk;
        }
        else if(CharacterType == Type.Enemy)
        {
            _enemyController = transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.GetComponent<EnemyController>();
            _attackParticleSystem = _enemyController.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
        }

        _effectFixedPosition = _attackParticleSystem.GetComponent<EffectFixedPosition>();

        if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
            text = GameObject.FindWithTag("Test").GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CharacterType == Type.Player)
        {
            if (other.CompareTag("Enemy"))
            {
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                _effectFixedPosition.SetPosition(contactPoint);
                _attackParticleSystem.Play();
                other.GetComponent<EnemyController>().IsHit_attack = true;
            }
            else if (other.CompareTag("Player") /*&& !other.gameObject.Equals(_playerCharacter.gameObject)*/)
            {
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                _effectFixedPosition.SetPosition(contactPoint);
                _attackParticleSystem.Play();
                text.text = "";

                //if (_photonView.IsMine) other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacterData.Atk);
                //else other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacter.Atk);

                //if (!_photonView.IsMine)
                //{
                //    other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _atk);
                //    Debug.Log("!IsMine의 Atk : " + _atk);
                //    text.text = "!IsMine의 Atk : " + _atk.ToString();
                //}
                //else
                //{
                //    other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacter.Atk);
                //}

                //else
                //{
                //    other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacterData.Atk);
                //    //Debug.Log("!IsMine의 Atk : " + _playerCharacter.Atk);
                //}

                if(_photonView.IsMine) _photonView.RPC("HitRPC", RpcTarget.AllViaServer, other, _playerCharacterData.Atk);
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

    [PunRPC]
    public void HitRPC(Collider other, float atk)
    {
        //if (!_photonView.IsMine)
        //{
        //    other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _atk);
        //    Debug.Log("!IsMine의 Atk : " + _atk);
        //    text.text = "!IsMine의 Atk : " + _atk.ToString();
        //}
        //else
        //{
        //    other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacterData.Atk);
        //    Debug.Log("IsMine의 Atk : " + _playerCharacterData.Atk);
        //    text.text = "!IsMine의 Atk : " + _atk.ToString();
        //}

        //if (!_photonView.IsMine)
        //{
        //    other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _atk);
        //    Debug.Log("!IsMine의 Atk : " + _atk);
        //    text.text = "!IsMine의 Atk : " + _atk.ToString();
        //}

        other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, atk);
        text.text = "RPC 실행 Atk : " + atk.ToString();
    }
}
