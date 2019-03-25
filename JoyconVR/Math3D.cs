//=============================================
// Downloaded From                            |
// Visual C# Kicks - http://www.vcskicks.com/ |
//=============================================

using System;

namespace Math3D
{
	class Math3D
	{
		const double PIOVER180 = Math.PI / 180.0;


		public static Vector3D RotateX(Vector3D point3D, float degrees)
		{
			//[ a  b  c ] [ x ]   [ x*a + y*b + z*c ]
			//[ d  e  f ] [ y ] = [ x*d + y*e + z*f ]
			//[ g  h  i ] [ z ]   [ x*g + y*h + z*i ]

			//[ 1    0        0   ]
			//[ 0   cos(x)  sin(x)]
			//[ 0   -sin(x) cos(x)]

			double cDegrees = degrees * PIOVER180;
			double cosDegrees = Math.Cos(cDegrees);
			double sinDegrees = Math.Sin(cDegrees);

			double y = (point3D.y * cosDegrees) + (point3D.z * sinDegrees);
			double z = (point3D.y * -sinDegrees) + (point3D.z * cosDegrees);

			return new Vector3D(point3D.x, y, z);
		}

		public static Vector3D RotateY(Vector3D point3D, float degrees)
		{
			//[ cos(x)   0    sin(x)]
			//[   0      1      0   ]
			//[-sin(x)   0    cos(x)]

			double cDegrees = degrees * PIOVER180;
			double cosDegrees = Math.Cos(cDegrees);
			double sinDegrees = Math.Sin(cDegrees);

			double x = (point3D.x * cosDegrees) + (point3D.z * sinDegrees);
			double z = (point3D.x * -sinDegrees) + (point3D.z * cosDegrees);

			return new Vector3D(x, point3D.y, z);
		}

		public static Vector3D RotateZ(Vector3D point3D, float degrees)
		{
			//[ cos(x)  sin(x) 0]
			//[ -sin(x) cos(x) 0]
			//[    0     0     1]

			double cDegrees = degrees * PIOVER180;
			double cosDegrees = Math.Cos(cDegrees);
			double sinDegrees = Math.Sin(cDegrees);

			double x = (point3D.x * cosDegrees) + (point3D.y * sinDegrees);
			double y = (point3D.x * -sinDegrees) + (point3D.y * cosDegrees);

			return new Vector3D(x, y, point3D.z);
		}

		public static Vector3D Translate(Vector3D points3D, Vector3D oldOrigin, Vector3D newOrigin)
		{
			Vector3D difference = new Vector3D(newOrigin.x - oldOrigin.x, newOrigin.y - oldOrigin.y,
				newOrigin.z - oldOrigin.z);
			points3D.x += difference.x;
			points3D.y += difference.y;
			points3D.z += difference.z;
			return points3D;
		}

		public static Vector3D[] RotateX(Vector3D[] points3D, float degrees)
		{
			for (int i = 0; i < points3D.Length; i++)
			{
				points3D[i] = RotateX((Vector3D) points3D[i], degrees);
			}

			return points3D;
		}

		public static Vector3D[] RotateY(Vector3D[] points3D, float degrees)
		{
			for (int i = 0; i < points3D.Length; i++)
			{
				points3D[i] = RotateY((Vector3D) points3D[i], degrees);
			}

			return points3D;
		}

		public static Vector3D[] RotateZ(Vector3D[] points3D, float degrees)
		{
			for (int i = 0; i < points3D.Length; i++)
			{
				points3D[i] = RotateZ((Vector3D) points3D[i], degrees);
			}

			return points3D;
		}

		public static Vector3D[] Translate(Vector3D[] points3D, Vector3D oldOrigin, Vector3D newOrigin)
		{
			for (int i = 0; i < points3D.Length; i++)
			{
				points3D[i] = Translate(points3D[i], oldOrigin, newOrigin);
			}

			return points3D;
		}

		public class Vector3D
		{
			public float x;
			public float y;
			public float z;

			public Vector3D(int _x, int _y, int _z)
			{
				x = _x;
				y = _y;
				z = _z;
			}

			public Vector3D(double _x, double _y, double _z)
			{
				x = (float) _x;
				y = (float) _y;
				z = (float) _z;
			}

			public Vector3D(float _x, float _y, float _z)
			{
				x = _x;
				y = _y;
				z = _z;
			}

			public Vector3D()
			{
			}

			public override string ToString()
			{
				return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
			}
		}

		internal class Camera
		{
			public Vector3D position = new Vector3D();
		}
	}
}