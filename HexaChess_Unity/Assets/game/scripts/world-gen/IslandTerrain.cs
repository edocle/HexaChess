
using System;
using System.Collections;
using UnityEngine;

namespace hexaChess.worldGen
{
    public abstract class IslandTerrain : MonoBehaviour
    {
        public IslandTerrain()
        {

        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        #region Generation

        public abstract IEnumerator GenerateData(IslandGeneratorParameters parameters, Action callback);

        public abstract IEnumerator GenerateIsland(IslandGeneratorParameters parameters, Action callback);

        #endregion Generation

        #region Relief

        public abstract IEnumerator RefreshRelief(IslandGeneratorParameters parameters, Action callback);

        protected float GenerateTileHeight(float coordPosX, float coordPosY, float coordDistance, IslandGeneratorParameters parameters)
        {
            int mapRadius = parameters.MapRadius;
            // noise coordinate x
            // from [-64;64] to [0;1]
            float noiseCoordinateX = (float)(coordPosX + mapRadius) / (mapRadius * 2);
            float noiseCoordinateY = (float)(coordPosY + mapRadius) / (mapRadius * 2);

            // ad offset and range
            float noiseX = (parameters.LargePerlinNoiseParameters.m_RandomOffsetX + noiseCoordinateX) * parameters.LargePerlinNoiseParameters.m_NoiseRange;
            float noiseY = (parameters.LargePerlinNoiseParameters.m_RandomOffsetY + noiseCoordinateY) * parameters.LargePerlinNoiseParameters.m_NoiseRange;

            float perlinNoiseHeight = Mathf.PerlinNoise(noiseX, noiseY);

            // Add it with global Relief
            float distanceToCenter = coordDistance / mapRadius;
            float ReliefHeight = parameters.IslandGlobalRelief.Evaluate(distanceToCenter);

            return ((perlinNoiseHeight - 0.5f) * parameters.LargePerlinNoiseParameters.m_Intensity) + (ReliefHeight * parameters.IslandGlobalReliefIntensity);
        }

        #endregion Relief

        #region Callback

        public abstract Tile GetTile(int coordX, int coordY);

        #endregion Callback
    }
}
