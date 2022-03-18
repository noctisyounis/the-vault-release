using UnityEngine;

namespace Universe
{
    public static class UKeyCodeExtensions
    {
        public static KeyCode GetUnityLegacyKeyCode(this UKeyCode keyCode)
        {
            var uKeyIndex = (int)keyCode;
            return (KeyCode)uKeyIndex;
        }
    }
}