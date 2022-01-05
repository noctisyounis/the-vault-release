using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "GameObject Receptor")]
    public class UniverseGameObjectSignalReceptor : ReceptorBase<GameObject, GameObjectSignal, GameObjectUnityEvent> { }
}