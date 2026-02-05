
using System;
using UnityEngine;

namespace edocle.tools
{
    /// <summary>
    /// WIP
    /// will handle all user inputs & manage them based on context
    /// </summary>
    public class UserInput : MonoBehaviour
    {
        private Camera m_Camera;

        public void SetCamera(Camera camera)
        {
            m_Camera = camera;
        }

        private void Update()
        {
            if ( Input.GetMouseButtonDown(0) )
            {
                GetCursorRaycast();
            }
        }

        void GetCursorRaycast()
        {
            RaycastHit hit;
            Ray rayOrigin = Camera.main.ScreenPointToRay( Input.mousePosition );
            if (Physics.Raycast(rayOrigin, out hit))
            {
                // Debug.Log($"> {hit.collider.gameObject.name} ({hit.point})");
                m_DrawPoint = true;
                m_Point = hit.point;
                OnRaycastHitObject?.Invoke(hit);
            }
        }

        public Action<RaycastHit> OnRaycastHitObject = null;

        Vector3 m_Point;
        bool m_DrawPoint = false;

        private void OnDrawGizmos()
        {
            if (m_DrawPoint)
                Gizmos.DrawSphere(m_Point, 1f);
        }
    }
}

