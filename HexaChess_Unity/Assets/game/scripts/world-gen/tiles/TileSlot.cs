
using hexaChess;
using System.Xml;
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

    private WorldEntity m_Worldentity = null;
    public bool IsEmpty => m_Worldentity == null;
    public void SetupEntity(WorldEntity worldEntity)
    {
        m_Worldentity = worldEntity;
    }
}

public enum SlotType
{
    main,
    side,
    micro
}
