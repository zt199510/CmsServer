using Common;
using Common.Cache;
using Common.CryptHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMSAPI.LoginModel
{
    public class LoginModel
    {
        
        public List<string> RsaKey { get; set; }

        /// <summary>
        /// 增加一个随机数保障浏览器和刷新导致密钥丢失问题
        /// </summary>
        public string Number { get; set; }


        public  void OnGet()
        {
            Number = Utils.Number(15);
            RsaKey = RSACrypt.GetKey();
            //获得公钥和私钥
             MemoryCacheService.Default.SetCache("LOGINKEY_" + Number, RsaKey, 5);
      
        }
    }
}
