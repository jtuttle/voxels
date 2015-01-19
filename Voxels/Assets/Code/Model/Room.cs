using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Room {
    public HashSet<XY> Coords;

    // when you have internet, find a way to make this immutable?
    private HashSet<XY> _perimeter;
    public HashSet<XY> Perimeter { get; private set; }

    public float Elevation  { get; private set; }
    public float Intensity { get; private set; }

    // Parent and Children are used while building a spanning tree.
    public Room Parent { get; private set; }

    private List<Room> _children;
    public ReadOnlyCollection<Room> Children {
        get { return _children.AsReadOnly(); }
    }

    // Convenience method for getting all connected Rooms.
    public List<Room> Neighbors {
        get {
            List<Room> neighbors = new List<Room>(Children);
            neighbors.Add(Parent);
            return neighbors;
        }
    }

    public Room(float elevation, float intensity) {
        Elevation = elevation;
        Intensity = intensity;

        Coords = new HashSet<XY>();
        _perimeter = new HashSet<XY>();
    }

    public void AddCoord(XY coord, bool isEdge) {
        Coords.Add(coord);
        if(isEdge) _perimeter.Add(coord);
    }
}
