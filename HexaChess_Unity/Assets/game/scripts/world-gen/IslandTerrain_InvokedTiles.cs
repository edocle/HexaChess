
//#define DEBUG_ISLANDTERRAIN_INVOKEDTILES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hexaChess.worldGen
{
    public class IslandTerrain_InvokedTiles : IslandTerrain_Tiles
    {
        /// <summary>
        /// Physical invoked tiles generated directly on island terrain
        /// </summary>
        public Dictionary<(int, int), TileGameObject> m_InvokedTiles { get; private set; }

        public IslandTerrain_InvokedTiles() : base()
        {
            m_InvokedTiles = new Dictionary<(int, int), TileGameObject>();
        }

        #region Generation

        public override IEnumerator GenerateIsland(IslandGeneratorParameters parameters, Action callback)
        {
            // debug infos
            float initTime = Time.time;
            // speed control
            int count = 0;
            int tempCount = 0;

            int mapRadius = parameters.MapRadius;

            var tiles = Tiles.ToArray();
            foreach (var tile in tiles)
            {
                TryInvokeTile(tile, parameters);
                count++;
                if (count > (tempCount + parameters.GeneratedTilesPerFrame))
                {
                    yield return null;
                    tempCount = count;
                }
            }

#if DEBUG_ISLAND_TERRAIN
            Debug.Log($"<color=blue>Island Terrain></color> generate invoked tiles: DONE: time: {Time.time - initTime} seconds; Hexagons: {count}");
#endif
            callback?.Invoke();
        }

        public bool TryInvokeTile(Tile tile, IslandGeneratorParameters parameters)
        {
            var key = (tile.m_CoordX, tile.m_CoordY);

            if (!m_InvokedTiles.ContainsKey(key))
            {
                TileGameObject invokedTile = Instantiate(parameters.Tile, transform).GetComponent<TileGameObject>();
                invokedTile.Init(tile);
                m_InvokedTiles.Add(key, invokedTile);
                return true;
            }

            return false;
        }

        #endregion Generation

        #region Relief

        public override IEnumerator RefreshRelief(IslandGeneratorParameters parameters, Action callback)
        {
            RefreshTilesHeight(parameters);

#if DEBUG_ISLANDTERRAIN_INVOKEDTILES
            Debug.Log($"Island terrain> Invoked tiles> Refreshed tiles height data");
#endif
            yield return null;

            foreach (var tile in m_InvokedTiles)
                tile.Value.RefreshPositionZ();

#if DEBUG_ISLANDTERRAIN_INVOKEDTILES
            Debug.Log($"Island terrain> Invoked tiles> Refreshed positions");
#endif
            yield return null;

            SetupTileMaterials(parameters);
            if (parameters.TileNeedMesh)
                yield return StartCoroutine(RefreshTileVertices(parameters));

            callback?.Invoke();

#if DEBUG_ISLANDTERRAIN_INVOKEDTILES
            Debug.Log($"Island terrain> Invoked tiles> Refreshed vertices");
#endif
            yield return null;
        }

        void SetupTileMaterials(IslandGeneratorParameters parameters)
        {
            foreach (var tile in m_InvokedTiles)
            {
                if (tile.Value.m_Data.m_WorldPosZ > parameters.BeachMaxHeight)
                {
                    tile.Value.SetupMaterial(parameters.GrassMaterial);
                }
                else if (tile.Value.m_Data.m_WorldPosZ > parameters.BeachMinHeight)
                {
                    tile.Value.SetupMaterial(parameters.BeachMaterial);
                }
                else
                {
                    tile.Value.SetupMaterial(parameters.SeaMaterial);
                }
            }
        }

        IEnumerator RefreshTileVertices(IslandGeneratorParameters parameters)
        {
            // debug infos
            float initTime = Time.time;
            // speed control
            int count = 0;
            int tempCount = 0;

            foreach (var tile in m_InvokedTiles)
            {
                tile.Value.GenerateMesh();
                count++;
                if (count > (tempCount + parameters.ReliefedTilesPerFrame))
                {
                    yield return null;
                    tempCount = count;
                }
            }
        }

        #endregion Relief
    }
}
