using Test.Ioc.Services;
using Twino.Ioc;
using Twino.Ioc.Exceptions;
using Xunit;

namespace Test.Ioc
{
    public class MixedTest
    {
        #region Transient

        [Fact]
        public void TransientInScoped()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScoped<IParentService, ParentService>();
            services.AddTransient<IFirstChildService, FirstChildService>();
            services.AddTransient<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            //services in scoped service is created only once (cuz they created via parent)
            IParentService p = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p.Foo);
            Assert.Equal(parent.First.Foo, p.First.Foo);
            Assert.Equal(parent.Second.Foo, p.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>(scope);
            Assert.NotEqual(parent.First.Foo, first.Foo);

            ISecondChildService second = services.Get<ISecondChildService>(scope);
            Assert.NotEqual(parent.Second.Foo, second.Foo);
        }

        [Fact]
        public void TransientInSingleton()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddSingleton<IParentService, ParentService>();
            services.AddTransient<IFirstChildService, FirstChildService>();
            services.AddTransient<ISecondChildService, SecondChildService>();

            IParentService parent = services.Get<IParentService>();
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            //services in singleton service is created only once (cuz they created via parent)
            IParentService p = services.Get<IParentService>();
            Assert.Equal(parent.Foo, p.Foo);
            Assert.Equal(parent.First.Foo, p.First.Foo);
            Assert.Equal(parent.Second.Foo, p.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>();
            Assert.NotEqual(parent.First.Foo, first.Foo);

            ISecondChildService second = services.Get<ISecondChildService>();
            Assert.NotEqual(parent.Second.Foo, second.Foo);
        }

        [Fact]
        public void TransientInTransientPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransientPool<IParentService, ParentService>();
            services.AddTransient<IFirstChildService, FirstChildService>();
            services.AddTransient<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p = services.Get<IParentService>(scope);
            Assert.NotEqual(parent.Foo, p.Foo);
            Assert.NotEqual(parent.First.Foo, p.First.Foo);
            Assert.NotEqual(parent.Second.Foo, p.Second.Foo);

            IFirstChildService f1 = services.Get<IFirstChildService>();
            IFirstChildService f2 = services.Get<IFirstChildService>(scope);
            Assert.NotEqual(parent.First.Foo, f1.Foo);
            Assert.NotEqual(parent.First.Foo, f2.Foo);

            ISecondChildService s1 = services.Get<ISecondChildService>();
            ISecondChildService s2 = services.Get<ISecondChildService>(scope);
            Assert.NotEqual(parent.Second.Foo, s1.Foo);
            Assert.NotEqual(parent.Second.Foo, s2.Foo);
        }

        [Fact]
        public void TransientInScopedPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScopedPool<IParentService, ParentService>();
            services.AddTransient<IFirstChildService, FirstChildService>();
            services.AddTransient<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            //we are getting same instance of parent. so, children are same.
            IParentService p = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p.Foo);
            Assert.Equal(parent.First.Foo, p.First.Foo);
            Assert.Equal(parent.Second.Foo, p.Second.Foo);

            //we are getting individual children, so they are created new and different
            IFirstChildService f1 = services.Get<IFirstChildService>();
            IFirstChildService f2 = services.Get<IFirstChildService>(scope);
            Assert.NotEqual(parent.First.Foo, f1.Foo);
            Assert.NotEqual(parent.First.Foo, f2.Foo);

            //we are getting individual children, so they are created new and different
            ISecondChildService s1 = services.Get<ISecondChildService>();
            ISecondChildService s2 = services.Get<ISecondChildService>(scope);
            Assert.NotEqual(parent.Second.Foo, s1.Foo);
            Assert.NotEqual(parent.Second.Foo, s2.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
        }

        #endregion

        #region Scoped

        [Fact]
        public void ScopedInTransient()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransient<IParentService, ParentService>();
            services.AddScoped<IFirstChildService, FirstChildService>();
            services.AddScoped<ISecondChildService, SecondChildService>();

            Assert.Throws<ScopeException>(() => services.Get<IParentService>());
            Assert.Throws<ScopeException>(() => services.Get<IFirstChildService>());
            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p1 = services.Get<IParentService>(scope);
            Assert.NotEqual(parent.Foo, p1.Foo);
            Assert.Equal(parent.First.Foo, p1.First.Foo);
            Assert.Equal(parent.Second.Foo, p1.Second.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.NotEqual(parent.First.Foo, p2.First.Foo);
            Assert.NotEqual(parent.Second.Foo, p2.Second.Foo);
        }

        [Fact]
        public void ScopedInSingleton()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddSingleton<IParentService, ParentService>();
            services.AddScoped<IFirstChildService, FirstChildService>();
            services.AddScoped<ISecondChildService, SecondChildService>();

            Assert.Throws<ScopeException>(() => services.Get<IParentService>());
            Assert.Throws<ScopeException>(() => services.Get<IFirstChildService>());
            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p1 = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p1.Foo);
            Assert.Equal(parent.First.Foo, p1.First.Foo);
            Assert.Equal(parent.Second.Foo, p1.Second.Foo);

            //in same scope, individual scoped items should equal
            IFirstChildService f1 = services.Get<IFirstChildService>(scope);
            ISecondChildService s1 = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.First.Foo, f1.Foo);
            Assert.Equal(parent.Second.Foo, s1.Foo);

            //scoped services in singleton should equal (because parent is same)
            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.Equal(parent.Foo, p2.Foo);
            Assert.Equal(parent.First.Foo, p2.First.Foo);
            Assert.Equal(parent.Second.Foo, p2.Second.Foo);

            //but individual created scoped items in different scope should not equal
            IFirstChildService f2 = services.Get<IFirstChildService>(scope2);
            ISecondChildService s2 = services.Get<ISecondChildService>(scope2);
            Assert.NotEqual(parent.First.Foo, f2.Foo);
            Assert.NotEqual(parent.Second.Foo, s2.Foo);
        }

        [Fact]
        public void ScopedInTransientPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransientPool<IParentService, ParentService>();
            services.AddScoped<IFirstChildService, FirstChildService>();
            services.AddScoped<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p = services.Get<IParentService>(scope);
            Assert.NotEqual(parent.Foo, p.Foo);
            Assert.Equal(parent.First.Foo, p.First.Foo);
            Assert.Equal(parent.Second.Foo, p.Second.Foo);

            Assert.Throws<ScopeException>(() => services.Get<IFirstChildService>());
            IFirstChildService f2 = services.Get<IFirstChildService>(scope);
            Assert.Equal(parent.First.Foo, f2.Foo);

            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());
            ISecondChildService s2 = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.Second.Foo, s2.Foo);
        }

        [Fact]
        public void ScopedInScopedPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScopedPool<IParentService, ParentService>();
            services.AddScoped<IFirstChildService, FirstChildService>();
            services.AddScoped<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p.Foo);
            Assert.Equal(parent.First.Foo, p.First.Foo);
            Assert.Equal(parent.Second.Foo, p.Second.Foo);

            Assert.Throws<ScopeException>(() => services.Get<IFirstChildService>());
            IFirstChildService f2 = services.Get<IFirstChildService>(scope);
            Assert.Equal(parent.First.Foo, f2.Foo);

            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());
            ISecondChildService s2 = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.Second.Foo, s2.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.NotEqual(parent.First.Foo, p2.First.Foo);
            Assert.NotEqual(parent.Second.Foo, p2.Second.Foo);
        }

        #endregion

        #region Singleton

        [Fact]
        public void SingletonInTransient()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransient<IParentService, ParentService>();
            services.AddSingleton<IFirstChildService, FirstChildService>();
            services.AddSingleton<ISecondChildService, SecondChildService>();

            IFirstChildService first = services.Get<IFirstChildService>();
            first.Foo = "first";

            IParentService parent = services.Get<IParentService>();
            parent.Foo = "parent";
            parent.Second.Foo = "second";
            Assert.Equal(first.Foo, parent.First.Foo);

            ISecondChildService second = services.Get<ISecondChildService>();
            Assert.Equal(second.Foo, parent.Second.Foo);

            IParentService p = services.Get<IParentService>();
            Assert.NotEqual(p.Foo, parent.Foo);
            Assert.Equal(p.First.Foo, parent.First.Foo);
            Assert.Equal(p.Second.Foo, parent.Second.Foo);
        }

        [Fact]
        public void SingletonInScoped()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScoped<IParentService, ParentService>();
            services.AddSingleton<IFirstChildService, FirstChildService>();
            services.AddSingleton<ISecondChildService, SecondChildService>();
            IContainerScope scope = services.CreateScope();

            IFirstChildService first = services.Get<IFirstChildService>(scope);
            first.Foo = "first";

            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.Second.Foo = "second";
            Assert.Equal(first.Foo, parent.First.Foo);

            ISecondChildService second = services.Get<ISecondChildService>(scope);
            Assert.Equal(second.Foo, parent.Second.Foo);

            IParentService p1 = services.Get<IParentService>(scope);
            Assert.Equal(p1.Foo, parent.Foo);
            Assert.Equal(p1.First.Foo, parent.First.Foo);
            Assert.Equal(p1.Second.Foo, parent.Second.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(p2.Foo, parent.Foo);
            Assert.Equal(p2.First.Foo, parent.First.Foo);
            Assert.Equal(p2.Second.Foo, parent.Second.Foo);
        }

        [Fact]
        public void SingletonInTransientPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransientPool<IParentService, ParentService>();
            services.AddSingleton<IFirstChildService, FirstChildService>();
            services.AddSingleton<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p = services.Get<IParentService>(scope);
            Assert.NotEqual(parent.Foo, p.Foo);
            Assert.Equal(parent.First.Foo, p.First.Foo);
            Assert.Equal(parent.Second.Foo, p.Second.Foo);

            IFirstChildService f2 = services.Get<IFirstChildService>(scope);
            IFirstChildService f3 = services.Get<IFirstChildService>();
            Assert.Equal(parent.First.Foo, f2.Foo);
            Assert.Equal(parent.First.Foo, f3.Foo);

            ISecondChildService s2 = services.Get<ISecondChildService>(scope);
            ISecondChildService s3 = services.Get<ISecondChildService>();
            Assert.Equal(parent.Second.Foo, s2.Foo);
            Assert.Equal(parent.Second.Foo, s3.Foo);

            IParentService p2 = services.Get<IParentService>();
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.Equal(parent.First.Foo, p2.First.Foo);
            Assert.Equal(parent.Second.Foo, p2.Second.Foo);
        }

        [Fact]
        public void SingletonInScopedPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScopedPool<IParentService, ParentService>();
            services.AddSingleton<IFirstChildService, FirstChildService>();
            services.AddSingleton<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();
            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p.Foo);
            Assert.Equal(parent.First.Foo, p.First.Foo);
            Assert.Equal(parent.Second.Foo, p.Second.Foo);

            IFirstChildService f2 = services.Get<IFirstChildService>(scope);
            IFirstChildService f3 = services.Get<IFirstChildService>();
            Assert.Equal(parent.First.Foo, f2.Foo);
            Assert.Equal(parent.First.Foo, f3.Foo);

            ISecondChildService s2 = services.Get<ISecondChildService>(scope);
            ISecondChildService s3 = services.Get<ISecondChildService>();
            Assert.Equal(parent.Second.Foo, s2.Foo);
            Assert.Equal(parent.Second.Foo, s3.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.Equal(parent.First.Foo, p2.First.Foo);
            Assert.Equal(parent.Second.Foo, p2.Second.Foo);
        }

        #endregion

        #region Pool

        [Fact]
        public void PoolsInTransient()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransient<IParentService, ParentService>();
            services.AddTransientPool<IFirstChildService, FirstChildService>();
            services.AddScopedPool<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();

            Assert.Throws<ScopeException>(() => services.Get<IParentService>());
            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());

            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p1 = services.Get<IParentService>(scope);
            Assert.NotEqual(parent.Foo, p1.Foo);
            Assert.NotEqual(parent.First.Foo, p1.First.Foo);
            Assert.Equal(parent.Second.Foo, p1.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>(scope);
            IFirstChildService f2 = services.Get<IFirstChildService>();
            Assert.NotEqual(parent.First.Foo, first.Foo);
            Assert.NotEqual(parent.First.Foo, f2.Foo);

            ISecondChildService second = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.Second.Foo, second.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.NotEqual(parent.First.Foo, p2.First.Foo);
            Assert.NotEqual(parent.Second.Foo, p2.Second.Foo);
        }

        [Fact]
        public void PoolsInScoped()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScoped<IParentService, ParentService>();
            services.AddTransientPool<IFirstChildService, FirstChildService>();
            services.AddScopedPool<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();

            Assert.Throws<ScopeException>(() => services.Get<IParentService>());
            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());

            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            //getting same parent instance (cuz of scoped) so, transients in the same parent instances should be same
            IParentService p1 = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p1.Foo);
            Assert.Equal(parent.First.Foo, p1.First.Foo);
            Assert.Equal(parent.Second.Foo, p1.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>(scope);
            IFirstChildService f2 = services.Get<IFirstChildService>();
            Assert.NotEqual(parent.First.Foo, first.Foo);
            Assert.NotEqual(parent.First.Foo, f2.Foo);

            ISecondChildService second = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.Second.Foo, second.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.NotEqual(parent.First.Foo, p2.First.Foo);
            Assert.NotEqual(parent.Second.Foo, p2.Second.Foo);
        }

        [Fact]
        public void PoolsInSingleton()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddSingleton<IParentService, ParentService>();
            services.AddTransientPool<IFirstChildService, FirstChildService>();
            services.AddScopedPool<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();

            Assert.Throws<ScopeException>(() => services.Get<IParentService>());
            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());

            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            //getting same parent instance (cuz of singleton) so, transients in the same parent instances should be same
            IParentService p1 = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p1.Foo);
            Assert.Equal(parent.First.Foo, p1.First.Foo);
            Assert.Equal(parent.Second.Foo, p1.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>(scope);
            IFirstChildService f2 = services.Get<IFirstChildService>();
            Assert.NotEqual(parent.First.Foo, first.Foo);
            Assert.NotEqual(parent.First.Foo, f2.Foo);

            ISecondChildService second = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.Second.Foo, second.Foo);

            //children in same singleton instance are same
            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.Equal(parent.Foo, p2.Foo);
            Assert.Equal(parent.First.Foo, p2.First.Foo);
            Assert.Equal(parent.Second.Foo, p2.Second.Foo);
        }

        [Fact]
        public void PoolsInTransientPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddTransientPool<IParentService, ParentService>();
            services.AddTransientPool<IFirstChildService, FirstChildService>();
            services.AddScopedPool<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();

            Assert.Throws<ScopeException>(() => services.Get<IParentService>());
            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());

            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            IParentService p1 = services.Get<IParentService>(scope);
            Assert.NotEqual(parent.Foo, p1.Foo);
            Assert.NotEqual(parent.First.Foo, p1.First.Foo);
            Assert.Equal(parent.Second.Foo, p1.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>(scope);
            IFirstChildService f2 = services.Get<IFirstChildService>();
            Assert.NotEqual(parent.First.Foo, first.Foo);
            Assert.NotEqual(parent.First.Foo, f2.Foo);

            ISecondChildService second = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.Second.Foo, second.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.NotEqual(parent.First.Foo, p2.First.Foo);
            Assert.NotEqual(parent.Second.Foo, p2.Second.Foo);
        }

        [Fact]
        public void PoolsInScopedPool()
        {
            ServiceContainer services = new ServiceContainer();
            services.AddScopedPool<IParentService, ParentService>();
            services.AddTransientPool<IFirstChildService, FirstChildService>();
            services.AddScopedPool<ISecondChildService, SecondChildService>();

            IContainerScope scope = services.CreateScope();

            Assert.Throws<ScopeException>(() => services.Get<IParentService>());
            Assert.Throws<ScopeException>(() => services.Get<ISecondChildService>());

            IParentService parent = services.Get<IParentService>(scope);
            parent.Foo = "parent";
            parent.First.Foo = "first";
            parent.Second.Foo = "second";

            //getting same parent instance (cuz of scoped) so, transients in the same parent instances should be same
            IParentService p1 = services.Get<IParentService>(scope);
            Assert.Equal(parent.Foo, p1.Foo);
            Assert.Equal(parent.First.Foo, p1.First.Foo);
            Assert.Equal(parent.Second.Foo, p1.Second.Foo);

            IFirstChildService first = services.Get<IFirstChildService>(scope);
            IFirstChildService f2 = services.Get<IFirstChildService>();
            Assert.NotEqual(parent.First.Foo, first.Foo);
            Assert.NotEqual(parent.First.Foo, f2.Foo);

            ISecondChildService second = services.Get<ISecondChildService>(scope);
            Assert.Equal(parent.Second.Foo, second.Foo);

            IContainerScope scope2 = services.CreateScope();
            IParentService p2 = services.Get<IParentService>(scope2);
            Assert.NotEqual(parent.Foo, p2.Foo);
            Assert.NotEqual(parent.First.Foo, p2.First.Foo);
            Assert.NotEqual(parent.Second.Foo, p2.Second.Foo);
        }

        #endregion
    }
}