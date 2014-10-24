using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneComp.Models;
using PhoneComp.DAL;
using PhoneComp.Lib;
using PagedList;

namespace PhoneComp.Controllers
{
    public class RoleController : BaseController
    {
        private PhoneCompContext db = new PhoneCompContext();

        //
        // GET: /Role/
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(int? pageIndex, int? pageSize)
        {
            var roles =
                from num in db.Roles
                where (num.IsDeleted == false)
                orderby num.CreateDate descending
                select num;
            var roleList= roles.ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            return View(roleList);
        }
        
        /// <summary>
        /// 新增或编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateEdit(int? id)
        {
            Role role = new Role();
            if (id.GetValueOrDefault() != 0)
            {
                role = db.Roles.Find(id);
                if (role == null || role.IsDeleted == true)
                {
                    role = new Role();
                }
            }
            //create
            return View(role);
        }
        [HttpPost]
        public ActionResult CreateEdit(Role role)
        {
            try
            {
                if (string.IsNullOrEmpty(role.RoleName))
                {
                    ViewBag.errorMsg = "姓名不能为空！";
                    return View(role);
                }
                if (role.RoleID > 0)
                {
                    db.Entry(role).State = EntityState.Modified;
                    role.IsDeleted = false;
                    role.LastUpdateDate = DateTime.Now;
                    role.LastUpdateUserID = CurrentUser.UID;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    role.IsDeleted = false;
                    role.CreateDate = DateTime.Now;
                    role.CreateUserID = CurrentUser.UID;
                    var hasRole = db.Roles.Where(r => r.IsDeleted == false && r.RoleName.Trim().ToLower() == role.RoleName.Trim().ToLower()).FirstOrDefault();
                    if (hasRole != null)
                    {
                        ViewBag.errorMsg = "已存在此角色名称！";
                        return View(role);
                    }
                    else
                    {
                        db.Roles.Add(role);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
        
        // 释放对象
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}