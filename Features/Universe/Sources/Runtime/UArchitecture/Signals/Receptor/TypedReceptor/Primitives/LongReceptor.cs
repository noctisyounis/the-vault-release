using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Long Receptor")]
    public class LongReceptor : ReceptorBase<long, LongSignal, LongUnityEvent> { }
}