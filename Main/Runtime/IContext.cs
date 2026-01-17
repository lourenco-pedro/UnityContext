namespace ppl.ContextSystem
{
    public interface IContext
    {
        void RegisterContextData(IContextData data);
        IContextData GetContextData();
        void Start(ContextArgs args);
        void Update(ContextArgs args);
        void Dispose();
    }
}