using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class EnemyPooler : Singleton<EnemyPooler>
{
    #region Variables
    [SerializeField] EnemyPoolerData[] m_enemiesToPool;
    float m_netWeight;
    #endregion

    #region Public Methods
    public EnemyClass SpawnEnemy(int index) => m_enemiesToPool?[index].pooler.Get();

    /// <summary>
    /// Disables and plays an enemy's death particle
    /// </summary>
    /// <param name="relativeTransform">The transform of where to play the particle</param>
    /// <param name="obj">The gameobject to disable</param>
    public void SequenceMobDeath(Transform relativeTransform, GameObject obj)
    {
        StartCoroutine(SequenceMobDisableInternal(relativeTransform, obj));
    }
    
    public void SequenceMobItemDeath(Transform relativeTransform, GameObject obj)
    {
        StartCoroutine(SequenceMobItemDisableInternal(relativeTransform, obj));
    }

    /// <summary>
    /// Spawn a random enemy given their probabilities.
    /// </summary>
    /// <returns>A random enemy from the array pool.</returns>
    public EnemyClass SpawnRandomEnemy()
    {
        float random = Random.value * m_netWeight;
        for (int i = 0; i < m_enemiesToPool.Length; i++)
        {
            if (random <= m_enemiesToPool[i].Weight)
                return m_enemiesToPool[i].pooler.Get();
        }

        return null;
    }

    public void AddSpawnProbability(float amount)
    {
        foreach (var t in m_enemiesToPool)
        {
            if (Mathf.Approximately(t.probability, 1)) continue;
            t.probability += amount; //save this to a file later.
            if (t.probability > 1)
                t.probability = 1;
        }
    }
    #endregion


    protected override void Awake()
    {
        base.Awake();
        m_netWeight = 0; //prob have to reset this after every level.
        foreach (var t in m_enemiesToPool)
        {
            t.CreatePooler();
            m_netWeight += t.probability;
            t.Weight = m_netWeight;
        }
    }

    #region Private Methods
    IEnumerator SequenceMobDisableInternal(Transform pos, GameObject objectToDisable)
    {
        yield return new WaitForSeconds(1f);
        VFXManager.PlayEnemyDeathVFX(pos.position);
        objectToDisable.SetActive(false);
    }
    IEnumerator SequenceMobItemDisableInternal(Transform pos, GameObject objectToDisable)
    {
        yield return new WaitForSeconds(1f);
        VFXManager.PlayEnemyItemVFX(pos.position);
        objectToDisable.SetActive(false);
    }
    
    #endregion

    [System.Serializable]
    private class EnemyPoolerData
    {
        [FormerlySerializedAs("m_enemyToPool")] public EnemyClass enemyToPool;

        public int maxSize;
        public LinkedPool<EnemyClass> pooler;
        [SerializeField, Range(0, 1)] public float probability;
        float weight;

        public float Weight
        {
            get => weight;
            set => weight = value;
        }

        public void CreatePooler()
        {
            pooler = new LinkedPool<EnemyClass>(Create, OnGet, OnRelease, Destroy,
                maxSize: maxSize); //might need to change over time.
        }

        EnemyClass Create()
        {
            return Instantiate(enemyToPool);
        }

        void OnGet(EnemyClass enemy)
        {
            enemy.gameObject.SetActive(true);
            enemy.OnDeath += Release;
        }

        void OnRelease(EnemyClass enemy)
        {
            enemy.gameObject.SetActive(false);
            enemy.OnDeath -= Release;
        }

        public void Release(EnemyClass enemy) => pooler.Release(enemy);
        private void Destroy(EnemyClass enemy) => Object.Destroy(enemy.gameObject);
    }
}