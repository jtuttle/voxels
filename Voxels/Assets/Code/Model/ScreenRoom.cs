using UnityEngine;
using System.Collections.Generic;

public class ScreenRoom {
    public HashSet<XY> Coords;
    public HashSet<XY> EdgeCoords;
    public float Elevation;

    public ScreenRoom(float elevation) {
        Elevation = elevation;

        Coords = new HashSet<XY>();
        EdgeCoords = new HashSet<XY>();
    }

    public void AddCoord(XY coord, bool isEdge) {
        Coords.Add(coord);
        if(isEdge) EdgeCoords.Add(coord);
    }
}
