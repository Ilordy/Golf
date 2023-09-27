using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Abstract class to inherit from when making an object pooler of type T.
/// </summary>
/// <typeparam name="T">A MonoBehaviour object you'd like to perform pooling on.</typeparam>
[HelpURL("https://www.youtube.com/@Tarodev/videos")]
public abstract class BasePooler<T> : MonoBehaviour where T : Component
{
    private T _prefab;
    private ObjectPool<T> _pool;

    private ObjectPool<T> Pool
    {
        get
        {
            if (_pool == null) throw new InvalidOperationException("You need to call InitPool before using it.");
            return _pool;
        }
        set => _pool = value;
    }

    protected void InitPool(T prefab, int initial = 10, int max = 20, bool collectionChecks = false)
    {
        _prefab = prefab;
        Pool = new ObjectPool<T>(
            CreateSetup,
            GetSetup,
            ReleaseSetup,
            DestroySetup,
            collectionChecks,
            initial,
            max);
    }

    #region Overrides
    protected virtual T CreateSetup() => Instantiate(_prefab);
    protected virtual void GetSetup(T obj) => obj.gameObject.SetActive(true);
    protected virtual void ReleaseSetup(T obj) => obj.gameObject.SetActive(false);
    protected virtual void DestroySetup(T obj) => Destroy(obj.gameObject);
    #endregion

    #region Getters
    public T Get() => Pool.Get();
    public void Release(T obj) => Pool.Release(obj);
    #endregion
}