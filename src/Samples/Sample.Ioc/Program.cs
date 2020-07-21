using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Twino.Ioc;

namespace Sample.Ioc
{
    public interface ITest
    {
        int Value { get; set; }
        ITest2 T2 { get; set; }
        ITest3 T3 { get; }
    }

    public class Test : ITest
    {
        public int Value { get; set; }
        public ITest2 T2 { get; set; }
        public ITest3 T3 { get; }

        public Test(ITest3 test3)
        {
            T3 = test3;
        }
    }

    public interface ITest2
    {
    }

    public class Test2 : ITest2
    {
    }

    public interface ITest3
    {
    }

    public class Test3 : ITest3
    {
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection container = new ServiceContainer();
            container.AddTransient<ITest2, Test2>();
            container.AddTransient<ITest3, Test3>();
            container.AddSingleton<ITest, Test>();

            //optional, for checking missing references and circular references.
            //if there is a missing ref or circularity, throws exception
            //container.CheckServices();

            //var t1 = await container.Get<ITest>();
            //var t2 = container.GetService<ITest>();
            //Console.WriteLine(t1.Value);
            //Console.WriteLine(t2.Value);


            var provider = container.BuildServiceProvider();

            var test = provider.GetService<ITest>();
            Console.WriteLine(test.Value);

            Console.WriteLine("Hello World!");
        }
    }
}