
//#define DEBUG_ISLAND_GENERATOR
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace hexaChess.worldGen
{
    public class IslandGenerator : MonoBehaviour
    {
        private void LateUpdate()
        {
            UpdateDebug();
        }

        #region Generate tiles

        [SerializeField] private List<IslandGeneratorParameterId> m_Parameters;
        [SerializeField] private string m_ActiveParameterId;
        IslandGeneratorParameters ActiveParameter => m_Parameters.Find(f => f.Id == m_ActiveParameterId).Parameter;

        [SerializeField] private Transform m_GroundContent;

        private IslandTerrain m_IslandTerrain;

        public void SetupActiveParameter(string activeParameterId)
        {
            m_ActiveParameterId = activeParameterId;

            TryDebugIsland();
        }

        public void GenerateTerrain()
        {
            GenerateTerrain(null);
        }

        public void GenerateTerrain(Action callback)
        {
            if (m_IslandTerrain != null)
                m_IslandTerrain.Destroy();

            GameObject islandTerrain = new GameObject("IslandTerrain");
            islandTerrain.transform.SetParent(m_GroundContent, false);
            islandTerrain.transform.localPosition = Vector3.zero;

            StartCoroutine(LateGenerateTerrain(islandTerrain, callback));
        }

        IEnumerator LateGenerateTerrain(GameObject islandTerrain, Action callback)
        {
            m_IslandGenerationStartDate = DateTime.Now;
            m_IslandGenerationInProgress = true;
            yield return null;

            switch (ActiveParameter.GeneratorMode)
            {
                case IslandGeneratorMode.Batches:
                    m_IslandTerrain = islandTerrain.AddComponent<IslandTerrain_Chunks>();
                    break;
                case IslandGeneratorMode.Tiles:
                    m_IslandTerrain = islandTerrain.AddComponent<IslandTerrain_InvokedTiles>();
                    break;
                case IslandGeneratorMode.OneTileContainerObject:
                case IslandGeneratorMode.IslandPieces:
                    m_IslandTerrain = islandTerrain.AddComponent<IslandTerrain_TileContainers>();
                    break;
                case IslandGeneratorMode.DotsTiles:
                    // not done yet
                    break;
            }

            // Generate all tiles data
            yield return StartCoroutine(m_IslandTerrain.GenerateData(ActiveParameter, null));
#if DEBUG_ISLAND_GENERATOR
            Debug.Log($"<color=red>IslandGenerator></color> Generated data");
#endif

            // Invoke island objects
            yield return StartCoroutine(m_IslandTerrain.GenerateIsland(ActiveParameter, callback));
#if DEBUG_ISLAND_GENERATOR
            Debug.Log($"<color=red>IslandGenerator></color> Generated island");
#endif
            yield return null;
            m_IslandGenerationInProgress = false;

            // Initialize Relief
            RefreshIslandRelief();
        }

        #endregion Generate tiles

        #region Generate height

        public void RefreshIslandRelief()
        {
#if DEBUG_ISLAND_GENERATOR
            Debug.Log($"<color=red>IslandGenerator></color> Start refresh island Relief");
#endif
            InitializeIslandRelief();

            StartCoroutine(LateRefreshIslandRelief());
        }

        IEnumerator LateRefreshIslandRelief()
        {
            m_ReliefGenerationStartDate = DateTime.Now;
            m_ReliefGenerationInProgress = true;
            yield return null;

            yield return StartCoroutine(m_IslandTerrain.RefreshRelief(ActiveParameter, null));

            yield return null;
            m_ReliefGenerationInProgress = false;
        }

        void InitializeIslandRelief()
        {
            ActiveParameter.LargePerlinNoiseParameters.SetRandomRange();
            ActiveParameter.SmallPerlinNoiseParameters.SetRandomRange();
        }

        #endregion Generate height


        #region Debug
        [Space(4)]
        [SerializeField] private TextMeshProUGUI m_IslandGenerationParameterText;
        [SerializeField] private TextMeshProUGUI m_IslandGenerationTimerText;
        [SerializeField] private TextMeshProUGUI m_ReliefGenerationTimerText;

        private DateTime m_IslandGenerationStartDate;
        bool m_IslandGenerationInProgress = false;
        private DateTime m_ReliefGenerationStartDate;
        bool m_ReliefGenerationInProgress = false;

        void UpdateDebug()
        {
            if (m_IslandGenerationInProgress && m_IslandGenerationTimerText != null)
            {
                TimeSpan span = DateTime.Now - m_IslandGenerationStartDate;
                m_IslandGenerationTimerText.text = $"{span.Seconds.ToString("00")}:{((int)((float)(span.Milliseconds) / 10f)).ToString("00")}";
            }
            if (m_ReliefGenerationInProgress && m_IslandGenerationTimerText != null)
            {
                TimeSpan span = DateTime.Now - m_ReliefGenerationStartDate;
                m_ReliefGenerationTimerText.text = $"{span.Seconds.ToString("00")}:{((int)((float)(span.Milliseconds) / 10f)).ToString("00")}";
            }
        }

        void TryDebugIsland()
        {
            if (m_IslandGenerationParameterText != null)
                m_IslandGenerationParameterText.text = m_ActiveParameterId;

            if (m_IslandGenerationParameterText != null)
                m_IslandGenerationTimerText.text = "00:00";
            if (m_IslandGenerationParameterText != null)
                m_ReliefGenerationTimerText.text = "00:00";
        }

        #endregion Debug
    }

    [System.Serializable]
    public class IslandGeneratorParameterId
    {
        [SerializeField] string m_Id;
        [SerializeField] IslandGeneratorParameters m_Parameter;

        public string Id => m_Id;
        public IslandGeneratorParameters Parameter => m_Parameter;
    }

}