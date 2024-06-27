using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : MonoBehaviour, IEnemyState
{
    private EnemyController _enemyController;
    private float _time;
    private Vector3 _dir;
    private bool _isHit;

    // Start문과 동일하게 사용
    public void Handle(EnemyController enemyController)
    {
        if (!_enemyController)
            _enemyController = enemyController;

        Debug.Log("Hit 상태 시작");
        _enemyController.EnemyAnimator.SetBool("Attack", false);
        _enemyController.Rigidbody.isKinematic = false;
        _time = 0f;
        _isHit = false;
        _dir = (transform.position - _enemyController.Target.transform.position).normalized;
        if(_enemyController.IsHit_attack)
        {
            _enemyController.Rigidbody.velocity = _dir * (_enemyController.PlayerData.Atk - _enemyController.Def);
        }
        else if(_enemyController.IsHit_skill)
        {
            _enemyController.Rigidbody.velocity = _dir * (_enemyController.PlayerData.SkillAtk - _enemyController.Def);
        }
        StartCoroutine(COUpdate());
    }

    // Update문과 동일하게 사용
    private IEnumerator COUpdate()
    {
        while (true)
        {
            _time += Time.deltaTime;

            if(_enemyController.IsGround())
            {
                if (_time >= 0.5f)
                {
                    _enemyController.IsHit_attack = false;
                    _enemyController.IsHit_skill = false;
                    if (!_isHit && _time >= 0.2f)
                    {
                        _isHit = true;
                        _enemyController.Rigidbody.isKinematic = true;
                    }

                    _enemyController.WalkStart();
                    break;
                }
            }
            else
            {
                if (transform.position.y <= -10f)
                {
                    transform.gameObject.SetActive(false);
                    _enemyController.StageController.StageCoin += _enemyController.EnemyData.Coin;
                    _enemyController.StageController.StageExp += _enemyController.EnemyData.Exp;
                    _enemyController.StageController.CoinSetting();
                    break;
                }
            }

            yield return null;
        }
    }
}

