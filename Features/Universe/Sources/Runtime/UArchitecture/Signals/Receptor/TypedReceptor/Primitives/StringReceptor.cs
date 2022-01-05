using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "String Receptor")]
    public class StringReceptor : ReceptorBase<string, StringSignal, StringUnityEvent> { }
}