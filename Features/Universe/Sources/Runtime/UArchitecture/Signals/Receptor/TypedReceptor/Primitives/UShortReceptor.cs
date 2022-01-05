using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "UShort Receptor")]
    public class UShortReceptor : ReceptorBase<ushort, UShortSignal, UShortUnityEvent> { }
}