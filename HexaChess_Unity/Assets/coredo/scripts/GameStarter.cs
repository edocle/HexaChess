
using UnityEngine;

namespace edocle.core
{
    public abstract class GameStarter : MonoBehaviour
    {
        [SerializeField] private Router m_Router = null;
        protected Router Router => m_Router;

        void Awake()
        {
            
        }

        void Start()
        {
            if (!CanGameStart())
                return;

            m_Router.InitRouter();
            StartGame();
        }

        bool CanGameStart()
        {
            if (m_Router == null)
            {
                Debug.LogError($"[GameStarter] Game cannot start: Missing router link");
                return false;
            }

            return true;
        }

        protected abstract void StartGame();
    }
}