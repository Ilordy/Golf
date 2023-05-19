using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class EnemyPooler : MonoBehaviour
{
    [SerializeField] EnemyPoolerData[] m_enemiesToPool;
    private LinkedPool<EnemyClass>[] m_enemyPoolers; //change to regular object pool later.
    float m_netWeight;

    public EnemyClass SpawnEnemy(int index) => m_enemiesToPool?[index].pooler.Get();

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


    void Start()
    {
        m_netWeight = 0; //prob have to reset this after every level.
        foreach (var t in m_enemiesToPool)
        {
            t.CreatePooler();
            m_netWeight += t.probability;
            t.Weight = m_netWeight;
        }
    }

    [System.Serializable]
    private class EnemyPoolerData
    {
        [FormerlySerializedAs("m_enemyToPool")]
        public EnemyClass enemyToPool;

        public int maxSize;
        public LinkedPool<EnemyClass> pooler;
        [SerializeField, Range(0, 1)] public float probability;
        float m_weight;

        public float Weight
        {
            get => m_weight;
            set => m_weight = value;
        }

        public void CreatePooler()
        {
            pooler = new LinkedPool<EnemyClass>(Create, OnGet, Release, Destroy,
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
            //gotta do some more stuff here...
        }

        void Release(EnemyClass enemy)
        {
            enemy.gameObject.SetActive(false);
            enemy.OnDeath -= Release;
            //gotta do some more stuff here...
        }

        private void Destroy(EnemyClass enemy) => Object.Destroy(enemy.gameObject);
    }
}