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
using PagedList;
using PhoneComp.Models;
using PhoneComp.DAL;
using PhoneComp.Lib;

namespace PhoneComp.Controllers
{
    public class UsedWorkerController : BaseController
    {
        private PhoneCompContext db = new PhoneCompContext();

        //
        // GET: /UsedWorker/

        public ActionResult Index(UsedWorker entity, int? pageIndex, int? pageSize)
        {
            var usedWorkers =
                from num in db.UsedWorkers
                where (num.IsDeleted == false)
                select num;
            if (!string.IsNullOrEmpty(entity.Name))
            {
                usedWorkers = usedWorkers.Where(n => n.Name.ToUpper().Contains(entity.Name.ToUpper()));
            }
            if (!string.IsNullOrEmpty(entity.IDcard))
            {
                usedWorkers = usedWorkers.Where(n => n.IDcard.ToUpper().Contains(entity.IDcard.ToUpper()));
            }
            if (!string.IsNullOrEmpty(entity.Phone1))
            {
                usedWorkers = usedWorkers.Where(n => n.Phone1.ToUpper().Contains(entity.Phone1.ToUpper())
                    || n.Phone2.ToUpper().Contains(entity.Phone1.ToUpper())
                    || n.Phone3.ToUpper().Contains(entity.Phone1.ToUpper())                     
                    );
            }
            
            var userdWorkers = usedWorkers.OrderByDescending(n => n.UsedWorkerID).ToPagedList(pageIndex ?? 1, pageSize ?? 10);
                        
            return View(userdWorkers);
        }  

        /// <summary>
        /// 新增或编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateEdit(int? id)
        {
            UsedWorker usedworker = new UsedWorker();
            if (id.GetValueOrDefault() > 0)
            {
                // edit
                usedworker = db.UsedWorkers.Find(id);
                if (usedworker == null || usedworker.IsDeleted == true)
                {
                    usedworker = new UsedWorker();
                }
            }
            //create
            return View(usedworker);
        }

        [HttpPost]
        public ActionResult CreateEdit(UsedWorker usedworker)
        {
            try
            {
                string result = string.Empty;
                if (string.IsNullOrEmpty(usedworker.IDcard.Trim()))
                {
                    result = "身份证号码不能为空！";
                    return Content("0|" + result);
                }
                if (string.IsNullOrEmpty(usedworker.Name.Trim()))
                {
                    result = "姓名不能为空";
                    return Content("0|" + result);
                }
                if (usedworker.UsedWorkerID > 0)
                {
                    //编辑
                    var flag = IsExistIDCard(usedworker.IDcard, usedworker.UsedWorkerID);
                    if (!flag)
                    {
                        usedworker.LastUpdateDate = DateTime.Now;
                        usedworker.LastUpdateUserID = CurrentUser.UID;
                        usedworker.IsDeleted = false;
                        db.Entry(usedworker).State = EntityState.Modified;
                        result = db.SaveChanges() == 1 ? "1|信息更新成功|/UsedWorker/index" : "0|信息更新失败";
                        return Content(result);
                    }
                    else
                    {
                        result = "身份证号码不能重复";
                        return Content("0|" + result);
                    }
                }
                else
                {
                    // 新增
                    var aflag = IsExistIDCard(usedworker.IDcard, 0);
                    if (!aflag)
                    {
                        usedworker.CreateDate = DateTime.Now;
                        usedworker.CreateUserID = CurrentUser.UID;
                        usedworker.IsDeleted = false;
                        db.UsedWorkers.Add(usedworker);
                        result = db.SaveChanges() == 1 ? "1|新增成功|/UsedWorker/index" : "0|新增失败";
                        return Content(result);
                    }
                    else
                    {
                        result = "身份证号码不能重复";
                        return Content("0|" + result);
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 二手人员详情页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id)
        {
            var user = new UsedWorker();
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                user = db.UsedWorkers.Where(u => u.IsDeleted == false&& u.UsedWorkerID==id).FirstOrDefault();             
            }
            return View(user);
        }

        //
        // POST: /UsedWorker/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                UsedWorker usedworker = db.UsedWorkers.Find(id);
                usedworker.IsDeleted = true;
                db.Entry(usedworker).State = EntityState.Modified;
                var result = db.SaveChanges();
                return Content(result.ToString());
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region 批量导入二手工作人员名片
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult StationImport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StationImport(HttpPostedFileBase filebase)
        {
            try
            {
                string result = string.Empty;
                int successCount = 0;
                int totalCount = 0; // 待导入总条数          
                string FileName;
                string savePath;
                string fileEx;

                HttpPostedFileBase file = Request.Files["files"];
                if (file == null || file.ContentLength <= 0)
                {
                    result = "文件不能为空";
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
                    myCommand.Fill(myDataSet, "ExcelInfo");
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    return Content("0|" + result);
                }
                DataTable table = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();

                using (TransactionScope transaction = new TransactionScope())
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        UsedWorker usedworker = new UsedWorker();
                        usedworker.Name = table.Rows[i]["姓名"].ToString();
                        usedworker.IDcard = table.Rows[i]["身份证编号"].ToString();
                        usedworker.Job = table.Rows[i]["从事行业"].ToString();
                        usedworker.JobAddress = table.Rows[i]["从事行业地点"].ToString();
                        usedworker.HomeAddress = table.Rows[i]["家庭地址"].ToString();
                        usedworker.Phone1 = table.Rows[i]["联系方式1"].ToString();
                        usedworker.Phone2 = table.Rows[i]["联系方式2"].ToString();
                        usedworker.Phone3 = table.Rows[i]["联系方式3"].ToString();
                        usedworker.IsDeleted = false;
                        usedworker.CreateUserID = CurrentUser.UID;
                        usedworker.CreateDate = DateTime.Now;

                        // 身份证号码唯一
                        if (!string.IsNullOrEmpty(usedworker.IDcard) && !string.IsNullOrEmpty(usedworker.Name))
                        {
                            if (db.UsedWorkers.Where(c => c.IDcard == usedworker.IDcard && c.IsDeleted == false).Count() <= 0)
                            {
                                totalCount++;
                                db.UsedWorkers.Add(usedworker);
                                if (db.SaveChanges() == 1)
                                {
                                    successCount++;
                                }
                            }
                        }
                    }
                    transaction.Complete();
                }
                if (totalCount > 0)
                {
                    if (successCount > 0)
                    {
                        result = "1|成功导入" + successCount + "条记录。|/UsedWorker/index";
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
                System.Threading.Thread.Sleep(1000);
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content("0|" + ex.Message.ToString());
            }

        }


        [HttpPost]
        public ActionResult UsedWorkerImport(HttpPostedFileBase filebase)
        {
            try
            {
                string result = string.Empty;
                int successCount = 0;
                int totalCount = 0; // 待导入总条数          
                string FileName;
                string savePath;
                string fileEx;

                HttpPostedFileBase file = Request.Files["files"];
                if (file == null || file.ContentLength <= 0)
                {
                    result = "文件不能为空";
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
                    if (Directory.Exists(Server.MapPath("~/uploads/excle")) == false)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/uploads/excel"));
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
                    myCommand.Fill(myDataSet, "ExcelInfo");
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    return Content("0|" + result);
                }
                DataTable table = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();

                using (TransactionScope transaction = new TransactionScope())
                {
                    DataRow dr = null;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        totalCount++;
                        dr = table.Rows[i];
                        if (InsertUsedWorkerSQL(dr,"UsedWorker")==1)
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
                        result = "1|成功导入" + successCount + "条记录。|/UsedWorker/index";
                    }
                    else
                    {
                        result = "0|导入不成功，或数据已存在。";
                    }
                }
                else
                {
                    result = "0|没有要导入的数据。";
                }
                System.Threading.Thread.Sleep(1000);
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content("0|" + ex.Message.ToString());
            }

        }

        // importUsedWorker
        public int InsertUsedWorkerSQL(DataRow dr, string tableName)
        {
            //excel表中的列名和数据库中的列名一定要对应  
            int executeCount = 0;
            int queryCount = 0;
            var name = dr["姓名"].ToString();
            var idcard = dr["身份证编号"].ToString();
            var job = dr["从事行业"].ToString();
            var jobaddress = dr["从事行业地点"].ToString();
            var homeaddress = dr["家庭地址"].ToString();
            var phone1 = dr["联系方式1"].ToString();
            var phone2 = dr["联系方式2"].ToString();
            var phone3 = dr["联系方式3"].ToString();
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(idcard))
            {
                string querySql = "SELECT COUNT(*) FROM " + tableName + " WHERE IsDeleted=0 AND IDCard='" + idcard + "'";
                queryCount = db.Database.SqlQuery<int>(querySql).First();
                if (queryCount > 0) return executeCount;
                string sql = "insert into " + tableName +
                    "([Name],[IDCard],[Job],[JobAddress],[HomeAddress],[Phone1],[Phone2],[Phone3],[IsDeleted],[CreateDate],[CreateUserID]) VALUES " +
                    "('"+name+"','" + idcard + "','" + job + "','" +jobaddress + "','" + homeaddress +"','"+phone1+"','"+phone2 +"','"+phone3+"',0,GETDATE()," + CurrentUser.UID + ")";
                executeCount = db.Database.ExecuteSqlCommand(sql);
            }
            return executeCount;
        }


        #endregion
                
        /// <summary>
        /// 是否存在身份证号码
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public bool IsExistIDCard(string idcard, int? usedworkerid)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(idcard))
            {
                var user = db.UsedWorkers.Where(u => u.IsDeleted == false && u.IDcard.ToUpper().Equals(idcard.ToUpper()));
                if (usedworkerid.GetValueOrDefault() > 0)
                {
                    user= db.UsedWorkers.Where(u=> u.IsDeleted == false &&u.UsedWorkerID != usedworkerid && u.IDcard.ToUpper().Equals(idcard.ToUpper()));
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