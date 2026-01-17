using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ppl.ContextSystem
{
    public static class ContextManager
    {
        private static IDisposable _disposableEveryUpdate;
        
        private static List<IContext> _stackedContexts = new List<IContext>();
        
        public static string CurrentContext => _stackedContexts.Count == 0 ? string.Empty : _stackedContexts.Last().GetType().Name;
        public static bool Initialized => _disposableEveryUpdate != null;

        private static void ForeachUpdateOfStackedContexts()
        {
            Dictionary<string, IContextData> accumulatedContextDatas = new Dictionary<string, IContextData>();
            
            foreach (var ctx in _stackedContexts)
            {
                try
                {
                    IContextData data = ctx.GetContextData();
                    if (data != null)
                        accumulatedContextDatas[data.GetType().Name] = data;
                }
                catch(Exception e)
                {
                    Debug.LogError("Could not spread context data. Error below");
                    Debug.LogException(e);
                }
                
                ctx.Update(new ContextArgs(accumulatedContextDatas));
            }
        }
        
        public static void Tick()
        {
            ForeachUpdateOfStackedContexts();
        }

        public static void Terminate()
        {
            _disposableEveryUpdate?.Dispose();
        }
        
        public static void SwitchContext<TContextModel>(IContextData contextData = null)
        where TContextModel : IContext, new()
        {
            PopContext();
            PushContext<TContextModel>(contextData);
        }

        public static void PushContext<TContextModel>(IContextData contextData = null)
        where TContextModel : IContext, new()
        {
            try
            {
                Dictionary<string, IContextData> accumulatedContextDatas = new Dictionary<string, IContextData>();
                foreach (var ctx in _stackedContexts)
                {
                    IContextData data = ctx.GetContextData();
                    if (data != null)
                        accumulatedContextDatas[data.GetType().Name] = data;
                }
                
                TContextModel context = new TContextModel();
                _stackedContexts.Add(context);

                if (null != contextData)
                {
                    context.RegisterContextData(contextData);
                    accumulatedContextDatas.Add(contextData.GetType().Name, contextData);
                }

                context.Start(new ContextArgs(accumulatedContextDatas));
            }
            catch(Exception e)
            {
                Debug.LogError("Could not spread context data. Error below");
                Debug.LogException(e);
            }
            
        }

        public static void PopContext()
        {
            if(_stackedContexts.Count == 0)
                return;
            
            _stackedContexts.Last().Dispose();
            _stackedContexts.RemoveAt(_stackedContexts.Count - 1);
        }
    }
}