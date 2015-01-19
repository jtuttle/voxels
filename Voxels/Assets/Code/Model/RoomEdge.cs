using UnityEngine;
using System.Collections;

public class RoomEdge {
    public Room Room { get; private set; }
    //public Symbol Symbol { get; private set; }

    public RoomEdge(Room room) {
        Room = room;
    }
}
