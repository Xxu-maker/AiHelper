using OctoberStudio.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio.Enemy
{
    public class EnemyMegaSlimeBehavior : EnemyBehavior
    {
        private static readonly int SPAWN_TRIGGER = Animator.StringToHash("Spawn");
        private static readonly int CHARGE_TRIGGER = Animator.StringToHash("Charge");

        [SerializeField] Animator animator;

        [Header("Swords-武器")]
        [Tooltip("The prefab of the sword \n 武器的预制件")]
        [SerializeField] GameObject swordProjectilePrefab;
        [Tooltip("The amount of waves of the sword attack\n 武器攻击的波数")]
        [SerializeField] int swordsWavesCount = 3;
        [Tooltip("Time between waves of the sword attack\n 武器攻击波之间的时间")]
        [SerializeField] float durationBetweenSwordWaves = 1f;
        [Tooltip("The damage of each sword\n 每把武器的伤害")]
        [SerializeField] float swordDamage = 10f;
        [Tooltip("The swords will spawn at this points\n 武器将在这些点生成")]
        [SerializeField] List<Transform> swordSpawnPoints;

        [Header("Moving-移动")]
        [Tooltip("Time the slime moves between attacks\n 史莱姆在攻击之间移动的时间")]
        [SerializeField] float movingDuration = 3f;

        [Header("Spawning-生成")]
        [Tooltip("The type of an enemy that will be spawned during this attack\n 在此攻击期间将生成的敌人类型")]
        [SerializeField] EnemyType spawnedEnemyType = EnemyType.Slime;
        [Tooltip("The amount of waves of the spawn attack\n 生成攻击的波数")]
        [SerializeField] int spawningWavesCount;
        [Tooltip("The amount of enemies that will be spawned in one wave of the spawn attack\n 在生成攻击的一波中生成的敌人数量")]
        [SerializeField] int spawnedEnemiesCount;
        [Tooltip("The time between waves of spawnAttacks/n生成攻击波之间的时间")]
        [SerializeField] float durationBetweenSpawning = 1f;
        [Tooltip("The time the warning circle is active before the enemy is spawned\n 在生成敌人之前警告圈活动的时间")]
        [SerializeField] float durationBetweenWarningAndSpawning = 0.5f;

        [Space]
        [SerializeField] ParticleSystem spawningParticle;

        List<WarningCircleBehavior> warningCircles = new List<WarningCircleBehavior>();

        private PoolComponent<EnemyMegaSlimeProjectileBehavior> swordsPool;

        private List<EnemyMegaSlimeProjectileBehavior> swords = new List<EnemyMegaSlimeProjectileBehavior>();

        protected override void Awake()
        {
            base.Awake();

            swordsPool = new PoolComponent<EnemyMegaSlimeProjectileBehavior>(swordProjectilePrefab, 6);
        }

        public override void Play()
        {
            base.Play();

            StartCoroutine(BehaviorCoroutine());
        }

        private IEnumerator BehaviorCoroutine()
        {
            while(true)
            {
                IsMoving = false;

                for (int i = 0; i < swordsWavesCount; i++) {

                    animator.SetTrigger(CHARGE_TRIGGER);
                    yield return new WaitForSeconds(durationBetweenSwordWaves);
                }
                IsMoving = true;

                yield return new WaitForSeconds(movingDuration);

                IsMoving = false;

                for (int i = 0; i < spawningWavesCount; i++)
                {
                    StartCoroutine(SpawnCoroutine());

                    yield return new WaitForSeconds(durationBetweenSpawning);
                }

                IsMoving = true;

                yield return new WaitForSeconds(movingDuration);
            }
        }

        public void SwordsAttack()
        {
            StartCoroutine(SwordsAttackCoroutine());
        }

        private IEnumerator SpawnCoroutine()
        {
            spawningParticle.Play();

            animator.SetTrigger(SPAWN_TRIGGER);

            for (int j = 0; j < spawnedEnemiesCount; j++)
            {
                var spawnPosition = StageController.FieldManager.Fence.GetRandomPointInside(0.5f);

                var warningCircle = StageController.PoolsManager.GetEntity<WarningCircleBehavior>("Warning Circle");

                warningCircle.transform.position = spawnPosition;

                warningCircle.Play(1f, 0.3f, 100, null);

                warningCircles.Add(warningCircle);
            }

            yield return new WaitForSeconds(durationBetweenWarningAndSpawning);

            for(int i = 0; i < spawnedEnemiesCount; i++)
            {
                var warningCircle = warningCircles[i];

                StageController.EnemiesSpawner.Spawn(spawnedEnemyType, warningCircle.transform.position);

                warningCircle.gameObject.SetActive(false);
            }

            warningCircles.Clear();

            spawningParticle.Stop();
        }

        private IEnumerator SwordsAttackCoroutine()
        {
            for(int i = 0; i < swordSpawnPoints.Count; i++)
            {
                var spawnPoint = swordSpawnPoints[i];

                var sword = swordsPool.GetEntity();

                sword.Init(spawnPoint.transform.position, Vector2.zero);
                sword.Damage = StageController.Stage.EnemyDamage * swordDamage;
                sword.onFinished += OnSwordFinished;

                swords.Add(sword);

                yield return new WaitForSeconds(0.2f);
            }
        }

        private void OnSwordFinished(EnemyMegaSlimeProjectileBehavior sword)
        {
            sword.onFinished -= OnSwordFinished;

            swords.Remove(sword);
        }

        protected override void Die(bool flash)
        {
            base.Die(flash);

            for(int i = 0; i < swords.Count; i++)
            {
                var sword = swords[i];

                sword.onFinished -= OnSwordFinished;

                sword.Clear();
            }

            for(int i = 0; i < warningCircles.Count; i++)
            {
                warningCircles[i].gameObject.SetActive(false);
            }

            warningCircles.Clear();

            StopAllCoroutines();
        }
    }
}