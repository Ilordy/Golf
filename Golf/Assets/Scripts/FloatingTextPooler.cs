using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using TMPro;
using DG.Tweening;
using UnityEditor;

namespace MobileTools
{
    /// <summary>
    /// Simple class for pooling text and making it float. Works on both screen and world space canvas.
    /// <br>
    /// Make sure TextMeshPro and Dotween are installed before use!
    /// </br>
    /// </summary>
    [HelpURL("https://www.youtube.com/watch?v=7EZ2F-TzHYw")]
    public class FloatingTextPooler : MonoBehaviour
    {
        #region Variables
        [SerializeField] Canvas _canvas;
        [SerializeField] TextMeshProUGUI _textPrefab;
        [SerializeField] FloatingTextTweener _tweenerType;
        [SerializeField] Vector2 _startPosition, _endPosition;

        [Header("POOL SETTINGS"), Space(5)]
        [SerializeField,
        Tooltip("Safety check for making sure released objects do not get released again at the cost of more CPU cycles. Set to true by default, Set to false to save some CPU time. Check out the HelpURL for more info on this variable.")]
        bool _collectionChecks = true;
        [SerializeField,
        Tooltip("Starting array size for the pool. Set to 10 by default, check out the HelpURL for more info on this variable.")]
        int _poolCapacity = 10;
        [SerializeField,
         Tooltip("Max size the pool array can be. Set to 15 by default, check out the HelpURL for more info on this variable.")]
        int _maxPoolSize = 15;
        private ObjectPool<TextMeshProUGUI> _pool;
        #endregion

        #region Life Cycle
        void Start()
        {
            _pool = new ObjectPool<TextMeshProUGUI>
            (CreateText,
            OnTextGet,
            OnTextReleased,
            DestroyText,
            _collectionChecks,
            _poolCapacity,
            _maxPoolSize);
        }

        void OnValidate()
        {
            if (_poolCapacity < 1)
                _poolCapacity = 1;
            if (_maxPoolSize < _poolCapacity)
                _maxPoolSize = _poolCapacity;
        }

        void OnDrawGizmosSelected()
        {
            if (!_canvas) return;
            //Make sure to have these icon names in your Assets/Gizmos folder or use a different icon name.
            if (_canvas.renderMode == RenderMode.WorldSpace)
            {
                var ct = _canvas.transform;
                Gizmos.DrawIcon(ct.TransformPoint(_startPosition), "FloatingTextSpawnPoint");
                Gizmos.DrawIcon(ct.TransformPoint(_endPosition), "FloatingTextEndPoint");
            }
            else
            {
                Gizmos.DrawIcon(WorldToScreenSpace(_startPosition), "FloatingTextSpawnPoint");
                Gizmos.DrawIcon(WorldToScreenSpace(_endPosition), "FloatingTextEndPoint");
            }
        }
        #endregion

        #region Private Methods
        //Create Position handles to easily specify startPos and EndPos for the texts in the scene. 
        Vector3 WorldToScreenSpace(Vector2 worldPos)
        {
            RectTransform canvasTransform = _canvas.transform as RectTransform;
            Vector3 screenPos = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasTransform, worldPos, null, out screenPos);
            screenPos = canvasTransform.TransformPoint(screenPos);
            return screenPos;
        }

        TextMeshProUGUI CreateText()
        {
            return Instantiate(_textPrefab, _canvas.transform);
        }

        void OnTextGet(TextMeshProUGUI text)
        {
            text.gameObject.SetActive(true);
            text.transform.localPosition = _startPosition;
        }

        void OnTextReleased(TextMeshProUGUI text)
        {
            text.gameObject.SetActive(false);
        }

        void DestroyText(TextMeshProUGUI text)
        {
            Destroy(text.gameObject);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a text object from the pool (or creates a new one if the pool is empty) and tweens it using the specificed Tweener Type.
        /// </summary>
        /// <param name="textValue">The value to give to the text.</param>
        public void CreateText(string textValue)
        {
            var pooledText = _pool.Get();
            var text = pooledText.GetComponent<TextMeshProUGUI>();
            text.text = textValue;
            _tweenerType.TweenText(text, _endPosition, _pool);
        }
        #endregion
    }
}
