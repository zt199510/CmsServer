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
using Newtonsoft.Json;
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
    public class AdminController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<AdminController> _logger;
        private readonly ISysAdminService _adminService;
        private readonly LoginModel.LoginModel _loginModel;

        public AdminController(ILogger<AdminController> logger, ISysAdminService adminService,LoginModel.LoginModel loginModel)
        {
            _logger = logger;
            ///测试IOC
            _adminService = adminService;
            _loginModel = loginModel;
        }

        /// <summary>
        /// 获取公钥和缓存key
        /// </summary>
        /// <returns></returns>
        [HttpPost("adminGet")]
        [AllowAnonymous]
        public   ActionResult GetRSAKey()
        {
            var apiRes = new ApiResult<object>();
             _loginModel.OnGet();
            apiRes.data = new
            {
                number=_loginModel.Number,
                publicKey=_loginModel.RsaKey[1]
            };
            apiRes.success = true;
            return  Ok(apiRes);
        }

        /// <summary>
        /// 登入验证
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]SysAdminLogin parm)
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
                var ras = new RSACrypt(rsaKey[0], rsaKey[1]);
                parm.password = ras.Decrypt(parm.password);


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
