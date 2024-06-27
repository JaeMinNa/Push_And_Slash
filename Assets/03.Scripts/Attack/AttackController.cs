using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] private BoxCollider[] _weaponColliders;
    [SerializeField] private BoxCollider _skillCollider;

    private ParticleSystem _skillParticleSystem;
    private EffectFixedPosition _effectFixedPosition;

    private void Start()
    {
        if(transform.parent.CompareTag("Player"))
        {
            _skillParticleSystem = transform.parent.GetChild(2).GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
            _effectFixedPosition = _skillParticleSystem.GetComponent<EffectFixedPosition>();
        }
    }

    // Player, Enemy
    public void AttackColliderActive(float time)
    {
        for (int i = 0; i < _weaponColliders.Length; i++)
        {
            _weaponColliders[i].enabled = true;
        }

        //StartCoroutine(COAttackColliderInactive(time));
    }

    private IEnumerator COAttackColliderInactive(float time)
    {
        yield return new WaitForSeconds(time);

        for (int i = 0; i < _weaponColliders.Length; i++)
        {
            _weaponColliders[i].enabled = false;
        }
    }

    // Player
    public void SkillColliderActive(float time)
    {

        _skillCollider.enabled = true;
        _effectFixedPosition.SetPosition(new Vector3(_skillCollider.transform.position.x, _skillCollider.transform.position.y, _skillCollider.transform.position.z));
        _skillParticleSystem.Play();
        StartCoroutine(COSkillColliderInactive(time));
    }

    private IEnumerator COSkillColliderInactive(float time)
    {
        yield return new WaitForSeconds(time);

        _skillCollider.enabled = false;
    }
}
