
using edocle.tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace edocle.core
{
    public sealed partial class GlobalMediator
    {
        private readonly Router m_Router;

        public GlobalMediator(Router router)
        {
            m_Router = router;
        }
    }

    // GUI
    public partial class GlobalMediator
    {
        bool m_GuiEnabled = true;

        float m_DeltaTime = 0;
        float m_FramePerSeconds = 0;

        public void Update()
        {
            m_DeltaTime = Time.unscaledDeltaTime;
            m_FramePerSeconds = 1.0f / m_DeltaTime;
        }

        public void OnGUI(GuiStyles styles)
        {
            if (!m_GuiEnabled)
                return;

            styles.m_UniversalMargin.y = Screen.height;
            float posX = Screen.width / 2;
            float posY = Screen.height / 2;
            float width = Screen.width / 5;
            float height = Screen.height / 30;
            Rect box = new Rect(0, 0, width, height);

            GUI.Box(box, $"Framerate: {(m_DeltaTime * 1000):0.0} ms ({m_FramePerSeconds:0.} fps)", styles.m_StandardGuistyle);
#if UNITY_EDITOR
            box.y += height;
            GUI.Box(box, $"Drawcalls: {UnityEditor.UnityStats.drawCalls}", styles.m_StandardGuistyle);
#endif
        }

        void ToggleMediatorGui(bool toggle)
        {
            m_GuiEnabled = toggle;
            m_Router.ToggleEventSystemEnabled(!toggle);
        }
    }
}