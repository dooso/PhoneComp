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
    public class SuspectController : BaseController
    {
        private PhoneCompContext db = new PhoneCompContext();

        //
        // GET: /Suspect/

        public ActionResult Index(Suspects entity, int? pageIndex, int? pageSize)
        {
            var numbers =
                from num in db.Suspectses
                where (num.IsDeleted == false)
                select num;

            if (!string.IsNullOrEmpty(entity.SuspectName))
            {
                numbers = numbers.Where(n => n.SuspectName.ToUpper().Contains(entity.SuspectName.ToUpper()));
            }
            if (!string.IsNullOrEmpty(entity.SuspectMobile))
            {
                numbers = numbers.Where(n => n.SuspectMobile.ToUpper().Contains(entity.SuspectMobile.ToUpper()));
            }

            var suspectList = numbers.OrderByDescending(n=>n.SuspectID).ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            return View(suspectList);
        }

       
        /// <summary>
        /// 新增或编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateEdit(int? id)
        {            
            Suspects suspects = new Suspects();
            if (id.GetValueOrDefault() != 0)
            {
                suspects = db.Suspectses.Find(id);
                if (suspects== null|| suspects.IsDeleted == true)
                {
                    suspects = new Suspects();
                }
            }
            //create
            return View(suspects);
        }

        [HttpPost]
        public ActionResult CreateEdit(Suspects suspect)
        {
            try
            {
                string result = string.Empty;
                if (string.IsNullOrEmpty(suspect.SuspectName))
                {
                    result = "姓名不能为空！";
                    return Content("0|" + result);
                }
                if (string.IsNullOrEmpty(suspect.SuspectMobile))
                {
                    result = "手机号码不能为空！";
                    return Content("0|" + result);
                }
                if (suspect.SuspectID > 0)
                {
                    suspect.IsDeleted = false;
                    suspect.LastUpdateDate = DateTime.Now;
                    suspect.LastUpdateUserID = CurrentUser.UID;

                    var user = db.Suspectses.Where(u => u.IsDeleted == false && u.SuspectID != suspect.SuspectID && u.SuspectMobile.ToUpper().Equals(suspect.SuspectMobile.ToUpper()));

                    if (user.Count() > 0)
                    {
                        result = "0|已经存在此手机号！";
                    }
                    else
                    {
                        db.Entry(suspect).State = EntityState.Modified;
                        result = db.SaveChanges() == 1 ? "1|操作成功|/Suspect/index" : "0|操作失败";
                    }                    
                    return Content(result);
                }
                else
                {
                    suspect.IsDeleted = false;
                    suspect.CreateDate = DateTime.Now;
                    suspect.CreateUserID = CurrentUser.UID;

                    var aflat=  IsExistMobile(suspect.SuspectMobile, 0);
                    if (aflat)
                    {
                        result = "0|已经存在此手机号！";
                    }
                    else
                    {
                        db.Suspectses.Add(suspect);
                        result = db.SaveChanges() == 1 ? "1|操作成功|/Suspect/index" : "0|操作失败";                        
                    }
                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                return Content("0|"+ex.Message.ToString());
            }
        }

        //
        // POST: /Suspect/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                string result = string.Empty;
                Suspects suspects = db.Suspectses.Find(id);
                if (suspects != null)
                {
                    suspects.IsDeleted = true;
                    db.Entry(suspects).State = EntityState.Modified;

                    string contactSql = " UPDATE Contact SET IsDeleted=1 WHERE SuspectID={0}";
                    string callRecordSql = " UPDATE CallRecord SET IsDeleted=1 WHERE SuspectID={0}";
                    //db.Contacts.SqlQuery(sql, null);
                    db.Database.ExecuteSqlCommand(contactSql, id);
                    db.Database.ExecuteSqlCommand(callRecordSql, id);
                    result = db.SaveChanges() == 1 ? "1|操作成功" : "0|操作失败";
                } 
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content("0|" + ex.Message.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 嫌疑人列表中手机号码是否已存在
        /// </summary>
        /// <param name="suspectMobile"></param>
        /// <param name="suspectID"></param>
        /// <returns></returns>
        public bool IsExistMobile(string suspectMobile, int? suspectID)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(suspectMobile))
            {
                var user = db.Suspectses.Where(u => u.IsDeleted == false && u.SuspectMobile.ToUpper().Equals(suspectMobile.ToUpper()));
                if (suspectID.GetValueOrDefault() > 0)
                {
                    user = db.Suspectses.Where(u => u.IsDeleted == false && u.SuspectID != suspectID && u.SuspectMobile.ToUpper().Equals(suspectMobile.ToUpper()));
                }
                if (user.Count() > 0)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}