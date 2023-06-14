using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BossSliderInputHandler : MonoBehaviour
{
    [SerializeField] Image sliderImage;
    [SerializeField] RectTransform arrow;
    private float timeActivated;
    void Start()
    {
        sliderImage.DOFillAmount(1, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        arrow.DOAnchorPosX(0, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        timeActivated = Time.time + .5f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && timeActivated < Time.time)
        {
            Manager.I.DoFinalSwing(sliderImage.fillAmount * 100);
            gameObject.SetActive(false);
        }
    }
}
