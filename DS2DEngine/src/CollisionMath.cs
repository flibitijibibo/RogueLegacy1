using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class CollisionMath
    {
        // Static variables used to calculate rotated rect collision
        private static Vector2[] aRectangleAxis = new Vector2[4];
        private static int[] aRectangleAScalars = new int[4];
        private static int[] aRectangleBScalars = new int[4];

        //Function that detects if two rectangles collide.
        public static bool Intersects(Rectangle a, Rectangle b)
        {
            return a.Intersects(b);
        }

        public static Vector2 RotatedRectIntersectsMTD(Rectangle a, float rotationA, Vector2 originA, Rectangle b, float rotationB, Vector2 originB)
        {
            if (rotationA == 0 && rotationB == 0)
                return CalculateMTD(a, b);

            //List<Vector2> aRectangleAxis = new List<Vector2>();
            Vector2 upperLeftA = new Vector2();
            Vector2 upperRightA = new Vector2();
            Vector2 lowerLeftA = new Vector2();
            Vector2 lowerRightA = new Vector2();
            CalcCorners(ref upperLeftA, ref upperRightA, ref lowerLeftA, ref lowerRightA, a, rotationA, originA);
            Vector2 upperLeftB = new Vector2();
            Vector2 upperRightB = new Vector2();
            Vector2 lowerLeftB = new Vector2();
            Vector2 lowerRightB = new Vector2();
            CalcCorners(ref upperLeftB, ref upperRightB, ref lowerLeftB, ref lowerRightB, b, rotationB, originB);

            aRectangleAxis[0] = (upperRightA - upperLeftA);
            aRectangleAxis[1] = (upperRightA - lowerRightA);
            aRectangleAxis[2] = (upperLeftB - lowerLeftB);
            aRectangleAxis[3] = (upperLeftB - upperRightB);

            float shortestLength = int.MaxValue;
            Vector2 shortestMTD = Vector2.Zero;
            foreach (Vector2 aAxis in aRectangleAxis)
            {
                aAxis.Normalize();
                Vector2 mtd = AxisCollisionMTD(a, rotationA, originA, b, rotationB, originB, aAxis);
                if (mtd.Length() < shortestLength)
                {
                    shortestLength = mtd.Length();
                    shortestMTD = mtd;
                }
            }
            return new Vector2((int)shortestMTD.X, (int)shortestMTD.Y);
            //return shortestMTD;
        } 

        //Do not convert to radians. The internal functions will do that.
        //This version of the method assumes the origin of rotation is in the centre of the rectangle.
        public static bool RotatedRectIntersects(Rectangle a, float rotationA, Rectangle b, float rotationB)
        {
            if (rotationA == 0 && rotationB == 0)
                return a.Intersects(b);

            //List<Vector2> aRectangleAxis = new List<Vector2>();
            Vector2 upperLeftA = new Vector2();
            Vector2 upperRightA = new Vector2();
            Vector2 lowerLeftA = new Vector2();
            Vector2 lowerRightA = new Vector2();
            CalcCorners(ref upperLeftA, ref upperRightA, ref lowerLeftA, ref lowerRightA, a, rotationA);
            Vector2 upperLeftB = new Vector2();
            Vector2 upperRightB = new Vector2();
            Vector2 lowerLeftB = new Vector2();
            Vector2 lowerRightB = new Vector2();
            CalcCorners(ref upperLeftB, ref upperRightB, ref lowerLeftB, ref lowerRightB, b, rotationB);

            aRectangleAxis[0] = (upperRightA - upperLeftA);
            aRectangleAxis[1] = (upperRightA - lowerRightA);
            aRectangleAxis[2] = (upperLeftB - lowerLeftB);
            aRectangleAxis[3] = (upperLeftB - upperRightB);

            foreach (Vector2 aAxis in aRectangleAxis)
            {
                if (!IsAxisCollision(a, rotationA, b, rotationB, aAxis))
                    return false;
            }

            return true;
        }

        //Do not convert to radians. The internal functions will do that.
        public static bool RotatedRectIntersects(Rectangle a, float rotationA, Vector2 originA, Rectangle b, float rotationB, Vector2 originB)
        {
            if (rotationA == 0 && rotationB == 0)
                return a.Intersects(b);

            //List<Vector2> aRectangleAxis = new List<Vector2>();
            Vector2 upperLeftA = new Vector2();
            Vector2 upperRightA = new Vector2();
            Vector2 lowerLeftA = new Vector2();
            Vector2 lowerRightA = new Vector2();
            CalcCorners(ref upperLeftA, ref upperRightA, ref lowerLeftA, ref lowerRightA, a, rotationA, originA);
            Vector2 upperLeftB = new Vector2();
            Vector2 upperRightB = new Vector2();
            Vector2 lowerLeftB = new Vector2();
            Vector2 lowerRightB = new Vector2();
            CalcCorners(ref upperLeftB, ref upperRightB, ref lowerLeftB, ref lowerRightB, b, rotationB, originB);

            aRectangleAxis[0] = (upperRightA - upperLeftA);
            aRectangleAxis[1] = (upperRightA - lowerRightA);
            aRectangleAxis[2] = (upperLeftB - lowerLeftB);
            aRectangleAxis[3] = (upperLeftB - upperRightB);

            foreach (Vector2 aAxis in aRectangleAxis)
            {
                aAxis.Normalize(); // This doesn't seem necessary.
                if (!IsAxisCollision(a, rotationA, originA, b, rotationB, originB, aAxis))
                    return false;
            }

            return true;
        }
        // Do not convert to radians.
        private static Vector2 RotatePoint(Vector2 thePoint, Vector2 theOrigin, float theRotation)
        {
            double cos = Math.Cos(theRotation);
            double sin = Math.Sin(theRotation);
            Vector2 aTranslatedPoint = new Vector2();
            aTranslatedPoint.X = (float)(theOrigin.X + (thePoint.X - theOrigin.X) * cos
                - (thePoint.Y - theOrigin.Y) * sin);
            aTranslatedPoint.Y = (float)(theOrigin.Y + (thePoint.Y - theOrigin.Y) * cos
                + (thePoint.X - theOrigin.X) * sin);
            return aTranslatedPoint;
        }

        private static void RotatePoint(Vector2 thePoint, Vector2 theOrigin, double cos, double sin, ref Vector2 output)
        {
            float x = (float)(theOrigin.X + (thePoint.X - theOrigin.X) * cos
                - (thePoint.Y - theOrigin.Y) * sin);
            float y = (float)(theOrigin.Y + (thePoint.Y - theOrigin.Y) * cos
                + (thePoint.X - theOrigin.X) * sin);
            output.X = x;
            output.Y = y;
        }

        public static Vector2 UpperLeftCorner(Rectangle rect, float rotation)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 origin = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
            Vector2 aUpperLeft = new Vector2(rect.Left, rect.Top);
            aUpperLeft = RotatePoint(aUpperLeft, aUpperLeft + origin, rotation);
            return aUpperLeft;
        }

        public static Vector2 UpperLeftCorner(Rectangle rect, float rotation, Vector2 origin)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 aUpperLeft = new Vector2(rect.Left, rect.Top);
            aUpperLeft = RotatePoint(aUpperLeft, aUpperLeft + origin, rotation);
            return aUpperLeft;
        }

        public static Vector2 UpperRightCorner(Rectangle rect, float rotation)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 origin = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
            Vector2 aUpperRight = new Vector2(rect.Right, rect.Top);
            aUpperRight = RotatePoint(aUpperRight, aUpperRight + new Vector2(-origin.X, origin.Y), rotation);
            return aUpperRight;
        }

        public static Vector2 UpperRightCorner(Rectangle rect, float rotation, Vector2 origin)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 aUpperRight = new Vector2(rect.Right, rect.Top);
            Vector2 aUpperLeft = new Vector2(rect.Left, rect.Top);
            aUpperRight = RotatePoint(aUpperRight, aUpperLeft + origin, rotation);
            return aUpperRight;
        }

        public static Vector2 LowerLeftCorner(Rectangle rect, float rotation)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 origin = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
            Vector2 aLowerLeft = new Vector2(rect.Left, rect.Bottom);
            aLowerLeft = RotatePoint(aLowerLeft, aLowerLeft + new Vector2(origin.X, -origin.Y), rotation);
            return aLowerLeft;
        }

        public static Vector2 LowerLeftCorner(Rectangle rect, float rotation, Vector2 origin)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 aLowerLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 aUpperLeft = new Vector2(rect.Left, rect.Top);
            aLowerLeft = RotatePoint(aLowerLeft, aUpperLeft + origin, rotation);
            return aLowerLeft;
        }

        public static Vector2 LowerRightCorner(Rectangle rect, float rotation)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 origin = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
            Vector2 aLowerRight = new Vector2(rect.Right, rect.Bottom);
            aLowerRight = RotatePoint(aLowerRight, aLowerRight + new Vector2(-origin.X, -origin.Y), rotation);
            return aLowerRight;
        }

        public static Vector2 LowerRightCorner(Rectangle rect, float rotation, Vector2 origin)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 aLowerRight = new Vector2(rect.Right, rect.Bottom);
            Vector2 aUpperLeft = new Vector2(rect.Left, rect.Top);
            aLowerRight = RotatePoint(aLowerRight, aUpperLeft + origin, rotation);
            return aLowerRight;
        }

        // More optimized to call this than each corner function separately
        public static void CalcCorners(ref Vector2 upperLeft, ref Vector2 upperRight, ref Vector2 lowerLeft, ref Vector2 lowerRight, Rectangle rect, float rotation)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 aUpperLeft = new Vector2(rect.Left, rect.Top);
            Vector2 aUpperRight = new Vector2(rect.Right, rect.Top);
            Vector2 aLowerLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 aLowerRight = new Vector2(rect.Right, rect.Bottom);
            Vector2 origin = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);

            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);
            RotatePoint(aUpperLeft, aUpperLeft + origin, cos, sin, ref upperLeft);
            RotatePoint(aUpperRight, aUpperRight + new Vector2(-origin.X, origin.Y), cos, sin, ref upperRight);
            RotatePoint(aLowerLeft, aLowerLeft + new Vector2(origin.X, -origin.Y), cos, sin, ref lowerLeft);
            RotatePoint(aLowerRight, aLowerRight + new Vector2(-origin.X, -origin.Y), cos, sin, ref lowerRight);
        }

        // More optimized to call this than each corner function separately
        public static void CalcCorners(ref Vector2 upperLeft, ref Vector2 upperRight, ref Vector2 lowerLeft, ref Vector2 lowerRight, Rectangle rect, float rotation, Vector2 origin)
        {
            rotation = MathHelper.ToRadians(rotation);
            Vector2 aUpperLeft = new Vector2(rect.Left, rect.Top);
            Vector2 aUpperRight = new Vector2(rect.Right, rect.Top);
            Vector2 aLowerLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 aLowerRight = new Vector2(rect.Right, rect.Bottom);

            double cos = Math.Cos(rotation);
            double sin = Math.Sin(rotation);
            RotatePoint(aUpperLeft, aUpperLeft + origin, cos, sin, ref upperLeft);
            RotatePoint(aUpperRight, aUpperLeft + origin, cos, sin, ref upperRight);
            RotatePoint(aLowerLeft, aUpperLeft + origin, cos, sin, ref lowerLeft);
            RotatePoint(aLowerRight, aUpperLeft + origin, cos, sin, ref lowerRight);
        }


        /// <summary>
        /// Determines if a collision has occurred on an Axis of one of the
        /// planes parallel to the Rectangle
        /// </summary>
        /// <param name="theRectangle"></param>
        /// <param name="aAxis"></param>
        /// <returns></returns>
        /// Do not convert to Radians.
        private static bool IsAxisCollision(Rectangle a, float rotationA, Rectangle b, float rotationB, Vector2 aAxis)
        {
            Vector2 upperLeftA = new Vector2();
            Vector2 upperRightA = new Vector2();
            Vector2 lowerLeftA = new Vector2();
            Vector2 lowerRightA = new Vector2();
            CalcCorners(ref upperLeftA, ref upperRightA, ref lowerLeftA, ref lowerRightA, a, rotationA);
            Vector2 upperLeftB = new Vector2();
            Vector2 upperRightB = new Vector2();
            Vector2 lowerLeftB = new Vector2();
            Vector2 lowerRightB = new Vector2();
            CalcCorners(ref upperLeftB, ref upperRightB, ref lowerLeftB, ref lowerRightB, b, rotationB);

            //Project the corners of the Rectangle we are checking on to the Axis and
            //get a scalar value of that project we can then use for comparison
            //List<int> aRectangleAScalars = new List<int>();
            aRectangleAScalars[0] = (GenerateScalar(upperLeftB, aAxis));
            aRectangleAScalars[1] = (GenerateScalar(upperRightB, aAxis));
            aRectangleAScalars[2] = (GenerateScalar(lowerLeftB, aAxis));
            aRectangleAScalars[3] = (GenerateScalar(lowerRightB, aAxis));

            //Project the corners of the current Rectangle on to the Axis and
            //get a scalar value of that project we can then use for comparison
            //List<int> aRectangleBScalars = new List<int>();
            aRectangleBScalars[0] = (GenerateScalar(upperLeftA, aAxis));
            aRectangleBScalars[1] = (GenerateScalar(upperRightA, aAxis));
            aRectangleBScalars[2] = (GenerateScalar(lowerLeftA, aAxis));
            aRectangleBScalars[3] = (GenerateScalar(lowerRightA, aAxis));

            //Get the Maximum and Minium Scalar values for each of the Rectangles
            int aRectangleAMinimum = aRectangleAScalars.Min();
            int aRectangleAMaximum = aRectangleAScalars.Max();
            int aRectangleBMinimum = aRectangleBScalars.Min();
            int aRectangleBMaximum = aRectangleBScalars.Max();

            //If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            //then we are detecting a collision between the rectangles on this Axis
            if (aRectangleBMinimum <= aRectangleAMaximum && aRectangleBMaximum >= aRectangleAMaximum)
                return true;
            else if (aRectangleAMinimum <= aRectangleBMaximum && aRectangleAMaximum >= aRectangleBMaximum)
                return true;

            return false;
        }

        private static bool IsAxisCollision(Rectangle a, float rotationA, Vector2 originA, Rectangle b, float rotationB, Vector2 originB, Vector2 aAxis)
        {

            //Project the corners of the Rectangle we are checking on to the Axis and
            //get a scalar value of that project we can then use for comparison
            //List<int> aRectangleAScalars = new List<int>();
            Vector2 upperLeft = new Vector2();
            Vector2 upperRight = new Vector2();
            Vector2 lowerLeft = new Vector2();
            Vector2 lowerRight = new Vector2();
            CalcCorners(ref upperLeft, ref upperRight, ref lowerLeft, ref lowerRight, b, rotationB, originB);
            aRectangleAScalars[0] = (GenerateScalar(upperLeft, aAxis));
            aRectangleAScalars[1] = (GenerateScalar(upperRight, aAxis));
            aRectangleAScalars[2] = (GenerateScalar(lowerLeft, aAxis));
            aRectangleAScalars[3] = (GenerateScalar(lowerRight, aAxis));

            //Project the corners of the current Rectangle on to the Axis and
            //get a scalar value of that project we can then use for comparison
            //List<int> aRectangleBScalars = new List<int>();
            CalcCorners(ref upperLeft, ref upperRight, ref lowerLeft, ref lowerRight, a, rotationA, originA);
            aRectangleBScalars[0] = (GenerateScalar(upperLeft, aAxis));
            aRectangleBScalars[1] = (GenerateScalar(upperRight, aAxis));
            aRectangleBScalars[2] = (GenerateScalar(lowerLeft, aAxis));
            aRectangleBScalars[3] = (GenerateScalar(lowerRight, aAxis));

            //Get the Maximum and Minium Scalar values for each of the Rectangles
            int aRectangleAMinimum = aRectangleAScalars.Min();
            int aRectangleAMaximum = aRectangleAScalars.Max();
            int aRectangleBMinimum = aRectangleBScalars.Min();
            int aRectangleBMaximum = aRectangleBScalars.Max();

            //If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            //then we are detecting a collision between the rectangles on this Axis
            if (aRectangleBMinimum <= aRectangleAMaximum && aRectangleBMaximum >= aRectangleAMaximum)
            {
                //Console.WriteLine((aRectangleAMaximum - aRectangleBMinimum) * -aAxis);
                return true;
            }
            else if (aRectangleAMinimum <= aRectangleBMaximum && aRectangleAMaximum >= aRectangleBMaximum)
            {
                //Console.WriteLine((aRectangleBMaximum - aRectangleAMinimum) * -aAxis);
                return true;
            }

            return false;
        }

        private static Vector2 AxisCollisionMTD(Rectangle a, float rotationA, Vector2 originA, Rectangle b, float rotationB, Vector2 originB, Vector2 aAxis)
        {
            //Project the corners of the Rectangle we are checking on to the Axis and
            //get a scalar value of that project we can then use for comparison
            //List<int> aRectangleAScalars = new List<int>();
            Vector2 upperLeft = new Vector2();
            Vector2 upperRight = new Vector2();
            Vector2 lowerLeft = new Vector2();
            Vector2 lowerRight = new Vector2();
            CalcCorners(ref upperLeft, ref upperRight, ref lowerLeft, ref lowerRight, b, rotationB, originB);

            aRectangleAScalars[0] = (GenerateScalar(upperLeft, aAxis));
            aRectangleAScalars[1] = (GenerateScalar(upperRight, aAxis));
            aRectangleAScalars[2] = (GenerateScalar(lowerLeft, aAxis));
            aRectangleAScalars[3] = (GenerateScalar(lowerRight, aAxis));

            //Project the corners of the current Rectangle on to the Axis and
            //get a scalar value of that project we can then use for comparison
            //List<int> aRectangleBScalars = new List<int>();
            CalcCorners(ref upperLeft, ref upperRight, ref lowerLeft, ref lowerRight, a, rotationA, originA);
            aRectangleBScalars[0] = (GenerateScalar(upperLeft, aAxis));
            aRectangleBScalars[1] = (GenerateScalar(upperRight, aAxis));
            aRectangleBScalars[2] = (GenerateScalar(lowerLeft, aAxis));
            aRectangleBScalars[3] = (GenerateScalar(lowerRight, aAxis));

            //Get the Maximum and Minium Scalar values for each of the Rectangles
            int aRectangleAMinimum = aRectangleAScalars.Min();
            int aRectangleAMaximum = aRectangleAScalars.Max();
            int aRectangleBMinimum = aRectangleBScalars.Min();
            int aRectangleBMaximum = aRectangleBScalars.Max();
            //If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            //then we are detecting a collision between the rectangles on this Axis
            if (aRectangleBMinimum <= aRectangleAMaximum && aRectangleBMaximum >= aRectangleAMaximum)
            {
                return (aRectangleAMaximum - aRectangleBMinimum) * aAxis;
            }
            else if (aRectangleAMinimum <= aRectangleBMaximum && aRectangleAMaximum >= aRectangleBMaximum)
            {
                return (aRectangleBMaximum - aRectangleAMinimum) * -aAxis;
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Generates a scalar value that can be used to compare where corners of 
        /// a rectangle have been projected onto a particular axis. 
        /// </summary>
        /// <param name="theRectangleCorner"></param>
        /// <param name="theAxis"></param>
        /// <returns></returns>
        private static int GenerateScalar(Vector2 theRectangleCorner, Vector2 theAxis)
        {
            //Using the formula for Vector projection. Take the corner being passed in
            //and project it onto the given Axis
            float aNumerator = (theRectangleCorner.X * theAxis.X) + (theRectangleCorner.Y * theAxis.Y);
            float aDenominator = (theAxis.X * theAxis.X) + (theAxis.Y * theAxis.Y);
            float aDivisionResult = aNumerator / aDenominator;
            Vector2 aCornerProjected = new Vector2(aDivisionResult * theAxis.X, aDivisionResult * theAxis.Y);

            //Now that we have our projected Vector, calculate a scalar of that projection
            //that can be used to more easily do comparisons
            float aScalar = (theAxis.X * aCornerProjected.X) + (theAxis.Y * aCornerProjected.Y);
            return (int)aScalar;
        }

        /*
         * 
         * This function takes the first hitbox, finds the center, then does a distance
         * check to the center of the second hitbox
         * 
         */
        public static bool CircleIntersects( Rectangle a, float radius, Rectangle b)
        {
        // check if two Rectangles intersect
        //if( a.Left
        float deltaX = a.Center.X-b.Center.X;
        float deltaY = a.Center.Y-b.Center.Y;
        float distanceBetween = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY );

        if (distanceBetween <= radius)
            {
            return true;
            }
        else
            {
            return false;
            }
        }

        // Thank you Microsoft for intellisense with a name like that. ;)
        public static Vector2 CalculateMTD(Rectangle left, Rectangle right)
        {
            // Our displacement result vector containing the translation (movement) information
            // that resolves our intersection.
            Vector2 result = Vector2.Zero;

            // This is re-used to calculate the difference in distance between sides.
            float difference = 0.0f;

            // This stores the absolute minimum distance we'll need to separate our colliding object.
            float minimumTranslationDistance = 0f;

            // Axis stores the value of X or Y.  X = 0, Y = 1.
            // Side stores the value of left (-1) or right (+1).
            // They're used later in calculating the result vector.
            int axis = 0, side = 0;

            // Left
            difference = left.Right - right.Left;
            if (difference < 0.0f)
            {
                return Vector2.Zero;
            }

            {
                // These braces are superfluous but should make it more 
                //clear that they're similiar to the if statements below.
                minimumTranslationDistance = difference;
                axis = 0;
                side = -1;
            }

            // Right
            difference = right.Right - left.Left;
            if (difference < 0.0f)
            {
                return Vector2.Zero;
            }
            if (difference < minimumTranslationDistance)
            {
                minimumTranslationDistance = difference;
                axis = 0;
                side = 1;
            }

            // Down
            difference = left.Bottom - right.Top;
            if (difference < 0.0f)
            {
                return Vector2.Zero;
            }
            if (difference < minimumTranslationDistance)
            {
                minimumTranslationDistance = difference;
                axis = 1;
                side = -1;
            }

            // Up
            difference = right.Bottom - left.Top;
            if (difference < 0.0f)
            {
                return Vector2.Zero;
            }
            if (difference < minimumTranslationDistance)
            {
                minimumTranslationDistance = difference;
                axis = 1;
                side = 1;
            }

            // Intersection occurred:
            if (axis == 1) // Y Axis
                result.Y = (float)side * minimumTranslationDistance;
            else // X Axis
                result.X = (float)side * minimumTranslationDistance;

            return result;
        }

        public static Rectangle RotatedRectBounds(Rectangle rect, Vector2 rotationOrigin, float degreesRotation)
        {
            float leftBound = float.MaxValue, topBound = float.MaxValue, rightBound = -float.MaxValue, bottomBound = -float.MaxValue;
            Vector2 upperLeft = new Vector2();
            Vector2 upperRight = new Vector2();
            Vector2 lowerLeft = new Vector2();
            Vector2 lowerRight = new Vector2();
            CalcCorners(ref upperLeft, ref upperRight, ref lowerLeft, ref lowerRight, rect, degreesRotation, rotationOrigin);

            if (upperLeft.X < leftBound) leftBound = upperLeft.X;
            if (upperRight.X < leftBound) leftBound = upperRight.X;
            if (lowerLeft.X < leftBound) leftBound = lowerLeft.X;
            if (lowerRight.X < leftBound) leftBound = lowerRight.X;

            if (upperLeft.Y < topBound) topBound = upperLeft.Y;
            if (upperRight.Y < topBound) topBound = upperRight.Y;
            if (lowerLeft.Y < topBound) topBound = lowerLeft.Y;
            if (lowerRight.Y < topBound) topBound = lowerRight.Y;

            if (upperLeft.X > rightBound) rightBound = (upperLeft.X);
            if (upperRight.X  > rightBound) rightBound = (upperRight.X );
            if (lowerLeft.X > rightBound) rightBound = (lowerLeft.X );
            if (lowerRight.X  > rightBound) rightBound = (lowerRight.X );

            if (upperLeft.Y  > bottomBound) bottomBound = (upperLeft.Y);
            if (upperRight.Y  > bottomBound) bottomBound = (upperRight.Y);
            if (lowerLeft.Y > bottomBound) bottomBound = (lowerLeft.Y);
            if (lowerRight.Y  > bottomBound) bottomBound = (lowerRight.Y);

            return new Rectangle((int)leftBound, (int)topBound, (int)(rightBound - leftBound), (int)(bottomBound - topBound));
        }

        public static Vector2 LineToLineIntersect(Vector2 pt1Start, Vector2 pt1End, Vector2 pt2Start, Vector2 pt2End )
        {
            float ua = (pt2End.X - pt2Start.X) * (pt1Start.Y - pt2Start.Y) - (pt2End.Y - pt2Start.Y) * (pt1Start.X - pt2Start.X);
            float ub = (pt1End.X - pt1Start.X) * (pt1Start.Y - pt2Start.Y) - (pt1End.Y - pt1Start.Y) * (pt1Start.X - pt2Start.X);
            float denominator = (pt2End.Y - pt2Start.Y) * (pt1End.X - pt1Start.X) - (pt2End.X - pt2Start.X) * (pt1End.Y - pt1Start.Y);

            Vector2 intersectionPoint = Vector2.Zero;

            if (Math.Abs(denominator) <= 0.00001f)
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    intersectionPoint = (pt1Start + pt1End) / 2;
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    intersectionPoint.X = pt1Start.X + ua * (pt1End.X - pt1Start.X);
                    intersectionPoint.Y = pt1Start.Y + ua * (pt1End.Y - pt1Start.Y);
                }
            }

            return intersectionPoint;
        }

        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
}
