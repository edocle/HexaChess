
//#define DEBUG_ISLANDTERRAIN_TILE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hexaChess.worldGen
{
    public abstract class IslandTerrain_Tiles : IslandTerrain
    {

        public IslandTerrain_Tiles() : base()
        {

        }

        /// <summary>
        /// Tiles generated directly
        /// </summary>
        public Tile[] Tiles { get; private set; }

        // Old way of generating terrain:
        // 1°) generate all tiles
        // 2°) link tiles
        // 3°) generate invoked tiles (1 per tile) or generate big objects containing batch of tiles

        #region Generation

        public override IEnumerator GenerateData(IslandGeneratorParameters parameters, Action callback)
        {
            List<(int, int)> tileCoordinates = GetAllTileCoordinates(parameters);
            yield return StartCoroutine(GetAllTilesData(tileCoordinates, parameters));

            yield return StartCoroutine(LinkTiles(parameters));
        }

        List<(int, int)> GetAllTileCoordinates(IslandGeneratorParameters parameters)
        {
            int count = 0;
            List<(int, int)> list = new List<(int, int)>();
            int mapRadius = parameters.MapRadius;

            for (int i = -mapRadius + 1; i < mapRadius; i++)
            {
                for (int j = -mapRadius + 1; j < mapRadius; j++)
                {
                    if (Mathf.Abs(i + j) >= mapRadius)
                    {
                        // Debug.Log($"i: {i} / j: {j} / total: {Mathf.Abs(i + j)} vs. radius: {mapRadius}");
                        continue;
                    }

                    list.Add((i, j));
                    count++;
                }
            }

#if DEBUG_ISLANDTERRAIN_TILE
            Debug.Log($"<color=blue>IslandGenerator></color> Generated {count} tiles");
#endif
            return list;
        }

        IEnumerator GetAllTilesData(List<(int, int)> coords, IslandGeneratorParameters parameters)
        {
            yield return null;
            List<Tile> tilesData = new List<Tile>();
            // debug infos
            float initTime = Time.time;
            // speed control
            int count = 0;
            int tempCount = 0;

            foreach (var coord in coords)
            {
                tilesData.Add(new Tile(coord.Item1, coord.Item2, parameters.TileRadius));
                count++;
                if (count > (tempCount + parameters.GeneratedTilesPerFrame))
                {
                    // yield return null;
                    tempCount = count;
                }
            }

#if DEBUG_ISLANDTERRAIN_TILE
            Debug.Log($"<color=blue>IslandGenerator></color> generate tiles data: DONE: time: {Time.time - initTime} seconds; Hexagons: {count}");
#endif
            Tiles = tilesData.ToArray();
        }

        IEnumerator LinkTiles(IslandGeneratorParameters parameters)
        {
            // debug infos
            float initTime = Time.time;
            // speed control
            int count = 0;
            int tempCount = 0;

            var tiles = Tiles;
            foreach (var tile in tiles)
            {
                tile.TryRegisterAdjacents(tiles);
                count++;
                if (count > (tempCount + parameters.LinkedTilesPerFrame))
                {
                    yield return null;
                    tempCount = count;
                }
            }
        }

        #endregion Generation

        #region Relief

        protected void RefreshTilesHeight(IslandGeneratorParameters parameters)
        {
            var tiles = new Tile[Tiles.Length];
            Array.Copy(Tiles, tiles, Tiles.Length);
            foreach (var tile in tiles)
                tile.RefreshHeight(GenerateTileHeight(tile.m_CoordPos.x, tile.m_CoordPos.y, tile.m_CoordPosDistanceToOrigin, parameters));
        }

        #endregion Relief


    }
}
