using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class CDGMath
    {
        private static Random m_randomSeed = new Random();

        public static float AngleBetweenPts(Vector2 pt1, Vector2 pt2)
        {
            float x1 = pt2.X - pt1.X;
            float y1 = pt2.Y - pt1.Y;
            float desiredAngle = (float)Math.Atan2(y1,x1);
            float difference = MathHelper.WrapAngle(desiredAngle);
            return MathHelper.ToDegrees(difference);
        }

        public static float VectorToAngle(Vector2 pt)
        {
            return AngleBetweenPts(Vector2.Zero, pt);
        }

        public static Vector2 AngleToVector(float angle)
        {
            angle = MathHelper.ToRadians(angle);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static Vector2 VectorBetweenPts(Vector2 pt1, Vector2 pt2)
        {
            return new Vector2(pt2.X - pt1.X, pt2.Y - pt1.Y);
        }

        public static float DistanceBetweenPts(Vector2 pt1, Vector2 pt2)
        {
            float dx = pt2.X - pt1.X;
            float dy = pt2.Y - pt1.Y;
            return (float)Math.Sqrt(dx* dx + dy * dy);
        }

        public static float DotProduct(Vector2 pt1, Vector2 pt2)
        {
            return pt1.X * pt2.X + pt1.Y * pt2.Y;
        }
      
        public static Vector2 RotatedPoint(Vector2 pt, float rotationAngle)
        {
            float radianPt = MathHelper.ToRadians(rotationAngle);
            double cos = Math.Cos(radianPt);
            double sin = Math.Sin(radianPt);
            float x2 = (float)(cos * pt.X - sin * pt.Y);
            float y2 = (float)(sin * pt.X + cos * pt.Y);
            return new Vector2(x2, y2);
        }

        public static Vector2 RotatedPoint(Vector2 thePoint, Vector2 theOrigin, float theRotation)
        {
            theRotation = MathHelper.ToRadians(theRotation);
            double cos = Math.Cos(theRotation);
            double sin = Math.Sin(theRotation);
            Vector2 aTranslatedPoint = new Vector2();
            aTranslatedPoint.X = (float)(theOrigin.X + (thePoint.X - theOrigin.X) * cos
                - (thePoint.Y - theOrigin.Y) * sin);
            aTranslatedPoint.Y = (float)(theOrigin.Y + (thePoint.Y - theOrigin.Y) * cos
                + (thePoint.X - theOrigin.X) * sin);
            return aTranslatedPoint;
        }

        public static string ConvertToHMS(uint milliseconds)
        {
            uint mills = milliseconds;
            uint hours = ((mills/1000)/3600);
            uint mins = ((mills/1000)/60) % 60;
            uint secs = ((mills/1000) % 60);
            
            string hoursString;
            string minsString;
            string secsString;

			if (hours < 10) hoursString = "0" + hours.ToString();
			else hoursString = hours.ToString();
			
			if (mins < 10) minsString = "0" + mins.ToString();
			else minsString = mins.ToString();
			
			if (secs < 10) secsString = "0" + secs.ToString();
			else secsString = secs.ToString();
				
			return (hoursString + ":" + minsString + ":" + secsString);
        }

        /// <summary>
        /// Returns a random positive integer.
        /// </summary>
        /// <returns>returns a random positive integer.</returns>
        public static int RandomInt()
        {
            return m_randomSeed.Next();
        }

        public static int RandomInt(int min, int max)
        {
            return m_randomSeed.Next(min, max + 1); // Plus 1 because Next(int, int) returns a number less than maxvalue.
        }

        /// <summary>
        /// Returns a random float from 0.0 to 1.0.
        /// </summary>
        /// <returns>Returns a random float from 0.0 to 1.0.</returns>
        public static float RandomFloat()
        {
            return (float)m_randomSeed.NextDouble();
        }

        public static float RandomFloat(float min, float max)
        {
            double range = (double)(max - min);
            double sample = m_randomSeed.NextDouble();
            double scaled = (sample * range) + min;
            return (float)scaled;
        }
        /*
		public static float RandomMinMaxDecimal(float min, float max)
        {
            if (max < min) throw new Exception("CDGMath.RandomMinMax(): Cannot create random number when max is less than min.");
            Random rand = new Random();
            double range = (double)(max - min);
            double sample = rand.NextDouble();
            double scaled = (sample * range) + min;
            return (float)scaled;
        }*/
	
        public static int RandomPlusMinus()
        {
            /*var rand : Number = Math.random() * 2;
			if (rand < 1) return -1
			else return 1;*/
            int rand = RandomInt(0, 2);
            if (rand < 1) return -1;
                return 1;
        }

        public static void TurnToFace(GameObj obj, float angle)
        {
            float desiredAngle = angle;
            float difference = MathHelper.WrapAngle(desiredAngle - obj.Orientation);

            difference = MathHelper.Clamp(difference, -obj.TurnSpeed, obj.TurnSpeed);
            obj.Orientation = MathHelper.WrapAngle(obj.Orientation + difference);

            obj.HeadingX = (float)Math.Cos(obj.Orientation);
            obj.HeadingY = (float)Math.Sin(obj.Orientation);
        }

        public static void TurnToFace(GameObj obj, Vector2 facePosition)
        {
            float x = facePosition.X - obj.Position.X;
            float y = facePosition.Y - obj.Position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = MathHelper.WrapAngle(desiredAngle - obj.Orientation);

            difference = MathHelper.Clamp(difference, -obj.TurnSpeed, obj.TurnSpeed);
            obj.Orientation = MathHelper.WrapAngle(obj.Orientation + difference);

            obj.HeadingX = (float)Math.Cos(obj.Orientation);
            obj.HeadingY = (float)Math.Sin(obj.Orientation);
        }

        // Placeholder code for bezier curves.
        // t represents a time between 0 and 1.
        public static Vector2 GetBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float cx = 3 * (p1.X - p0.X);
            float cy = 3 * (p1.Y - p0.Y);

            float bx = 3 * (p2.X - p1.X) - cx;
            float by = 3 * (p2.Y - p1.Y) - cy;

            float ax = p3.X - p0.X - cx - bx;
            float ay = p3.Y - p0.Y - cy - by;

            float Cube = t * t * t;
            float Square = t * t;

            float resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.X;
            float resY = (ay * Cube) + (by * Square) + (cy * t) + p0.Y;

            return new Vector2(resX, resY);
        }

        public static Vector2 GetCirclePosition(float angle, float distance, Vector2 centre)
        {
            angle = MathHelper.ToRadians(angle);
            return new Vector2((float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle))) + centre;
        }

        public static void Shuffle<T>(List<T> array)
        {
            for (int i = array.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = RandomInt(0, i - 1); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = RandomInt(0, i - 1); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        public static int NextPowerOf2(int x)
        {
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return ++x;
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900); //EDIT: i've typed 400 instead 900
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
    }
}
