
//#define DEBUG_ISLANDTERRAIN_TILECONTAINER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hexaChess.worldGen
{
    public class IslandTerrain_TileContainers : IslandTerrain_Tiles
    {
        /// <summary>
        /// Game objects that contain tiles in order to generate mesh
        /// used for:
        /// - invoked tiles
        /// - giant island mesh
        /// - tiles batched together
        /// </summary>
        List<TileContainer> m_TileContainers = null;
        public List<TileContainer> TileContainers => m_TileContainers;

        public IslandTerrain_TileContainers() : base()
        {

        }

        #region Generation

        public override IEnumerator GenerateIsland(IslandGeneratorParameters parameters, Action callback)
        {
            if (parameters.GeneratorMode == IslandGeneratorMode.OneTileContainerObject)
            {
                yield return StartCoroutine(GenerateOneBigObject(parameters, callback));
            }
            else
            {
                yield return StartCoroutine(GenerateIslandPieces(parameters, callback));
            }
        }

        IEnumerator GenerateOneBigObject(IslandGeneratorParameters parameters, Action callback)
        {
            yield return null;

            TileContainer tileContainer = gameObject.AddComponent<TileContainer>();
            tileContainer.Init(parameters.GrassMaterial);
            tileContainer.UpdateMeshIndexFormat(UnityEngine.Rendering.IndexFormat.UInt32);

            m_TileContainers = new List<TileContainer> { tileContainer };

            var tiles = new Tile[Tiles.Length];
            Array.Copy(Tiles, tiles, Tiles.Length);

            tileContainer.SetupOriginTile(tiles[0]);
            tileContainer.SetupTiles(tiles);

            callback?.Invoke();
        }

        IEnumerator GenerateIslandPieces(IslandGeneratorParameters parameters, Action callback)
        {
            yield return null;

            Debug.Log($"> Start adding island pieces...");
            // Get all objects
            int meshRadius = parameters.IslandPiecesRadius;

            List<(int, int)> islandPiecesCoords = new List<(int, int)>();
            int maxIslandPiecesRadius = parameters.MapRadius / parameters.IslandPiecesRadius;

            // List all mesh object coordinates; all coordinates are tile coordinate * meshRadius
            TryAddIslandPieceMesh(0, 0, islandPiecesCoords, parameters.IslandPiecesRadius, maxIslandPiecesRadius);
            yield return null;
            Debug.Log($"> Try add mesh object occurences: {m_TryAddIslandPieceCount}");
            Debug.Log($"> All island pieces (no one should go over {maxIslandPiecesRadius}: {string.Join(", ", islandPiecesCoords)}");

            // Invoke all mesh objects
            m_TileContainers = new List<TileContainer>();

            int count = 0;
            int tempCount = 0;

            foreach (var islandPieceCoord in islandPiecesCoords)
            {
                GenerateIslandPiece(islandPieceCoord, parameters);

                count++;
                if (count > (tempCount + parameters.GeneratedTileContainersPerFrame))
                {
                    yield return null;
                    Debug.Log($"<color=blue>Generate island multiple meshes</color>> Generated {count} mesh object coords");
                    tempCount = count;
                }
            }

            Debug.Log($"<color=blue>Generate island multiple meshes</color>> Generated {count} mesh object coords");

            // Dispatch all tiles in tile containers
            List<Tile> unassignedTiles = new List<Tile>(Tiles);
            foreach (var tileContainer in m_TileContainers)
            {
                DispatchTilesInIslandPieces(ref unassignedTiles, tileContainer, meshRadius);
            }

            Debug.Log($"<color=blue>Generate island multiple meshes</color>> Assigned tiles to {count} mesh objects");

            if (unassignedTiles.Count > 0)
                Debug.LogError($"<color=red>Generate island multiple meshes</color>> There is still {unassignedTiles.Count} unassigned tiles after dispatching them to mesh objects! ({string.Join(", ", unassignedTiles.Select(f => "" + f.m_CoordX + ";" + f.m_CoordY))})");

            callback?.Invoke();
        }



        int m_TryAddIslandPieceCount = 0;
        void TryAddIslandPieceMesh(int coordX, int coordY, List<(int, int)> meshObjectCoordinates, int radius, int maxRadius)
        {
            if (Mathf.Abs(coordX) > maxRadius * radius || Mathf.Abs(coordY) > maxRadius * radius || Mathf.Abs(coordX + coordY) > maxRadius * radius)
                return;

            if (meshObjectCoordinates.Contains((coordX, coordY)))
                return;

            meshObjectCoordinates.Add((coordX, coordY));
            m_TryAddIslandPieceCount++;

            TryAddIslandPieceMesh(coordX - ((1 * radius)), coordY + ((2 * radius) - 1), meshObjectCoordinates, radius, maxRadius); // top
            TryAddIslandPieceMesh(coordX - ((2 * radius) - 1), coordY + ((1 * radius) - 1), meshObjectCoordinates, radius, maxRadius); // top left
            TryAddIslandPieceMesh(coordX - ((1 * radius) - 1), coordY - ((1 * radius)), meshObjectCoordinates, radius, maxRadius); // bot left
            TryAddIslandPieceMesh(coordX + ((1 * radius)), coordY - ((2 * radius) - 1), meshObjectCoordinates, radius, maxRadius); // bot
            TryAddIslandPieceMesh(coordX + ((2 * radius) - 1), coordY - ((1 * radius) - 1), meshObjectCoordinates, radius, maxRadius); // bot right
            TryAddIslandPieceMesh(coordX + ((1 * radius) - 1), coordY + ((1 * radius)), meshObjectCoordinates, radius, maxRadius); // top right
        }

        void GenerateIslandPiece((int, int) islandPieceCoord, IslandGeneratorParameters parameters)
        {
            GameObject tileContainerObject = new GameObject($"tileContainer_{islandPieceCoord.Item1}_{islandPieceCoord.Item2}");
            tileContainerObject.transform.SetParent(transform, false);

            int coordX = islandPieceCoord.Item1;
            int coordY = islandPieceCoord.Item2;
            int coordTileX = coordX;
            int coordTileY = coordY;
            var defaultBaseTile = Tiles.FirstOrDefault(f => f.m_CoordX == coordTileX && f.m_CoordY == coordTileY);

            if (defaultBaseTile == null)
            {
                Debug.LogError($"Cannot find base tile for island piece at coord {islandPieceCoord}. Skipping this piece.");
                return;
            }
            tileContainerObject.transform.localPosition = defaultBaseTile.WorldPos3D;

            TileContainer tileContainer = tileContainerObject.AddComponent<TileContainer>();
            tileContainer.Init(parameters.GrassMaterial);

            TileContainers.Add(tileContainer);
            tileContainer.SetupOriginTile(defaultBaseTile);
        }

        void DispatchTilesInIslandPieces(ref List<Tile> unassignedTiles, TileContainer container, int meshRadius)
        {
            int baseCoordX = container.OriginTile.m_CoordX;
            int baseCoordY = container.OriginTile.m_CoordY;
            int absCoord = Mathf.Abs(baseCoordX + baseCoordY);

            Tile[] tilesToTransfer = unassignedTiles.Where(tile =>
                tile.m_CoordX > baseCoordX - meshRadius && tile.m_CoordX < baseCoordX + meshRadius &&
                tile.m_CoordY > baseCoordY - meshRadius && tile.m_CoordY < baseCoordY + meshRadius &&
                Mathf.Abs((tile.m_CoordX - baseCoordX) + (tile.m_CoordY - baseCoordY)) < meshRadius
                ).ToArray();

            unassignedTiles.RemoveAll(t => tilesToTransfer.Contains(t));
            container.SetupTiles(tilesToTransfer);
        }


        #endregion Generation

        #region Relief

        public override IEnumerator RefreshRelief(IslandGeneratorParameters parameters, Action callback)
        {
            RefreshTilesHeight(parameters);

            // debug infos
            float initTime = Time.time;
            // speed control
            int count = 0;
            int tempCount = 0;

            foreach (var tileContainer in TileContainers)
            {
                tileContainer.RefreshMesh();
#if DEBUG_ISLANDTERRAIN_TILECONTAINER
                Debug.Log($"> Refreshed tile container {tileContainer.OriginTile.m_CoordX} {tileContainer.OriginTile.m_CoordY}");
#endif
                count++;
                if (count > (tempCount + parameters.ReliefedTileContainersPerFrame))
                {
                    yield return null;
                    tempCount = count;
                }
            }

            callback?.Invoke();
        }

        #endregion Relief
    }
}
