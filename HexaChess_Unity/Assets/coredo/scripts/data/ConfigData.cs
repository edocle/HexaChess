
using System;
using UnityEngine;

namespace edocle.core
{
    public class ConfigDataHandler<T> : ConfigDataHandler where T : ConfigData
    {
        protected T m_Data;

        public ConfigDataHandler(T data)
        {
            m_Data = data;
        }
    }

    public class ConfigDataHandler : ScriptableObject
    {

    }

    public abstract class SystemConfigData : ConfigData
    {
        // add any specific system data
        public int m_MaxSaves = 3;
    }

    public abstract class GameConfigData : ConfigData
    {
        // add any specific game data
    }

    [Serializable]
    public class ConfigData
    {

    }
}