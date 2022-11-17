using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class Shield : MonoBehaviour
{
    private int _health;
    private const string dissolvePropety = "_Disolve", fresnelProperty = "_FresnelColor", edgeColorProperty = "_DisolveEdgeColor";
    private bool canBeDamaged = true;
    private Material mat;
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Color startColor, damagedColor, edgeStartColor, edgeDeadColor;
    [SerializeField] int damageCooldown;
    public int Health { get => _health; set => _health = value; }
    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }
    void OnEnable()
    {

    }

    public void InitShield()
    {
        mat.SetColor(fresnelProperty, startColor);
        mat.SetColor(edgeColorProperty, edgeStartColor);
        healthSlider.value = _health;
        healthText.text = _health.ToString();
        if (healthSlider.transform.localScale.x == Vector3.one.x * 0)
            healthSlider.transform.localScale = Vector3.one * 0;
        mat.DOFloat(0, dissolvePropety, 2f)
        .OnComplete(() => healthSlider.transform.DOScale(0.001532384f, 1f)
        .SetEase(Ease.OutElastic));
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && canBeDamaged)
        {
            canBeDamaged = false;//also play audio here.
            _health--;
            healthText.text = _health.ToString();
            healthSlider.DOValue(_health, 1f);
            if (_health <= 0)
                mat.DOColor(damagedColor, fresnelProperty, .2f).OnComplete(() =>

                    healthSlider.transform.DOScale(0, .2f).OnComplete(() => DestroyShield())
                );
            else
            {
                mat.DOColor(damagedColor, fresnelProperty, .2f).SetLoops(2, LoopType.Yoyo);
                StartCoroutine(DamageCoolDown());
            }
        }
    }

    IEnumerator DamageCoolDown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canBeDamaged = true;
    }

    public void DestroyShield()
    {
        mat.SetColor(edgeColorProperty, edgeDeadColor);
        mat.DOFloat(0.62f, dissolvePropety, .5f).OnComplete(() =>
        {
            StopAllCoroutines();
            canBeDamaged = true;
            healthSlider.transform.localScale = Vector3.one * 0;
            gameObject.SetActive(false);
            //resetting everything
        });
    }
}
