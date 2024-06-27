using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SkillCollider : MonoBehaviour
{
    private CameraShake _cameraShake;
    private PlayerCharacter _playerCharacter;
    private PhotonView _photonView;
    private CharacterData _playerCharacterData;
    private float _skillAtk;
    private TMP_Text text;

    private void Start()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        _playerCharacter = transform.parent.GetComponent<PlayerCharacter>();
        _photonView = transform.parent.GetComponent<PhotonView>();
        _playerCharacterData = GameManager.I.DataManager.PlayerData;

        if (!_photonView.IsMine) _skillAtk = _playerCharacter.SkillAtk;

        if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
            text = GameObject.FindWithTag("Test").GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(_cameraShake.COShake(0.8f, 0.5f));
            other.GetComponent<EnemyController>().IsHit_skill = true;
        }
        else if (other.CompareTag("Player") /*&& !other.gameObject.Equals(_playerCharacter.gameObject)*/)
        {
            StartCoroutine(_cameraShake.COShake(0.8f, 0.5f));

            //if(_photonView.IsMine) other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacterData.SkillAtk);
            //else other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacter.SkillAtk);

            if (!_photonView.IsMine)
            {
                other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _skillAtk);
                Debug.Log("!IsMine¿« SkillAtk : " + _skillAtk);
                text.text = "!IsMine¿« SkillAtk : " + _skillAtk.ToString();
            }
            else
            {
                other.GetComponent<PlayerCharacter>().PlayerNuckback(_playerCharacter.transform.position, _playerCharacterData.SkillAtk);
            }
        }
    }
}
