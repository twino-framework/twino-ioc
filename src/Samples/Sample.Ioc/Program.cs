using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        string Value { get; set; }
    }

    public class Test2 : ITest2
    {
        public string Value { get; set; }
    }

    public interface ITest3
    {
        string Value { get; set; }
    }

    public class Test3 : ITest3
    {
        public string Value { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceContainer container = new ServiceContainer();
            container.AddSingleton<IConfigureOptions<Test2>>(new ConfigureOptions<Test2>(t => t.Value = "Hello"));

            container.AddTransient<ITest2, Test2>();
            container.AddTransient<ITest3, Test3>();
            container.AddSingleton<ITest, Test>();

            container.CheckServices();

            //optional, for checking missing references and circular references.
            //if there is a missing ref or circularity, throws exception
            //container.CheckServices();

            //var t1 = await container.Get<ITest>();bi
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