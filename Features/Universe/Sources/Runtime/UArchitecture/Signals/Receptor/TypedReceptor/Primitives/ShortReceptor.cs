using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Short Receptor")]
    public class ShortReceptor : ReceptorBase<short, ShortSignal, ShortUnityEvent> { }
}