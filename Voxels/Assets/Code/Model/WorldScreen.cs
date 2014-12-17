using UnityEngine;
using System.Collections.Generic;

public class WorldScreen {
    public XY Coord { get; private set; }
    public List<ScreenRoom> Rooms { get; private set; }
	
    public WorldScreen(XY coord) {
        Coord = coord;

        Rooms = new List<ScreenRoom>();
    }

    public void AddRoom(ScreenRoom room) {
        Rooms.Add(room);
    }
}
