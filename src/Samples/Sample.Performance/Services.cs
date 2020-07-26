namespace Sample.Performance
{
    public interface IServiceA
    {
    }

    public interface IServiceB
    {
    }

    public interface IServiceC
    {
    }

    public interface IParentService
    {
    }


    public class ServiceA : IServiceA
    {
        private readonly IServiceC _serviceC;

        public ServiceA(IServiceC serviceC)
        {
            _serviceC = serviceC;
        }
    }


    public class ServiceB : IServiceB
    {
    }

    public class ServiceC : IServiceC
    {
    }


    public class ParentService : IParentService
    {
        private readonly IServiceA _serviceA;
        private readonly IServiceB _serviceB;

        public ParentService(IServiceA serviceA, IServiceB serviceB)
        {
            _serviceA = serviceA;
            _serviceB = serviceB;
        }

    }
}