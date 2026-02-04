
using UnityEngine;

namespace edocle.tools
{
    [CreateAssetMenu(fileName = "GuiStyles", menuName = "edocle/GuiStyles")]
    public class GuiStyles : ScriptableObject
    {
        public GUIStyle m_StandardGuistyle = null;
        public GUIStyle m_InvisibleGuistyle = null;

        public Vector2 m_UniversalMargin = Vector2.zero;

    }
}
