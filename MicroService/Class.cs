namespace MicroService
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        public string Message { get; }

        public TestAttribute(string message)
        {
            Message = message;
        }
    }
}