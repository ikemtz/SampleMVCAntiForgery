using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using SampleMVC;
using System.IO;

namespace SampleMVCTest.IntegrationTests
{
    public abstract class BaseIntegrationTest
    {
        public string AntiForgeryFormTokenName { get; set; }
        public string AntiForgeryCookieName { get; set; }
        public BaseIntegrationTest()
        {
            AntiForgeryFormTokenName = "AntiForgeryFormTokenName";
            AntiForgeryCookieName = "AntiForgeryCookieName";
        }

        protected TestServer InitTestServer()
        {
            var builder = TestServer.CreateBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>()
                .UseServices(x =>
                {
                    //This is crucial as the FormFieldName is a randomly generated string by default.
                    x.ConfigureAntiforgery(t =>
                    {
                        t.CookieName = AntiForgeryCookieName;
                        t.FormFieldName = AntiForgeryFormTokenName;
                    });
                    //This allows the TestServer to locate the Views.
                    var env = new TestApplicationEnvironment();
                    env.ApplicationName = "SampleMVC";
                    //This must be set to the root of the web folder
                    env.ApplicationBasePath = Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                                    "..",  env.ApplicationName));
                    x.AddInstance<IApplicationEnvironment>(env);
                });
            return new TestServer(builder);
        }
    }
}