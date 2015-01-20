using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils {
    public static float ConvertRange(float oldMin, float oldMax, float newMin, float newMax, float value) {
        float scale = (float)(newMax - newMin) / (oldMax - oldMin);
        return newMin + ((value - oldMin) * scale);
    }

    public static Vector2 CalculateCentroid(List<XY> vertices) {
        float signedArea = 0;
        float cx = 0;
        float cy = 0;
        
        for(int i = 0; i < vertices.Count; i++) {
            XY current = vertices[i];
            XY next = vertices[(i + 1) % vertices.Count];
            
            signedArea += current.X * next.Y - next.X * current.Y;
            cx += (current.X + next.X) * (current.X * next.Y - next.X * current.Y);
            cy += (current.Y + next.Y) * (current.X * next.Y - next.X * current.Y);
        }
        
        signedArea /= 2;
        cx /= (6 * signedArea);
        cy /= (6 * signedArea);
        
        return new Vector2(cx, cy);
    }

    // This method uses Bresenham's line algorithm to computer the integer
    // coordinates for drawing a line between two points.
    public static List<XY> CalculateLineCoords(XY from, XY to) {
        List<XY> points = new List<XY>();

        int w = to.X - from.X;
        int h = to.Y - from.Y;
        int dx1, dy1, dx2, dy2 = 0;

        dx1 = (w < 0 ? -1 : (w > 0 ? 1 : 0));
        dy1 = (h < 0 ? -1 : (h > 0 ? 1 : 0));
        dx2 = (w < 0 ? -1 : (h > 0 ? 1 : 0));

        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);

        if(longest <= shortest) {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);

            dy2 = (h < 0 ? -1 : (h > 0 ? 1 : 0));
            dx2 = 0;
        }

        int numerator = longest >> 1;

        XY point = from;

        for(int i = 0; i <= longest; i++) {
            points.Add(point);
            numerator += shortest;

            if(numerator >= longest) {
                numerator -= longest;
                point += new XY(dx1, dy1);
            } else {
                point += new XY(dx2, dy2);
            }
        }

        return points;
    }
}
