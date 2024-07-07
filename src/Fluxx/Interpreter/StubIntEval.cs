namespace Fluxx.Interpreter
{
    public class StubIntEval : IntEval
    {
        public static readonly StubIntEval Instance = new StubIntEval();

        public override int Eval()
        {
            throw new System.NotImplementedException("This method should never be executed");
        }

        public override void Push()
        {
            throw new System.NotImplementedException("This method should never be executed");
        }
    }
}
