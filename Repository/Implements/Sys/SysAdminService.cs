using Common;
using Common.ClientApi;
using Common.EnumHelper;
using Domain.Model.Sys;
using Repository.DtoModel;
using Repository.DtoModel.Sys;
using Repository.Interfaces;
using Repository.Repositories;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
   public class SysAdminService : RepositoryBase<SysAdmin>, ISysAdminService
    {
        public Task<ApiResult<string>> AddAsync(SysAdmin parm)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<string>> DeleteAsync(string parm)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<Page<SysAdmin>>> GetPagesAsync(PageParm parm)
        {
            throw new NotImplementedException();
        }
        public Task<ApiResult<string>> ModifyAsync(SysAdmin parm)
        {
            throw new NotImplementedException();
        }



        #region 用户登录和授权菜单查询
        public async Task<ApiResult<SysAdminMenuDto>> LoginAsync(SysAdminLogin parm)
        {

            var res = new ApiResult<SysAdminMenuDto>() {statusCode=(int)ApiEnum.Error };
            try
            {
                var adminModel = new SysAdminMenuDto();
                parm.password = parm.password;
                var model = await Db.Queryable<SysAdmin>()
                       .Where(m => m.LoginName == parm.loginname).FirstAsync();
                if (model == null)
                {
                    res.message = "账号错误";
                    return res;
                }
                if (!model.LoginPwd.Equals(parm.password))
                {
                    res.message = "密码错误~";
                    return res;
                }
                if (!model.Status)
                {
                    res.message = "登录账号被冻结，请联系管理员~";
                    return res;
                }
                adminModel.menu = GetMenuByAdmin(model.Guid);
                if (adminModel == null)
                {
                    res.message = "当前账号没有授权功能模块，无法登录~";
                    return res;
                }
            }
            catch (Exception)
            {

                res = null;
            }
            return res;

        }

     



        /// <summary>
        /// 根据登录账号，返回菜单信息
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        List<SysMenuDto> GetMenuByAdmin(string admin)
        {
            List<SysMenuDto> res = null;
            try
            {
                //根据用户查询角色列表， 一个用户对应多个角色
                var roleList = SysPermissionsDb.GetList(m => m.AdminGuid == admin && m.Types == 2).Select(m => m.RoleGuid).ToList();
                //根据角色查询菜单，并查询到菜单涉及的功能
                var query = Db.Queryable<SysMenu, SysPermissions>((sm, sp) => new object[]{
                    JoinType.Inner,sm.Guid==sp.MenuGuid
                })
                .Where((sm, sp) => roleList.Contains(sp.RoleGuid) && sp.Types == 3 && sm.Status)
                .OrderBy((sm, sp) => sm.Sort)
                .Select((sm, sp) => new SysMenuDto()
                {
                    guid = sm.Guid,
                    parentGuid = sm.ParentGuid,
                    parentName = sm.ParentName,
                    name = sm.Name,
                    nameCode = sm.NameCode,
                    parentGuidList = sm.ParentGuidList,
                    layer = sm.Layer,
                    urls = sm.Urls,
                    icon = sm.Icon,
                    sort = sm.Sort,
                    btnJson = sp.BtnFunJson
                })
                .Mapper((it, cache) => {
                    var codeList = cache.Get(list =>
                    {
                        return Db.Queryable<SysCode>().Where(m => m.ParentGuid == "a88fa4d3-3658-4449-8f4a-7f438964d716")
                            .Select(m => new SysCodeDto()
                            {
                                guid = m.Guid,
                                name = m.Name,
                                codeType = m.CodeType
                            })
                            .ToList();
                    });
                    if (!string.IsNullOrEmpty(it.btnJson))
                    {
                        it.btnFun = codeList.Where(m => it.btnJson.Contains(m.guid)).ToList();
                    }
                });
                var result = query.ToList();
                res = result.CurDistinct(m => m.guid).ToList();
            }
            catch
            {
                res = null;
            }
            return res;
        }
        #endregion



       
    }
}
