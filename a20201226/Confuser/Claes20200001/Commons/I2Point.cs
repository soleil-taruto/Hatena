using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	public struct I2Point
	{
		public int X;
		public int Y;

		public I2Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public D2Point ToD2Point()
		{
			return new D2Point(this.X, this.Y);
		}

		public static I2Point operator +(I2Point a, I2Point b)
		{
			return new I2Point(a.X + b.X, a.Y + b.Y);
		}

		public static I2Point operator -(I2Point a, I2Point b)
		{
			return new I2Point(a.X - b.X, a.Y - b.Y);
		}

		public static I2Point operator *(I2Point a, int b)
		{
			return new I2Point(a.X * b, a.Y * b);
		}

		public static I2Point operator /(I2Point a, int b)
		{
			return new I2Point(a.X / b, a.Y / b);
		}
	}
}
