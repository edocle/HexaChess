
using UnityEngine;

public class TileSlot
{
    private Vector3 m_Position;
    private SlotType m_slotType;

    public Vector3 Position => m_Position;
    // @todo need to calculate angle and height based on tile angle

    public TileSlot(Vector3 position, SlotType slotType)
    {
        m_Position = position;
        m_slotType = slotType;
    }
}

public enum SlotType
{
    main,
    side,
    micro
}
