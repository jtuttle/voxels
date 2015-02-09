using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class World {
    public WorldConfig Config { get; private set; }
    public float[,] Noise { get; private set; }

    private Dictionary<XY, WorldScreen> _screens;
    public ReadOnlyCollection<WorldScreen> Screens {
        get { return new List<WorldScreen>(_screens.Values).AsReadOnly(); }
    }

    private List<Room> _rooms;
    public ReadOnlyCollection<Room> Rooms { 
        get { return _rooms.AsReadOnly(); }
    }

    public World(WorldConfig config, float[,] noise) {
        Config = config;
        Noise = noise;

        _screens = new Dictionary<XY, WorldScreen>();
        _rooms = new List<Room>();
    }

    public void AddScreen(XY coord, WorldScreen screen) {
        _screens[coord] = screen;

        foreach(Room room in screen.Rooms)
            _rooms.Add(room);
    }

    public WorldScreen GetScreen(XY coord) {
        if(!_screens.ContainsKey(coord)) return null;
        return _screens[coord];
    }
}
