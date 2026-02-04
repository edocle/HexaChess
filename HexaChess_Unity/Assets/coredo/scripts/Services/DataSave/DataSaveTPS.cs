

namespace edocle.core
{
    public class DataSaveTPS : ThirdPartyService<DataSaveTPA>
    {
        #region LifeCycle

        DataSaveActor_PlayerPrefs m_MainActor;

        public DataSaveTPS(ThirdPartyServiceContext context): base(context)
        {
            m_MainActor = Get<DataSaveActor_PlayerPrefs>();
        }

        public override void Kill()
        {

        }

        #endregion LifeCycle

        #region Calls

        public void SaveData()
        {
            m_MainActor.SaveData();
        }

        public void RecoverData()
        {
            m_MainActor.RecoverData();
        }

        public void ClearData()
        {
            m_MainActor.ClearData();
        }

        #endregion Calls
    }
}
