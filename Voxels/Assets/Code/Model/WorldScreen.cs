using UnityEngine;
using System.Collections.Generic;

public class WorldScreen {
    public XY Coord { get; private set; }
    public List<Room> Rooms { get; private set; }
	
    public WorldScreen(XY coord) {
        Coord = coord;

        Rooms = new List<Room>();
    }

    public void AddRoom(Room room) {
        Rooms.Add(room);
    }
}
