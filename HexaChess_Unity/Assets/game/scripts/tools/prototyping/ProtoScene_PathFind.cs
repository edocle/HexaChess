
using edocle.tools;
using hexaChess.worldGen;
using UnityEngine;

namespace hexaChess.prototyping.pathFind
{
    public class ProtoScene_PathFind : MonoBehaviour
    {
        [SerializeField] Camera m_Camera = null;
        [SerializeField] IslandGenerator m_IslandGenerator = null;
        [SerializeField] UserInput m_UserInput = null;

        void Start()
        {
            m_UserInput.SetCamera(m_Camera);
            // Listeners
            m_UserInput.OnRaycastHitObject += CallbackRaycastHitObject;

            GenerateTerrain();
        }

        void GenerateTerrain()
        {
            Debug.Log($"Generate terain");
            m_IslandGenerator.GenerateTerrain(() => GenerateUnit());
            // @todo get tiles & chunks data
        }

        void GenerateUnit()
        {
            Debug.Log($"Generate unit");
            // @todo
        }

        void CallbackRaycastHitObject(RaycastHit hit)
        {
            ChunkGameObject chunk = hit.collider.gameObject.GetComponent<ChunkGameObject>();
            if (chunk != null)
            {
                // Debug.Log($"Chunk ! target tile at {hit.point.x} {hit.point.z} ?");
                chunk.m_Chunk.GetTargetedTile(new Vector2(hit.point.x, hit.point.z));
            }
        }

        // @todo get user input
        // @todo get coordinate of tile clicked (when clicking on chunk)
        // -> how to get the position of a tile in the chunk ? raycast ?
        // @todo try to move unit from current tile to target tile
        // -> highlight path found
        // -> implement how a unit moves
        // -> make the unit move tile by tile
    }
}