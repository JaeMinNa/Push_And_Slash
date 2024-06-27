using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpicToonFX;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Transform _shootPosition;
    //private EnemyData _enemyData;
    private EnemyController _enemyController;

    private void Start()
    {
        //_enemyData = transform.parent.GetComponent<EnemyController>().EnemyData;
        _enemyController = transform.parent.GetComponent<EnemyController>();
    }

    public void StartSFX(string name)
    {
        GameManager.I.SoundManager.StartSFX(name);
    }

    public void StartRangedSFX(string name)
    {
        GameManager.I.SoundManager.StartSFX(name, transform.position);
    }

    public void ShootRangedAttack(string name)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Skills/Enemy/" + name), _shootPosition.position, Quaternion.identity);
        
        if(_enemyController.Type == EnemyController.EnemyType.Enemy4)
        {
            obj.GetComponent<ETFXProjectileScript>().Atk = _enemyController.Atk;
        }
        else if(_enemyController.Type == EnemyController.EnemyType.Enemy5)
        {
            obj.GetComponent<Arrow>().Atk = _enemyController.Atk;
        }
    }
}
