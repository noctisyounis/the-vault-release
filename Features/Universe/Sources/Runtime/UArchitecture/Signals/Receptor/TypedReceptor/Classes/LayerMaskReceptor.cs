using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "LayerMask Receptor")]
    public class LayerMaskReceptor : ReceptorBase<LayerMask, LayerMaskSignal, LayerMaskUnityEvent> { }
}