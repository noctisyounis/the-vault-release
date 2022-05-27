using UnityEngine;

namespace Universe
{
    [AddComponentMenu(UniverseUtility.RECEPTOR_SUBMENU + "Byte Receptor")]
    public class ByteReceptor : ReceptorBase<byte, ByteSignal, ByteUnityEvent> { }
}