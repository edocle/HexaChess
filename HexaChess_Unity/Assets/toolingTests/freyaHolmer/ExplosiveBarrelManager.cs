
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace toolingTests.freyaHolmer
{
    [ExecuteAlways]
    public class ExplosiveBarrelManager : MonoBehaviour
    {
        public static List<ExplosiveBarrel> allTheBArrels = new List<ExplosiveBarrel>();


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Vector3 managerPos = transform.position;
            foreach (var barrel in allTheBArrels)
            {
                Vector3 barrelPos = barrel.transform.position;
                float halfHeight = (managerPos.y - barrelPos.y) * .5f;
                Vector3 offset = Vector3.up * halfHeight;

                Handles.DrawBezier(managerPos, barrelPos, managerPos - offset, barrelPos + offset,
                                barrel.barrelType.color, EditorGUIUtility.whiteTexture,
                                1f);
            }
        }

        void OnDrawGizmosSelected()
        {
            foreach (var barrel in allTheBArrels)
            {
                Gizmos.DrawLine(transform.position, barrel.transform.position);
                // Handles.DrawAAPolyLine(managerPos, barrelPos);
            }
        }
    }

#endif
}
