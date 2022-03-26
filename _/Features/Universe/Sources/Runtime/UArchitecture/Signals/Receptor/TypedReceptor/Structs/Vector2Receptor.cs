using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Vector2 Receptor")]
    public class Vector2Receptor : ReceptorBase<Vector2, Vector2Signal, Vector2UnityEvent> { }
}