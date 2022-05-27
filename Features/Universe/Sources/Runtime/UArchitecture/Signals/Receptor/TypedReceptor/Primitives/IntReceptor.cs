using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Int Receptor")]
    public class IntReceptor : ReceptorBase<int, IntSignal, IntUnityEvent> { }
}