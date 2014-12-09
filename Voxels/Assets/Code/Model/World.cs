using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour {
    public float[,] Samples { get; private set; }
    public Dictionary<XY, WorldScreenComponent> Screens { get; private set; }

    public World(float[,] samples) {
        Samples = samples;

        Screens = new Dictionary<XY, WorldScreen>();
    }
}
