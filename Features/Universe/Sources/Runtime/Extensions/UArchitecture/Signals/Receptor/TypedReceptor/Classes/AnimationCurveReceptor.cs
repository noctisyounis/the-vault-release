using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "AnimationCurve Receptor")]
    public class AnimationCurveReceptor : GameReceptorGeneric<AnimationCurve, AnimationCurveSignal, AnimationCurveUnityEvent> { }
}