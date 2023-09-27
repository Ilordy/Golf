using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// MonoBehaviour to help with particle system pooling.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class VFXPoolElement : MonoBehaviour
{
    private ObjectPool<VFXPoolElement> associatedPooler;
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        var main = particle.main;
        main.playOnAwake = false;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
    

    public void SetPooler(ObjectPool<VFXPoolElement> pooler)
    {
        associatedPooler = pooler;
    }

    private void OnEnable()
    {
        particle.Play();
    }
    
    private void OnParticleSystemStopped()
    {
        associatedPooler.Release(this);
    }
}