using Test.Ioc.Services;
using Twino.Ioc;
using Xunit;

namespace Test.Ioc
{
    public class SingletonTest
    {
        [Fact]
        public void Instanced()
        {
            SingleService singleService = new SingleService();
            singleService.Foo = "singleton";

            ServiceContainer services = new ServiceContainer();
            services.AddSingleton<ISingleService, SingleService>(singleService);

            ISingleService s1 = services.Get<ISingleService>();
            Assert.Equal(singleService.Foo, s1.Foo);

            IContainerScope scope = services.CreateScope();
            ISingleService s2 = services.Get<ISingleService>(scope);
            Assert.Equal(singleService.Foo, s2.Foo);
        }

        [Fact]
        public void Single()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddSingleton<ISingleService, SingleService>();

            ISingleService singleton = services.Get<ISingleService>();
            singleton.Foo = "singleton";

            ISingleService s1 = services.Get<ISingleService>();
            Assert.Equal(singleton.Foo, s1.Foo);

            IContainerScope scope = services.CreateScope();
            ISingleService s2 = services.Get<ISingleService>(scope);
            Assert.Equal(singleton.Foo, s2.Foo);
        }

        [Fact]
        public void Nested()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddSingleton<IParentService, ParentService>();
            services.AddSingleton<IFirstChildService, FirstChildService>();
            services.AddSingleton<ISecondChildService, SecondChildService>();

            IParentService singleton = services.Get<IParentService>();
            singleton.Foo = "singleton";
            singleton.First.Foo = "first";
            singleton.Second.Foo = "second";

            IParentService p1 = services.Get<IParentService>();
            Assert.Equal(singleton.Foo, p1.Foo);
            Assert.Equal(singleton.First, p1.First);
            Assert.Equal(singleton.Second, p1.Second);
            Assert.Equal(singleton.First.Foo, p1.First.Foo);
            Assert.Equal(singleton.Second.Foo, p1.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>();
            Assert.Equal(singleton.First.Foo, first.Foo);

            ISecondChildService second = services.Get<ISecondChildService>();
            Assert.Equal(singleton.Second.Foo, second.Foo);

            IContainerScope scope = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope);
            Assert.Equal(singleton.Foo, p2.Foo);
            Assert.Equal(singleton.First, p2.First);
            Assert.Equal(singleton.Second, p2.Second);
            Assert.Equal(singleton.First.Foo, p2.First.Foo);
            Assert.Equal(singleton.Second.Foo, p2.Second.Foo);
        }

        [Fact]
        public void MultipleNestedDoubleParameter()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddSingleton<INestParentService, NestParentService>();
            services.AddSingleton<ISingleService, SingleService>();
            services.AddSingleton<IParentService, ParentService>();
            services.AddSingleton<IFirstChildService, FirstChildService>();
            services.AddSingleton<ISecondChildService, SecondChildService>();

            INestParentService nest = services.Get<INestParentService>();
            nest.Foo = "nest";
            nest.Parent.Foo = "parent";
            nest.Parent.First.Foo = "first";
            nest.Parent.Second.Foo = "second";
            nest.Single.Foo = "single";

            INestParentService n1 = services.Get<INestParentService>();
            Assert.Equal(nest.Foo, n1.Foo);
            Assert.Equal(nest.Single.Foo, n1.Single.Foo);
            Assert.Equal(nest.Parent.Foo, n1.Parent.Foo);
            Assert.Equal(nest.Parent.First.Foo, n1.Parent.First.Foo);
            Assert.Equal(nest.Parent.Second.Foo, n1.Parent.Second.Foo);

            IParentService parent = services.Get<IParentService>();
            Assert.Equal(nest.Parent.Foo, parent.Foo);
            Assert.Equal(nest.Parent.First.Foo, parent.First.Foo);
            Assert.Equal(nest.Parent.Second.Foo, parent.Second.Foo);

            ISingleService single = services.Get<ISingleService>();
            Assert.Equal(nest.Single.Foo, single.Foo);

            IFirstChildService first = services.Get<IFirstChildService>();
            Assert.Equal(nest.Parent.First.Foo, first.Foo);

            ISecondChildService second = services.Get<ISecondChildService>();
            Assert.Equal(nest.Parent.Second.Foo, second.Foo);

            IContainerScope scope = services.CreateScope();
            INestParentService n2 = services.Get<INestParentService>(scope);
            Assert.Equal(nest.Foo, n2.Foo);
            Assert.Equal(nest.Single.Foo, n2.Single.Foo);
            Assert.Equal(nest.Parent.Foo, n2.Parent.Foo);
            Assert.Equal(nest.Parent.First.Foo, n2.Parent.First.Foo);
            Assert.Equal(nest.Parent.Second.Foo, n2.Parent.Second.Foo);
        }
    }
}