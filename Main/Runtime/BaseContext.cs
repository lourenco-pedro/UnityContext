using System.Collections.Generic;
namespace ppl.ContextSystem
{
    public abstract class BaseContext : IContext
    {
        private IContextData _contextData = null;

        public abstract void Start(ContextArgs args);
        public abstract void Update(ContextArgs args);
        public abstract void Dispose();

        public void RegisterContextData(IContextData contextData)
        {
            _contextData = contextData;
        }
        
        public IContextData GetContextData()
        {
            return _contextData;
        }
    }
}