using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

/*
TLXLabelPaint
LabelShop API 开发接口
V 6.10
2019-08-07
京成云马（北京）科技有限公司
http://www.360Code.com

更详细的接口说明，请参考 C++ 的接口定义

如果开发中有任何问题，请发邮件到：
Yanhs@360Code.com
*/
namespace LabelShop
{
    public class TLXLabelPaintCLS
    {
        //  返回值和错误代码
        public enum RET
        {
            TLXLP_OK = 0,               //  成功返回
            TLXLP_ERROR,                //  出错
            TLXLP_INVALIDDOCUMENT,      //  无效的文档
            TLXLP_OPENDOCUMENT,         //  打开文档失败
            TLXLP_SAVEDOCUMENT,         //  保存文档失败
            TLXLP_NOTFOUNDRECORD,       //  未查找到数据库记录
            TLXLP_PRINTLABELS,          //  打印标签失败
            TLXLP_INVALIDVARIABLE,      //  无效的变量
            TLXLP_BUFFERTOOSMALL,       //  缓冲区内存空间太小
            TLXLP_INVALIDLICENSEKEY,    //  无效的授权码
            TLXLP_UNAUTHORIZED,         //  未授权
            TLXLP_NOTFOUNDPRINTER,      //  未找到目标打印机
            TLXLP_NOTFOUNDLABELSHOP,    //  没有安装 LabelShop
            TLXLP_VERSIONTOOLOW,        //  LabelShop 版本太低，需要 LabelShop 5.21 或者更高的版本
            TLXLP_NOTCHANGE,            //  未发生改变
            TLXLP_INVALIDROWNUMBER,     //	无效的记录行号	
            TLXLP_NONSUPPORT = 255      //  不支持的功能
        }

        //  授权级别
        public enum LICENSETYPE
        {
            TLXLP_LICENSE_NONE,         //  未授权
            TLXLP_LICENSE_BASIC,        //  标准版授权
            TLXLP_LICENSE_PROFESSIONAL, //  专业版授权
            TLXLP_LICENSE_ENTERPRISE    //  企业版授权
        }

        //  文档等级
        public enum DOCLEVEL
        {
            TLXLP_DOCUMENT_BASIC,       //  标准版模板
            TLXLP_DOCUMENT_PROFESSIONAL //  专业版模板
        }

        //  标签类型
        public enum LABELTYPE
        {
            TLXLP_LABELTYPE_ROLL,       //  卷筒标签
            TLXLP_LABELTYPE_PAGE        //  A4页式标签
        }

        //	略图选项
        public const int TLXLP_THUMB_RECTLABELFRAME  = 1;    //  画直角边框
        public const int TLXLP_THUMB_CUTLABELFRAME   = 2;    //  标签略图仅画标签内部的部分
        public const int TLXLP_THUMB_WORKSPACECOLOR  = 4;    //  使用系统工作区背景色，而非白色背景


        //  初始化库
        [DllImport("TLXLabelPaint.dll", EntryPoint= "TLXInitLibrary", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET InitLibrary();


        //  设置授权
        [DllImport("TLXLabelPaint.dll", EntryPoint= "TLXSetLicense", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET SetLicense(string key);

        //	获取授权信息
        [DllImport("TLXLabelPaint.dll", EntryPoint= "TLXGetLicense", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET GetLicense(ref LICENSETYPE license);

        //  模板的等级
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXGetDocumentLevel", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET GetDocumentLevel(int handle, ref DOCLEVEL level);

        //  打开模板文档
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXOpenDocument", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET OpenDocument(string lpszFileName, ref int handle);

        //  打开XML模板
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXOpenDocumentXML", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET OpenDocumentXML(string strXML, ref int handle, bool bOpenDatabase);

        //  获取模板XML，内部使用
        [DllImport("TLXLabelPaint.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern RET TLXGetDocumentXML(int handle, ref IntPtr pXML);
        //  获取模板XML
        public static RET GetDocumentXML(int handle, ref string strXML)
        {
            IntPtr inStr = new IntPtr();
            RET ret = TLXGetDocumentXML(handle, ref inStr);

            strXML = Marshal.PtrToStringUni(inStr);

            return ret;
        }

        //  保存模板文档
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXSaveDocument", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET SaveDocument(int handle);

        //  另存模板文档
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXSaveAsDocument", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET SaveAsDocument(int handle, string strPathName);

        //  关闭模板文档
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXCloseDocument", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET CloseDocument(int handle);
        
        //  设置变量内容
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXSetNamedVariable", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET SetNamedVariable(int handle, string strVarName, string strNewValue);

        //  获取命名变量名称，内部使用
        [DllImport("TLXLabelPaint.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern RET TLXGetNamedVarNames(int handle, ref IntPtr pNames);
        //  获取命名变量名称
        public static RET GetNamedVarNames(int handle, ref string strNames)
        {
            IntPtr inStr = new IntPtr();
            RET ret = TLXGetNamedVarNames(handle, ref inStr);

            strNames = Marshal.PtrToStringUni(inStr);

            return ret;
        }

        //  获取命名变量值，内部使用
        [DllImport("TLXLabelPaint.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern RET TLXGetNamedVarValue(int handle, string strVarName, ref IntPtr pValue);
        //  获取命名变量值
        public static RET GetNamedVarValue(int handle, string strVarName, ref string strValue)
        {
            IntPtr inStr = new IntPtr();
            RET ret = TLXGetNamedVarValue(handle, strVarName, ref inStr);

            strValue = Marshal.PtrToStringUni(inStr);

            return ret;
        }

        //  打印模板文档
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXOutputLabel", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET OutputLabel(int handle, int copy, bool bAutoClose);

        //  打印模板文档
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXOutputDocument", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET OutputDocument(int handle, int qty, int copy, int err);

        //  预览模板文档
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXPreviewDocument", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET PreviewDocument(int handle, int qty, int copy, int err);

        //  获取打印机名称，内部使用
        [DllImport("TLXLabelPaint.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern RET TLXGetPrinterName(int handle, ref IntPtr pPrinter);
        //  获取打印机名称
        public static RET GetPrinterName(int handle, ref string strPrinter)
        {
            IntPtr inStr = new IntPtr();
            RET ret = TLXGetPrinterName(handle, ref inStr);

            strPrinter = Marshal.PtrToStringUni(inStr);

            return ret;
        }

        //  设置目标打印机
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXSetPrinterName", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET SetPrinterName(int handle, string strPrinter);

        //  设置打印机属性
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXPrinterProperties", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET PrinterProperties(int handle, IntPtr hWnd);

        //  获取LabelShop支持的打印机列表，内部使用
        [DllImport("TLXLabelPaint.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private static extern RET TLXGetSysPrinterNames(ref IntPtr pPrinters);
        //  获取打印机名称
        public static RET GetSysPrinterNames(ref string strPrinters)
        {
            IntPtr inStr = new IntPtr();
            RET ret = TLXGetSysPrinterNames(ref inStr);

            strPrinters = Marshal.PtrToStringUni(inStr);

            return ret;
        }

        //  获取标签略图，内部使用
        [DllImport("TLXLabelPaint.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern RET TLXGetThumbnail(int handle, ref IntPtr hbit, int cx);
        //  获取标签略图
        public static RET GetThumbnail(int handle, ref Bitmap bit, int cx)
        {
            IntPtr hBitmap = new IntPtr();

            RET ret = TLXGetThumbnail(handle, ref hBitmap, cx);
            bit = (Bitmap)Bitmap.FromHbitmap(hBitmap);
            return ret;
        }

        //  获取标签略图，内部使用
        [DllImport("TLXLabelPaint.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern RET TLXGetThumbnailEx(int handle, ref IntPtr hbit, int x, int y, int option, int varid);
        //  获取标签略图
        public static RET GetThumbnailEx(int handle, ref Bitmap bit, int cx, int cy, int option , int varid)
        {
            IntPtr hBitmap = new IntPtr();

            RET ret = TLXGetThumbnailEx(handle,ref hBitmap, cx, cy, option, varid);
            if (ret != RET.TLXLP_OK)
                return ret;

            bit = (Bitmap)Bitmap.FromHbitmap(hBitmap);
            return ret;
        }

        //  获取文档的标签布局格式
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXGetDocumentFormat", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET GetDocumentFormat(int handle, ref LABELTYPE type, ref int width, ref int height, ref int cols, ref int rows);

        //  编辑模板
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXEditTemplate", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET EditTemplate(string strFileName);

        //  编辑模板，内部使用
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXEditTemplateXML", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern RET TLXEditTemplateXML(string strXML, ref IntPtr pNewXML);
         //  编辑模板
       public static RET EditTemplateXML(string strXML, ref string strNewXML)
        {
            IntPtr inStr = new IntPtr();
            RET ret = TLXEditTemplateXML(strXML, ref inStr);

            strNewXML = Marshal.PtrToStringUni(inStr);

            return ret;
        }

        //  更新变量的内容
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXAfterPrintChange", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET AfterPrintChange(int handle, int qty, int copy, int option);

        //  设置标签偏移
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXSetLabelOffect", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET SetLabelOffect(int handle, int x, int y);

        //  清空命名变量数据集
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXNamedVarsClearData", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET NamedVarsClearData(int handle);

        //  设置命名变量数据集数据
        [DllImport("TLXLabelPaint.dll", EntryPoint = "TLXNamedVarsAddVarData", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern RET NamedVarsAddVarData(int handle, int row ,string strName, string strValue);

    }
}
