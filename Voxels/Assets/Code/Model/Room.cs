using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum RoomSymbols {
    INITIAL, HAS_KEY
}

public class Room {
    public HashSet<XY> Coords;

    public List<RoomSymbols> Symbols { get; private set; }

    // when you have internet, find a way to make this immutable?
    private HashSet<XY> _perimeter;
    public HashSet<XY> Perimeter { get { return _perimeter; } }

    public float Elevation  { get; private set; }
    public int KeyLevel;
    public float Intensity { get; private set; }

    private List<Room> _neighbors;
    public ReadOnlyCollection<Room> Neighbors {
        get { return _neighbors.AsReadOnly(); }
    }

    // Parent and Children are used while building a spanning tree.
    public Room Parent { get; private set; }

    private List<Room> _children;
    public ReadOnlyCollection<Room> Children {
        get { return _children.AsReadOnly(); }
    }

    public Room(float elevation) {
        Elevation = elevation;
        //KeyLevel = keyLevel;
        //Intensity = intensity;

        Coords = new HashSet<XY>();
        _perimeter = new HashSet<XY>();

        _neighbors = new List<Room>();
        _children = new List<Room>();
    }

    public void AddCoord(XY coord, bool isEdge) {
        Coords.Add(coord);
        if(isEdge) _perimeter.Add(coord);
    }

    public void AddSymbol(RoomSymbols symbol) {
        if(Symbols.Contains(symbol))
            throw new Exception("Symbol already added to room.");

        Symbols.Add(symbol);
    }

    public void AddNeighbor(Room neighbor) {
        if(!_neighbors.Contains(neighbor))
            _neighbors.Add(neighbor);
    }

    public void SetParent(Room parent) {
        Parent = parent;
    }

    public void AddChild(Room child) {
        _children.Add(child);
    }
}
