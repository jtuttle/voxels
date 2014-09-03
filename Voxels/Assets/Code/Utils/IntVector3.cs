using UnityEngine;
using System.Collections;

public class IntVector3 {
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }

	public IntVector3(int x, int y, int z) {
		X = x;
		Y = y;
		Z = z;
	}

	public static IntVector3 operator +(IntVector3 a, IntVector3 b) {
		return new IntVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	public static IntVector3 operator -(IntVector3 a, IntVector3 b) {
		return new IntVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}
}
