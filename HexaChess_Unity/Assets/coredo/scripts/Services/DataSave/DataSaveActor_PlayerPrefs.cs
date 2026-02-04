
using UnityEngine;

namespace edocle.core
{
    public class DataSaveActor_PlayerPrefs : DataSaveTPA
    {
        #region LifeCycle

        public DataSaveActor_PlayerPrefs(ThirdPartyServiceContext context) : base(context)
        {

        }

        #endregion LifeCycle

        #region Calls

        public override void SaveData()
        {
            Debug.Log($"> Save data");
        }

        public override void RecoverData()
        {
            Debug.Log($"> Recover data");
        }

        public override void ClearData()
        {
            Debug.Log($"> Clear data");
        }

        #endregion Calls
    }
}

// PlayerPrefs.DeleteKey(key);
// PlayerPrefs.SetString(key, data.ToString());
// PlayerPrefs.Save();
// PlayerPrefs.GetString(key);