namespace MicroService
{
    public class TestServices
    {
        public interface ITransientService
        {
            void Serve();
        }

        public class TransientService : ITransientService
        {
            public void Serve() => Console.WriteLine("Transient Service serving.");
        }

        public interface IScopedService
        {
            void Serve();
        }

        public class ScopedService : IScopedService
        {
            public void Serve() => Console.WriteLine("Scoped Service serving.");
        }

        public interface ISingletonService
        {
            void Serve();
        }

        public class SingletonService : ISingletonService
        {
            public void Serve() => Console.WriteLine("Singleton Service serving.");
        }

    }
}