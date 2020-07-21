using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Twino.Ioc;

namespace Sample.Ioc
{
    public interface ITest
    {
        int Value { get; set; }
    }

    public class Test : ITest
    {
        public int Value { get; set; }
    }

    public interface ITest2
    {
    }

    public class Test2 : ITest2
    {
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceContainer container = new ServiceContainer();
            container.AddSingleton<ITest>(new Test {Value = 123});
            container.AddTransient<ITest2, Test2>();

            //optional, for checking missing references and circular references.
            //if there is a missing ref or circularity, throws exception
            container.CheckServices();

            var t1 = await container.Get<ITest>();
            var t2 = container.GetService<ITest>();
            Console.WriteLine(t1.Value);
            Console.WriteLine(t2.Value);

            var provider = container.BuildServiceProvider();

            var test = provider.GetService<ITest>();
            Console.WriteLine(test.Value);

            Console.WriteLine("Hello World!");
        }
    }
}