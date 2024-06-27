using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2;
using EpicToonFX;
using Photon.Pun;
using Photon.Realtime;
using ECM2.Examples.Slide;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Transform _shootPosition;
    private Character _character;
    private Animator _animator;
    private CharacterData _playerData;
    private GameObject _player;
    private PhotonView _photonView;
    private PlayerCharacter _playerCharacter;

    private void Start()
    {
        _character = transform.parent.GetComponent<Character>();
        _playerCharacter = transform.parent.GetComponent<PlayerCharacter>();
        _animator = GetComponent<Animator>();
        _playerData = GameManager.I.DataManager.PlayerData;
        _player = GameManager.I.PlayerManager.Player;
        _photonView = transform.parent.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            if (_character.IsGrounded())
            {
                _animator.SetBool("Ground", true);
                _animator.SetBool("Jump", false);

                if (_character.GetSpeed() > 0) _animator.SetBool("Run", true);
                else _animator.SetBool("Run", false);
            }
            else
            {
                _animator.SetBool("Ground", false);
                _animator.SetBool("Jump", true);
            }
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if(GameManager.I.PlayerManager.Player.GetComponent<PhotonView>().IsMine)
            {
                if (_character.IsGrounded())
                {
                    _animator.SetBool("Ground", true);
                    _animator.SetBool("Jump", false);

                    if (_character.GetSpeed() > 0) _animator.SetBool("Run", true);
                    else _animator.SetBool("Run", false);
                }
                else
                {
                    _animator.SetBool("Ground", false);
                    _animator.SetBool("Jump", true);
                }
            }
        }
    }

    public void StartSFX(string name)
    {
        GameManager.I.SoundManager.StartSFX(name, transform.position);
    }

    public void ShootRangedAttack(string name)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Skills/Player/" + name), _shootPosition.position, Quaternion.identity);

        if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (_photonView.IsMine)
            {
                if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerAttack)
                    obj.GetComponent<ETFXProjectileScript>().SetInit(_playerData.Atk, new Vector3(_player.transform.forward.x, 0, _player.transform.forward.z));
                else if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerSkill)
                    obj.GetComponent<ETFXProjectileScript>().SetInit(_playerData.SkillAtk, new Vector3(_player.transform.forward.x, 0, _player.transform.forward.z));
            }
            else
            {
                if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerAttack)
                    obj.GetComponent<ETFXProjectileScript>().SetInit(_playerCharacter.Atk, _playerCharacter.PlayerDirection);
                else if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerSkill)
                    obj.GetComponent<ETFXProjectileScript>().SetInit(_playerCharacter.SkillAtk, _playerCharacter.PlayerDirection);
            }
        }
        else if(GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerAttack)
                obj.GetComponent<ETFXProjectileScript>().SetInit(_playerData.Atk, new Vector3(_player.transform.forward.x, 0, _player.transform.forward.z));
            else if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerSkill)
                obj.GetComponent<ETFXProjectileScript>().SetInit(_playerData.SkillAtk, new Vector3(_player.transform.forward.x, 0, _player.transform.forward.z));
        }

        //_photonView.RPC("ShootRangedAttackRPC", RpcTarget.AllBuffered, name);
    }

    //[PunRPC]
    //private void ShootRangedAttackRPC(string name)
    //{
    //    GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Skills/Player/" + name), _shootPosition.position, Quaternion.identity);

    //    if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerAttack)
    //        obj.GetComponent<ETFXProjectileScript>().Atk = _playerData.Atk;
    //    else if (obj.GetComponent<ETFXProjectileScript>().CharacterType == ETFXProjectileScript.Type.PlayerSkill)
    //        obj.GetComponent<ETFXProjectileScript>().Atk = _playerData.SkillAtk;
    //}


    public void ShootArrowAttack(string name)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Skills/Player/" + name), _shootPosition.position, Quaternion.identity);

        if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (_photonView.IsMine)
                obj.GetComponent<Arrow>().SetInit(_playerData.Atk, new Vector3(_player.transform.forward.x, 0, _player.transform.forward.z));
            else
                obj.GetComponent<Arrow>().SetInit(_playerCharacter.Atk, _playerCharacter.PlayerDirection);
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            obj.GetComponent<Arrow>().SetInit(_playerData.Atk, new Vector3(_player.transform.forward.x, 0, _player.transform.forward.z));
        }

    }
}
