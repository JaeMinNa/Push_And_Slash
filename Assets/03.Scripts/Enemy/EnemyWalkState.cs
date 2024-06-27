using System.Collections;
using UnityEngine;

public class EnemyWalkState : MonoBehaviour, IEnemyState
{
    private EnemyController _enemyController;
    private Vector3 _dir;

    // Start문과 동일하게 사용
    public void Handle(EnemyController enemyController)
    {
        if (!_enemyController)
            _enemyController = enemyController;

        Debug.Log("Walk 상태 시작");
        StartCoroutine(COUpdate());
    }

    // Update문과 동일하게 사용
    private IEnumerator COUpdate()
    {
        while (true)
        {
            _dir = (_enemyController.Target.transform.position - transform.position).normalized;
            transform.position += _dir * _enemyController.Speed * Time.deltaTime;

            if (_enemyController.CheckPlayer())
            {
                _enemyController.AttackStart();
                _enemyController.EnemyAnimator.SetBool("Attack", true);
                break;
            }

            if (_enemyController.IsHit_attack || _enemyController.IsHit_skill)
            {
                _enemyController.HitStart();
                _enemyController.EnemyAnimator.SetTrigger("Hit");
                break;
            }

            yield return null;
        }
    }
}