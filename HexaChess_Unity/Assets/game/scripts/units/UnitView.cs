
using UnityEngine;

namespace hexaChess.unit
{
    public class UnitView : MonoBehaviour
    {
        private Unit m_Unit;

        public UnitView(Unit unit)
        {
            m_Unit = unit;
        }
    }
}
