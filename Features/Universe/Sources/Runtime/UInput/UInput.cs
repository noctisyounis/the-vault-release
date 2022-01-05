namespace Universe
{
    public static class UInput
    {
        public static float UGetAxis(this UBehaviour source, string axis)
        {
            return U3rdPartyWrapper.GetInputProvider().UGetAxis(axis);
        }

        public static bool UGetKeyDown(this UBehaviour source, UKeyCode keycode)
        {
            return U3rdPartyWrapper.GetInputProvider().UGetKeyDown(keycode);
        }

        public static bool UGetKey(this UBehaviour source, UKeyCode keycode)
        {
            return U3rdPartyWrapper.GetInputProvider().UGetKey(keycode);
        }

        public static bool UGetKeyUp(this UBehaviour source, UKeyCode keycode)
        {
            return U3rdPartyWrapper.GetInputProvider().UGetKeyUp(keycode);
        }
    }
}
