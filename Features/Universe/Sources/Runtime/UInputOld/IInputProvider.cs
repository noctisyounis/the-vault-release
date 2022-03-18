namespace Universe
{
    public abstract class IInputProvider : UBehaviour
    {
        public abstract float UGetAxis(string axis);

        public abstract bool UGetKey(UKeyCode keyCode);

        public abstract bool UGetKeyDown(UKeyCode keyCode);

        public abstract bool UGetKeyUp(UKeyCode keyCode);
    }
}