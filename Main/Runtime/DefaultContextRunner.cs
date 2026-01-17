using UnityEngine;

namespace ppl.ContextSystem
{
    public class DefaultContextRunner : MonoBehaviour
    {
        private void Update()
        {
            ContextManager.Tick();
        }
    }
}