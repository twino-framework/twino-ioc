using System;
using System.Threading;
using System.Threading.Tasks;
using Test.Ioc.Services;
using Twino.Ioc;
using Twino.Ioc.Exceptions;
using Xunit;

namespace Test.Ioc
{
    public class PoolTest
    {
        [Fact]
        public void Transient()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransientPool<ISingleService, SingleService>();

            IContainerScope scope = services.CreateScope();

            ISingleService single = services.Get<ISingleService>(scope);
            single.Foo = "single";

            ISingleService s1 = services.Get<ISingleService>(scope);
            Assert.NotEqual(single.Foo, s1.Foo);

            ISingleService s2 = services.Get<ISingleService>();
            Assert.NotEqual(single.Foo, s2.Foo);

            IContainerScope scope2 = services.CreateScope();
            ISingleService s3 = services.Get<ISingleService>(scope2);
            Assert.NotEqual(single.Foo, s3.Foo);
        }

        [Fact]
        public void Scoped()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScopedPool<ISingleService, SingleService>();

            IContainerScope scope = services.CreateScope();

            ISingleService single = services.Get<ISingleService>(scope);
            single.Foo = "single";

            ISingleService s1 = services.Get<ISingleService>(scope);
            Assert.Equal(single.Foo, s1.Foo);

            Assert.Throws<ScopeException>(() => services.Get<ISingleService>());

            IContainerScope scope2 = services.CreateScope();
            ISingleService s3 = services.Get<ISingleService>(scope2);
            Assert.NotEqual(single.Foo, s3.Foo);
        }

        [Fact]
        public async Task LongRunning()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScopedPool<ISingleService, SingleService>();

            for (int i = 0; i < 50; i++)
            {
                IContainerScope scope1 = services.CreateScope();
                IContainerScope scope2 = services.CreateScope();

                ISingleService service1 = services.Get<ISingleService>(scope1);
                ISingleService service2 = services.Get<ISingleService>(scope2);

                Assert.NotNull(service1);
                Assert.NotNull(service2);
                Assert.NotEqual(service1, service2);

                await Task.Delay(10);

                scope1.Dispose();
                scope2.Dispose();
            }
        }

        [Fact]
        public void WaitLimitAndGet()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransientPool<ISingleService, SingleService>(o =>
            {
                o.PoolMaxSize = 10;
                o.ExceedLimitWhenWaitTimeout = false;
                o.WaitAvailableDuration = TimeSpan.FromMilliseconds(5000);
            });

            IContainerScope scope = services.CreateScope();
            for (int i = 0; i < 10; i++)
            {
                ISingleService service = services.Get<ISingleService>(scope);
                Assert.NotNull(service);
            }

            DateTime start = DateTime.UtcNow;
            Thread th = new Thread(() =>
            {
                Thread.Sleep(500);
                scope.Dispose();
            });
            th.Start();

            IContainerScope scope2 = services.CreateScope();
            ISingleService s = services.Get<ISingleService>(scope2);
            Assert.NotNull(s);

            DateTime end = DateTime.UtcNow;
            TimeSpan time = end - start;

            Assert.True(time > TimeSpan.FromMilliseconds(490));
            Assert.True(time < TimeSpan.FromMilliseconds(750));
        }

        [Fact]
        public void WaitLimitAndTimeout()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransientPool<ISingleService, SingleService>(o =>
            {
                o.PoolMaxSize = 10;
                o.ExceedLimitWhenWaitTimeout = false;
                o.WaitAvailableDuration = TimeSpan.FromMilliseconds(1500);
            });

            IContainerScope scope = services.CreateScope();
            for (int i = 0; i < 10; i++)
            {
                ISingleService service = services.Get<ISingleService>(scope);
                Assert.NotNull(service);
            }

            DateTime start = DateTime.UtcNow;
            Assert.Throws<NullReferenceException>(() => services.Get<ISingleService>(scope));
            DateTime end = DateTime.UtcNow;
            Assert.True(end - start > TimeSpan.FromMilliseconds(1490));
        }
    }
}