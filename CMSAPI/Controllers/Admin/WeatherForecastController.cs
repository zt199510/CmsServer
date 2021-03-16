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
                ////Ras解密密码
                //var ras = new RSACrypt(rsaKey[0], rsaKey[1]);
                //parm.password = ras.Decrypt(parm.password);

                ////获得用户登录限制次数
                //var configLoginCount = Convert.ToInt32(ConfigExtensions.Configuration[KeyHelper.LOGINCOUNT]);
                ////获得登录次数和过期时间
                //var loginConfig = MemoryCacheService.Default.GetCache<SysAdminLoginConfig>(KeyHelper.LOGINCOUNT) ?? new SysAdminLoginConfig();
                //if (loginConfig.Count != 0 && loginConfig.DelayMinute != null)
                //{
                //    //说明存在过期时间，需要判断
                //    if (DateTime.Now <= loginConfig.DelayMinute)
                //    {
                //        apiRes.message = "您的登录以超过设定次数，请稍后再次登录~";
                //        return Ok(apiRes);
                //    }
                //    else
                //    {
                //        //已经过了登录的预设时间，重置登录配置参数
                //        loginConfig.Count = 0;
                //        loginConfig.DelayMinute = null;
                //    }
                //}
                ////查询登录结果
                //var dbres = await _adminService.LoginAsync(parm);
                //if (dbres.statusCode != 200)
                //{
                //    //增加登录次数
                //    loginConfig.Count += 1;
                //    //登录的次数大于配置的次数，则提示过期时间
                //    if (loginConfig.Count == configLoginCount)
                //    {
                //        var configDelayMinute = Convert.ToInt32(ConfigExtensions.Configuration[KeyHelper.LOGINDELAYMINUTE]);
                //        //记录过期时间
                //        loginConfig.DelayMinute = DateTime.Now.AddMinutes(configDelayMinute);
                //        apiRes.message = "登录次数超过" + configLoginCount + "次，请" + configDelayMinute + "分钟后再次登录";
                //        return Ok(apiRes);
                //    }
                //    //记录登录次数，保存到session
                //    MemoryCacheService.Default.SetCache(KeyHelper.LOGINCOUNT, loginConfig);
                //    //提示用户错误和登录次数信息
                //    apiRes.message = dbres.message + "　　您还剩余" + (configLoginCount - loginConfig.Count) + "登录次数";
                //    return Ok(apiRes);
                //}

                //var user = dbres.data.admin;
                //var identity = new ClaimsPrincipal(
                // new ClaimsIdentity(new[]
                //     {
                //              new Claim(ClaimTypes.Sid,user.Guid),
                //              new Claim(ClaimTypes.Role,user.DepartmentName),
                //              new Claim(ClaimTypes.Thumbprint,user.HeadPic),
                //              new Claim(ClaimTypes.Name,user.LoginName),
                //              new Claim(ClaimTypes.WindowsAccountName,user.LoginName),
                //              new Claim(ClaimTypes.UserData,user.UpLoginDate.ToString())
                //     }, CookieAuthenticationDefaults.AuthenticationScheme)
                //);
                ////如果保存用户类型是Session，则默认设置cookie退出浏览器 清空
                //if (ConfigExtensions.Configuration[KeyHelper.LOGINSAVEUSER] == "Session")
                //{
                //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
                //    {
                //        AllowRefresh = false
                //    });
                //}
                //else
                //{
                //    //根据配置保存浏览器用户信息，小时单位
                //    var hours = int.Parse(ConfigExtensions.Configuration[KeyHelper.LOGINCOOKIEEXPIRES]);
                //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity, new AuthenticationProperties
                //    {
                //        ExpiresUtc = DateTime.UtcNow.AddHours(hours),
                //        IsPersistent = true,
                //        AllowRefresh = false
                //    });
                //}
                ////获得第一条站点，并保存到session中
                //var site = await _siteService.GetListAsync(m => !m.IsDel, m => m.AddTime, DbOrderEnum.Asc);
                ////把权限存到缓存里
                //var menuSaveType = ConfigExtensions.Configuration[KeyHelper.LOGINAUTHORIZE];
                //if (menuSaveType == "Redis")
                //{
                //    RedisHelper.Set(KeyHelper.ADMINMENU + "_" + dbres.data.admin.Guid, dbres.data.menu);
                //    RedisHelper.Set(KeyHelper.NOWSITE, site.data.FirstOrDefault());
                //}
                //else
                //{
                //    MemoryCacheService.Default.SetCache(KeyHelper.NOWSITE, site.data.FirstOrDefault());
                //    MemoryCacheService.Default.SetCache(KeyHelper.ADMINMENU + "_" + dbres.data.admin.Guid, dbres.data.menu, 600);
                //}
                //token = JwtHelper.IssueJWT(new TokenModel()
                //{
                //    Uid = user.Guid,
                //    UserName = user.LoginName,
                //    Role = "Admin",
                //    TokenType = "Web"
                //});
                //MemoryCacheService.Default.RemoveCache("LOGINKEY_" + parm.number);
                //MemoryCacheService.Default.RemoveCache(KeyHelper.LOGINCOUNT);

                //#region 保存日志
                //var agent = HttpContext.Request.Headers["User-Agent"];
                //var log = new SysLog()
                //{
                //    Guid = Guid.NewGuid().ToString(),
                //    Logged = DateTime.Now,
                //    Logger = LogEnum.LOGIN.GetEnumText(),
                //    Level = "Info",
                //    Message = "登录：" + parm.loginname,
                //    Callsite = "/fytadmin/login",
                //    IP = Utils.GetIp(),
                //    User = parm.loginname,
                //    Browser = agent.ToString()
                //};
                //await _logService.AddAsync(log);
                //#endregion
            }
            catch (Exception ex)
            {
                apiRes.message = ex.Message;
                apiRes.statusCode = (int)ApiEnum.Error;

                #region 保存日志
                //    var agent = HttpContext.Request.Headers["User-Agent"];
                //    var log = new SysLog()
                //    {
                //        Guid = Guid.NewGuid().ToString(),
                //        Logged = DateTime.Now,
                //        Logger = LogEnum.LOGIN.GetEnumText(),
                //        Level = "Error",
                //        Message = "登录失败！" + ex.Message,
                //        Exception = ex.Message,
                //        Callsite = "/fytadmin/login",
                //        IP = Utils.GetIp(),
                //        User = parm.loginname,
                //        Browser = agent.ToString()
                //    };
                //    await _logService.AddAsync(log);
                  #endregion
                }
                apiRes.statusCode = (int)ApiEnum.Status;
                apiRes.data = token;
                return Ok(apiRes);
          
        }
    }
}
