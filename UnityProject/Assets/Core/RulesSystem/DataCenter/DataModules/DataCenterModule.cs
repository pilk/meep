namespace DataCenter
{
    public abstract class DataCenterModule<T> where T : class
    {
        public delegate void DataLog(string text);
        public DataLog LogError = null;
        public DataLog Log = null;
    };
}