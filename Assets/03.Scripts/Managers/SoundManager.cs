using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("CameraAudioSource")]
    [SerializeField] private AudioSource _cameraBGMAudioSource;
    [SerializeField] private AudioSource _cameraSFXAuidoSource;

    [Header("ETCAudioSource")]
    [SerializeField] private AudioSource[] _etcSFXAudioSources;

    private Dictionary<string, AudioClip> _bgm;
    private Dictionary<string, AudioClip> _sfx;
    private int _index;
    [SerializeField] private float _maxDistance = 50f;
    [Range(0f, 1f)] public float StartVolume = 0.1f;

    public void Init()
    {
        // 초기 셋팅
        _bgm = new Dictionary<string, AudioClip>();
        _sfx = new Dictionary<string, AudioClip>();

        _cameraBGMAudioSource.loop = true;
        _cameraBGMAudioSource.volume = StartVolume;
        _cameraSFXAuidoSource.playOnAwake = false;
        _cameraSFXAuidoSource.volume = StartVolume;
        for (int i = 0; i < _etcSFXAudioSources.Length; i++)
        {
            _etcSFXAudioSources[i].playOnAwake = false;
            _etcSFXAudioSources[i].volume = StartVolume;
        }

        // BGM
        _bgm.Add("BattleScene", Resources.Load<AudioClip>("Sounds/BGM/Battle Theme"));
        _bgm.Add("MultiScene", Resources.Load<AudioClip>("Sounds/BGM/Boss Battle Theme"));
        _bgm.Add("MultiReady", Resources.Load<AudioClip>("Sounds/BGM/Downtime"));
        _bgm.Add("BossScene", Resources.Load<AudioClip>("Sounds/BGM/Final Boss Battle"));
        _bgm.Add("StartScene", Resources.Load<AudioClip>("Sounds/BGM/Overworld 1"));
        _bgm.Add("LobbyScene", Resources.Load<AudioClip>("Sounds/BGM/Town Theme"));
        _bgm.Add("Victory", Resources.Load<AudioClip>("Sounds/BGM/Victory Fanfare"));

        // SFX
        _sfx.Add("PlayerDash", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerDash"));
        _sfx.Add("PlayerHit", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerHit"));
        _sfx.Add("PlayerJump", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerJump"));
        _sfx.Add("PlayerJumpEnd", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerJumpEnd"));
        _sfx.Add("PlayerSwordAttack0", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerSwordAttack0"));
        _sfx.Add("PlayerSwordAttack1", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerSwordAttack1"));
        _sfx.Add("PlayerSwordSkill", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerSwordSkill"));
        _sfx.Add("EnemyStickAttack", Resources.Load<AudioClip>("Sounds/SFX/Enemy/EnemyStickAttack"));
        _sfx.Add("EnemyFireBallExplosion", Resources.Load<AudioClip>("Sounds/SFX/Enemy/EnemyFireBallExplosion"));
        _sfx.Add("EnemyFireBallShoot", Resources.Load<AudioClip>("Sounds/SFX/Enemy/EnemyFireBallShoot"));
        _sfx.Add("ArrowHit", Resources.Load<AudioClip>("Sounds/SFX/Common/ArrowHit"));
        _sfx.Add("ArrowShoot", Resources.Load<AudioClip>("Sounds/SFX/Common/ArrowShoot"));
        _sfx.Add("ButtonClick", Resources.Load<AudioClip>("Sounds/SFX/UI/ButtonClick"));
        _sfx.Add("ButtonClickMiss", Resources.Load<AudioClip>("Sounds/SFX/UI/ButtonClickMiss"));
        _sfx.Add("EnemySwordAttack", Resources.Load<AudioClip>("Sounds/SFX/Enemy/EnemySwordAttack"));
        _sfx.Add("EnemySpearAttack", Resources.Load<AudioClip>("Sounds/SFX/Enemy/EnemySpearAttack"));
        _sfx.Add("EnemyTwoHandAttack", Resources.Load<AudioClip>("Sounds/SFX/Enemy/EnemyTwoHandAttack"));
        _sfx.Add("PlayerPunchAttack0", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerPunchAttack0"));
        _sfx.Add("PlayerPunchAttack1", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerPunchAttack1"));
        _sfx.Add("PlayerNovaExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNovaExplosion"));
        _sfx.Add("PlayerNovaShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNovaShoot"));
        _sfx.Add("PlayerShadowExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerShadowExplosion"));
        _sfx.Add("PlayerShadowShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerShadowShoot"));
        _sfx.Add("PlayerLightningExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerLightningExplosion"));
        _sfx.Add("PlayerLightningShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerLightningShoot"));
        _sfx.Add("PlayerMagicExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerMagicExplosion"));
        _sfx.Add("PlayerMagicShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerMagicShoot"));
        _sfx.Add("PlayerWandAttack", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerWandAttack"));
        _sfx.Add("PlayerNukeExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNukeExplosion"));
        _sfx.Add("PlayerNukeShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNukeShoot"));
        _sfx.Add("PlayerFireBallShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerFireBallShoot"));
        _sfx.Add("PlayerFireBallExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerFireBallExplosion"));
        _sfx.Add("PlayerNovaPinkExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNovaExplosion"));
        _sfx.Add("PlayerNovaPinkShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNovaShoot"));
        _sfx.Add("PlayerOneSwordSkill", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerOneSwordSkill"));
        _sfx.Add("PlayerTHSAttack", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerTHSAttack"));
        _sfx.Add("PlayerTHSSkill", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerTHSSkill"));
        _sfx.Add("PlayerSpearAttack0", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerSpearAttack0"));
        _sfx.Add("PlayerSpearAttack1", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerSpearAttack1"));
        _sfx.Add("PlayerSpearSkill", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerSpearSkill"));
        _sfx.Add("PlayerArrowReady", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerArrowReady"));
        _sfx.Add("PlayerLightning_ArrowExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerLightning_ArrowExplosion"));
        _sfx.Add("PlayerLightning_ArrowShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerLightning_ArrowShoot"));
        _sfx.Add("PlayerNuke_ArrowExplosion", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNukeExplosion"));
        _sfx.Add("PlayerNuke_ArrowShoot", Resources.Load<AudioClip>("Sounds/SFX/Player/PlayerNukeShoot"));
        _sfx.Add("EquipButton", Resources.Load<AudioClip>("Sounds/SFX/UI/EquipButton"));
        _sfx.Add("CharacterGetButton", Resources.Load<AudioClip>("Sounds/SFX/UI/CharacterGetButton"));
        _sfx.Add("Ready", Resources.Load<AudioClip>("Sounds/SFX/UI/Ready"));
        _sfx.Add("Win", Resources.Load<AudioClip>("Sounds/SFX/UI/WinStar"));
        _sfx.Add("Lose", Resources.Load<AudioClip>("Sounds/SFX/UI/Lose"));
        _sfx.Add("Coin", Resources.Load<AudioClip>("Sounds/SFX/UI/Coin"));
    }

    private void Start()
    {
        GameManager.I.UIManager.SoundSetting();
    }

    // 메모리 해제
    public void Release()
    {

    }

    // 다른 오브젝트에서 출력되는 사운드
    // 2D에서는 Vector2.Distance 사용
    public void StartSFX(string name, Vector3 position)
    {
        _index = _index % _etcSFXAudioSources.Length;

        float distance = Vector3.Distance(position, GameManager.I.PlayerManager.Player.transform.position);
        float volume = 1f - (distance / _maxDistance);
        _etcSFXAudioSources[_index].volume = Mathf.Clamp01(volume) * StartVolume;
        _etcSFXAudioSources[_index].PlayOneShot(_sfx[name]);

        _index++;
    }

    // Player에서 출력되는 사운드
    public void StartSFX(string name)
    {
        _cameraSFXAuidoSource.PlayOneShot(_sfx[name]);
    }

    public void StartBGM(string name)
    {
        _cameraBGMAudioSource.Stop();
        _cameraBGMAudioSource.clip = _bgm[name];
        _cameraBGMAudioSource.Play();
    }

    public void StopBGM()
    {
        if (_cameraBGMAudioSource != null) _cameraBGMAudioSource.Stop();
    }
}
