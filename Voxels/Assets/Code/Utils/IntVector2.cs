using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntVector2 {
    public int X { get; set; }
    public int Y { get; set; }
    
    public IntVector2(int x, int y) {
        X = x;
        Y = y;
    }
    
    public static IntVector2 operator +(IntVector2 a, IntVector2 b) {
        return new IntVector2(a.X + b.X, a.Y + b.Y);
    }
    
    public static IntVector2 operator -(IntVector2 a, IntVector2 b) {
        return new IntVector2(a.X - b.X, a.Y - b.Y);
    }

    public static bool operator ==(IntVector2 a, IntVector2 b) {
        // If both are null, or both are same instance, return true.
        if(System.Object.ReferenceEquals(a, b)) return true;
        
        // If one is null, but not both, return false.
        if(((object)a == null) || ((object)b == null)) return false;
        
        // Return true if the fields match:
        return a.X == b.X && a.Y == b.Y;
    }
    
    public static bool operator !=(IntVector2 a, IntVector2 b) {
        return !(a == b);
    }
    
    public override bool Equals(System.Object otherObject) {
        if(otherObject == null) return false;
        
        IntVector2 other = otherObject as IntVector2;
        if(other == null) return false;
        
        return (X == other.X) && (Y == other.Y);
    }
    
    public bool Equals(IntVector2 other) {
        if(other == null) return false;
        return (X == other.X) && (Y == other.Y);
    }
    
    public override int GetHashCode() {
        return X ^ Y;
    }

    public Vector2 ToVector2() {
        return new Vector2(X, Y);
    }

    public override string ToString() {
        return string.Format("[IntVector2: X={0}, Y={1}]", X, Y);
    }
}
