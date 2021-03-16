using Common;
using Common.Cache;
using Common.ClientApi;
using Common.CryptHelper;
using Common.EnumHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository.DtoModel.Sys;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMSAPI.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/Admin")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ISysAdminService _adminService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ISysAdminService adminService)
        {
            _logger = logger;
            ///测试IOC
            _adminService = adminService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login( SysAdminLogin parm)
        {
            var apiRes = new ApiResult<string>() { statusCode = (int)ApiEnum.HttpRequestError };
            var token = "";
            try
            {
                ////获得公钥私钥，解密
                var rsaKey = MemoryCacheService.Default.GetCache<List<string>>("LOGINKEY_" + parm.number);
                if (rsaKey == null)
                {
                    apiRes.message = "登录失败，请刷新浏览器再次登录";
                    return Ok(apiRes);
                }
             
            }
            catch (Exception ex)
            {
                apiRes.message = ex.Message;
                apiRes.statusCode = (int)ApiEnum.Error;

              
                }
                apiRes.statusCode = (int)ApiEnum.Status;
                apiRes.data = token;
                return Ok(apiRes);
          
        }
    }
}
