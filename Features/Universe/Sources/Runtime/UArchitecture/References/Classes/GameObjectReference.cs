using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class GameObjectReference : ReferenceBase<GameObject, GameObjectFact>
    {
        public GameObjectReference() : base() { }
        public GameObjectReference(GameObject value) : base(value) { }
    }
}