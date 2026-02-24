
using edocle.tools;
using hexaChess.tool;
using hexaChess.worldGen;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace hexaChess.prototyping.pathFind
{
    public class ProtoScene_PathFind : MonoBehaviour
    {
        [SerializeField] Camera m_Camera = null;
        [SerializeField] IslandGenerator m_IslandGenerator = null;
        [SerializeField] UserInput m_UserInput = null;

        Tile m_CurrentUnitTile = null;
        List<Tile> LastPath = null;

        void Start()
        {
            m_UserInput.SetCamera(m_Camera);
            // Listeners
            m_UserInput.OnRaycastHitObject += CallbackRaycastHitObject;

            m_IslandGenerator.OnTerrainGenerated += OnTerrainGenerated;
            m_IslandGenerator.OnReliefGenerated += OnReliefGenerated;

            GenerateTerrain();
            SetupPathFindObjects();
            SetupSlotObjects();
        }

        #region Island generation setup

        void GenerateTerrain()
        {
            Debug.Log($"Generate terrain");
            m_IslandGenerator.GenerateTerrain();
            // @todo get tiles & chunks data
        }

        void OnTerrainGenerated()
        {
            m_CurrentUnitTile = m_IslandGenerator.GetTile(0, 0);
            Debug.Log($"> {m_CurrentUnitTile.m_CoordX} {m_CurrentUnitTile.m_CoordY}");

            GenerateUnit();
        }

        void OnReliefGenerated()
        {
            ResetDisplayPath();
            ResetDisplaySlots();
        }

        #endregion Island generation setup

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
                FindPath( chunk.m_Chunk.GetTargetedTile(new Vector2(hit.point.x, hit.point.z)) );
            }
        }

        #region Path finding

        void FindPath(Tile tile)
        {
            PathFindManager pathFind = new PathFindManager();
            LastPath = pathFind.FindPath(m_CurrentUnitTile, tile);

            DisplayPath();
            DisplaySlots();
        }

        #region Display path finding

        LineRenderer m_LineRenderer = null;
        GameObject m_TargetSphere = null;

        void SetupPathFindObjects()
        {
            // Target sphere
            m_TargetSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_TargetSphere.transform.localScale = Vector3.one / 2f;
            m_TargetSphere.transform.SetParent(transform, false);
            m_TargetSphere.transform.localPosition = Vector3.zero;
            var renderer = m_TargetSphere.GetComponent<MeshRenderer>();
            renderer.material = m_IslandGenerator.ActiveParameter.BeachMaterial;

            // Line renderer
            m_LineRenderer = gameObject.AddComponent<LineRenderer>();

            // Set the material
            m_LineRenderer.material = new Material(Shader.Find("Sprites/Default"));

            // Set the color
            m_LineRenderer.startColor = Color.red;
            m_LineRenderer.endColor = Color.yellow;

            // Set the width
            m_LineRenderer.startWidth = 0.2f;
            m_LineRenderer.endWidth = 0.2f;
        }

        void ResetDisplayPath()
        {
            LastPath = null;
            m_LineRenderer.positionCount = 0;
            m_TargetSphere.transform.localPosition = Vector3.zero;
        }

        void DisplayPath()
        {
            m_LineRenderer.positionCount = LastPath.Count;
            for (int i = 0; i < LastPath.Count; i++)
            {
                Tile tileA = LastPath[i];
                m_LineRenderer.SetPosition(i, new Vector3(tileA.m_CoordPos.x, tileA.m_CoordPosZ + 0.5f, tileA.m_CoordPos.y));
            }

            Tile lastTile = LastPath.Last();
            m_TargetSphere.transform.localPosition = new Vector3(lastTile.m_CoordPos.x, lastTile.m_CoordPosZ + 0.5f, lastTile.m_CoordPos.y);
        }

        #endregion Display path finding

        #endregion Path finding

        #region Display slots


        GameObject[] m_SlotSpheres = null;

        void SetupSlotObjects()
        {
            m_SlotSpheres = new GameObject[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject sphere = null;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = Vector3.one / 2.5f;
                sphere.transform.SetParent(transform, false);
                sphere.transform.localPosition = Vector3.zero;
                var renderer = sphere.GetComponent<MeshRenderer>();
                renderer.material = m_IslandGenerator.ActiveParameter.BeachMaterial;

                m_SlotSpheres[i] = sphere;
            }
        }

        void ResetDisplaySlots()
        {
            for (int i = 0; i < 3; i++)
            {
                m_SlotSpheres[i].transform.localPosition = Vector3.zero;
            }
        }

        void DisplaySlots()
        {
            Tile lastTile = LastPath.Last();
            Debug.Log($"> Tile position: {lastTile.m_CoordPos} ({lastTile.WorldPos3D})");
            for (int i = 0; i < 3; i++)
            {
                TileSlot slot = lastTile.SideSlots[i];
                Debug.Log($"> Tile side position [{i}]: {slot.Position}");
                m_SlotSpheres[i].transform.localPosition = new Vector3(slot.Position.x, slot.Position.y + 0.1f, slot.Position.z);
            }
        }

        #endregion Display slots
    }
}