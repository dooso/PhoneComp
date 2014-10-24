using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using PhoneComp.Lib;
using PhoneComp.Models;
using PhoneComp.DAL;
using PagedList;

namespace PhoneComp.Controllers
{
    public class UserController : BaseController
    {
        private PhoneCompContext db = new PhoneCompContext();
             

        /// <summary>
        /// 管理员用户列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(Member entity, int? pageIndex, int? pageSize)
        {
            string where = string.Empty;

            var numbers =
                from num in db.Members
                where (num.IsDeleted == false)
                select num;
            if (!string.IsNullOrEmpty(entity.UserName))
            {
                numbers = numbers.Where(n => n.UserName.ToUpper().Contains(entity.UserName.ToUpper()));
            }
            if (!string.IsNullOrEmpty(entity.RealName))
            {
                numbers = numbers.Where(n => n.RealName.ToUpper().Contains(entity.RealName.ToUpper()));
            }
            if (!string.IsNullOrEmpty(entity.Mobile))
            {
                numbers = numbers.Where(n => n.Mobile.ToUpper().Contains(entity.Mobile.ToUpper()));
            }

            var memberList = numbers.OrderByDescending(n => n.MemberID).ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            return View(memberList);
        }

        #region 注册管理员用户
        /// <summary>
        /// 注册管理员用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Register()
        {
            var roles = db.Roles.Where(r => r.IsDeleted == false);
            ViewBag.RoleID = new SelectList(roles, "RoleID", "RoleName");
            Member member = new Member();

            return View(member);
        }

        [HttpPost]
        public ActionResult Register(Member member)
        {
            try
            {
                string result = string.Empty;
                if (string.IsNullOrEmpty(member.IDcard.Trim()))
                {
                    result = "身份证号码不能为空！";
                    return Content("0|" + result);
                }
                if (string.IsNullOrEmpty(member.UserName.Trim()))
                {
                    result = "用户名不能为空";
                    return Content("0|" + result);
                }
                if (string.IsNullOrEmpty(member.RealName.Trim()))
                {
                    result = "真实姓名不能为空";
                    return Content("0|" + result);
                }
                var hasuser =
                    from num in db.Members
                    where (num.IsDeleted == false && num.UserName == member.UserName)
                    select num;

                if (hasuser.Count() > 0)
                {
                    result = "用户名已存在";
                    return Content("0|" + result);
                }
                var numbers =
                from num in db.Members
                where (num.IsDeleted == false && num.IDcard == member.IDcard)
                select num;

                if (numbers.Count() >0)
                {
                    result = "身份证号码不能重复";
                    return Content("0|" + result);
                }

                member.CreateDate = DateTime.Now;
                member.CreateMemberID = CurrentUser.UID;
                member.PasswordNotMD5 = member.Password;
                member.Password = Character.EncrytPassword(member.Password);
                db.Members.Add(member);
                result = db.SaveChanges() == 1 ? "1|注册成功|/User/index" : "0|注册失败";
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
        #endregion

        #region 修改密码
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChangePassword(int? id)
        {
            if (id.GetValueOrDefault() > 0)
            {
                var member = db.Members.Find(id);
                if (member == null || member.IsDeleted == true)
                {
                    member = new Member();
                }
                return View(member);
            }
            else
            {
                return Redirect("Index");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(Member member, FormCollection collection)
        {
            try
            {
                string result = string.Empty;
                var repwd = collection["RePassword"];
                if (!string.IsNullOrEmpty(member.Password) && !string.IsNullOrEmpty(repwd))
                {
                    if (member.Password != repwd)
                    {
                        result = "两次输入密码不一致！";
                        return Content("0|" + result);
                    }
                }
                else
                {
                    result = "请输入密码及确认密码！";
                    return Content("0|" + result);
                }

                if (member.MemberID > 0)
                {
                    var obj = db.Members.Find(member.MemberID);
                    if (obj == null || obj.IsDeleted == true)
                    {
                        result = "密码更改失败！";
                        return Content("0|" + result);
                    }
                    obj.LastUpdateDate = DateTime.Now;
                    obj.LastUpdateUserID = CurrentUser.UID;
                    obj.Password = Character.EncrytPassword(member.Password);
                    obj.PasswordNotMD5 = member.Password;
                    db.Entry(obj).State = EntityState.Modified;
                    result = db.SaveChanges() == 1 ? "1|密码更改成功|/user/index" : "0|密码更改失败";

                    return Content(result);
                }
                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }

        }
        #endregion

        #region 编辑管理员信息
        /// <summary>
        /// 编辑管理员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            try
            {
                var roles = db.Roles.Where(r => r.IsDeleted == false);
                ViewBag.RoleID = new SelectList(roles, "RoleID", "RoleName");
                if (id.GetValueOrDefault() > 0)
                {
                    var member = db.Members.Find(id);
                    if (member == null || member.IsDeleted == true)
                    {
                        member = new Member();
                    }
                    return View(member);
                }
                else
                {
                    return Redirect("Index");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult Edit(Member member)
        {
            try
            {
                string result = string.Empty;
                if (string.IsNullOrEmpty(member.IDcard.Trim()))
                {
                    result = "身份证号码不能为空！";
                    return Content("0|" + result);
                }
                if (string.IsNullOrEmpty(member.RealName.Trim()))
                {
                    result = "真实姓名不能为空";
                    return Content("0|" + result);
                }
                if (member.MemberID > 0)
                {
                    var obj = db.Members.Find(member.MemberID);
                    if (obj == null || obj.IsDeleted == true)
                    {
                        result = "信息更新失败！";
                        return Content("0|" + result);
                    }
                    var numbers =
                    from num in db.Members
                    where (num.IsDeleted == false && num.IDcard == member.IDcard && num.MemberID != member.MemberID)
                    select num;

                    if (numbers.Count() > 0)
                    {
                        result = "身份证号码不能重复";
                        return Content("0|" + result);
                    }
                    else
                    {
                        obj.RoleID = member.RoleID;
                        obj.Mobile = member.Mobile;
                        obj.IDcard = member.IDcard;
                        obj.RealName = member.RealName;
                        obj.Job = member.Job;
                        obj.Address = member.Address;
                        obj.Remark = member.Remark;
                        obj.LastUpdateDate = DateTime.Now;
                        obj.LastUpdateUserID = CurrentUser.UID;
                        db.Entry(obj).State = EntityState.Modified;
                        result = db.SaveChanges() == 1 ? "1|信息更新成功|/user/index" : "0|信息更新失败";
                        return Content(result);
                    }
                }
                else
                {
                    return Content("0|没有找到此对象");
                }
            }
            catch (Exception ex)
            {
                return Content("0|"+ex.Message.ToString());
            }
        }
        #endregion

        /// <summary>
        /// 是否存在用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsExistUserName(string username)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(username))
            {
                var user = db.Members.Where(u => u.IsDeleted == false&& u.UserName.ToUpper().Equals(username.ToUpper()));
                if (user.Count() > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 是否存在身份证号码
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public bool IsExistIDCard(string idcard)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(idcard))
            {
                var user = db.Members.Where(u => u.IsDeleted == false && u.IDcard.ToUpper().Equals(idcard.ToUpper()));
                if (user.Count() > 0)
                {
                    result = true;
                }
            }
            return result;
        }
        ///// <summary>
        ///// 删除管理员帐号
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        Member member = db.Members.Find(id);
        //        member.IsDeleted = true;
        //        db.Entry(member).State = EntityState.Modified;
        //        var result = db.SaveChanges();
        //        return Content(result.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message.ToString());
        //    }
        //}
    }
}
