using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Double Receptor")]
    public class DoubleReceptor : ReceptorBase<double, DoubleSignal, DoubleUnityEvent> { }
}