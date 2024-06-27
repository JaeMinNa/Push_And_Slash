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
        //if (_enemyController.Type == EnemyController.EnemyType.Enemy5)
        //{
        //    _enemyController.BowAnimator.SetTrigger("Attack");
        //    _enemyController.ArrowAnimator.SetTrigger("Attack");
        //}
        _time = 0f;
        StartCoroutine(COUpdate());
    }

    // Update문과 동일하게 사용
    private IEnumerator COUpdate()
    {
        while (true)
        {
            _time += Time.deltaTime;

            if(_time >= _enemyController.EnemyData.AttackCoolTime)
            {
                if(_enemyController.CheckPlayer())
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

            if(_enemyController.IsHit_attack || _enemyController.IsHit_skill)
            {
                _enemyController.HitStart();
                _enemyController.EnemyAnimator.SetTrigger("Hit");
                break;
            }

            yield return null;
        }
    }
}
