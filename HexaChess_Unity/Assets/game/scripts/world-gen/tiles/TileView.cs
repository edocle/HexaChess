
using UnityEngine;

namespace hexaChess.worldGen
{
    public class TileView : MonoBehaviour
    {
        public Tile m_Data { get; private set; }

        [SerializeField] protected Transform m_Transform = null;

        [SerializeField] private MeshRenderer m_MeshRenderer = null;

        public void Init(Tile tile)
        {
            m_Data = tile;
            SetTilePosition(m_Data.m_CoordX, m_Data.m_CoordY, 0, m_Data.m_Radius);
        }

        #region TilePosition

        private void SetTilePosition(int posX, int posY, float posZ, float radius)
        {
            m_Transform.localPosition = new Vector3(m_Data.m_WorldPos.x, m_Data.m_WorldPosZ, m_Data.m_WorldPos.y);
        }

        public void RefreshPositionZ()
        {
            m_Transform.localPosition = new Vector3(m_Transform.localPosition.x, m_Data.m_WorldPosZ, m_Transform.localPosition.z);
        }

        #endregion TilePosition

        public void SetupMaterial(Material material)
        {
            m_MeshRenderer.material = material;
        }

        #region Links

        // only if needed
        public virtual void GenerateMesh()
        {

        }

        #endregion Links

        #region Detection

        private void OnMouseDown()
        {
            Debug.Log($"> coords: {m_Data.m_CoordX} {m_Data.m_CoordY} / coord pos: {m_Data.m_CoordPos.x} {m_Data.m_CoordPos.y} / distance to origin: {m_Data.m_CoordPosDistanceToOrigin}");
        }

        #endregion Detection
    }
}