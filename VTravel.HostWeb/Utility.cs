using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.HostWeb
{
    public class Utility
    {
        public static bool IsPointInPolygon(PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }


}


//unit test
//var pointsStr = "23.490627910988014 32.70915833919074,22.435940410988014 24.7023836767226,23.139065410988014 21.46887559507499,27.182034160988014 19.49296203792163,38.43203416098802 20.484100655194226,36.67422166098802 23.740559945772738,34.03750291098802 27.386849268809094,32.10390916098802 30.159813585874208,30.346096660988014 32.70915833919074";
//var pontsArray = pointsStr.Split(',');
//PointF[] points = new PointF[pontsArray.Length];
//            for(int i=0;i<pontsArray.Length;i++)
//            {
//                var pointXY = pontsArray[i].Split(' ');
//points[i] = new PointF(float.Parse(pointXY[0]), float.Parse(pointXY[1]));
//            }

//            PointF p = new PointF(latitude, longitude);
//            if (Utility.IsPointInPolygon(points, p))
//            {
//                var result = true;
//            }