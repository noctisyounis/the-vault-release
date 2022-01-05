using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class AnimationCurveReference : ReferenceBase<AnimationCurve, AnimationCurveFact>
    {
        public AnimationCurveReference() : base() { }
        public AnimationCurveReference(AnimationCurve value) : base(value) { }
    }
}