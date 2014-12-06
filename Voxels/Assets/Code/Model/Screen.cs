using UnityEngine;
using System.Collections.Generic;

public class Screen {
    public Rect Bounds { get; private set; }
    public List<Room> Rooms { get; private set; }
	
    public Screen(Rect bounds) {
        Bounds = bounds;

        Rooms = new List<Room>();
    }
}
