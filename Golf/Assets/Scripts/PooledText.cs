using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MobileTools
{
    /// <summary>
    /// This class will be in charge of releasing itself back to the pool once it reaches it's end point.
    /// </summary>
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class PooledText : MonoBehaviour
    {
        private Vector2 m_endPoint = Vector2.positiveInfinity;
        ObjectPool<PooledText> m_pool;
        System.Action m_onEndPointReached;

        /// <summary>
        /// Initializes the release to the floating text pooler for this text object.
        /// </summary>
        /// <param name="onDestinationReached">Callback to invoke once the floating text reaches it's destination point. (We want to release it here)</param>
        /// <param name="endPoint">The end point of where the text will release itself once it reaches this point.</param>
        public void InitRelease(System.Action onDestinationReached, Vector2 endPoint, ObjectPool<PooledText> pool)
        {
            m_pool = pool;
            m_onEndPointReached = onDestinationReached;
            m_endPoint = endPoint;
        }

        public void Release()
        {
            m_pool.Release(this);
        }

        void Update()
        {
            if (m_endPoint == Vector2.positiveInfinity) return;
            if (Vector3.Distance(transform.localPosition, m_endPoint) <= .001f)//small offset, usually the distance will never return 0...
            {
                m_endPoint = Vector2.positiveInfinity;
                //m_onEndPointReached?.Invoke();
            }
        }
    }
}
