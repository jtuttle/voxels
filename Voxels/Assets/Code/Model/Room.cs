using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Room {
    public HashSet<XY> Coords;

    // when you have internet, find a way to make this immutable?
    private HashSet<XY> _perimeter;
    public HashSet<XY> Perimeter { get { return _perimeter; } }

    public float Elevation  { get; private set; }
    public float Intensity { get; private set; }

    // Parent and Children are used while building a spanning tree.
    public Room Parent { get; private set; }

    private List<Room> _children;
    public ReadOnlyCollection<Room> Children {
        get { return _children.AsReadOnly(); }
    }

    /*
    private List<Room> _neighbors;
    public ReadOnlyCollection<Room> Neighbors {
        get { return _neighbors.AsReadOnly(); }
    }
    */

    public Room(float elevation, float intensity) {
        Elevation = elevation;
        Intensity = intensity;

        Coords = new HashSet<XY>();
        _perimeter = new HashSet<XY>();

        _children = new List<Room>();
    }

    public void AddCoord(XY coord, bool isEdge) {
        Coords.Add(coord);
        if(isEdge) _perimeter.Add(coord);
    }

    public void SetParent(Room parent) {
        Parent = parent;
        //_neighbors.Add(parent);
    }

    public void AddChild(Room child) {
        _children.Add(child);
    }
}
