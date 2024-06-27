using UnityEngine;
using System.Collections;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;

namespace EpicToonFX
{
    public class ETFXProjectileScript : MonoBehaviour
    {
        public enum Type
        {
            PlayerAttack,
            PlayerSkill,
            Enemy,
        }

        public GameObject impactParticle; // Effect spawned when projectile hits a collider
        public GameObject projectileParticle; // Effect attached to the gameobject as child
        public GameObject muzzleParticle; // Effect instantly spawned when gameobject is spawned
        [Header("Adjust if not using Sphere Collider")]
        public float colliderRadius = 1f;
        [Range(0f, 1f)] // This is an offset that moves the impact effect slightly away from the point of impact to reduce clipping of the impact effect
        public float collideOffset = 0.15f;

        public Type CharacterType;
        private GameObject _player;
        private Vector3 _dir;
        private LayerMask _layerMask;
        private CameraShake _cameraShake;
        private Vector3 _skillScale;
        [HideInInspector] public float Atk;
        [SerializeField] private Collider[] _targets;
        [SerializeField] private float _overlapSphereRange;
        [SerializeField] private float _speed;

        private void Awake()
        {
            _cameraShake = Camera.main.transform.GetComponent<CameraShake>();
        }

        void Start()
        {
            _player = GameManager.I.PlayerManager.Player;
            _skillScale = new Vector3(2.5f, 2.5f, 2.5f);

            if (CharacterType == Type.Enemy)
            {
                _layerMask = LayerMask.NameToLayer("Player");
                _dir = (_player.transform.position - transform.position).normalized;
                transform.LookAt(transform.position + _dir);
            }
            else
            {
                if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1") _layerMask = LayerMask.NameToLayer("Enemy");
                else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1") _layerMask = LayerMask.NameToLayer("Player");
            }

            projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
            if (CharacterType == Type.PlayerSkill) projectileParticle.transform.localScale = _skillScale;
            projectileParticle.transform.parent = transform;
            if (muzzleParticle)
            {
                muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
                if(CharacterType == Type.PlayerSkill) muzzleParticle.transform.localScale = _skillScale;
                Destroy(muzzleParticle, 1.5f); // 2nd parameter is lifetime of effect in seconds
            }

            StartCoroutine(CODestroyAttack());
        }

        void FixedUpdate()
        {
            //if (GetComponent<Rigidbody>().velocity.magnitude != 0)
            //{
            //    transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity); // Sets rotation to look at direction of movement
            //}

            transform.position += _dir * _speed * Time.deltaTime;

            RaycastHit hit;

            float radius; // Sets the radius of the collision detection
            if (transform.GetComponent<SphereCollider>())
                radius = transform.GetComponent<SphereCollider>().radius;
            else
                radius = colliderRadius;

            //Vector3 direction = transform.GetComponent<Rigidbody>().velocity; // Gets the direction of the projectile, used for collision detection
            //if (transform.GetComponent<Rigidbody>().useGravity)
            //    direction += Physics.gravity * Time.deltaTime; // Accounts for gravity if enabled
            //direction = direction.normalized;

            //float detectionDistance = transform.GetComponent<Rigidbody>().velocity.magnitude * Time.deltaTime; // Distance of collision detection for this frame

            if (Physics.SphereCast(transform.position, radius, _dir, out hit, 50f * Time.deltaTime)) // Checks if collision will happen
            {
                transform.position = hit.point + (hit.normal * collideOffset); // Move projectile to point of collision

                GameObject impactP = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject; // Spawns impact effect
                if (CharacterType == Type.PlayerSkill) impactParticle.transform.localScale = _skillScale;

                ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>(); // Gets a list of particle systems, as we need to detach the trails
                //Component at [0] is that of the parent i.e. this object (if there is any)
                for (int i = 1; i < trails.Length; i++) // Loop to cycle through found particle systems
                {
                    ParticleSystem trail = trails[i];

                    if (trail.gameObject.name.Contains("Trail"))
                    {
                        trail.transform.SetParent(null); // Detaches the trail from the projectile
                        Destroy(trail.gameObject, 2f); // Removes the trail after seconds
                    }
                }

                Destroy(projectileParticle, 3f); // Removes particle effect after delay
                Destroy(impactP, 3.5f); // Removes impact effect after delay
                Targetting();
                Destroy(gameObject); // Removes the projectile

                string name = gameObject.name.Substring(0, gameObject.name.Length - 7);
                if (CharacterType == Type.Enemy) GameManager.I.SoundManager.StartSFX("Enemy" + name + "Explosion", transform.position);
                else GameManager.I.SoundManager.StartSFX("Player" + name + "Explosion", transform.position);
            }
        }

        public void SetInit(float atk, Vector3 dir)
        {
            _dir = dir;
            transform.LookAt(transform.position + dir);

            Atk = atk;
        }

        private void Targetting()
        {
            int layerMask = (1 << _layerMask);  // Layer 설정
            _targets = Physics.OverlapSphere(transform.position, _overlapSphereRange, layerMask);
            
            if(CharacterType == Type.Enemy)
            {
                if (_targets != null)
                {
                    StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));

                    for (int i = 0; i < _targets.Length; i++)
                    {
                        if (!_targets[i].GetComponent<PlayerCharacter>().IsSkill)
                        {
                            _targets[i].GetComponent<PlayerCharacter>().PlayerNuckback(transform.position, Atk);
                        }
                    }
                }
            }
            else
            {
                if (_targets != null)
                {
                    if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
                    {
                        if (CharacterType == Type.PlayerAttack)
                        {
                            StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));

                            for (int i = 0; i < _targets.Length; i++)
                            {
                                _targets[i].GetComponent<EnemyController>().IsHit_attack = true;
                            }
                        }
                        else if (CharacterType == Type.PlayerSkill)
                        {
                            StartCoroutine(_cameraShake.COShake(0.8f, 0.5f));

                            for (int i = 0; i < _targets.Length; i++)
                            {
                                _targets[i].GetComponent<EnemyController>().IsHit_skill = true;
                            }
                        }
                    }
                    else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
                    {
                        if (CharacterType == Type.PlayerAttack)
                        {
                            StartCoroutine(_cameraShake.COShake(0.3f, 0.3f));

                            for (int i = 0; i < _targets.Length; i++)
                            {
                                _targets[i].GetComponent<PlayerCharacter>().PlayerNuckback(transform.position, Atk);
                            }
                        }
                        else if (CharacterType == Type.PlayerSkill)
                        {
                            StartCoroutine(_cameraShake.COShake(0.8f, 0.5f));

                            for (int i = 0; i < _targets.Length; i++)
                            {
                                _targets[i].GetComponent<PlayerCharacter>().PlayerNuckback(transform.position, Atk);
                            }
                        }
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

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, _overlapSphereRange);
        }

    }
}