
using UnityEngine;

namespace hexaChess
{
    public class WorldEntity : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;

        const string m_AnimIsReady = "isReady";

        void Start()
        {
            m_Animator.SetTrigger(m_AnimIsReady);
        }
    }
}