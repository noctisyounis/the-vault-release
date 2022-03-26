using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Char Receptor")]
    public class CharReceptor : ReceptorBase<char, CharSignal, CharUnityEvent> { }
}