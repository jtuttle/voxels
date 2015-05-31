using System;
using System.Collections;
using System.Collections.Generic;

public class XYZ {
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    public List<XYZ> Neighbors {
        get {
            return new List<XYZ>() {
                new XYZ(X, Y + 1, Z),
                new XYZ(X - 1, Y, Z),
                new XYZ(X + 1, Y, Z),
                new XYZ(X, Y - 1, Z),
                new XYZ(X, Y, Z + 1),
                new XYZ(X, Y, Z - 1)
            };
        }
    }

    public XYZ(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    public XYZ(Hashtable json) {
        FromJson(json);
    }

    public void FromJson(Hashtable json) {
        X = int.Parse(json["X"].ToString());
        Y = int.Parse(json["Y"].ToString());
        Z = int.Parse(json["Z"].ToString());
    }

    public Hashtable ToJson() {
        Hashtable json = new Hashtable();

        json["X"] = X;
        json["Y"] = Y;
        json["Z"] = Z;

        return json;
    }

    public override string ToString() {
        return string.Format("XY ({0}, {1}, {2})", X, Y, Z);
    }

    public static XYZ operator +(XYZ a, XYZ b) {
        return new XYZ(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static XYZ operator -(XYZ a, XYZ b) {
        return new XYZ(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static XYZ operator *(XYZ a, int b) {
        return new XYZ(a.X * b, a.Y * b, a.Z * b);
    }

    public static XYZ operator *(XYZ a, XYZ b) {
        return new XYZ(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    }

    public static XYZ operator /(XYZ a, int b) {
        return new XYZ(a.X / b, a.Y / b, a.Z / b);
    }

    public static bool operator ==(XYZ a, XYZ b) {
        // If both are null, or both are same instance, return true.
        if(System.Object.ReferenceEquals(a, b)) return true;

        // If one is null, but not both, return false.
        if(((object)a == null) || ((object)b == null)) return false;

        // Return true if the fields match:
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(XYZ a, XYZ b) {
        return !(a == b);
    }

    public override bool Equals(Object other) {
        if(other == null) return false;

        XYZ otherXYZ = other as XYZ;
        if(otherXYZ == null) return false;

        return (X == otherXYZ.X) && (Y == otherXYZ.Y) && (Z == otherXYZ.Z);
    }

    public bool Equals(XYZ other) {
        if(other == null) return false;
        return (X == other.X) && (Y == other.Y) && (Z == other.Z);
    }

    public override int GetHashCode() {
        return X ^ Y ^ Z;
    }
}
