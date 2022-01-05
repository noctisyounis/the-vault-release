using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Vector3 Receptor")]
    public class Vector3Receptor : ReceptorBase<Vector3, Vector3Signal, Vector3UnityEvent> { }
}