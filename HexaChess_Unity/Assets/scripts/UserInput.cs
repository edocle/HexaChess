
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

        #region Lifecycle

        void Start()
        {
            // Depending on the platform, we will use different methods to track user activity
#if UNITY_EDITOR || UNITY_STANDALONE
            UpdateAction = Update_Computer;
#else
            UpdateAction = Update_Mobile;
#endif
        }

        Action UpdateAction = null;
        void Update()
        {
            UpdateAction?.Invoke();
        }

        #endregion Lifecycle

        #region Camera

        // Not sure it is useful, but can be used to get some position
        private Camera m_Camera;

        public void SetCamera(Camera camera)
        {
            m_Camera = camera;
        }

        #endregion Camera

        #region Action

        /// <summary>
        /// Inputs we want to track when running on a computer
        /// </summary>
        void Update_Computer()
        {
            Vector2 mousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                StartDrag(mousePos);
            }

            if (Input.GetMouseButton(0))
            {
                Drag(mousePos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!EndDrag(mousePos))
                    SendRaycast(mousePos);
            }
        }

        void Update_Mobile()
        {
            // empty for now
        }

        #endregion Action

        #region Drag

        [Header("Drag")]
        [SerializeField] private bool m_EnableDrag = false;
        [Tooltip("Minimal distance to trigger the drag.")]
        [SerializeField] private float m_DragThreshold = 10f;

        bool m_IsDragging;
        Vector2 m_DragStartPos;
        Vector2 m_DragCurrentPos;

        public Action<Vector2> OnDragStart;
        public Action<Vector2> OnDrag;
        public Action<Vector2> OnDragEnd;

        void StartDrag(Vector2 cursorPos)
        {
            m_IsDragging = false;
            m_DragStartPos = cursorPos;
        }

        void Drag(Vector2 cursorPos)
        {
            if (!m_EnableDrag)
                return;

            m_DragCurrentPos = cursorPos;
            Vector2 delta = m_DragCurrentPos - m_DragStartPos;

            // Check if we can enable drag or not: Need a non negligable diff from the initial position
            if (!m_IsDragging && delta.magnitude > m_DragThreshold)
            {
                m_IsDragging = true;
                OnDragStart?.Invoke(m_DragStartPos);
            }

            if (m_IsDragging)
            {
                OnDrag?.Invoke(m_DragCurrentPos);
            }
        }

        bool EndDrag(Vector2 cursorPos)
        {
            if (m_IsDragging)
            {
                m_IsDragging = false;
                OnDragEnd?.Invoke(cursorPos);
                return true;
            }

            return false;
        }

        #endregion Drag

        #region Raycast

        [Header("Raycast")]
        [SerializeField] private bool m_EnableRaycast = false;

        public Action<RaycastHit> OnRaycastHitObject = null;
        void SendRaycast(Vector2 cursorPos)
        {
            if (!m_EnableRaycast)
                return;

            RaycastHit hit;
            Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayOrigin, out hit))
            {
                OnRaycastHitObject?.Invoke(hit);
            }
        }

        #endregion Raycast
    }
}