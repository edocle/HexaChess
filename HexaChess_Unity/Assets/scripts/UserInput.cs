
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
                OnRaycastHitObject?.Invoke(hit);
            }
        }

        public Action<RaycastHit> OnRaycastHitObject = null;

    }
}

