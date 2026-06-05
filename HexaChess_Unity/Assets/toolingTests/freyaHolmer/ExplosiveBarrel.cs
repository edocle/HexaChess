
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace toolingTests.freyaHolmer
{
    [ExecuteAlways]
    public class ExplosiveBarrel : MonoBehaviour
    {
        public BarrelType barrelType;

        static readonly int shPropcolor = Shader.PropertyToID("_BASE_COLOR");

        MaterialPropertyBlock mpb;
        MaterialPropertyBlock Mpb
        {
            get
            {
                if (mpb == null)
                {
                    mpb = new MaterialPropertyBlock();
                    GetComponent<MeshRenderer>().GetPropertyBlock(mpb);
                }
                return mpb;
            }
        }

        MeshRenderer rnd;
        MeshRenderer Rnd
        {
            get
            {
                if (rnd == null)
                {
                    rnd = GetComponent<MeshRenderer>();
                }
                return rnd;
            }
        }

        void ApplyColor()
        {
            Debug.Log($"> Apply color");
            Mpb.SetColor(shPropcolor, barrelType.color);
            Rnd.SetPropertyBlock(Mpb);
        }

        void OnEnable()
        { ExplosiveBarrelManager.allTheBArrels.Add(this); TryListen(); ApplyColor(); }

        void OnDisable()
        { ExplosiveBarrelManager.allTheBArrels.Remove(this); TryUnlisten(); }

        void TryListen()
        {
            if (barrelType != null)
            {
                Debug.Log($"> Try listen to {barrelType.name}");
                barrelType.TriggerValidated += ApplyColor;
            }
        }

        void TryUnlisten()
        {
            if (barrelType != null)
            {
                Debug.Log($"> Try unlisten to {barrelType.name}");
                barrelType.TriggerValidated -= ApplyColor;
            }
        }

        private void OnDrawGizmos()
        {
            // Gizmos.DrawWireSphere(transform.position, radius);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Handles.DrawWireDisc(transform.position, transform.up, barrelType.radius);
            // Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}