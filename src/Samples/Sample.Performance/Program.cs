using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Twino.Ioc;

namespace Sample.Performance
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServiceContainer services = new ServiceContainer();

            services.AddTransient<IServiceB, ServiceB>();
            services.AddTransient<IServiceC, ServiceC>();
            services.AddTransient<IServiceA, ServiceA>();
            services.AddTransient<IParentService, ParentService>(p =>
            {
            });

            ITwinoServiceProvider provider = services.GetProvider();
            IContainerScope scope = provider.CreateScope();
            IParentService service = services.Get<IParentService>(scope);

            Console.WriteLine(service);
            Console.ReadLine();
            object s;
            Type t = typeof(IParentService);
            while (true)
            {
                Stopwatch swx = new Stopwatch();
                swx.Start();
                for (int i = 0; i < 10000000; i++)
                {
                    s = provider.Get<IParentService>(); //scope);
                }

                swx.Stop();
                Console.WriteLine("Total : " + swx.ElapsedMilliseconds);
                Console.ReadLine();
            }
        }
    }
}