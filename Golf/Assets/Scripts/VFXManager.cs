using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VFXManager : Singleton<VFXManager>
{
    #region Variables
    [SerializeField] private VFXPoolElement enemyDeathVFX;
    [SerializeField] private VFXPoolElement ballHitVFX;
    [SerializeField] private VFXPoolElement enemyItemVFX;
    private ObjectPool<VFXPoolElement> enemyDeathVFXPool, ballHitVFXPool, enemyItemVFXPool;
    #endregion

    #region Methods
    private void Start()
    {
        enemyDeathVFXPool = new ObjectPool<VFXPoolElement>(() => CreateSetUp(enemyDeathVFXPool, enemyDeathVFX), GetSetUp, ReleaseSetUp,
            DestroySetUp, false, 10, 15);
        enemyItemVFXPool = new ObjectPool<VFXPoolElement>(() => CreateSetUp(enemyItemVFXPool, enemyItemVFX), GetSetUp, ReleaseSetUp,
            DestroySetUp, false, 10, 15);
        // ballHitVFXPool = new ObjectPool<VFXPoolElement>(() => CreateSetUp(ballHitVFX), GetSetUp, ReleaseSetUp,
        //DestroySetUp, false, 10, 15);
    }

    public static void PlayEnemyDeathVFX(Vector3 position)
    {
        var vfxInstance = I.enemyDeathVFXPool.Get();
        vfxInstance.transform.position = position;
    }
    
    public static void PlayEnemyItemVFX(Vector3 position)
    {
        var vfxInstance = I.enemyItemVFXPool.Get();
        vfxInstance.transform.position = position;
    }

    public void PlayBallHitVFX(Vector3 position)
    {
        var vfxInstance = ballHitVFXPool.Get();
        vfxInstance.transform.position = position;
        //vfxInstance.Play();
    }

    public static void PlayVFX(ParticleSystem vfx, Vector3 position)
    {
        var vfxInstance = Instantiate(vfx, position, Quaternion.identity);
        var main = vfxInstance.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        vfxInstance.Play();
    }
    #endregion

    #region Pooler Actions
    private VFXPoolElement CreateSetUp(ObjectPool<VFXPoolElement> pooler, VFXPoolElement prefab)
    {
        var obj = Instantiate(prefab);
        obj.SetPooler(pooler);
        return obj;
    }

    private void GetSetUp(VFXPoolElement particleSystem) => particleSystem.gameObject.SetActive(true);

    private void ReleaseSetUp(VFXPoolElement particleSystem) => particleSystem.gameObject.SetActive(false);

    private void DestroySetUp(VFXPoolElement particleSystem) => Destroy(particleSystem.gameObject);
    #endregion
}