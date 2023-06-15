using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ParticleSystem))]
public class EmojiController : MonoBehaviour
{
    #region Variables
    ParticleSystem emojiPS;
    [SerializeField] ParticleSystemRenderer psRenderer;
    Material emojiMat;
    [SerializeField] Texture2D[] emojiTextures;
    #endregion

    #region Methods
    void Start()
    {
        emojiPS = GetComponent<ParticleSystem>();
        emojiMat = GetComponent<ParticleSystemRenderer>().material;
    }

    void OnEnable()
    {
        Enemy.OnBecameHostile += Init;
    }

    void OnDisable()
    {
        Enemy.OnBecameHostile -= Init;
        StopAllCoroutines();
    }

    public void Init() => StartCoroutine(ShowEmoji());

    private IEnumerator ShowEmoji()
    {
        yield return new WaitForSeconds(Random.Range(3, 10));
        if (Random.value >= 0.5f)
            psRenderer.flip = Vector3.right;
        else
            psRenderer.flip = Vector3.zero;
        transform.localPosition = new Vector3(psRenderer.flip.x < 1 ? -1.2f : 1.2f, transform.localPosition.y, 0);
        emojiMat.SetTexture("_BaseMap", emojiTextures[Random.Range(0, emojiTextures.Length)]);
        emojiPS.Play();
        StartCoroutine(ShowEmoji());
    }
    #endregion
}