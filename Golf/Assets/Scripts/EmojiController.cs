using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EmojiController : MonoBehaviour
{
    #region Variables
    [SerializeField] ParticleSystem emojiPS;
    [SerializeField] Material emojiMat;
    [SerializeField] Texture2D[] emojiTextures;
    #endregion
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Methods
    public void Init()
    {

    }

    private IEnumerator ShowEmoji(){
        
        yield return new WaitForSeconds(Random.Range(3, 10));
        emojiMat.SetTexture("_BaseMap", emojiTextures[Random.Range(0, emojiTextures.Length)]);
        
        emojiPS.Play();

        StartCoroutine(ShowEmoji());
    }
    #endregion
}
