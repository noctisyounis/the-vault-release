using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Sbyte Receptor")]
    public class SByteReceptor : ReceptorBase<sbyte, SByteSignal, SByteUnityEvent> { }
}