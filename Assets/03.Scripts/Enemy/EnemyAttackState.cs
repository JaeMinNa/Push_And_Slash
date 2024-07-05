using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : MonoBehaviour, IEnemyState
{
    private EnemyController _enemyController;
    private float _time;

    // Start문과 동일하게 사용
    public void Handle(EnemyController enemyController)
    {
        if (!_enemyController)
            _enemyController = enemyController;

        Debug.Log("Attack 상태 시작");
        _time = 0f;
        StartCoroutine(COUpdate());
    }

    // Update문과 동일하게 사용
    private IEnumerator COUpdate()
    {
        while (true)
        {
            if(!_enemyController.IsBoss)
            {
                _time += Time.deltaTime;

                if (_time >= _enemyController.EnemyData.AttackCoolTime)
                {
                    if (_enemyController.CheckPlayer())
                    {
                        _enemyController.AttackStart();
                        _enemyController.EnemyAnimator.SetTrigger("ReAttack");
                        break;
                    }
                    else
                    {
                        _enemyController.WalkStart();
                        _enemyController.EnemyAnimator.SetBool("Attack", false);
                        break;
                    }
                }
            }
            else // Boss
            {
                if (_enemyController.BossAttackTime >= _enemyController.EnemyData.AttackCoolTime)
                {
                    if (_enemyController.CheckPlayer())
                    {
                        if (_enemyController.BossAttackCount > _enemyController.EnemyData.MeleeSkillCount)
                        {
                            Debug.Log("Boss의 근접 스킬공격");
                            _enemyController.EnemyAnimator.SetTrigger("MeleeSkill");
                            _enemyController.BossAttackTime = 0f;
                            _enemyController.BossAttackCount = 0;
                        }
                        else
                        {
                            Debug.Log("Boss의 근접 기본공격");
                            _enemyController.EnemyAnimator.SetTrigger("MeleeAttack");
                            _enemyController.BossAttackTime = 0f;
                            _enemyController.BossAttackCount++;
                        }
                    }                                         
                    else
                    {
                        _enemyController.WalkStart();
                        _enemyController.EnemyAnimator.SetBool("Attack", false);
                        break;
                    }
                }

                if (_enemyController.BossRangeSkillTime >= _enemyController.EnemyData.RangedSkillCoolTime)
                {
                    Debug.Log("Boss의 원거리 스킬공격");
                    StartCoroutine(COStartWalkState());
                    _enemyController.EnemyAnimator.SetTrigger("RangedSkill");
                    _enemyController.BossRangeSkillTime = 0f;
                    break;
                }
            }

            if(_enemyController.IsHit_attack || _enemyController.IsHit_skill)
            {
                _enemyController.HitStart();
                _enemyController.EnemyAnimator.SetTrigger("Hit");
                break;
            }

            yield return null;
        }
    }

    IEnumerator COStartWalkState()
    {
        yield return new WaitForSeconds(3f);

        _enemyController.WalkStart();
        _enemyController.EnemyAnimator.SetBool("Attack", false);
    }
}
