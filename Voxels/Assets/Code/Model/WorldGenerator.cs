using UnityEngine;
using System.Collections;

public class WorldGenerator {

	
    public World GenerateWorld(WorldConfig worldConfig) {
        // 0. validate world config
        // Maybe store the information such that it shouldn't need to be
        // validated and the potentially invalid values are computed.
        //worldConfig.Validate();

        // 1. generate noise
        WorldNoiseGenerator worldNoiseGen = new WorldNoiseGenerator(Random.Range(1, 65536));
        float[,] worldNoise = worldNoiseGen.GenerateWorldNoise(worldConfig.WorldChunksX, 
                                                               worldConfig.WorldChunksZ,
                                                               worldConfig.WorldChunksY);

        // 2. split world into "screens"
        // - evenly dividing world samples into rectangular areas

        // 3. split "screens" into "rooms"
        // - build a list of screen coordinates 
        // - use BFS bounded by screen size, remove each coord
        //   from the list as it is visited
        // - keep track of edge coordinates for each room, which we 
        //   can use to create edges in the lock and key graph

        // 4. build graph from rooms
        // - for each room, create one graph node
        // - for each edge node in each room, create one graph edge
        //   if it doesn't already exist

        // 5. apply lock and key algorithm to rooms
        // - keys will be dungeons for the overworld



        return null;
    }

    private bool ValidateWorldConfig(WorldConfig config) {


        return true;
    }
}
