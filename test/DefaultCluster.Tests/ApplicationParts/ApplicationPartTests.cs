using Microsoft.Extensions.DependencyInjection;
using Microsoft.Orleans.ApplicationParts;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using Xunit;

namespace DefaultCluster.Tests.ApplicationParts
{
    public class ApplicationPartTests
    {
        [Fact]
        public void T1()
        {
            var silo = new SiloBuilder()
                .UseConfiguration(ClusterConfiguration.LocalhostPrimarySilo())
                // DefaultCluster.dll (this)
                .AddApplicationPart(typeof(DefaultCluster.Tests.ActivationsLifeCycleTests.GrainActivateDeactivateTests).Assembly)
                // Tester.dll
                .AddApplicationPart(typeof(Tester.GrainServiceTests).Assembly)
                // TestGrainInterfaces.dll
                .AddApplicationPart(typeof(TestGrainInterfaces.CircularStateTestState).Assembly)
                // TestGrains.dll
                .AddApplicationPart(typeof(TestGrains.AccountGrain).Assembly)
                // TestInternalGrains.dll
                .AddApplicationPart(typeof(TestInternalGrains.ProxyGrain).Assembly)
                .Build();

            var partManager = silo.Services.GetService<ApplicationPartManager>();

        }
    }
}
