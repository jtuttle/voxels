using System;
using System.Collections;
using System.Collections.Generic;

public class XY {
    public int X { get; private set; }
    public int Y { get; private set; }

    public List<XY> Neighbors {
        get {
            return new List<XY>() {
                new XY(X, Y + 1),
                new XY(X - 1, Y),
                new XY(X + 1, Y),
                new XY(X, Y - 1)
            };
        }
    }

    public XY(int x, int y) {
        X = x;
        Y = y;
    }

    public XY(Hashtable json) {
        FromJson(json);
    }

    public void FromJson(Hashtable json) {
        X = int.Parse(json["X"].ToString());
        Y = int.Parse(json["Y"].ToString());
    }

    public Hashtable ToJson() {
        Hashtable json = new Hashtable();

        json["X"] = X;
        json["Y"] = Y;

        return json;
    }

    public override string ToString() {
        return string.Format("XY ({0}, {1})", X, Y);
    }

    public static XY operator +(XY a, XY b) {
        return new XY(a.X + b.X, a.Y + b.Y);
    }

    public static XY operator -(XY a, XY b) {
        return new XY(a.X - b.X, a.Y - b.Y);
    }

    public static XY operator *(XY a, int b) {
        return new XY(a.X * b, a.Y * b);
    }

    public static XY operator /(XY a, int b) {
        return new XY(a.X / b, a.Y / b);
    }

    public static bool operator ==(XY a, XY b) {
        // If both are null, or both are same instance, return true.
        if(System.Object.ReferenceEquals(a, b)) return true;

        // If one is null, but not both, return false.
        if(((object)a == null) || ((object)b == null)) return false;

        // Return true if the fields match:
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(XY a, XY b) {
        return !(a == b);
    }

    public override bool Equals(Object other) {
        if(other == null) return false;

        XY otherXY = other as XY;
        if(otherXY == null) return false;

        return (X == otherXY.X) && (Y == otherXY.Y);
    }

    public bool Equals(XY other) {
        if(other == null) return false;
        return (X == other.X) && (Y == other.Y);
    }

    public override int GetHashCode() {
        return X ^ Y;
    }

    public static XY Average(List<XY> coords) {
        XY avg = new XY(0, 0);

        if(coords.Count == 0) return avg;

        foreach(XY coord in coords)
            avg += coord;

        return new XY((int)Math.Round((double)avg.X / coords.Count),
                      (int)Math.Round((double)avg.Y / coords.Count));
    }
}
