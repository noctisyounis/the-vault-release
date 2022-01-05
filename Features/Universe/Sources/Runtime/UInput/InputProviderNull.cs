namespace Universe
{
    public class InputProviderNull : IInputProvider
    {
        public override float UGetAxis(string axis)
        {
            throw new System.NotImplementedException();
        }

        public override bool UGetKey(UKeyCode keyCode)
        {
            throw new System.NotImplementedException();
        }

        public override bool UGetKeyDown(UKeyCode keyCode)
        {
            throw new System.NotImplementedException();
        }

        public override bool UGetKeyUp(UKeyCode keyCode)
        {
            throw new System.NotImplementedException();
        }
    }
}