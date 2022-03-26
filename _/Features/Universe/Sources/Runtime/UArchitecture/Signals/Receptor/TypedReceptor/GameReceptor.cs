using UnityEngine;
using UnityEngine.Events;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Signal Receptor")]
    [ExecuteInEditMode]
    public class GameReceptor : ReceptorBase<SignalBase, UnityEvent> {}
}