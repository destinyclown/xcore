//------------------------------------------------------------------------------
//	文件名称：System\strUtil.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:15:20
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using System.Web.Security;
using System.Text;
using System.Management;
namespace System {
    /// <summary>
    /// 其它工具栏
    /// </summary>
    public partial class Util
    {
        /// <summary>
        /// 获取机器码（根据XCore定制的规则）
        /// </summary>
        /// <returns>当前主机的机器码</returns>
        public static String GetComputerId()
        {
            String DiskDriveid = String.Empty;
            String CPUid = String.Empty;
            ManagementClass mcdisk = new ManagementClass(strUtil.GetStringByUnicode(@"\u0057\u0069\u006e\u0033\u0032\u005f\u0044\u0069\u0073\u006b\u0044\u0072\u0069\u0076\u0065"));
            foreach (ManagementObject mo in mcdisk.GetInstances())
            {
                DiskDriveid = (string)mo.Properties["Model"].Value;
                break;
            }
            ManagementClass mccpu = new ManagementClass(strUtil.GetStringByUnicode(@"\u0057\u0069\u006e\u0033\u0032\u005f\u0050\u0072\u006f\u0063\u0065\u0073\u0073\u006f\u0072"));
            foreach (ManagementObject mo in mccpu.GetInstances())
            {
                CPUid = (string)mo.Properties["ProcessorId"].Value;
                break;
            }
            Int64 diskid = DiskDriveid.GetHashCode();
            Int64 cpuid = CPUid.GetHashCode();
            Int64 computerid = ((diskid > 0 ? diskid : -diskid) % 10000000) * ((cpuid > 0 ? cpuid : -cpuid) % 10000000);
            if (computerid > 100000000000)
                computerid = computerid % 1000000000000;
            else if (computerid < 100000000000)
                computerid += 100000000000;
            return computerid.ToString();
        }
        private static RegisterInfo _RegisterInfo = new RegisterInfo();
        /// <summary>
        /// 获取当前客户端注册信息
        /// </summary>
        /// <returns></returns>
        public static RegisterInfo GetRegister()
        {
            return GetRegister(Data.KvTableUtil.GetString("SoftCode"));
        }
        public static RegisterInfo GetRegister(String softcode)
        {
            if (!_RegisterInfo.Success)
            {
                string filename = PathHelper.Map(cfgHelper.WebRoot + "Wlniao.dll");
                if (!file.Exists(filename))
				{
                    string content = "Sorry,您的程序需要注册！\t\t";
                    content += "\n\n机器码：" + GetComputerId() + "\t\t";
					content += "\n\n请联系开发商进行注册\t\t";
                    //content += "\n电话：13075463371\t\t";
                    //content += "\n网址：http://www.wlniao.com\t\t";
					System.IO.StreamWriter sw = new System.IO.StreamWriter(PathHelper.Map(cfgHelper.WebRoot + "Wlniao.dll"));
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    string content = "";
                    try
                    {
                        System.IO.StreamReader sr = new System.IO.StreamReader(filename);
                        //content = sr.ReadToEnd();
                        //content = "";
                        for (int i = 0; i < 9; i++)
                        {
                            if (i >= 3 && i < 7)
                            {
                                content += sr.ReadLine();
                            }
                            else
                            {
                                sr.ReadLine();
                            }
                        }
                        int length = Convert.ToInt32(content.Substring(0, 4));
                        content = content.Substring(4, length);
                        if (string.IsNullOrEmpty(softcode))
                        {
                            softcode = "";
                        }
                        content = Encryptor.DesDecrypt(content, GetComputerId() + softcode);
                        _RegisterInfo = Json.ToObject<RegisterInfo>(content);
                    }
                    catch { }
                }
            }
            return _RegisterInfo;
        }
		public static void Register(string computerid,bool rightone,bool righttwo,string config,DateTime endtime,string filename,string softcode){
			RegisterInfo ri=new RegisterInfo();
			ri.RightOne=rightone;
			ri.RightTwo=righttwo;
			ri.Config=config;
			ri.EndTiem=endtime;
			string content=Json.ToStringEx (ri);
            if (string.IsNullOrEmpty(softcode))
            {
                softcode = "";
            }
            content = Encryptor.DesEncrypt(content, computerid + softcode);
			content=content.Length.ToString ().PadLeft (4,'0')+content+Rand.Str_char(1020,true).ToUpper();
			content=content.Substring (0,1024);

			if(file.Exists(filename)){
				file.Delete (filename);
			}
			System.IO.StreamWriter sw=new System.IO.StreamWriter(filename);
			sw.WriteLine (Rand.Str_char(256,true).ToUpper());
			sw.WriteLine (Rand.Str_char(256,true).ToUpper());
			sw.WriteLine (Rand.Str_char(256,true).ToUpper());
			sw.WriteLine (content.Substring(0,256));
			sw.WriteLine (content.Substring(256,256));
			sw.WriteLine (content.Substring(512,256));
			sw.WriteLine (content.Substring(768,256));
			sw.WriteLine (Rand.Str_char(256,true).ToUpper());
			sw.WriteLine (Rand.Str_char(256,true).ToUpper());
			sw.Flush();
			sw.Close();
		}

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式 HW:H:W:Cut</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
            try
            {
                //以jpg格式保存缩略图
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(thumbnailPath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(thumbnailPath));
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }
	public class RegisterInfo
	{
		public bool Success
		{
			get{return DateTools.GetNow()<_EndTiem;}
		}
		private DateTime _EndTiem;
        public DateTime EndTiem
        {
            get { return _EndTiem; }
            internal set { _EndTiem = value; }
        }
		private bool _RightOne;
		public bool RightOne
		{
			get{return _RightOne;}
            internal set { _RightOne = value; }
		}
		private bool _RightTwo;
		public bool RightTwo
		{
			get{return _RightTwo;}
            internal set { _RightTwo = value; }
		}
		private string _Config;
		public string Config
		{
			get{return _Config;}
            internal set { _Config = value; }
		}
		public RegisterInfo(){
			_EndTiem=DateTime.MinValue;
			_RightOne=false;
			_RightTwo=false;
			_Config="";
		}
	}
}

