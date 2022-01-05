using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "ULong Receptor")]
    public class ULongReceptor : ReceptorBase<ulong, ULongSignal, ULongUnityEvent> { }
}