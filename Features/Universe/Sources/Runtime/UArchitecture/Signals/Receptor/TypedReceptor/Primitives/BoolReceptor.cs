using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Bool Receptor")]
    public class BoolReceptor : ReceptorBase<bool, BoolSignal, BoolUnityEvent> { }
}