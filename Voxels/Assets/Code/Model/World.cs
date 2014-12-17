using UnityEngine;
using System.Collections.Generic;

public class World {
    public WorldConfig Config { get; private set; }
    public float[,] Noise { get; private set; }

    public Dictionary<XY, WorldScreen> Screens { get; private set; }

    public World(WorldConfig config, float[,] noise) {
        Config = config;
        Noise = noise;

        Screens = new Dictionary<XY, WorldScreen>();
    }

    public void AddScreen(XY coord, WorldScreen screen) {
        Screens[coord] = screen;
    }
}
