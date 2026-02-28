
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
            // SetupSlotObjects();
            SetupTiltObject();
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
            // ResetDisplaySlots();
            ResetDisplayTilt();
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
            // DisplaySlots();
            DisplayTilt();
        }

        #region Display path finding

        LineRenderer m_PathLine = null;
        GameObject m_PathSphere = null;

        void SetupPathFindObjects()
        {
            m_PathSphere = GenerateSphere("path finding sphere", 0.3f);
            m_PathLine = GenerateLine("path finding line", Color.red, Color.yellow);
        }

        void ResetDisplayPath()
        {
            LastPath = null;
            ResetLine(m_PathLine);
            ResetSphere(m_PathSphere);
        }

        void DisplayPath()
        {
            m_PathLine.positionCount = LastPath.Count;
            for (int i = 0; i < LastPath.Count; i++)
            {
                Tile tileA = LastPath[i];
                m_PathLine.SetPosition(i, new Vector3(tileA.m_CoordPos.x, tileA.m_CoordPosZ + 0.5f, tileA.m_CoordPos.y));
            }

            Tile lastTile = LastPath.Last();
            m_PathSphere.transform.localPosition = new Vector3(lastTile.m_CoordPos.x, lastTile.m_CoordPosZ + 0.5f, lastTile.m_CoordPos.y);
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
                GameObject sphere = GenerateSphere($"slot sphere {i}", 0.25f);
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
                m_SlotSpheres[i].transform.localPosition = new Vector3(slot.Position.x, slot.Position.y, slot.Position.z);
            }
        }

        #endregion Display slots

        #region Display tilt direction

        LineRenderer m_UpLine = null;
        LineRenderer m_TiltLine = null;
        LineRenderer[] m_SlotsLine = null;
        LineRenderer[] m_TiltedSlotsLine = null;

        void SetupTiltObject()
        {
            m_UpLine = GenerateLine("up line", Color.yellow, Color.yellow);
            m_TiltLine = GenerateLine("tilt line", Color.blue, Color.yellow);
            // Slots
            m_SlotsLine = new LineRenderer[3];
            m_TiltedSlotsLine = new LineRenderer[3];
            for (int i = 0; i < 3; i++)
            {
                m_SlotsLine[i] = GenerateLine($"slot line {i}", Color.red, Color.red);
                m_TiltedSlotsLine[i] = GenerateLine($"tilted slot line {i}", Color.purple, Color.purple);
            }
        }

        void ResetDisplayTilt()
        {
            ResetLine(m_UpLine);
            ResetLine(m_TiltLine);
            for (int i = 0; i < 3; i++)
            {
                ResetLine(m_SlotsLine[i]);
                ResetLine(m_TiltedSlotsLine[i]);
            }
        }

        void DisplayTilt()
        {
            Tile lastTile = LastPath.Last();
            // up
            m_UpLine.positionCount = 2;
            m_UpLine.SetPosition(0, lastTile.WorldPos3D);
            m_UpLine.SetPosition(1, lastTile.WorldPos3D + Vector3.up);
            // tilt
            m_TiltLine.positionCount = 2;
            m_TiltLine.SetPosition(0, lastTile.WorldPos3D);
            m_TiltLine.SetPosition(1, lastTile.WorldPos3D + lastTile.TiltDirection);

            // slots
            for (int i = 0; i < 3; i++)
            {
                m_SlotsLine[i].positionCount = 2;
                m_SlotsLine[i].SetPosition(0, lastTile.WorldPos3D);
                m_SlotsLine[i].SetPosition(1, lastTile.SideSlots[i].Position);

                // attempt at tilt
                Vector3 slotVector = lastTile.SideSlots[i].Position - lastTile.WorldPos3D;
                Vector3 upVector = Vector3.up;
                Vector3 upTiltedVector = lastTile.TiltDirection;

                Vector3 firstCross = Vector3.Cross(slotVector, upVector);
                Vector3 tiltedVector = Vector3.Cross(-firstCross, upTiltedVector);

                m_TiltedSlotsLine[i].positionCount = 2;
                m_TiltedSlotsLine[i].SetPosition(0, lastTile.WorldPos3D);
                m_TiltedSlotsLine[i].SetPosition(1, lastTile.WorldPos3D + tiltedVector);
            }
        }

        #endregion display tilt direction

        LineRenderer GenerateLine(string name, Color startColor, Color endColor)
        {
            // setup game object
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.name = name;
            gameObject.transform.localScale = Vector3.one / 8f;
            gameObject.transform.SetParent(transform, false);
            gameObject.transform.localPosition = Vector3.zero;

            // setup line renderer
            LineRenderer renderer = gameObject.AddComponent<LineRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));

            // Set the color
            renderer.startColor = startColor;
            renderer.endColor = endColor;

            // Set the width
            renderer.startWidth = 0.1f;
            renderer.endWidth = 0.1f;

            return renderer;
        }

        void ResetLine(LineRenderer renderer)
        {
            renderer.positionCount = 0;
        }

        GameObject GenerateSphere(string name, float size)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = name;
            sphere.transform.localScale = Vector3.one * size;
            sphere.transform.SetParent(transform, false);
            sphere.transform.localPosition = Vector3.zero;
            var renderer = sphere.GetComponent<MeshRenderer>();
            renderer.material = m_IslandGenerator.ActiveParameter.BeachMaterial;

            return sphere;
        }

        void ResetSphere(GameObject sphere)
        {
            sphere.transform.localPosition = Vector3.zero;
        }
    }
}