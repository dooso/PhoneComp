using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.OleDb;
using System.Transactions;
using PhoneComp.Models;
using PhoneComp.DAL;
using PhoneComp.Lib;
using PagedList;

namespace PhoneComp.Controllers
{
    public class SContactController : BaseController
    {
        private PhoneCompContext db = new PhoneCompContext();

        //
        // GET: /SContact/

        public ActionResult Index(Contact entity, int? pageIndex, int? pageSize)
        {            
            var numbers =
                from num in db.Contacts
                where (num.IsDeleted == false )
                select num;
            if (!string.IsNullOrEmpty(entity.LinkerMobile))
            {
                numbers = numbers.Where(n => n.LinkerMobile.ToUpper().Contains(entity.LinkerMobile.ToUpper()));
            }
            if (!string.IsNullOrEmpty(entity.LinkerName))
            {
                numbers = numbers.Where(n => n.LinkerName.ToUpper().Contains(entity.LinkerName.ToUpper()));
            }
            var contacts = numbers.OrderByDescending(n=>n.ContactID).ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            
           // var contacts = db.Contacts.Include(c => c.Suspects);
            return View(contacts);
        }
       

        /// <summary>
        /// 新增或编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateEdit(int? id)
        {
            var suspects = db.Suspectses.Where(s => s.IsDeleted == false);
            ViewBag.SuspectID = new SelectList(suspects, "SuspectID", "SuspectMobile");
            Contact contact = new Contact();
            if (id.GetValueOrDefault() != 0)
            {
                contact = db.Contacts.Find(id);
               
                if (contact == null || contact.IsDeleted == true)
                {
                    contact = new Contact();
                }
            }
            //create
            return View(contact);
        }

        [HttpPost]
        public ActionResult CreateEdit(Contact contact)
        {
            try
            {
                string result = string.Empty;
                if (contact.SuspectID <= 0)
                {
                    result = "0|请先选择所属嫌疑人！";
                    return Content(result);
                }
                if (string.IsNullOrEmpty(contact.LinkerName))
                {
                    result = "0|联系人不能为空！";
                    return Content(result);
                }
                if (string.IsNullOrEmpty(contact.LinkerMobile))
                {
                    result = "0|联系人手机不能为空！";
                    return Content(result);
                }

                if (contact.ContactID > 0)
                {                    
                    db.Entry(contact).State = EntityState.Modified;
                    contact.IsDeleted = false;
                    contact.LastUpdateDate = DateTime.Now;
                    contact.LastUpdateUserID = CurrentUser.UID;
                    result = db.SaveChanges() == 1 ? "1|操作成功|/Scontact/Index" : "0|操作失败";
                    return Content(result);
                }
                else
                {
                    contact.IsDeleted = false;
                    contact.CreateDate = DateTime.Now;
                    contact.CreateUserID = CurrentUser.UID;
                    db.Contacts.Add(contact);
                    result = db.SaveChanges() == 1 ? "1|操作成功|/Scontact/Index" : "0|操作失败";
                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                return Content("0|" + ex.Message.ToString());
            }
        }

        //
        // POST: /SContact/Delete/5
        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                string result = string.Empty;
                Contact contact = db.Contacts.Find(id);
                contact.IsDeleted = true;
                db.Entry(contact).State = EntityState.Modified;
                result = db.SaveChanges() == 1 ? "1|操作成功" : "0|操作失败";
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

        #region 批量导入通讯录
        /// <summary>
        /// 批量导入嫌疑人通讯录
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchImport()
        {
            var suspects = db.Suspectses.Where(s => s.IsDeleted == false);
            ViewBag.SuspectID = new SelectList(suspects, "SuspectID", "SuspectMobile");
            return View();
        }
        [HttpPost]
        public ActionResult ContactImport(Contact contact)
        {
            try
            {
                string result = string.Empty;
                int totalCount = 0; // 待导入总条数  
                int successCount = 0;
                string FileName;
                string savePath;
                string fileEx;

                HttpPostedFileBase file = Request.Files["files"];
                if (contact.SuspectID <= 0)
                {
                    result = "请先选择所属嫌疑人！";
                    return Content("0|" + result);
                }

                if (file == null || file.ContentLength <= 0)
                {
                    result = "文件不能为空！";
                    return Content("0|" + result);
                }
                else
                {
                    string filename = Path.GetFileName(file.FileName);
                    int filesize = file.ContentLength;
                    fileEx = Path.GetExtension(filename);
                    string NoFileName = Path.GetFileNameWithoutExtension(filename);
                    int Maxsize = 4000 * 1024;
                    string FileType = ".xls,.xlsx";

                    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                    if (!FileType.Contains(fileEx))
                    {
                        result = "文件类型不对，只能导入xls和xlsx格式的文件";
                        return Content("0|" + result);
                    }
                    if (filesize >= Maxsize)
                    {
                        result = "上传文件超过8M，不能上传";
                        return Content("0|" + result);
                    }
                    string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/excel/";

                    savePath = Path.Combine(path, FileName);
                    file.SaveAs(savePath);
                }

                string strConn = string.Empty;
                if (fileEx.Equals(".xlsx"))
                {
                    strConn = "Provider=Microsoft.Ace.OleDb.12.0;Data Source=" + savePath + ";Extended Properties='Excel 12.0 Xml; HDR=YES; IMEX=1'";
                }
                else if (fileEx.Equals(".xls"))
                {
                    strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + savePath + ";Extended Properties='Excel 8.0; HDR=YES; IMEX=1'";
                }
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                OleDbDataAdapter myCommand = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", strConn);
                DataSet myDataSet = new DataSet();
                try
                {
                    myCommand.Fill(myDataSet, "ContactInfo");
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    return Content("0|" + result);
                }
                DataTable table = myDataSet.Tables["ContactInfo"].DefaultView.ToTable();

                using (TransactionScope transaction = new TransactionScope())
                {
                    int suspectID = contact.SuspectID;
                    DataRow dr = null;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        totalCount++;
                        dr = table.Rows[i];
                        if (insertContactSQL(dr, "Contact", suspectID) == 1)
                        {
                            successCount++;
                        }
                    }
                    transaction.Complete();
                }
                if (totalCount > 0)
                {
                    if (successCount > 0)
                    {
                        result = "1|成功导入" + successCount + "条记录。|/SContact/index";
                    }
                    else
                    {
                        result = "0|导入失败";
                    }
                }
                else
                {
                    result = "0|没有要导入的数据或数据已存在。";
                }
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content("0|" + ex.Message.ToString());
            }
        }


        private int insertContactSQL(DataRow dr, string tableName, int suspectID)
        {
            //excel表中的列名和数据库中的列名一定要对应  
            int flag = 0;
            int queryCount = 0;
            var linkerName = dr["联系人姓名"].ToString();
            var linkerMobile = dr["联系人号码"].ToString();
            if (!string.IsNullOrEmpty(linkerName) && !string.IsNullOrEmpty(linkerMobile))
            {
                string querySql = "SELECT COUNT(*) FROM " + tableName
                    + " WHERE IsDeleted=0 AND SuspectID='" + suspectID + "' AND LinkerName='" + linkerName + "' AND LinkerMobile='" + linkerMobile + "'";
                queryCount = db.Database.SqlQuery<int>(querySql).First();
                if (queryCount > 0) return flag;
                
                string sql = "insert into " + tableName +
                    "([SuspectID],[LinkerName],[LinkerMobile],[IsDeleted],[CreateDate],[CreateUserID]) VALUES " +
                    "(" + suspectID + ",'" + linkerName + "','" + linkerMobile + "',0,GETDATE()," + CurrentUser.UID + ")";
                flag = db.Database.ExecuteSqlCommand(sql);
            }
            return flag;
        }

        #endregion

        #region 通讯录比对

        public ActionResult ContactComp(int? pageIndex, int? pageSize, int? suspectId)
        {
            var suspects = db.Suspectses.Where(s => s.IsDeleted == false);
            ViewBag.SuspectID = new SelectList(suspects, "SuspectID", "SuspectMobile");
            if (suspectId.GetValueOrDefault() <= 0)
            {
                ViewBag.errorMsg = "请先选择要比对的嫌疑人！";
                return View();
            }
            var contacts = db.Contacts.Where(c => c.IsDeleted == false && c.SuspectID == suspectId).ToList();
            var usedworkers = db.UsedWorkers.Where(u => u.IsDeleted == false).ToList();

            //List<Contact> lcontact = new List<Contact>();
            List<UsedWorker> lusedworker = new List<UsedWorker>();
            foreach (var item in contacts)
            {
                foreach (var usedworker in usedworkers)
                {
                    if (item.LinkerMobile == usedworker.Phone1 || item.LinkerMobile == usedworker.Phone2 || item.LinkerMobile == usedworker.Phone3)
                    {
                        //lcontact.Add(item);
                        lusedworker.Add(usedworker);
                    }
                }
            }
            var result = lusedworker.ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            return View(result);
        }

        #endregion

        /// <summary>
        /// 联系人记录是否存在
        /// </summary>
        /// <param name="linkerName"></param>
        /// <param name="linkerMobile"></param>
        /// <param name="suspectID"></param>
        /// <returns></returns>
        public bool IsExistContact(string linkerName, string linkerMobile, int? suspectID)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(linkerName) && !string.IsNullOrEmpty(linkerMobile) && suspectID.GetValueOrDefault() > 0)
            {
                var contacts = db.Contacts.Where(c => c.SuspectID == suspectID && c.IsDeleted== false
                    && c.LinkerName.ToUpper() == linkerName.ToUpper()
                    && c.LinkerMobile.ToUpper() == linkerMobile.ToUpper());
                if (contacts.Count() > 0)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}