using System;
using System.Collections.Generic;
using UnityEngine;

namespace toolingTests.freyaHolmer
{
    [CreateAssetMenu(fileName = "BarrelType", menuName = "Scriptable Objects/BarrelType")]
    public class BarrelType : ScriptableObject
    {
        [Range(1f, 8f)]
        public float radius = 1;
        public float damage = 10;
        public Color color = Color.red;

        public Action TriggerValidated = null;
        public void OnValidate()
        {
            Debug.Log($"> OnValidate called for {name}");
            TriggerValidated?.Invoke();
        }

        public List<BarrelSubType> m_SubTypes;
    }

    [System.Serializable]
    public class BarrelSubType
    {
        public string name;
        public string description; 
    }

    [System.Serializable]
    public class BarrelSuperType : BarrelSubType
    {
        public float damage;
    }
}
