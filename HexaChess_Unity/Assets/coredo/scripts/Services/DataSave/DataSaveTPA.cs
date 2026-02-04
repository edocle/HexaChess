

namespace edocle.core
{
    public abstract class DataSaveTPA : ThirdPartyServiceActor
    {
        #region LifeCycle

        public DataSaveTPA(ThirdPartyServiceContext context) : base(context)
        {

        }

        public override void Init(AsyncProcessTask callback)
        {

        }

        public override void Kill()
        {

        }

        #endregion LifeCycle

        #region Calls

        public abstract void SaveData();

        public abstract void RecoverData();

        public abstract void ClearData();

        #endregion Calls
    }
}
