using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Quaternion Receptor")]
    public class QuaternionReceptor : ReceptorBase<Quaternion, QuaternionSignal, QuaternionUnityEvent> { }
}