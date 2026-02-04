
using System.Collections.Generic;
using UnityEngine;

namespace edocle.core
{
    [CreateAssetMenu(fileName = "GameStarterParameters", menuName = "edocle/Game Starter")]
    public class GameStarterParameters : ScriptableObject
    {
        // Datas: Configs

        // Datas: Saves
        [SerializeField] private SystemSaveDataHandler m_SystemSaveDataHandler;
        public SystemSaveDataHandler SystemSaveDataHandler => m_SystemSaveDataHandler;

        [SerializeField] private List<GameSaveDataHandler> m_GameSaveDataHandlers;
        public List<GameSaveDataHandler> GameSaveDataHandlers => m_GameSaveDataHandlers;
    }
}