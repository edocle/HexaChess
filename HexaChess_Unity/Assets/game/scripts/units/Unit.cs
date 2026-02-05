
using UnityEngine;

namespace hexaChess.unit
{
    public class Unit
    {
        // current coordinates
        public int m_CoordX { get; private set; }
        public int m_CoordY { get; private set; }

        UnitBehavior m_Behavior;

        public Unit(int coordX, int coordY, UnitBehavior behavior)
        {

        }
    }
}