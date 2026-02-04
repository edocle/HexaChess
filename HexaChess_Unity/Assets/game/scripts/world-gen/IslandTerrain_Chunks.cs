
//#define DEBUG_ISLANDTERRAIN_CHUNKS
using hexaChess.worldGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandTerrain_Chunks : IslandTerrain
{
    Chunk[] m_TileChunks = null;
    List<ChunkGameObject> m_TileChunkObjects = null;

    public IslandTerrain_Chunks() : base()
    {

    }

    // New way of generating terrain: (proper way)
    // 1°) generate chunks
    // 2°) in each chunk, generate tiles
    // 3°) link tiles in each chunk
    // 4°) Invoke an object per chunk

    #region Generation

    /// <summary>
    /// Generate all the data of chunks and tiles needed for the terrain generation
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public override IEnumerator GenerateData(IslandGeneratorParameters parameters, Action callback)
    {
#if DEBUG_ISLANDTERRAIN_CHUNKS
        Debug.Log($"Island terrain> Chunks> starting generation...");
#endif
        yield return null;

        // Generate the chunks
        // Recursive function that generate neighbours the neighbours of neighbours etc..
        List<Chunk> tileChunks = new List<Chunk>();
        TryGenerateTileChunk(null, 0, 0, parameters, tileChunks);

        m_TileChunks = tileChunks.ToArray();

#if DEBUG_ISLANDTERRAIN_CHUNKS
        Debug.Log($"Island terrain> Chunks> Chunks generated: {m_TileChunks.Length}");
#endif
        yield return null;

        // Generate tiles per chunks
        foreach (Chunk tileChunk in m_TileChunks)
            tileChunk.GenerateTiles();

#if DEBUG_ISLANDTERRAIN_CHUNKS
        Debug.Log($"Island terrain> Chunks> Tiles generated: {m_TileChunks[0].GetTilesCount()} per chunk");
#endif
        yield return null;

        int count = 0;
        int tempCount = 0;

        // Link tile neighbours in each chunk
        foreach (Chunk tileChunk in m_TileChunks)
        {
            tileChunk.LinkTiles(parameters);

            count++;
            if (count > (tempCount + parameters.LinkedChunkedTilesPerFrame))
            {
                yield return null;
                tempCount = count;
            }
        }

#if DEBUG_ISLANDTERRAIN_CHUNKS
        Debug.Log($"Island terrain> Chunks> Tiles linked !");
#endif
        yield return null;

        callback?.Invoke();
    }

    /// <summary>
    /// Recursive methods that will try to generate neighbours until limit is reached
    /// </summary>
    /// <param name="coordX"></param>
    /// <param name="coordY"></param>
    /// <param name="parameters"></param>
    /// <param name="m_TileChunks"></param>
    void TryGenerateTileChunk(Chunk previousChunk, int coordX, int coordY, IslandGeneratorParameters parameters, List<Chunk> m_TileChunks)
    {
        int maxRadius = parameters.MapRadius;
        if (Mathf.Abs(coordX) > maxRadius || Mathf.Abs(coordY) > maxRadius || Mathf.Abs(coordX + coordY) > maxRadius)
            return;

        Chunk tileChunk = m_TileChunks.FirstOrDefault(f => f.HasSameCoordinates(coordX, coordY));
        if (tileChunk != null)
        {
            tileChunk.TryAddNeighbour(previousChunk);
            return;
        }

        int radius = parameters.TileChunkRadius;
        tileChunk = new Chunk(coordX, coordY, radius, parameters.TileRadius);
        tileChunk.TryAddNeighbour(previousChunk);
        m_TileChunks.Add(tileChunk);

        // Generate neighbour chunks by recursivity
        // On some neighbour tiles, there is an offset (-1) to avoid overlapping or missing meshes (trust me bro)
        (int, int)[] neighbourCoords = new (int, int)[]
        {
                (coordX - (1 * radius), coordY + ((2 * radius) - 1)), // top
                (coordX - ((2 * radius) - 1), coordY + ((1 * radius) - 1)), // top left
                (coordX - ((1 * radius) - 1), coordY - ((1 * radius))), // bot left
                (coordX + ((1 * radius)), coordY - ((2 * radius) - 1)), // bot
                (coordX + ((2 * radius) - 1), coordY - ((1 * radius) - 1)), // bot right
                (coordX + ((1 * radius) - 1), coordY + ((1 * radius))) // top right
        };

        foreach (var neighbourCoord in neighbourCoords)
        {
            TryGenerateTileChunk(tileChunk, neighbourCoord.Item1, neighbourCoord.Item2, parameters, m_TileChunks);
        }
    }

    /// <summary>
    /// Use chunks data to invoke objects
    /// May be improved by only invoking visible objects
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public override IEnumerator GenerateIsland(IslandGeneratorParameters parameters, Action callback)
    {
        // Invoke tile chunk game objects
        m_TileChunkObjects = new List<ChunkGameObject>();
        foreach (Chunk tileChunk in m_TileChunks)
        {
            ChunkGameObject tileChunkGameObject = new GameObject($"Chunk_{tileChunk.m_CoordX}_{tileChunk.m_CoordY}")
                                                        .AddComponent<ChunkGameObject>();
            tileChunkGameObject.Init(parameters.GrassMaterial, tileChunk);

            tileChunkGameObject.transform.SetParent(transform, false);
            tileChunkGameObject.transform.localPosition = tileChunk.GetOriginTile().WorldPos3D;
            m_TileChunkObjects.Add(tileChunkGameObject);
        }

#if DEBUG_ISLANDTERRAIN_CHUNKS
        Debug.Log($"Island terrain> Chunks> Chunk game objects generated !");
#endif
        yield return null;

        callback?.Invoke();
    }

    #endregion Generation

    #region Relief

    /// <summary>
    /// Refresh relief of the island terrain using new topography
    /// May be improved by refreshing only tiles in visible chunks
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public override IEnumerator RefreshRelief(IslandGeneratorParameters parameters, Action callback)
    {

        foreach (Chunk tileChunk in m_TileChunks)
        {
            foreach (Tile tile in tileChunk.GetTiles())
            {
                tile.RefreshHeight(GenerateTileHeight(tile.m_CoordPos.x, tile.m_CoordPos.y, tile.m_CoordPosDistanceToOrigin, parameters));
            }
        }

        yield return StartCoroutine(RefreshChunksMeshRelief(parameters));
    }

    /// <summary>
    /// Refresh mesh of every chunks
    /// May be improved by refreshing only visible chunks
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    IEnumerator RefreshChunksMeshRelief(IslandGeneratorParameters parameters)
    {
        int count = 0;
        int tempCount = 0;
        foreach (ChunkGameObject tileChunkh in m_TileChunkObjects)
        {
            tileChunkh.RefreshMesh();

            count++;
            if (count > (tempCount + parameters.ReliefedChunksPerFrame))
            {
                yield return null;
                tempCount = count;
            }
        }

#if DEBUG_ISLANDTERRAIN_CHUNKS
        Debug.Log($"Island terrain> Chunks> All chunk meshes done !");
#endif
    }

    #endregion Relief
}
