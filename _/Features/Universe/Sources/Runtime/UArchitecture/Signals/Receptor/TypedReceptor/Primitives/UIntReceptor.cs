using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "UInt Receptor")]
    public class UIntReceptor : ReceptorBase<uint, UIntSignal, UIntUnityEvent> { }
}