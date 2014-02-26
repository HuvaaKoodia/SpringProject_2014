using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	/// <summary>    
	/// Author: Roy Triesscheijn (http://www.roy-t.nl)
	/// Point3D class mimics some of the Microsoft.Xna.Framework.Vector3
	/// but uses Int32's instead of floats.
	/// </summary>
	public class Point3D
	{
		public int X;
		public int Y;
		public int Z;        
		
		public Point3D(int X, int Y, int Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}
		
		public Point3D(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
			this.Z = 0;
		}
		
		public Point3D(Point3D p1, Point3D p2)
		{
			this.X = p1.X + p2.X;
			this.Y = p1.Y + p2.Y;
			this.Z = p1.Z + p2.Z;
		}
		
		public int GetDistanceSquared(Point3D point)
		{
			int dx = this.X - point.X;
			int dy = this.Y - point.Y;
			int dz = this.Z - point.Z;
			return (dx * dx) + (dy * dy) + (dz * dz);            
		}
		
		public int GetDistance(Point3D point)
		{
			int dx = this.X - point.X;
			int dy = this.Y - point.Y;
			int dz = this.Z - point.Z;
			return (int)Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
		}
		
		public bool EqualsSS(Point3D p)
		{
			if ((object)p == null)
			{
				if ((object)this == null)
					return true;
				else
					return false;
			}
			
			return p.X == this.X && p.Z == this.Z && p.Y == this.Y;
		}
		
		public override int GetHashCode()
		{
			return (X + " " + Y + " " + Z).GetHashCode();
		}
		
		public override string ToString()
		{
			return X + ", " + Y + ", " + Z;
		}
		
		public override bool Equals(object obj)
		{
			return this == (Point3D)obj;
		}
		
		public static bool operator ==(Point3D one, Point3D two)
		{
			if ((object)one == null)
			{
				if ((object)two == null)
					return true;
				
				return false;
			}
			return one.EqualsSS(two);
		}
		
		public static bool operator !=(Point3D one, Point3D two)
		{
			if ((object)one == null)
			{
				if ((object)two == null)
					return false;
				
				return true;
			}
			
			return !one.EqualsSS(two);
		}
		
		public static Point3D operator +(Point3D one, Point3D two)
		{
			return new Point3D(one.X + two.X, one.Y + two.Y, one.Z + two.Z);
		}
		
		public static Point3D operator -(Point3D one, Point3D two)
		{
			return new Point3D(one.X - two.X, one.Y - two.Y, one.Z - two.Z);
		}
		
		public static Point3D Zero = new Point3D(0, 0, 0);        
	}
