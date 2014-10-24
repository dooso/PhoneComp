using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Security.Cryptography;
using System.Web.Security;
using System.Collections;
using System.IO;

namespace PhoneComp.Lib
{
    public class Character
    {
        ///   <summary>   
        ///   去除所有HTML标记   
        ///   </summary>
        public static string ClearHtml(string strHtml)
        {
            if (!string.IsNullOrEmpty(strHtml))
            {
                strHtml = Regex.Replace(strHtml, @"<\/?[^>]*>", "", RegexOptions.Multiline);
            }
            return strHtml;
        }
        ///   <summary>   
        ///   去除样式，并设置字体的大小 
        ///   </summary>
        public static string ClearHtmlSetFontSize(string strHtml, int fontsize)
        {
            if (!string.IsNullOrEmpty(strHtml))
            {
                strHtml = Regex.Replace(strHtml, @"<P>", "~", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"<p>", "~", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"</P>", "/~", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"</p>", "/~", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"<BR>", "^", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"<br>", "^", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"<br />", "^", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"<BR />", "^", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"<BR />", "^", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"<\/?[^>]*>", "", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"/~", "</P>", RegexOptions.Multiline);
                strHtml = Regex.Replace(strHtml, @"~", "<P>", RegexOptions.Multiline);
                strHtml = strHtml.Replace("^", "<br/>");
            }
            if (strHtml != string.Empty)
                strHtml = "<span style='font-size:" + fontsize + "pt;'>" + strHtml + "</span>";
            return strHtml;
        }
        /// <summary>
        /// 把指定的datatable某列拼成为1,2,3,4,5....
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="RowName"></param>
        /// <returns></returns>
        public static string DataTableToString(DataTable dt, string RowName, string _char)
        {
            string str = "";
            string _c = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                str += _c + dt.Rows[i][RowName];
                _c = _char;
            }
            return str;
        }
        /// <summary>
        /// 把string[]转换成string 如["1","2","3"...] to "1,2,3..."
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ArrayToString(string[] list)
        {
            string str = "";
            for (int i = 0; i < list.Length; i++)
            {
                str += "," + list[i];
            }
            return str.Substring(1);
        }

        /// <summary>
        /// 把datatable转换成string[]
        /// </summary>
        /// <param name="p_Table"></param>
        /// <param name="RowName"></param>
        /// <returns></returns>
        public static string[] DataTableToArray(DataTable p_Table, string RowName)
        {
            string[] _ReturnText = new string[p_Table.Rows.Count];

            for (int i = 0; i != p_Table.Rows.Count; i++)
            {
                _ReturnText[i] = p_Table.Rows[i][RowName].ToString();
            }
            return _ReturnText;
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="obj">截取的对象</param>
        /// <param name="i">截取的长度</param>
        /// <returns>返回截取对象＋。。。</returns>
        public static string SubString(object obj, int length)
        {
            length = length * 2;
            string temp = ClearHtml(obj.ToString());
            int j = 0;
            int k = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
                {
                    j += 2;
                }
                else
                {
                    j += 1;
                }
                if (j <= length)
                {
                    k += 1;
                }
                if (j >= length)
                {
                    return temp.Substring(0, k) + "...";
                }
            }
            return temp;
        }

        #region 输出json
        /// <summary>
        /// 输出json
        /// </summary>
        public static string StringToJson(string key, string value)
        {
            return "{ \"" + key + "\":\"" + value + "\"}";
        }
        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        
            if (dt != null && dt.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return "";
            }
        }
        public static string DataTableToJsonFristDefault(DataTable dt)
        {
            StringBuilder JsonString = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                return JsonString.ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 内容分页
        /// <summary>
        /// 文章内容自动分页
        /// </summary>
        public static string ContentByPage(out string spanPage, string Body, int? Size, int? CurrentPage, string Url)
        {
            int m_intPageSize = Size ?? 10000;//文章每页大小
            int m_intCurrentPage = CurrentPage ?? 1;//设置第一页为初始页
            int m_intTotalPage = 0;
            int m_intArticlelength = Body.Length;//文章长度
            string m_strRet = "";
            spanPage = "";
            if (m_intPageSize < m_intArticlelength)
            {//如果每页大小大于文章长度时就不用分页了
                if (m_intArticlelength % m_intPageSize == 0)
                {//set total pages count 
                    m_intTotalPage = m_intArticlelength / m_intPageSize;
                }
                else
                {//if the totalsize
                    m_intTotalPage = m_intArticlelength / m_intPageSize + 1;
                }
                try
                {
                    if (m_intCurrentPage > m_intTotalPage)
                        m_intCurrentPage = m_intTotalPage;
                }
                catch
                {
                    m_intCurrentPage = m_intCurrentPage;
                }
                //set the page content 设置获取当前页的大小
                m_intPageSize = m_intCurrentPage < m_intTotalPage ? m_intPageSize : (m_intArticlelength - m_intPageSize * (m_intCurrentPage - 1));
                m_strRet = Body.Substring(m_intPageSize * (m_intCurrentPage - 1), m_intPageSize);
                string m_strPageInfo = "<p></p>";
                if (m_intTotalPage > 1)
                {
                    for (int i = 1; i <= m_intTotalPage; i++)
                    {
                        if (i == m_intCurrentPage)
                            m_strPageInfo += "<b>" + i + "</b>｜";
                        else
                            m_strPageInfo += "<a href=\"" + Url + "page=" + i + "\">" + i + "</a>｜";
                    }
                }
                if (m_intCurrentPage > 1)
                {
                    m_strPageInfo = " <a href=\"" + Url + "page=" + (m_intCurrentPage - 1) + "\">上一页 </a>" + m_strPageInfo;
                }

                if (m_intCurrentPage < m_intTotalPage)
                {
                    m_strPageInfo += " <a href=\"" + Url + "page=" + (m_intCurrentPage + 1) + "\">下一页 </a>";
                }
                //输出显示各个页码
                spanPage = m_strPageInfo;
            }
            return m_strRet;
        }
        /// <summary>
        /// 根据分隔符自动分页
        /// </summary>
        public static string ContentByPage(out string spanPage, string Body, string strSplit, int? CurrentPage, string Url)
        {
            string m_strRet = "";
            spanPage = "";
            //设置第一页为初始页
            int m_intCurrentPage = CurrentPage ?? 1;
            //设置显示页数
            int m_intTotalPage = StringSplit(Body, strSplit).Length;

            string[] strContent = StringSplit(Body, strSplit);

            if (m_intCurrentPage > m_intTotalPage)
                m_intCurrentPage = m_intTotalPage;
            else if (m_intCurrentPage < 1)
                m_intCurrentPage = 1;

            m_strRet += strContent[m_intCurrentPage - 1].ToString();
            string m_strPageInfo = "";

            if (m_intTotalPage > 1)
            {
                for (int i = 1; i <= m_intTotalPage; i++)
                {
                    if (i == m_intCurrentPage)
                    {
                        m_strPageInfo += "[" + i + "]";
                    }
                    else
                    {
                        m_strPageInfo += " <a href=\"" + Url + "page=" + i + "\">[" + i + "] </a> ";
                    }
                }
            }
            if (m_intCurrentPage > 1)
            {
                m_strPageInfo = " <a href=\"" + Url + "page=" + (m_intCurrentPage - 1) + "\">上一页 </a>" + m_strPageInfo;
            }

            if (m_intCurrentPage < m_intTotalPage)
            {
                m_strPageInfo += " <a href=\"" + Url + "page=" + (m_intCurrentPage + 1) + "\">下一页 </a>";
            }
            //输出显示各个页码 
            spanPage = " <p> </p>" + m_strPageInfo;

            return m_strRet;
        }
        #endregion

        #region 将字符串分割成数组

        /// <summary>
        /// 将字符串分割成数组
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strSplit"></param>
        /// <returns></returns>
        private static string[] StringSplit(string strSource, string strSplit)
        {
            string[] strtmp = new string[1];
            int index = strSource.IndexOf(strSplit, 0);
            if (index < 0)
            {
                strtmp[0] = strSource;
                return strtmp;
            }
            else
            {
                strtmp[0] = strSource.Substring(0, index);
                return StringSplit(strSource.Substring(index + strSplit.Length), strSplit, strtmp);
            }
        }
        /// <summary>
        /// 采用递归将字符串分割成数组
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strSplit"></param>
        /// <param name="attachArray"></param>
        /// <returns></returns>
        private static string[] StringSplit(string strSource, string strSplit, string[] attachArray)
        {
            string[] strtmp = new string[attachArray.Length + 1];
            attachArray.CopyTo(strtmp, 0);
            int index = strSource.IndexOf(strSplit, 0);
            if (index < 0)
            {
                strtmp[attachArray.Length] = strSource;
                return strtmp;
            }
            else
            {
                strtmp[attachArray.Length] = strSource.Substring(0, index);
                return StringSplit(strSource.Substring(index + strSplit.Length), strSplit, strtmp);
            }
        }
        #endregion

        #region ========加密========

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "litianping");
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密========


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "litianping");
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        #region Base64加密、解密
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncryptBase64(string code)
        {
            string encode = "";
            byte[] bytes = Encoding.Default.GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecryptBase64(string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.Default.GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        #endregion

        #region MD5加密、密码比较
        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="inputedPassword"></param>
        /// <returns></returns>
        public static string EncrytPassword(string inputedPassword)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(inputedPassword, "MD5");
        }
        /// <summary>
        /// 密码比较
        /// </summary>
        /// <param name="inputedPassword"></param>
        /// <param name="currentPassword"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string inputedPassword, string currentPassword)
        {
            string encryptedPassword = EncrytPassword(inputedPassword);
            return (encryptedPassword == currentPassword.ToUpper()) ? true : false;
        }
        #endregion

        #region 半角/全角 字符转换
        /// <summary>
        /// 全角=>半角
        /// </summary>
        /// <param name="src"></param>
        /// <returns>半角字符串</returns>
        public static string SBCToDBC(string src)
        {
            if (src == null || src == string.Empty)
                return string.Empty;

            char[] c = src.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }
        /// <summary>
        /// 半角=>全角
        /// </summary>
        /// <param name="src"></param>
        /// <returns>全角字符串</returns>
        public static string DBCToSBC(string src)
        {
            if (src == null || src == string.Empty)
                return string.Empty;

            char[] c = src.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 0)
                    {
                        b[0] = (byte)(b[0] - 32);
                        b[1] = 255;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }
        #endregion

        #region  生成制定长度的随机数
        /// <summary>
        /// 生成制定长度的随机数
        /// </summary>
        /// <param name="type">all:包括数字、大小写字母；num：0~9数字；lower：小写字母；upper：大写字母；lower&upper:大小写字母</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(string type, int length)
        {
            string allChar = "";
            if (type == "all")
                allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            else if (type == "num")
                allChar = "0,1,2,3,4,5,6,7,8,9";
            else if (type == "lower")
                allChar = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            else if (type == "upper")
                allChar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            else if (type == "lower&upper")
                allChar = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string RandomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
                }

                int t = rand.Next(allCharArray.Length);

                while (temp == t)
                {
                    t = rand.Next(allCharArray.Length);
                }

                temp = t;
                RandomCode += allCharArray[t];
            }

            return RandomCode;
        }
        #endregion

        #region 防止sql注入
        public static string ReplaceSqlKey(string text, int maxlength)
        {
            text = text.ToLower().Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (text.Length > maxlength)
                text = text.Substring(0, maxlength);

            text = Regex.Replace(text, "'", "");
            text = Regex.Replace(text, "\r\n", "");
            text = Regex.Replace(text, ";", "");
            return text;
        }
        #endregion

        #region 计算时间差值
        public static string DiffDate(DateTime dt1, DateTime dt2)
        {

            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(dt1.Ticks);
                TimeSpan ts2 = new TimeSpan(dt2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                if (ts.Days <= 3)
                {

                    if (ts.Minutes > 0)
                    {
                        dateDiff = ts.Minutes.ToString() + "分钟" + dateDiff;
                    }
                    if (ts.Hours > 0)
                    {
                        dateDiff = ts.Hours.ToString() + "小时" + dateDiff;
                    }
                    if (ts.Days > 0)
                    {
                        dateDiff = ts.Days.ToString() + "天" + dateDiff;
                    }
                    if (ts.Seconds > 0 && ts.TotalMinutes < 1)
                    {
                        dateDiff = ts.Seconds.ToString() + "秒";
                    }
                    dateDiff += "前";
                }
                else
                {
                    dateDiff = dt2.ToString("yyyy-MM-dd");
                }
            }
            catch (Exception e)
            {

            }
            return dateDiff;
        }
        #endregion

        /// <summary>
        /// 生成随访数
        /// </summary>
        /// <param name="min">最小</param>
        /// <param name="max">最大</param>
        /// <returns></returns>
        public static string Random(int min, int max)
        {
            return Convert.ToInt32(new Random().Next(min, max)).ToString();
        }
       
        public static string GetDecmail(decimal? data)
        {
            string result = "0";
            if (data != null && data > 0)
                result = data.Value.ToString("#.#");
            return result;
        }
        
      
        /// <summary>
        /// 将 Stream 转成 byte[]
        /// </summary>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        /// <summary>
        /// 将 byte[] 转成 Stream
        /// </summary>
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public static string ImageServer
        {
            get {
                if (ConfigurationSettings.AppSettings["ImageService"] != null)
                {
                    return ConfigurationSettings.AppSettings["ImageService"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            } 
        }

        public static string PayResultUrl
        {
            get {
                if (ConfigurationSettings.AppSettings["PayResult"] != null)
                {
                    return ConfigurationSettings.AppSettings["PayResult"].ToString();
                }
                else {
                    return string.Empty;
                }
            }
        }

        public static string DNSUrl
        {
            get
            {
                if (ConfigurationSettings.AppSettings["DNSUrl"] != null)
                {
                    return ConfigurationSettings.AppSettings["DNSUrl"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static string PayUrl
        {
            get
            {
                if (ConfigurationSettings.AppSettings["PayUrl"] != null)
                {
                    return ConfigurationSettings.AppSettings["PayUrl"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }


        public static string GetTradeNo(string tradeTitle)
        {
            return tradeTitle + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }

    public class ValueData
    {
        public int DisNo { get; set; }
        public string Searchchar { get; set; }
        public string address { get; set; }
        public string Distruename { get; set; }
    }
}
