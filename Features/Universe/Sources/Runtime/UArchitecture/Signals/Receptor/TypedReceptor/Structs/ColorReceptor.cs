using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Color Receptor")]
    public class ColorReceptor : ReceptorBase<Color, ColorSignal, ColorUnityEvent> { }
}