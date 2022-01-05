using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Object Receptor")]
    public class ObjectReceptor : ReceptorBase<Object, ObjectSignal, ObjectUnityEvent> { }
}