using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;

public class Arrow : MonoBehaviour
{
    public enum Type
    {
        Player,
        Enemy,
    }

    public Type CharacterType;
    [SerializeField] private float _speed;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [HideInInspector] public float Atk;
    private GameObject _player;
    private Vector3 _dir;
    private ParticleSystem _effect;
    private CameraShake _cameraShake;

    private void Awake()
    {
        _effect = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        _cameraShake = Camera.main.transform.GetComponent<CameraShake>();
    }

    private void Start()
    {
        _player = GameManager.I.PlayerManager.Player;

        if (CharacterType == Type.Enemy)
        {
            _dir = (_player.transform.position + new Vector3(0, 0.5f, 0) - transform.position).normalized;
            transform.LookAt(_player.transform.position + new Vector3(0, 0.5f, 0));
        }      
        else if (CharacterType == Type.Player)
        {
            
        }

        StartCoroutine(CODestroyAttack());
    }

    private void Update()
    {
        
        transform.position += _dir * _speed * Time.deltaTime;
        
    }

    public void SetInit(float atk, Vector3 dir)
    {
        _dir = dir;
        transform.LookAt(transform.position + _dir);

        Atk = atk;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(CharacterType == Type.Enemy)
        {
            if (other.CompareTag("Player"))
            {
                _player.GetComponent<PlayerCharacter>().PlayerNuckback(transform.position, Atk);
                GameManager.I.SoundManager.StartSFX("ArrowHit");
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                _effect.Play();
                _renderer.enabled = false;
            }
        }
        else if (CharacterType == Type.Player)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyController>().IsHit_attack = true;
                GameManager.I.SoundManager.StartSFX("ArrowHit", other.transform.position);
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                _effect.Play();
                _renderer.enabled = false;
            }
            else if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerCharacter>().PlayerNuckback(transform.position, Atk);
                GameManager.I.SoundManager.StartSFX("ArrowHit", other.transform.position);
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                _effect.Play();
                _renderer.enabled = false;
            }
        }
    }

    private IEnumerator CODestroyAttack()
    {
        yield return new WaitForSeconds(5f);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
