
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CursorDisplay : MonoBehaviour
{

#if UNITY_EDITOR
    private void Start()
    {
        Cursor.visible = true;
        Cursor.SetCursor(PlayerSettings.defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

#endif
}
