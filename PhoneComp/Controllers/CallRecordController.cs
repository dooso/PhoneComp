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
    public class CallRecordController : BaseController
    {
        private PhoneCompContext db = new PhoneCompContext();

        //
        // GET: /CallRecord/

        public ActionResult Index(int? pageIndex, int? pageSize, int? SuspectID)
        {
            var suspects = db.Suspectses.Where(s => s.IsDeleted == false);
            ViewBag.SuspectID = new SelectList(suspects, "SuspectID", "SuspectMobile");
            var lists =
                from num in db.CallRecords
                where (num.IsDeleted == false && num.Suspects.IsDeleted == false)
                select num;
            if (SuspectID.GetValueOrDefault() > 0)
            {
                lists = lists.Where(n => n.SuspectID == SuspectID);
            }
            var callrecords = lists.OrderByDescending(n => n.CallRecordID).ToPagedList(pageIndex ?? 1, pageSize ?? 10);

            return View(callrecords);
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
            //ViewBag.SuspectID = new SelectList(db.Suspectses, "SuspectID", "SuspectName");
            CallRecord callRecord = new CallRecord();
            if (id.GetValueOrDefault() != 0)
            {
                callRecord = db.CallRecords.Find(id);
                if (callRecord == null || callRecord.IsDeleted == true)
                {
                    callRecord = new CallRecord();
                }
            }
            return View(callRecord);
        }
        [HttpPost]
        public ActionResult CreateEdit(CallRecord callRecord)
        {
            try
            {
                string result = string.Empty;
                if (callRecord.SuspectID <= 0)
                {
                    result = "0|请先选择所属嫌疑人！";
                    return Content(result);
                }
                if (string.IsNullOrEmpty(callRecord.CalledMobile))
                {
                    result = "0|对方号码不能为空！";
                    return Content(result);
                }
                // 编辑
                if (callRecord.CallRecordID > 0)
                {
                    db.Entry(callRecord).State = EntityState.Modified;
                    callRecord.IsDeleted = false;
                    callRecord.LastUpdateDate = DateTime.Now;
                    callRecord.LastUpdateUserID = CurrentUser.UID;
                    result = db.SaveChanges() == 1 ? "1|操作成功|/CallRecord/Index" : "0|操作失败";
                    return Content(result);
                }
                else
                {   // 新增
                    callRecord.IsDeleted = false;
                    callRecord.CreateDate = DateTime.Now;
                    callRecord.CreateUserID = CurrentUser.UID;
                    db.CallRecords.Add(callRecord);
                    result = db.SaveChanges() == 1 ? "1|操作成功|/CallRecord/Index" : "0|操作失败";
                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                return Content("0|" + ex.Message.ToString());
            }
        }

        //
        // POST: /CallRecord/Delete/5
        /// <summary>
        /// 删除通话记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                string result = string.Empty;
                CallRecord callrecord = db.CallRecords.Find(id);
                callrecord.IsDeleted = true;
                db.Entry(callrecord).State = EntityState.Modified;
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

        #region 批量导入嫌疑人通话清单
        /// <summary>
        /// 批量导入嫌疑人通话清单
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchImport()
        {
            var suspects = db.Suspectses.Where(s => s.IsDeleted == false);
            ViewBag.SuspectID = new SelectList(suspects, "SuspectID", "SuspectMobile");
            return View();
        }
        /// <summary>
        /// 批量导入嫌疑人通话清单
        /// </summary>
        /// <param name="callrecord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportCallRecord(CallRecord callrecord)
        {
            try
            {
                string result = string.Empty;
                int successCount = 0;
                int totalCount = 0; // 待导入总条数   
                int msg = 0;
                string FileName;
                string savePath;
                string fileEx;

                HttpPostedFileBase file = Request.Files["files"];
                if (callrecord.SuspectID <= 0)
                {
                    result = "请先选择嫌疑人";
                    return Content("0|" + result);
                }

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
                OleDbDataAdapter myCommand = new OleDbDataAdapter("SELECT [对方号码],[呼叫类型],[通话时间], [通话时长] FROM [Sheet1$]", strConn);
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
                    int suspectID = callrecord.SuspectID;
                    DataRow dr = null;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        totalCount++;
                        dr = table.Rows[i];
                        if (insertCallRecordSQL(dr, "CallRecord", suspectID) == 1)
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
                        result = "1|成功导入" + successCount + "条记录。|/CallRecord/index";
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


        private int insertCallRecordSQL(DataRow dr, string tableName, int suspectID)
        {
            //excel表中的列名和数据库中的列名一定要对应  
            int flag = 0;
            var CalledMobile = dr["对方号码"].ToString();
            var LordCalled = dr["呼叫类型"].ToString();
            var CallTime = dr["通话时间"].ToString();
            var CallDuration = dr["通话时长"].ToString();
            if (!string.IsNullOrEmpty(CalledMobile) && !string.IsNullOrEmpty(LordCalled) && !string.IsNullOrEmpty(CallTime) && !string.IsNullOrEmpty(CallDuration))
            {
                string sql = "insert into " + tableName +
                    "([SuspectID],[CalledMobile],[LordCalled],[CallTime],[CallDuration],[IsDeleted],[CreateDate],[CreateUserID]) VALUES " +
                    "(" + suspectID + ",'" + CalledMobile + "','" + LordCalled + "','" + CallTime + "','" + CallDuration + "',0,GETDATE()," + CurrentUser.UID + ")";
                flag = db.Database.ExecuteSqlCommand(sql);
            }
            return flag;
        }

















        #endregion

        #region 嫌疑人通话记录比对
        /// <summary>
        /// 通话记录比对
        /// </summary>
        /// <returns></returns>
        public ActionResult CallRecordComp(int? pageIndex, int? pageSize, int? suspectId)
        {
            var suspects = db.Suspectses.Where(s => s.IsDeleted == false);
            ViewBag.SuspectID = new SelectList(suspects, "SuspectID", "SuspectMobile");

            if (suspectId <= 0)
            {
                ViewBag.errorMsg = "请先选择要比对的嫌疑人！";
                return View();
            }

            var recordList = db.CallRecords.Where(r => r.IsDeleted == false && r.SuspectID == suspectId).ToList();
            var usedworkers = db.UsedWorkers.Where(u => u.IsDeleted == false).ToList();

            List<CallRecord> recordResult = new List<CallRecord>();
            List<CallRecordUsedWorker> cruwResult = new List<CallRecordUsedWorker>();
            foreach (var item in recordList)
            {
                foreach (var usedworker in usedworkers)
                {
                    if (item.CalledMobile == usedworker.Phone1 || item.CalledMobile == usedworker.Phone2 || item.CalledMobile == usedworker.Phone3)
                    {
                        //recordResult.Add(item);
                        CallRecordUsedWorker cruw = new CallRecordUsedWorker();
                        cruw.CallRecord = item;
                        cruw.UsedWorker = usedworker;
                        cruwResult.Add(cruw);
                    }
                }
            }
            //var result = recordResult.ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            var result = cruwResult.ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            return View(result);
        }
        /// <summary>
        /// 嫌疑人通话记录比对
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="suspectId"></param>
        /// <returns></returns>

        public ActionResult _CallRecordComp(int? pageIndex, int? pageSize, int? suspectId)
        {
            if (suspectId <= 0)
            {
                ViewBag.errorMsg = "请先选择要比对的嫌疑人！";
                return View();
            }

            var recordList = db.CallRecords.Where(r => r.IsDeleted == false && r.SuspectID == suspectId).ToList();
            var usedworkers = db.UsedWorkers.Where(u => u.IsDeleted == false).ToList();

            List<CallRecord> recordResult = new List<CallRecord>();
            List<CallRecordUsedWorker> cruwResult = new List<CallRecordUsedWorker>();
            foreach (var item in recordList)
            {
                foreach (var usedworker in usedworkers)
                {
                    if (item.CalledMobile == usedworker.Phone1 || item.CalledMobile == usedworker.Phone2 || item.CalledMobile == usedworker.Phone3)
                    {
                        //recordResult.Add(item);
                        CallRecordUsedWorker cruw = new CallRecordUsedWorker();
                        cruw.CallRecord = item;
                        cruw.UsedWorker = usedworker;
                        cruwResult.Add(cruw);
                    }
                }
            }
            //var result = recordResult.ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            var result = cruwResult.ToPagedList(pageIndex ?? 1, pageSize ?? 10);
            return PartialView(result);
        }
        #endregion

        /// <summary>
        /// 判断通话记录是否存在
        /// </summary>
        /// <param name="calledMobile"></param>
        /// <param name="lordCalled"></param>
        /// <param name="callTime"></param>
        /// <param name="suspectID"></param>
        /// <returns></returns>
        public bool IsExistCallRecord(string calledMobile, string lordCalled, string callTime, int? suspectID)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(calledMobile) && !string.IsNullOrEmpty(lordCalled) && !string.IsNullOrEmpty(callTime) && suspectID.GetValueOrDefault() > 0)
            {
                var callRecords = db.CallRecords.Where(cr => cr.IsDeleted == false
                    && cr.CalledMobile.ToUpper().Equals(calledMobile.ToUpper())
                    && cr.LordCalled.ToUpper().Equals(lordCalled.ToUpper())
                    && cr.CallTime.ToUpper().Equals(callTime.ToUpper()));
                if (callRecords.Count() > 0)
                {
                    result = true;
                }
            }
            return result;
        }


    }
}