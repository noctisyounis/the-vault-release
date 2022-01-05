using UnityEngine;

namespace Universe
{
    public class InputProviderUnityLegacy : IInputProvider
    {
        public override float UGetAxis(string axis) 
            => Input.GetAxis(axis); 
        
        public override bool UGetKey(UKeyCode keyCode) 
            => Input.GetKey(keyCode.GetUnityLegacyKeyCode());
        
        public override bool UGetKeyDown(UKeyCode keyCode) 
            => Input.GetKeyDown(keyCode.GetUnityLegacyKeyCode()); 
        
        public override bool UGetKeyUp(UKeyCode keyCode)
            => Input.GetKeyUp(keyCode.GetUnityLegacyKeyCode());
    }
}