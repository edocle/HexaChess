
using edocle.tools;
using UnityEngine;

namespace hexaChess.worldGen
{
    [CreateAssetMenu(fileName = "IslandGeneratorParameters", menuName = "hexaChess.worldGen/IslandGeneratorParameters")]
    public class IslandGeneratorParameters : ScriptableObject
    {

        [SerializeField] private int m_MapRadius = 64;
        public int MapRadius => m_MapRadius;

        [SerializeField] private IslandGeneratorMode m_GeneratorMode;
        public IslandGeneratorMode GeneratorMode => m_GeneratorMode;


        [SerializeField] private float m_TileRadius = 1f;
        public float TileRadius => m_TileRadius;

        [Space(4)]
        /// <summary>
        /// Generate one object per tile
        /// 10 fps for a 128 tiles radius island (~49.000 hexagons)
        /// 52.000 drawcalls ??
        /// </summary>
        #region Tiles

        [SerializeField] private GameObject m_Tile = null;
        public GameObject Tile => m_Tile;
        [SerializeField] private bool m_TileNeedMesh = false;
        public bool TileNeedMesh => m_TileNeedMesh;

        [SerializeField] private int m_GeneratedTilesPerFrame = 100;
        public int GeneratedTilesPerFrame => m_GeneratedTilesPerFrame;

        [SerializeField] private int m_ReliefedTilesPerFrame = 100;
        public int ReliefedTilesPerFrame => m_ReliefedTilesPerFrame;

        [SerializeField] private int m_LinkedTilesPerFrame = 1000;
        public int LinkedTilesPerFrame => m_LinkedTilesPerFrame;

        #endregion Tiles

        [Space(4)]
        /// <summary>
        /// One big object is generated & the island is entirely generated in it.
        /// </summary>
        #region BigObject

        [SerializeField] private int m_BigObjectMultipleMeshRadius = 8;
        public int IslandPiecesRadius => m_BigObjectMultipleMeshRadius;

        [SerializeField] private int m_GeneratedTileContainersPerFrame = 100;
        public int GeneratedTileContainersPerFrame => m_GeneratedTileContainersPerFrame;

        [SerializeField] private int m_TopographedTileContainersPerFrame = 100;
        public int ReliefedTileContainersPerFrame => m_TopographedTileContainersPerFrame;

        #endregion BigObject

        [Space(4)]
        #region Batches

        [SerializeField] private int m_TileBatchRadius = 8;
        public int TileChunkRadius => m_TileBatchRadius;

        [SerializeField] private int m_TopographedBatchesPerFrame = 1000;
        public int ReliefedChunksPerFrame => m_TopographedBatchesPerFrame;

        [SerializeField] private int m_LinkedBatchedTilesPerFrame = 1000;
        public int LinkedChunkedTilesPerFrame => m_LinkedBatchedTilesPerFrame;

        #endregion Batches

        [Space(4)]
        /// <summary>
        /// Use DOTS to generate one object per tile
        /// 
        /// </summary>
        #region DOT tiles

        #endregion DOT tiles

        [Space(4)]
        /// <summary>
        /// Handle Relief of the island (height per position)
        /// 
        /// </summary>
        #region Relief

        [SerializeField] private Maths.PerlinNoiseParameters m_LargePerlinNoiseParameters;
        public Maths.PerlinNoiseParameters LargePerlinNoiseParameters => m_LargePerlinNoiseParameters;

        [SerializeField] private Maths.PerlinNoiseParameters m_SmallPerlinNoiseParameters;
        public Maths.PerlinNoiseParameters SmallPerlinNoiseParameters => m_SmallPerlinNoiseParameters;

        [SerializeField] private AnimationCurve m_IslandAnimationMinRelief;
        public AnimationCurve IslandAnimationMinRelief => m_IslandAnimationMinRelief;

        [SerializeField] private AnimationCurve m_IslandAnimationMaxRelief;
        public AnimationCurve IslandAnimationMaxRelief => m_IslandAnimationMaxRelief;

        [SerializeField] private AnimationCurve m_IslandGlobalRelief;
        public AnimationCurve IslandGlobalRelief => m_IslandGlobalRelief;

        [SerializeField] private float m_IslandGlobalReliefIntensity;
        public float IslandGlobalReliefIntensity => m_IslandGlobalReliefIntensity;

        #endregion Relief

        [Space(4)]
        /// <summary>
        /// Handle repartition of materials in the island
        /// 
        /// </summary>
        #region Materials

        [SerializeField] private float m_BeachMaxHeight;
        public float BeachMaxHeight => m_BeachMaxHeight;

        [SerializeField] private float m_BeachMinHeight;
        public float BeachMinHeight => m_BeachMinHeight;

        [SerializeField] private Material m_GrassMaterial;
        public Material GrassMaterial => m_GrassMaterial;

        [SerializeField] private Material m_BeachMaterial;
        public  Material BeachMaterial => m_BeachMaterial;

        [SerializeField] private Material m_SeaMaterial;
        public Material SeaMaterial => m_SeaMaterial;

        #endregion Materials

    }

    public enum IslandGeneratorMode
    {
        Tiles, // generate one object per tile
        OneTileContainerObject, // generate one object then mesh every tiles in it
        IslandPieces, // generate multiple objects each containing multiple tiles
        Batches, // generate one object per batch then generate tiles in it
        DotsTiles, // generate one object per tile using DOTS
    }
}