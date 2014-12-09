using UnityEngine;
using System.Collections.Generic;

public class WorldScreen {
    public XY Coord { get; private set; }
    public Rect Bounds { get; private set; }
    public List<Room> Rooms { get; private set; }
	
    public WorldScreen(XY coord, Rect bounds) {
        Coord = coord;
        Bounds = bounds;

        Rooms = new List<Room>();
    }
}
