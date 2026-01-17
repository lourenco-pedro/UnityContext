namespace ppl.ContextSystem
{
    public interface IContext
    {
        void RegisterContexData(IContextData data);
        IContextData GetContextData();
        void Start(ContextArgs args);
        void Update(ContextArgs args);
        void Dispose();
    }
}