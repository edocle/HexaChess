
using edocle.tools;
using hexaChess.tool;
using hexaChess.worldGen;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
            // SetupPathFindObjects();
            // SetupSlotObjects();
            SetupTiltObject();
            SetupTiltTests();
            SetupEdges();
            SetupEntities();
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
        }

        void OnReliefGenerated()
        {
            ResetDisplayPath();
            ResetDisplaySlots();
            ResetDisplayTilt();
            ResetDisplayTiltTests();
            ResetEdges();
        }

        #endregion Island generation setup

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
            DisplayTilt();
            DisplayTiltTests();
            DisplayEdges();
            GenerateNewEntity();
        }

        #region Display path finding

        LineRenderer m_PathLine = null;
        GameObject m_PathSphere = null;
        bool m_DisplayPath = false;

        void SetupPathFindObjects()
        {
            m_DisplayPath = true;
            m_PathSphere = GenerateSphere("path finding sphere", 0.3f);
            m_PathLine = GenerateLine("path finding line", Color.red, Color.yellow);
        }

        void ResetDisplayPath()
        {
            if (!m_DisplayPath)
                return;

            LastPath = null;
            ResetLine(m_PathLine);
            ResetSphere(m_PathSphere);
        }

        void DisplayPath()
        {
            if (!m_DisplayPath)
                return;

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
        bool m_DisplaySlots = false;

        void SetupSlotObjects()
        {
            m_DisplaySlots = true;

            m_SlotSpheres = new GameObject[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject sphere = GenerateSphere($"slot sphere {i}", 0.25f);
                m_SlotSpheres[i] = sphere;
            }
        }

        void ResetDisplaySlots()
        {
            if (!m_DisplaySlots)
                return;

            for (int i = 0; i < 3; i++)
            {
                m_SlotSpheres[i].transform.localPosition = Vector3.zero;
            }
        }

        void DisplaySlots()
        {
            if (!m_DisplaySlots)
                return;

            Tile lastTile = LastPath.Last();
            for (int i = 0; i < 3; i++)
            {
                TileSlot slot = lastTile.SideSlots[i];
                m_SlotSpheres[i].transform.localPosition = new Vector3(slot.Position.x, slot.Position.y, slot.Position.z);
            }
        }

        #endregion Display slots

        #region Display tilt direction

        LineRenderer m_UpLine = null;
        LineRenderer m_TiltLine = null;
        LineRenderer[] m_SlotsLine = null;
        bool m_DisplayTilt = false;

        void SetupTiltObject()
        {
            m_DisplayTilt = true;

            m_UpLine = GenerateLine("up line", Color.blue, Color.blue, 0.02f);
            m_TiltLine = GenerateLine("tilt line", Color.saddleBrown, Color.saddleBrown, 0.04f);
            // Slots
            m_SlotsLine = new LineRenderer[3];
            for (int i = 0; i < 3; i++)
            {
                m_SlotsLine[i] = GenerateLine($"slot line {i}", Color.saddleBrown, Color.saddleBrown, 0.02f);
            }
        }

        void ResetDisplayTilt()
        {
            if (!m_DisplayTilt)
                return;

            ResetLine(m_UpLine);
            ResetLine(m_TiltLine);
            for (int i = 0; i < 3; i++)
            {
                ResetLine(m_SlotsLine[i]);
            }
        }

        void DisplayTilt()
        {
            if (!m_DisplayTilt)
                return;

            Tile lastTile = LastPath.Last();
            var center = lastTile.Center;
            // up
            m_UpLine.positionCount = 2;
            m_UpLine.SetPosition(0, center);
            m_UpLine.SetPosition(1, center + Vector3.up);
            // tilt
            m_TiltLine.positionCount = 2;
            m_TiltLine.SetPosition(0, center);
            m_TiltLine.SetPosition(1, center + lastTile.TiltDirection);

            // slots
            for (int i = 0; i < 3; i++)
            {
                m_SlotsLine[i].positionCount = 2;
                m_SlotsLine[i].SetPosition(0, center);
                m_SlotsLine[i].SetPosition(1, lastTile.SideSlots[i].Position);
            }
        }

        #endregion display tilt direction

        #region display tilt tests

        LineRenderer[] m_TiltTests_Edges = null;
        LineRenderer[] m_TiltTests_Tilts = null;
        bool m_DisplayTiltTests = false;

        void SetupTiltTests()
        {
            m_TiltTests_Edges = new LineRenderer[6];
            m_TiltTests_Tilts = new LineRenderer[3];
            m_DisplayTiltTests = true;

            // tilt 1
            m_TiltTests_Edges[0] = GenerateLine("tilt test edge 0", Color.red, Color.red, 0.04f);
            m_TiltTests_Edges[1] = GenerateLine("tilt test edge 1", Color.red, Color.red, 0.04f);
            m_TiltTests_Tilts[0] = GenerateLine("tilt test tilt 0", Color.red, Color.red, 0.04f);

            // tilt 2
            m_TiltTests_Edges[2] = GenerateLine("tilt test edge 2", Color.purple, Color.purple, 0.04f);
            m_TiltTests_Edges[3] = GenerateLine("tilt test edge 3", Color.purple, Color.purple, 0.04f);
            m_TiltTests_Tilts[1] = GenerateLine("tilt test tilt 1", Color.purple, Color.purple, 0.04f);


            // tilt 3
            m_TiltTests_Edges[4] = GenerateLine("tilt test edge 4", Color.yellow, Color.yellow, 0.04f);
            m_TiltTests_Edges[5] = GenerateLine("tilt test edge 5", Color.yellow, Color.yellow, 0.04f);
            m_TiltTests_Tilts[2] = GenerateLine("tilt test tilt 2", Color.yellow, Color.yellow, 0.04f);
        }

        void ResetDisplayTiltTests()
        {
            if (!m_DisplayTiltTests)
                return;

            for (int i = 0; i < m_TiltTests_Edges.Length; i++)
            {
                ResetLine(m_TiltTests_Edges[i]);
            }
            for (int i = 0; i < m_TiltTests_Tilts.Length; i++)
            {
                ResetLine(m_TiltTests_Tilts[i]);
            }
        }

        void DisplayTiltTests()
        {
            if (!m_DisplayTiltTests)
                return;

            Tile lastTile = LastPath.Last();
            var edges = lastTile.Edges;
            var center = lastTile.Center;

            // tilt 1
            Vector3 a = (edges[1] - center);
            Vector3 b = (edges[0] - center);
            var ab = Vector3.Cross(a, b);

            m_TiltTests_Edges[0].positionCount = 2;
            m_TiltTests_Edges[0].SetPosition(0, center);
            m_TiltTests_Edges[0].SetPosition(1, center + a);

            m_TiltTests_Edges[1].positionCount = 2;
            m_TiltTests_Edges[1].SetPosition(0, center);
            m_TiltTests_Edges[1].SetPosition(1, center + b);

            m_TiltTests_Tilts[0].positionCount = 2;
            m_TiltTests_Tilts[0].SetPosition(0, center);
            m_TiltTests_Tilts[0].SetPosition(1, center + ab);

            // tilt 2
            Vector3 c = (edges[3] - center);
            Vector3 d = (edges[2] - center);
            var cd = Vector3.Cross(c, d);

            m_TiltTests_Edges[2].positionCount = 2;
            m_TiltTests_Edges[2].SetPosition(0, center);
            m_TiltTests_Edges[2].SetPosition(1, center + c);

            m_TiltTests_Edges[3].positionCount = 2;
            m_TiltTests_Edges[3].SetPosition(0, center);
            m_TiltTests_Edges[3].SetPosition(1, center + d);

            m_TiltTests_Tilts[1].positionCount = 2;
            m_TiltTests_Tilts[1].SetPosition(0, center);
            m_TiltTests_Tilts[1].SetPosition(1, center + cd);

            // tilt 3
            Vector3 e = (edges[5] - center);
            Vector3 f = (edges[4] - center);
            var ef = Vector3.Cross(e, f);

            m_TiltTests_Edges[4].positionCount = 2;
            m_TiltTests_Edges[4].SetPosition(0, center);
            m_TiltTests_Edges[4].SetPosition(1, center + e);

            m_TiltTests_Edges[5].positionCount = 2;
            m_TiltTests_Edges[5].SetPosition(0, center);
            m_TiltTests_Edges[5].SetPosition(1, center + f);

            m_TiltTests_Tilts[2].positionCount = 2;
            m_TiltTests_Tilts[2].SetPosition(0, center);
            m_TiltTests_Tilts[2].SetPosition(1, center + ef);
        }

        #endregion display tilt tests

        #region display edges

        GameObject[] m_EdgeSpheres = null;
        GameObject m_RealCenter;
        bool m_DisplayEdges = false;

        void SetupEdges()
        {
            m_DisplayEdges = true;
            m_EdgeSpheres = new GameObject[6];

            for (int i = 0; i < 6; i++)
            {
                m_EdgeSpheres[i] = GenerateSphere($"edge {i}", 0.1f);
            }
            m_RealCenter = GenerateSphere("real center", 0.1f);
        }

        void ResetEdges()
        {
            if (!m_DisplayEdges)
                return;

            for (int i = 0; i < 6; i++)
            {
                ResetSphere(m_EdgeSpheres[i]);
            }

            ResetSphere(m_RealCenter);
        }

        void DisplayEdges()
        {
            if (!m_DisplayEdges)
                return;

            Tile lastTile = LastPath.Last();
            var edges = lastTile.Edges;
            for (int i = 0; i < 6; i++)
            {
                m_EdgeSpheres[i].transform.localPosition = edges[i];
            }

            m_RealCenter.transform.localPosition = lastTile.Center;
        }

        #endregion display edges

        LineRenderer GenerateLine(string name, Color startColor, Color endColor, float thick = 0.1f)
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
            renderer.startWidth = thick;
            renderer.endWidth = thick;

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


        #region Generate entity

        [Space(8)]
        [Header("Generate entities")]
        [SerializeField] GameObject m_EntityContentPrefab = null;
        [SerializeField] GameObject[] m_EntityPrefabs;


        void SetupEntities()
        {
            UpdateGeneratedObject(0);
        }

        GameObject m_CurrentEntityPrefab;
        public void UpdateGeneratedObject(int index)
        {
            m_CurrentEntityPrefab = m_EntityPrefabs[index];
        }

        bool m_SetupNewEntity = true;

        void GenerateNewEntity(bool bigObject = false, bool tilted = true)
        {
            if (!m_SetupNewEntity)
                return;

            Tile lastTile = LastPath.Last();
            TileSlot[] sideSlots = lastTile.SideSlots;
            TileSlot emptySlot = null;

            var test = sideSlots.Where(f => f.IsEmpty).ToList();

            if (test.Count == 0)
                return;

            if (test.Count == 1)
                emptySlot = test[0];
            else
                emptySlot = test[UnityEngine.Random.Range(0, test.Count)];

            Quaternion quat = !tilted ? Quaternion.identity : Quaternion.FromToRotation(Vector3.up, lastTile.TiltDirection);
            WorldEntity entity = GenerateEntity(m_CurrentEntityPrefab, emptySlot.Position, quat, tilted);

            emptySlot.SetupEntity(entity);
        }

        WorldEntity GenerateEntity(GameObject prefab, Vector3 position, Quaternion rotation, bool tilted)
        {
            var entityContent = Instantiate(m_EntityContentPrefab, position, rotation);
            var entity = Instantiate(prefab);
            entity.transform.parent = entityContent.transform;
            entity.transform.localPosition = new Vector3();
            entity.transform.localRotation = Quaternion.identity;

            return entityContent.GetComponent<WorldEntity>();
        }

        #endregion Generate entity
    }
}