using LabelShop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Printing;
using static LabelShop.TLXLabelPaintCLS;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.X509;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using MySqlX.XDevAPI.Relational;
using Microsoft.VisualBasic;

namespace WebApplicationLocalAPI.Controllers
{

    /// <summary>
    /// 标签打印接口API
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LabelPrintController : ControllerBase
    {
        private int handle = 0;     //  用于保存标签模板的句柄
       
        /// <summary>
        ///标签打印初始化-注册信息
        /// </summary>
        /// <returns>ok ,return 版本信息or error infor</returns>
        [HttpGet(Name = "InitializeLable")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string InitializeLablePrintP()
        {
            TLXLabelPaintCLS.RET ret;
            TLXLabelPaintCLS.LICENSETYPE license = LICENSETYPE.TLXLP_LICENSE_ENTERPRISE;
            string licensetext = "";
            ret = TLXLabelPaintCLS.SetLicense("4G9CU-KPYUK-3H2UY-6K79Z-3DQJB");

            //  获取授权状态
            ret = TLXLabelPaintCLS.GetLicense(ref license);

            switch (license)
            {
                case TLXLabelPaintCLS.LICENSETYPE.TLXLP_LICENSE_BASIC:
                    licensetext = "授权模式：标准版";
                    break;
                case TLXLabelPaintCLS.LICENSETYPE.TLXLP_LICENSE_PROFESSIONAL:
                    licensetext = "授权模式：专业版";
                    break;
                case TLXLabelPaintCLS.LICENSETYPE.TLXLP_LICENSE_ENTERPRISE:
                    licensetext = "授权模式：企业版";
                    break;
                case TLXLabelPaintCLS.LICENSETYPE.TLXLP_LICENSE_NONE:
                    licensetext = "授权模式：未授权";
                    break;
            }
            return licensetext;
        }


        /// <summary>
        ///获得系统打印机名称
        /// </summary>
        /// <returns>List</returns>
        [HttpGet(Name = "GetPrinterName")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string  GetPrinterName()
        {
            TLXLabelPaintCLS.RET ret;
            List<string> printerstr = new List<string>();
            string strPrinters = "";
            ret = TLXLabelPaintCLS.GetSysPrinterNames(ref strPrinters);

            //  打印机名称是用 \n 分隔开的
            string[] sPrinterArray = strPrinters.Split(new char[] { '\n' });
            foreach (string sPrint in sPrinterArray)
                printerstr.Add(sPrint);
                
            return strPrinters;
        }


        /// <summary>
        ///打开标签模板
        /// </summary>
        /// <returns>string</returns>
        [HttpGet(Name = "OpenLabeTemplate")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string OpenLabeTemplate(string strFileName)
        {
            TLXLabelPaintCLS.RET ret;
            string errorinfor = "";
            ret = TLXLabelPaintCLS.OpenDocument(strFileName, ref handle);
            if (ret != TLXLabelPaintCLS.RET.TLXLP_OK)
            {
                errorinfor="OpenDoc_Failure";
                return errorinfor;
            }

            TLXLabelPaintCLS.DOCLEVEL level = TLXLabelPaintCLS.DOCLEVEL.TLXLP_DOCUMENT_BASIC;
            ret = TLXLabelPaintCLS.GetDocumentLevel(handle, ref level);

            if (ret != TLXLabelPaintCLS.RET.TLXLP_OK)
            {
                errorinfor = "OpenDoc_Failure_Level";
                return errorinfor;
                
            }
            else
            {
                switch (level)
                {
                    case TLXLabelPaintCLS.DOCLEVEL.TLXLP_DOCUMENT_BASIC:
                        errorinfor = "模板授权：未授权的模板";
                        break;
                    case TLXLabelPaintCLS.DOCLEVEL.TLXLP_DOCUMENT_PROFESSIONAL:
                        errorinfor = "模板授权：已授权的模板";
                        break;
                }
            }

            return errorinfor;
        }

        /// <summary>
        ///获得模板变量
        /// </summary>
        /// <returns>string</returns>

        [HttpGet(Name = "GetNamedVarNames")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string GetNamedVarNames()
        {
            string strNames = "";
            TLXLabelPaintCLS.RET ret;
            ret = TLXLabelPaintCLS.GetNamedVarNames(handle, ref strNames);

            return strNames;
        }

        /// <summary>
        ///设置模板变量的值
        /// </summary>
        /// <returns>string</returns>

        [HttpGet(Name = "SetNamedVariable")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string SetNamedVariable(string strVarName, string strVarValue)
        {

           
            if (handle == 0)
                return "please open labletemplate first";
            TLXLabelPaintCLS.RET ret;

            
            if (strVarName.Length <= 0)
                return "the length of varname is less than 0 ,please check";

            ret = TLXLabelPaintCLS.SetNamedVariable(handle, strVarName, strVarValue);

            return "ok";
        }

        [HttpGet(Name = "PrintLable")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string PrintLable(string strVarName, string strVarValue)
        {

            return "ok";
        }

        /// <summary>
        ///LablePrint for 特殊标签
        /// </summary>
        /// <returns>string</returns>
        [HttpGet(Name = "LableTranform")]
        [ApiExplorerSettings(GroupName = "v1")]
        public string LableTranform(string ArrivalOrder, string PartNumber,string QtySum,string LOT,string custcode)
        {

            InitializeLablePrintP();
            TimeSpan starttime = new TimeSpan(DateTime.Now.Ticks);
            string printflag = "P";
            if (ArrivalOrder==null|| ArrivalOrder=="")
            {

                return "NO,ArrivalOrder";
            }

            if (PartNumber == null || PartNumber == "")
            {
                return "NO,PartNumber";
            }

            if (QtySum == null || QtySum == "")
            {
                return "NO,QtySum";
            }

            QtySum=Convert.ToDouble(QtySum).ToString("0.000");
            if (custcode == null || custcode == "")
            {
                return "NO,custcode";
            }

            if (LOT!="" && LOT!="1")
            {
                printflag = "R";
            }


            string ERPsql = @"SELECT 'SH01' N'FCY',X5D.ITMREF_0 N'ITMREF',ITM.ITMDES1_0 N'ITMDES',X5D.AQTYUOM_0 N'QTY1',X5D.PUU_0 N'UOM1',
                           X5D.YCTRNUM_0 N'QTY2','KG'N'UOM2',X5D.YPALNUM_0 N'PALNUM',X5D.LOT_0 N'LOT',X5D.YBPSLOT_0 N'BPSLOT',
                           X5D.X5HNUM_0 N'VCRNUM',X5D.X5DLIN_0 N'VCRLIN',X5D.CREUSR_0 N'XCREUSR',LEFT(X5D.YPALNUM_0,4) N'PCK',
                           [PCKDES]=FORCE.get_pckdes_by_palnum(X5D.YPALNUM_0),X5H.BPSNUM_0+'/'+LEFT(X5D.YBPSLOT_0,5) N'BPANUM',
                           X5D.YPACQTY_0 N'TARE',N'物流部' N'WORKSTATION'
                           FROM FORCE.XCN05PTD X5D
                           LEFT JOIN FORCE.XCN05PTH X5H ON X5H.X5HNUM_0=X5D.X5HNUM_0
                           LEFT JOIN FORCE.ITMMASTER ITM ON X5D.ITMREF_0=ITM.ITMREF_0
                           WHERE X5D.X5HNUM_0='" + ArrivalOrder + "' and X5D.ITMREF_0='" + PartNumber + "'";


            DataTable dtTarget = new DataTable();
            DataTable dtTemp = new DataTable();
            DataBaseOperation dbo = new DataBaseOperation();
           
            TLXLabelPaintCLS.RET ret;
            string filename = "D:\\std_tag.lsdx";
            //string strPrinters = "TSC TTP-342M Pro";
            string strPrinters = "SHARP MX-M2608N PCL6";
            ret = TLXLabelPaintCLS.OpenDocument(filename, ref handle);
            


            dtTarget = dbo.SqlServeSqlQuery(ERPsql);
            if(dtTarget.Rows.Count<=0)
            {
                return "NO,ERP DATA";
            }

            for (int i=0;i< dtTarget.Rows.Count;i++)
            {
                string sqlmysqlcheck = @"select* from jql.r_lableinfor_t where VCRNUM = '" + ArrivalOrder + "' and BPANUM='CCFS/C"+ custcode.Trim() + "' AND ITMREF = '" + PartNumber + "' AND qty1=" + QtySum + "  and LOT = '" + dtTarget.Rows[i]["LOT"].ToString() + "'";
                dtTemp = dbo.MySqlQuery(sqlmysqlcheck);
               
                if (dtTemp.Rows.Count<=0)
                {
                    string mysqlinsert = @"insert into r_lableinfor_t (FCY,ITMREF,ITMDES,QTY1,UOM1,QTY2,UOM2,PALNUM,LOT,BPSLOT,VCRNUM,VCRLIN,XCREUSR,PCK, BPANUM,TARE,WORKSTATION,LableFlag,COPYDATE) values('" + dtTarget.Rows[i]["FCY"].ToString() + "','" + dtTarget.Rows[i]["ITMREF"].ToString() + "','" + dtTarget.Rows[i]["ITMDES"].ToString() + "'," + dtTarget.Rows[i]["QTY1"].ToString() + ",'" + dtTarget.Rows[i]["UOM1"].ToString() + "','" + dtTarget.Rows[i]["QTY2"].ToString() + "','" + dtTarget.Rows[i]["UOM2"].ToString() + "','" + dtTarget.Rows[i]["PALNUM"].ToString() + "','" + dtTarget.Rows[i]["LOT"].ToString() + "','" + dtTarget.Rows[i]["BPSLOT"].ToString() + "','" + dtTarget.Rows[i]["VCRNUM"].ToString() + "'," + dtTarget.Rows[i]["VCRLIN"] + ",'" + dtTarget.Rows[i]["XCREUSR"] + "','" + dtTarget.Rows[i]["PCK"] + "','" + dtTarget.Rows[i]["BPANUM"] + "','" + dtTarget.Rows[i]["TARE"] + "','" + dtTarget.Rows[i]["WORKSTATION"] + "','NO','"+ DateTime.Now.ToString() + "')";
                    dbo.MySQLExecuteNonQuery(mysqlinsert);
                }
                     
             }
            string sqlmysql;
            if (printflag=="P")
            {
                sqlmysql = @"select FCY,ITMREF,ITMDES,QTY1,'' AS YQTY1,UOM1,QTY2,'' AS YQTY2,UOM2,PALNUM,LOT,BPSLOT,VCRNUM,VCRLIN,XCREUSR,PCK,'外购包装' AS PCKDES,BPANUM,TARE,WORKSTATION,'' as OP,'' as NOTES from jql.r_lableinfor_t where VCRNUM = '" + ArrivalOrder + "' and BPANUM='CCFS/C"+ custcode.Trim() + "' AND ITMREF = '" + PartNumber + "' AND QTY1=" + QtySum + "  and LableFlag='NO' order by lot ";
            }
            else
            {
                sqlmysql = @"select FCY,ITMREF,ITMDES,QTY1,'' AS YQTY1,UOM1,QTY2,'' AS YQTY2,UOM2,PALNUM,LOT,BPSLOT,VCRNUM,VCRLIN,XCREUSR,PCK,'外购包装' AS PCKDES,BPANUM,TARE,WORKSTATION,'' as OP,'' as NOTES from jql.r_lableinfor_t where VCRNUM = '" + ArrivalOrder + "' and BPANUM='CCFS/C"+ custcode.Trim() + "' AND ITMREF = '" + PartNumber + "' AND QTY1=" + QtySum + "  and LableFlag='YES'  and LOT = '" + LOT + "' order by lot ";
            }
          
            DataTable tabletemp = new DataTable();
            tabletemp = dbo.MySqlQuery(sqlmysql);
            //DataColumn dc2 = new DataColumn("YQTY1", typeof(string));
            //dc2.DefaultValue = "";
            //tabletemp.Columns.Add(dc2);
            //DataColumn dc3 = new DataColumn("YQTY2", typeof(string));
            //dc3.DefaultValue = "";
            //tabletemp.Columns.Add(dc3);
            //DataColumn dc4 = new DataColumn("PCKDES", typeof(string));
            //dc4.DefaultValue = "";
            ///tabletemp.Columns.Add(dc4);

            //DataColumn dc5 = new DataColumn("OP", typeof(string));
            //dc5.DefaultValue = "";
            //tabletemp.Columns.Add(dc5);
           // DataColumn dc6 = new DataColumn("NOTES", typeof(string));
           // dc6.DefaultValue = "";
           // tabletemp.Columns.Add(dc6);

            if (tabletemp.Rows.Count<=0)
            {
                return "NO,MYSQL IS ALL USEED";
            }

               string jsonBuiler1=dbo.TableTOJson(tabletemp);
               
                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "json_tag", jsonBuiler1.ToString());

                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "ITMREF", tabletemp.Rows[0]["ITMREF"].ToString());
                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "ITMDES1", tabletemp.Rows[0]["ITMDES"].ToString());
                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "ITMDES2", "");
                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "ITMDES3", "");
                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "QTY1", tabletemp.Rows[0]["QTY1"].ToString());

                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "QTY2", tabletemp.Rows[0]["QTY2"].ToString());

                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "PALNUM", tabletemp.Rows[0]["PALNUM"].ToString());
                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "LOT", tabletemp.Rows[0]["LOT"].ToString());

                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "PCKDES", "外购包装");
                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "BPANUM", tabletemp.Rows[0]["BPANUM"].ToString());

                ret = TLXLabelPaintCLS.SetNamedVariable(handle, "TARE", Convert.ToDouble(tabletemp.Rows[0]["TARE"].ToString()).ToString("0.000"));

           TLXLabelPaintCLS.SetPrinterName(handle, strPrinters);
           ret = TLXLabelPaintCLS.OutputDocument(handle, 1, 1, 0);
            string strupdate;
            if (printflag == "P")
            {
                strupdate = @"update r_lableinfor_t set LableFlag = 'YES' , PRINTDATE='" + DateTime.Now.ToString() + "'  WHERE VCRNUM = '" + ArrivalOrder + "' and BPANUM='CCFS/C"+ custcode.Trim() + "' AND ITMREF = '" + PartNumber + "' AND qty1=" + QtySum + "  and LOT = '" + tabletemp.Rows[0]["LOT"].ToString() + "'";
            }
            else
            {
               strupdate = @"update r_lableinfor_t set  REPRINTDATE='" + DateTime.Now.ToString() + "'  WHERE VCRNUM = '" + ArrivalOrder + "' and BPANUM='CCFS/C"+ custcode.Trim() + "' AND ITMREF = '" + PartNumber + "' AND qty1=" + QtySum + "  and LOT = '" + tabletemp.Rows[0]["LOT"].ToString() + "' and LableFlag = 'YES' ";
            }

             dbo.MySQLExecuteNonQuery(strupdate);

            //TimeSpan endtime = new TimeSpan(DateTime.Now.Ticks);
            //TimeSpan ts = endtime.Subtract(starttime).Duration();
            dbo.ClostMysqlCON();
            //string spantime = ts.Hours.ToString() + " hours" + ts.Minutes.ToString() + " minutes" + ts.Seconds.ToString() + " second" + ts.Milliseconds.ToString();
                return tabletemp.Rows[0]["LOT"].ToString();
        }
            }


               
              
}
