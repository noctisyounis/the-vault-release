using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Float Receptor")]
    public class FloatReceptor : ReceptorBase<float, FloatSignal, FloatUnityEvent> { }
}