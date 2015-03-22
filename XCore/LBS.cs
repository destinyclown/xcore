using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
namespace System.Lbs
{
    /// <summary>
    /// 经纬度坐标
    /// </summary>    
    public class Degree
    {
        private double lat;
        private double lng;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="longitude">纬度 lat</param>
        /// <param name="latitude">经度 lng</param>
        public Degree(double longitude, double latitude)
        {
            lng = longitude;
            lat = latitude;
        }
        /// <summary>
        /// 纬度 lat
        /// </summary>
        public double Latitude
        {
            get { return lat; }
            set { lat = value; }
        }
        /// <summary>
        /// 经度 lng
        /// </summary>
        public double Longitude
        {
            get { return lng; }
            set { lng = value; }
        }
    }
    public class CoordDispose
    {
        private const double EARTH_RADIUS = 6378137.0;//地球半径(米)
        /// <summary>
        /// 角度数转换为弧度公式
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double radians(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 弧度转换为角度数公式
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double degrees(double d)
        {
            return d * (180 / Math.PI);
        }

        /// <summary>
        /// 计算两个经纬度之间的直接距离
        /// </summary>
        public static double GetDistance(Degree Degree1, Degree Degree2)
        {
            double radLat1 = radians(Degree1.Latitude);
            double radLat2 = radians(Degree2.Latitude);
            double a = radLat1 - radLat2;
            double b = radians(Degree1.Longitude) - radians(Degree2.Longitude);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
         Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        /// <summary>
        /// 计算两个经纬度之间的直接距离(google 算法)
        /// </summary>
        public static double GetDistanceGoogle(Degree Degree1, Degree Degree2)
        {
            double radLat1 = radians(Degree1.Latitude);
            double radLng1 = radians(Degree1.Longitude);
            double radLat2 = radians(Degree2.Latitude);
            double radLng2 = radians(Degree2.Longitude);

            double s = Math.Acos(Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Cos(radLng1 - radLng2) + Math.Sin(radLat1) * Math.Sin(radLat2));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 以一个经纬度为中心计算出四个顶点
        /// </summary>
        /// <param name="distance">半径(米)</param>
        /// <returns></returns>
        public static Degree[] GetDegreeCoordinates(Degree Degree1, double distance)
        {
            double dlng = 2 * Math.Asin(Math.Sin(distance / (2 * EARTH_RADIUS)) / Math.Cos(Degree1.Longitude));
            dlng = degrees(dlng);//转换成角度数

            double dlat = distance / EARTH_RADIUS;
            dlat = degrees(dlat);//转换成角度数

            return new Degree[] { new Degree(Math.Round(Degree1.Longitude + dlng,6), Math.Round(Degree1.Latitude - dlat,6)),//left-top
                                  new Degree(Math.Round(Degree1.Longitude - dlng,6), Math.Round(Degree1.Latitude - dlat,6)),//left-bottom
                                 new Degree(Math.Round(Degree1.Longitude + dlng,6), Math.Round(Degree1.Latitude + dlat,6)),//right-top
                                 new Degree(Math.Round(Degree1.Longitude - dlng,6), Math.Round(Degree1.Latitude + dlat,6)) //right-bottom
             };

        }

        public static Degree ConvertGPSToBaiDu(Degree Degree)
        {
            try
            {
                //http://api.map.baidu.com/ag/coord/convert?from=0&to=4&x=106.17559&y=29.36303&callback=BMap.Convertor.cbk_7594 

                //百度坐标转换API
                string path = "http://api.map.baidu.com/ag/coord/convert?from=0&to=4&x=" + Degree.Longitude + "&y=" + Degree.Latitude + "";
                //WebClient请求
                string strResult = new System.Net.WebClient().DownloadString(path);
                string lat = Json.GetFieldStr(strResult, "y");
                string lng = Json.GetFieldStr(strResult, "x");

                //进行Base64解码
                byte[] xBuffer = Convert.FromBase64String(lat);
                Degree.Latitude = Convert.ToDouble(Encoding.UTF8.GetString(xBuffer, 0, xBuffer.Length));

                byte[] yBuffer = Convert.FromBase64String(lng);
                Degree.Longitude = Convert.ToDouble(Encoding.UTF8.GetString(yBuffer, 0, yBuffer.Length));
            }
            catch { }
            return Degree;
        }

        public static Degree ConvertGPSToSoSo(Degree Degree)
        {
            try
            {
                //http://apis.map.qq.com/ws/coord/v1/translate?locations=39.12,116.83;30.21,115.43&type=3&key=OB4BZ-D4W3U-B7VVO-4PJWW-6TKDJ-WPB77 

                //百度坐标转换API
                string path = "http://apis.map.qq.com/ws/coord/v1/translate?locations=" + Degree.Latitude + "," + Degree.Longitude + "&type=1&key=OB4BZ-D4W3U-B7VVO-4PJWW-6TKDJ-WPB77";
                //WebClient请求
                string strResult = new System.Net.WebClient().DownloadString(path);
                var temp = Json.ToObject<SoSo>(strResult);
                if (temp != null)
                {
                    Degree.Latitude = temp.locations[0].lat;
                    Degree.Longitude = temp.locations[0].lng;
                }
                //string lon = Json.GetFieldStr(strResult, "x");
                //string lat = Json.GetFieldStr(strResult, "y");
            }
            catch { }
            return Degree;
        }

        public class SoSo
        {
            public int status { get; set; }
            public string message { get; set; }
            public List<SoSoDegree> locations { get; set; }
        }
        public class SoSoDegree
        {
            public double lng { get; set; }
            public double lat { get; set; }
        }
    }

    internal class Lbs
    {
        public void Test()
        {
            double a = CoordDispose.GetDistance(new Degree(39.947545, 116.412007), new Degree(39.947918, 116.412924));
            double b = CoordDispose.GetDistanceGoogle(new Degree(39.947545, 116.412007), new Degree(39.947918, 116.412924));
            Degree[] dd = CoordDispose.GetDegreeCoordinates(new Degree(39.947545, 116.412007), 102);
            Console.WriteLine(a + " " + b);
            Console.WriteLine(dd[0].Latitude + "," + dd[0].Longitude);
            Console.WriteLine(dd[3].Latitude + "," + dd[3].Longitude);
            Console.ReadLine();
        }
    }
}
