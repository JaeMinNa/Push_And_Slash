using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        Enemy0,
        Enemy1,
        Enemy2,
        Enemy3,
        Enemy4,
        Enemy5,
        Boss0,
        Boss1,
        Boss2,
        Boss3
    }

    public EnemyStateContext _enemyStateContext { get; private set; }

    public bool IsBoss;
    public EnemyType Type;
    [HideInInspector] public GameObject Target;
    [HideInInspector] public EnemyData EnemyData;
    [HideInInspector] public Animator EnemyAnimator;
    [HideInInspector] public Rigidbody Rigidbody;
    [HideInInspector] public RaycastHit ForwardHit;
    [HideInInspector] public RaycastHit DownHit;
    [HideInInspector] public CharacterData PlayerData;
    [HideInInspector] public StageController StageController;
    [HideInInspector] public float BossAttackTime;
    [HideInInspector] public float BossRangeSkillTime;
    [HideInInspector] public int BossAttackCount;
    [HideInInspector] public float Speed;
    [HideInInspector] public float Atk;
    [HideInInspector] public float Def;
    [HideInInspector] public bool IsHit_attack;
    [HideInInspector] public bool IsHit_skill;

    private IEnemyState _walkState;
    private IEnemyState _attackState;
    private IEnemyState _hitState;

    private void Awake()
    {
        EnemyAnimator = transform.GetChild(0).GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        StageController = GameObject.FindWithTag("StageController").GetComponent<StageController>();
    }

    private void Start()
    {
        Target = GameManager.I.PlayerManager.Player;
        PlayerData = GameManager.I.DataManager.PlayerData;

        _enemyStateContext = new EnemyStateContext(this);
        _walkState = gameObject.AddComponent<EnemyWalkState>();
        _attackState = gameObject.AddComponent<EnemyAttackState>();
        _hitState = gameObject.AddComponent<EnemyHitState>();

        EnemySetting();
        IsHit_attack = false;
        IsHit_skill = false;

        if(IsBoss)
        {
            BossAttackTime = 0f;
            BossAttackCount = 0;
            BossRangeSkillTime = 0f;
        }

        StartCoroutine(COWalkStart());
    }

    private void Update()
    {
        if (IsBoss)
        {
            BossRangeSkillTime += Time.deltaTime;
            BossAttackTime += Time.deltaTime;
        }

        CheckPlayer();
        transform.LookAt(Target.transform.position);

        if(transform.position.y > 0) transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public void WalkStart()
    {
        _enemyStateContext.Transition(_walkState);
    }

    public void AttackStart()
    {
        _enemyStateContext.Transition(_attackState);
    }

    public void HitStart()
    {
        _enemyStateContext.Transition(_hitState);
    }

    private void EnemySetting()
    {
        switch (Type)
        {
            case EnemyType.Enemy0:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[0];
                break;

            case EnemyType.Enemy1:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[1];
                break;

            case EnemyType.Enemy2:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[2];
                break;

            case EnemyType.Enemy3:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[3];
                break;

            case EnemyType.Enemy4:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[4];
                break;

            case EnemyType.Enemy5:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[5];
                break;
            case EnemyType.Boss0:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[6];
                break;
            case EnemyType.Boss1:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[7];
                break;
            case EnemyType.Boss2:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[8];
                break;
            case EnemyType.Boss3:
                EnemyData = GameManager.I.DataManager.DataWrapper.EnemyDatas[9];
                break;


            default:
                break;
        }

        Speed = EnemyData.Speed;
        Atk = EnemyData.Atk;
        Def = EnemyData.Def;
    }

    public bool CheckPlayer()
    {
        if(Type == EnemyType.Enemy4 || Type == EnemyType.Enemy5)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 0.7f, 0), transform.forward * 8.5f, Color.green);

            if (Physics.Raycast(transform.position + new Vector3(0, 0.7f, 0), transform.forward, out ForwardHit, 8.5f))
            {
                if (ForwardHit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        else
        {
            Debug.DrawRay(transform.position + new Vector3(0, 0.7f, 0), transform.forward * 1.3f, Color.green);

            if (Physics.Raycast(transform.position + new Vector3(0, 0.7f, 0), transform.forward, out ForwardHit, 1.3f))
            {
                if (ForwardHit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsGround()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out DownHit, 1f))
        {
            if (DownHit.transform.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator COWalkStart()
    {
        yield return new WaitForSeconds(0.1f);

        _enemyStateContext.Transition(_walkState);
    }
}