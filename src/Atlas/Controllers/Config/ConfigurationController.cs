using Atlas.Controllers.Config.Models;
using Iolaus;
using Iolaus.Config;
using static Iolaus.F;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;


namespace Atlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Router _router;

        public ConfigurationController(IConfigurationProvider configurationProvider, Router router)
        {
            _configurationProvider = configurationProvider;
            _router = router;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var configs = (await _configurationProvider
                .GetConfigurationsAsync())
                .Select(c => c.ToString())
                .ToArray();
            return Ok(JsonConvert.SerializeObject(configs, Formatting.None));
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRouteRequest request)
        {
            var response = Configuration
                .Parse(request.Configuration)
                .Match<Task<IActionResult>>(
                    None: async () => await Task.FromResult(BadRequest()),
                    Some: async (configuration) => 
                    {
                        await _configurationProvider.AddConfigurationAsync(configuration);
                        return Ok();
                    }
                );

            return await response;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(RemoveConfigurationRequest request)
        {
            var response = Configuration
                .Parse(request.Configuration)
                .Match<Task<IActionResult>>(
                    None: async () => await Task.FromResult(BadRequest()),
                    Some: async (configuration) =>
                    {
                        await _configurationProvider.RemoveConfigurationAsync(configuration.Pattern.PatternAsString);
                        return Ok();
                    }
                );

            return await response;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send(SendMessageRequest request)
        {
            var message = Iolaus.Message.Parse(request.Message);
            var result = message.Match<IObservable<Option<Message>>>(
                None: () => Observable.Return<Option<Message>>(None),
                Some: (x) => _router.GetFunc(x)(x));

            var timer = Observable.Timer(TimeSpan.FromSeconds(5));
            var timedResult = result.TakeUntil(timer);
            var resp = await timedResult.FirstOrDefaultAsync();

            return Ok(resp.Match(None: () => "Failed", Some: (r) => r.ToString()));
        }
    }
}
