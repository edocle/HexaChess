
using edocle.tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace edocle.core
{
    public class Router: MonoBehaviour
    {
        [SerializeField] private GameStarterParameters m_GameParameters = null;
        [SerializeField] private EventSystem m_EventSystem = null;
        [SerializeField] private GuiStyles m_GuiStyles = null;

        ThirdPartyServicesHandler m_ThirdPartyServicesHandler = null;
        GlobalMediator m_GlobalMediator = null;

        bool m_Initialized = false;

        public void InitRouter()
        {
            m_ThirdPartyServicesHandler = new ThirdPartyServicesHandler(m_GameParameters);
            m_GlobalMediator = new GlobalMediator(this);
            m_Initialized = true;
        }

        public ThirdPartyServicesHandler Services => m_ThirdPartyServicesHandler;
        public GlobalMediator GlobalMediator => m_GlobalMediator;

        private void Update()
        {
            if (!m_Initialized)
                return;

            m_GlobalMediator.Update();
        }


        private void OnGUI()
        {
            if (!m_Initialized)
                return;

            m_GlobalMediator.OnGUI(m_GuiStyles);
        }

        /// <summary>
        /// enable or disable the event system function; can be used when we want to lock the user interaction
        /// ex: when displaying GUI
        /// ex: when an ad is displayed
        /// ex: when a cinematic is played
        /// </summary>
        /// <param name="toggle"></param>
        public void ToggleEventSystemEnabled(bool toggle)
        {
            m_EventSystem.enabled = toggle;
        }
    }
}