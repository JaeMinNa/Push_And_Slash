using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;

public class Arrow : MonoBehaviour
{
    public enum Type
    {
        Player,
        Enemy,
    }

    public Type CharacterType;
    [SerializeField] private GameObject _arrowObject;
    [SerializeField] private float _speed;
    [HideInInspector] public float Atk;
    private GameObject _player;
    private BoxCollider _collider;
    private Vector3 _dir;
    private ParticleSystem _effect;
    private CameraShake _cameraShake;
    private bool _photonIsMine;

    private void Awake()
    {
        _effect = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        _cameraShake = Camera.main.transform.GetComponent<CameraShake>();
        _collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _player = GameManager.I.PlayerManager.Player;

        if (CharacterType == Type.Enemy)
        {
            //_dir = (_player.transform.position + new Vector3(0, 0.5f, 0) - transform.position).normalized;
            //transform.LookAt(_player.transform.position + new Vector3(0, 0.5f, 0));
        }
        else if (CharacterType == Type.Player)
        {

        }

        Debug.Log("Arrow¿« IsMine? : " + _photonIsMine);
        StartCoroutine(CODestroyAttack());
    }

    private void Update()
    {
        
        transform.position += _dir * _speed * Time.deltaTime;
        
    }

    public void SetInit(float atk, Vector3 dir, bool photon)
    {
        _dir = dir;
        transform.LookAt(transform.position + _dir);

        Atk = atk;
        _photonIsMine = photon;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            //Destroy(gameObject);
            _effect.Play();
            _arrowObject.SetActive(false);
            _collider.enabled = false;
        }

        if(CharacterType == Type.Enemy)
        {
            if (other.CompareTag("player"))
            {
                _player.GetComponent<PlayerCharacter>().PlayerNuckback(transform.position, Atk);
                GameManager.I.SoundManager.StartSFX("ArrowHit");
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                _effect.Play();
                _arrowObject.SetActive(false);
                _collider.enabled = false;
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
                _arrowObject.SetActive(false);
                _collider.enabled = false;
            }
            else if (other.CompareTag("Player"))
            {
                GameManager.I.SoundManager.StartSFX("ArrowHit", other.transform.position);
                StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));
                _effect.Play();
                _arrowObject.SetActive(false);
                _collider.enabled = false;

                if (!_photonIsMine)
                {
                    other.GetComponent<PlayerCharacter>().
                        PhotonView.RPC("RPCPlayerNuckback", RpcTarget.AllViaServer, new Vector3(transform.position.x, other.transform.position.y, transform.position.z), Atk);
                }
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
