using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class World {
    public string Name { get; private set; }
    public WorldConfig Config { get; private set; }
    public float[,] Noise { get; private set; }

    public int Seed { get; private set; }

    public Room InitialRoom;

    private Dictionary<XY, WorldScreen> _screens;
    public ReadOnlyCollection<WorldScreen> Screens {
        get { return new List<WorldScreen>(_screens.Values).AsReadOnly(); }
    }

    private List<Room> _rooms;
    public ReadOnlyCollection<Room> Rooms { 
        get { return _rooms.AsReadOnly(); }
    }

    public World(string name, WorldConfig config, float[,] noise) {
        Name = name;
        Config = config;
        Noise = noise;

        Seed = name.GetHashCode();
        Debug.Log(Name + " " + Seed);
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

    public WorldScreen GetScreen(Room room) {
        foreach(WorldScreen screen in _screens.Values) {
            if(screen.Rooms.Contains(room))
                return screen;
        }
        return null;
    }

    public float[,] GetScreenNoise(XY screenCoord) {
        XYZ screenChunks = Config.ScreenChunks;

        float[,] screenNoise = new float[screenChunks.X, screenChunks.Z];

        int startX = screenCoord.X * screenChunks.X;
        int startY = screenCoord.Y * screenChunks.Z;
        
        for(int x = 0; x < screenChunks.X; x++) {
            for(int y = 0; y < screenChunks.Z; y++) {
                screenNoise[x, y] = Noise[startX + x, startY + y];
            }
        }

        return screenNoise;
    }
}
