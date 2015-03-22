//------------------------------------------------------------------------------
//	文件名称：System\IO\FileEx.cs
//	运 行 库：2.0.50727.1882
//	最后修改：2012年9月8日 22:13:18
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Drawing;
namespace System.IO {
    /// <summary>
    /// 封装了文件常用操作方法
    /// </summary>
    public class FileEx
    {
        /// <summary>
        /// 读取文件的内容(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <returns>文件的内容</returns>
        public static byte[] ReadByte(String absolutePath)
        {
            try
            {
                FileStream fs = new FileStream(absolutePath, FileMode.Open);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                return buffer;
            }
            catch (Exception ex)
            {
            }
            return new byte[] { };
        }
        /// <summary>
        /// 读取文件的内容(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <returns>文件的内容</returns>
        public static void WriteByte(String absolutePath,byte[] buffer)
        {
            try
            {
                FileStream fs = new FileStream(absolutePath, FileMode.Create);                
                StreamWriter sw = new StreamWriter(absolutePath, false);
                sw.Write(buffer);
                sw.Close();

                MemoryStream ms = new MemoryStream(buffer);
                Bitmap bmp = new Bitmap(ms);
                //bmp.Save(txtFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(txtFileName + ".bmp", ImageFormat.Bmp);
                //bmp.Save(txtFileName + ".gif", ImageFormat.Gif);
                //bmp.Save(txtFileName + ".png", ImageFormat.Png);

            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 读取文件的内容(采用UTF8编码)
        /// </summary>
        /// <param name="stream">文件的绝对路径</param>
        /// <returns>文件的内容</returns>
        public static void WriteStream(String absolutePath, System.IO.Stream stream)
        {
            try
            {
                FileStream fs = new FileStream(absolutePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(absolutePath, false);
                sw = new StreamWriter(stream);
                sw.Flush();
                sw.Close();

            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 读取文件的内容(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <returns>文件的内容</returns>
        public static void WriteByteGif(String absolutePath, byte[] buffer)
        {
            try
            {
                MemoryStream ms = new MemoryStream(buffer);
                Bitmap bmp = new Bitmap(ms);
                //bmp.Save(txtFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(txtFileName + ".bmp", ImageFormat.Bmp);
                if (file.Exists(absolutePath))
                {
                    file.Delete(absolutePath);
                }
                bmp.Save(absolutePath, System.Drawing.Imaging.ImageFormat.Gif);
                //bmp.Save(txtFileName + ".png", ImageFormat.Png);

            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 读取文件的内容(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <returns>文件的内容</returns>
        public static void WriteByteJpg(String absolutePath, byte[] buffer)
        {
            try
            {
                MemoryStream ms = new MemoryStream(buffer);
                Bitmap bmp = new Bitmap(ms);
                bmp.Save(absolutePath, System.Drawing.Imaging.ImageFormat.Jpeg);

            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 读取文件的内容(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <returns>文件的内容</returns>
        public static String Read(String absolutePath)
        {
            return Read(absolutePath, Encoding.UTF8);
        }
        /// <summary>
        /// 读取文件的内容(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="autoCreateDir">读取文件事，如目录不存在是否自动创建目录</param>
        /// <returns>文件的内容</returns>
        public static String Read(String absolutePath, Boolean autoCreateDir)
        {
            return Read(absolutePath, Encoding.UTF8, autoCreateDir);
        }
        /// <summary>
        /// 以某种编码方式，读取文件的内容
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>文件的内容</returns>
        public static String Read( String absolutePath, Encoding encoding ) {
            return Read(absolutePath, Encoding.UTF8, false);
        }
        /// <summary>
        /// 以某种编码方式，读取文件的内容
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="autoCreateDir">是否自动创建目录</param>
        /// <returns>文件的内容</returns>
        public static String Read(String absolutePath, Encoding encoding, Boolean autoCreateDir)
        {
            if (autoCreateDir)
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(absolutePath));
            }
            using (StreamReader reader = new StreamReader(absolutePath, encoding))
            {
                return reader.ReadToEnd();
            }
        }
        /// <summary>
        /// 读取文件各行内容(采用UTF8编码)，以数组形式返回
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <returns>文件各行内容</returns>
        public static String[] ReadAllLines( String absolutePath ) {
            return ReadAllLines( absolutePath, Encoding.UTF8 );
        }
        /// <summary>
        /// 以某种编码方式，读取文件各行内容(采用UTF8编码)，以数组形式返回
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>文件各行内容</returns>
        public static String[] ReadAllLines( String absolutePath, Encoding encoding ) {
            ArrayList list = new ArrayList();
            using (StreamReader reader = new StreamReader( absolutePath, encoding )) {
                String str;
                while ((str = reader.ReadLine()) != null) {
                    list.Add( str );
                }
            }
            return (String[])list.ToArray( typeof( String ) );
        }
        /// <summary>
        /// 将字符串写入某个文件中(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="fileContent">需要写入文件的字符串</param>
        public static void Write(String absolutePath, String fileContent)
        {
            Write(absolutePath, fileContent, true);
        }
        /// <summary>
        /// 将字符串写入某个文件中(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="fileContent">需要写入文件的字符串</param>
        /// <param name="autoCreateDir">是否自动创建目录</param>
        public static void Write(String absolutePath, String fileContent, Boolean autoCreateDir)
        {
            if (autoCreateDir)
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(absolutePath));
            }
            Write(absolutePath, fileContent, Encoding.UTF8);
        }
        /// <summary>
        /// 将字符串写入某个文件中(需要指定文件编码方式)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="fileContent">需要写入文件的字符串</param>
        /// <param name="encoding">编码方式</param>
        public static void Write( String absolutePath, String fileContent, Encoding encoding ) {
            using (StreamWriter writer = new StreamWriter( absolutePath, false, encoding )) {
                writer.Write( fileContent );
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        public static void Delete( String absolutePath ) {
            System.IO.File.Delete( absolutePath );
        }
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <returns></returns>
        public static Boolean Exists( String absolutePath ) {
            return System.IO.File.Exists( absolutePath );
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFileName">原来的路径</param>
        /// <param name="destFileName">需要挪到的新路径</param>
        public static void Move(String sourceFileName, String destFileName)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destFileName));
            System.IO.File.Move( sourceFileName, destFileName );
        }
        /// <summary>
        /// 拷贝文件(如果目标存在，不覆盖)
        /// </summary>
        /// <param name="sourceFileName">原来的路径</param>
        /// <param name="destFileName">需要挪到的新路径</param>
        public static void Copy( String sourceFileName, String destFileName ) {
            System.IO.File.Copy( sourceFileName, destFileName, false );
        }
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourceFileName">原来的路径</param>
        /// <param name="destFileName">需要挪到的新路径</param>
        /// <param name="overwrite">如果目标存在，是否覆盖</param>
        public static void Copy( String sourceFileName, String destFileName, Boolean overwrite ) {
            System.IO.File.Copy( sourceFileName, destFileName, overwrite );
        }
        /// <summary>
        /// 将内容追加到文件中(采用UTF8编码)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="fileContent">需要追加的内容</param>
        public static void Append( String absolutePath, String fileContent ) {
            Append( absolutePath, fileContent, Encoding.UTF8 );
        }
        /// <summary>
        /// 将内容追加到文件中
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="fileContent">需要追加的内容</param>
        /// <param name="encoding">编码方式</param>
        public static void Append( String absolutePath, String fileContent, Encoding encoding ) {
            using (StreamWriter writer = new StreamWriter( absolutePath, true, encoding )) {
                writer.Write( fileContent );
            }
        }
        /// <summary>
        /// 使用ZIP格式压缩文件（未实现）
        /// </summary>
        /// <param name="sourceFileName">压缩的文件名</param>
        public static void Zip(String sourceFileName)
        {
            Zip(sourceFileName, sourceFileName + ".zip");
        }
        /// <summary>
        /// 使用ZIP格式压缩文件（未实现）
        /// </summary>
        /// <param name="sourceFileName">压缩的文件名</param>
        /// <param name="destFileName">压缩后输出的文件名</param>
        public static void Zip(String sourceFileName, String destFileName)
        {
            throw new Exception("Zip 方法未实现");
        }
        /// <summary>
        /// 解包ZIP压缩文件（未实现）
        /// </summary>
        /// <param name="sourceFileName">需要解包的ZIP文件名</param>
        public static void UnZip(String sourceFileName)
        {
            throw new Exception("UnZip 方法未实现");
        }
        /// <summary>
        /// 解包ZIP压缩文件（未实现）
        /// </summary>
        /// <param name="sourceFileName">需要解包的ZIP文件名</param>
        /// <param name="destFilePath">接包后的文件输出路径</param>
        public static void UnZip(String sourceFileName, String destFilePath)
        {
            throw new Exception("UnZip 方法未实现");
        }
        /// <summary>
        /// 检查文件真实的后缀名
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string CheckExtentName(string filepath)
        {
            string extentname = "";
            try
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(filepath))
                {
                    if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        return ".jpg";
                    else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                        return ".gif";
                    else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                        return ".bmp";
                    else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                        return ".png";
                    else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                        return ".ico";
                    else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                        return ".emf";
                    else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                        return ".tiff";
                    else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Wmf))
                        return ".wmf";
                }
            }
            catch { }
            return extentname;
        }
    }
}

