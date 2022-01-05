using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Vector4 Receptor")]
    public class Vector4Receptor : ReceptorBase<Vector4, Vector4Signal, Vector4UnityEvent> { }
}