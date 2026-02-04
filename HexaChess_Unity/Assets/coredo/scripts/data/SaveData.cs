
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace edocle.core
{
    public class SystemSaveDataHandler<T> : SystemSaveDataHandler where T : SystemSaveData
    {
        [SerializeField] protected T m_Data;

        public SystemSaveDataHandler(T data)
        {
            m_Data = data;
        }

        DataSaveTPS m_SaveService = null;

        bool m_Initialized = false;

        /// <summary>
        /// Registers the service used by the save data handler
        /// For now, only one save service per data handler (more than that may never be needed)
        /// </summary>
        /// <param name="service">data save service to register</param>
        public void Initialize(DataSaveTPS service)
        {
            if (m_Initialized)
                return;

            if (service == null)
                return;

            m_SaveService = service;
            m_Initialized = true;
        }
    }

    // allows to handle generic links: cannot do it with <T>
    public class SystemSaveDataHandler : SaveDataHandler
    { }

    public abstract class SystemSaveData : SaveData
    {
        // add any specific for system data
        public List<int> m_GameSlotIds;
        public int m_CurrentGameSlot;
    }

    //
    public class GameSaveDataHandler<T> : GameSaveDataHandler where T : GameSaveData
    {
        [SerializeField] protected T m_Data;

        public GameSaveDataHandler(T data)
        {
            m_Data = data;
        }

        DataSaveTPS m_SaveService = null;

        bool m_Initialized = false;

        /// <summary>
        /// Registers the service used by the save data handler
        /// For now, only one save service per data handler (more than that may never be needed)
        /// </summary>
        /// <param name="service">data save service to register</param>
        public void Initialize(DataSaveTPS service)
        {
            if (m_Initialized)
                return;

            if (service == null)
                return;

            m_SaveService = service;
            m_Initialized = true;
        }

        public void Save()
        {
            m_SaveService.SaveData();
        }

        public void Load()
        {
            m_SaveService.RecoverData();
        }
    }

    // allows to handle generic links: cannot do it with <T>
    public class GameSaveDataHandler : SaveDataHandler
    { }

    public abstract class GameSaveData : SaveData
    {
        // add any specific for game data
        public string m_Identifier;
        public bool m_Initialized = false;
    }


    //

    // Handler for any data that we want to save & load
    // used to manipulate datas
    public abstract class SaveDataHandler : ScriptableObject
    {

    }

    // Any data that we want to save & load
    public abstract class SaveData
    {

    }
}