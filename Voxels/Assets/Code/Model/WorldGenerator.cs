using UnityEngine;
using System.Collections.Generic;

public class WorldGenerator {
    public WorldGenerator() {

    }

    public World GenerateWorld(WorldConfig worldConfig) {
        // 0. validate world config
        // Maybe store the information such that it shouldn't need to be
        // validated and the potentially invalid values are computed.
        //worldConfig.Validate();

        // 1. generate noise
        float[,] worldNoise = GenerateWorldNoise(worldConfig);

        // 2. create world object
        World world = new World(worldConfig, worldNoise);

        // 3. split world into "screens"
        // - evenly dividing world samples into rectangular areas
        FindWorldRooms(world);


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



        //return new World(worldNoise);
        return world;
    }

    public float[,] GenerateWorldNoise(WorldConfig worldConfig) {
        XYZ worldChunks = worldConfig.WorldChunks;

        WorldNoiseGenerator worldNoiseGen = new WorldNoiseGenerator(Random.Range(1, 65536));
        float[,] worldNoise = worldNoiseGen.GenerateWorldNoise(worldChunks.X, 
                                                               worldChunks.Z,
                                                               worldChunks.Y);

        worldNoise = worldNoiseGen.ShiftNoise(0, 1, 0, worldChunks.Y, worldNoise);
        worldNoise = worldNoiseGen.DiscretizeDenormalizedNoise(worldNoise);

        return worldNoise;
    }

    public void FindWorldRooms(World world) {
        XYZ screenCount = world.Config.ScreenCount;
        XYZ screenChunks = world.Config.ScreenChunks;

        // Loop over world screens.
        for(int worldZ = 0; worldZ < screenCount.Z; worldZ++) {
            for(int worldX = 0; worldX < screenCount.X; worldX++) {
                XY screenCoord = new XY(worldX, worldZ);

                WorldScreen screen = new WorldScreen(screenCoord);

                // Create queue of coords to be searched for rooms.
                Dictionary<XY, RoomCoord> screenCoords = new Dictionary<XY, RoomCoord>();

                for(int screenZ = 0; screenZ < screenChunks.Z; screenZ++) {
                    for(int screenX = 0; screenX < screenChunks.X; screenX++) {
                        XY roomCoord = new XY(screenX, screenZ);

                        XY noiseCoord = screenCoord + roomCoord;
                        float elevation = world.Noise[noiseCoord.X, noiseCoord.Y];

                        screenCoords[roomCoord] = new RoomCoord(roomCoord, elevation);
                    }
                }

                FindScreenRooms(screen, screenCoords);

                world.AddScreen(screenCoord, screen);
            }
        }
    }

    // TODO: Could probably save some memory here by not using RoomCoords at all. Since we are
    // only storing XY values in our rooms, they aren't really necessary. They are somewhat useful
    // as a temp structure to store elevations while searching...

    // IDEA: pre-divide screencoords by elevation, so that each time you have to remove or 
    // check whether you've already seen a coord you're searching a smaller list.
    private void FindScreenRooms(WorldScreen worldScreen, Dictionary<XY, RoomCoord> screenCoords) {
        // Use queue to choose first coord of each new room from which to search.
        Queue<RoomCoord> coordQueue = new Queue<RoomCoord>(screenCoords.Values);

        // Use queue to fill out a room during the BFS stage.
        Queue<RoomCoord> searchQueue = new Queue<RoomCoord>();

        List<XY> neighborOffsets = new List<XY> { new XY(0, 1), new XY(-1, 0), new XY(1, 0), new XY(0, -1) };

        while(coordQueue.Count > 0) {
            RoomCoord startCoord = coordQueue.Dequeue();

            // Skip this coord if it has already been visited.
            if(screenCoords[startCoord.Coord] == null) continue;

            ScreenRoom room = new ScreenRoom(startCoord.Elevation);

            // Place first coord of room in search queue.
            searchQueue.Enqueue(startCoord);

            HashSet<RoomCoord> visited = new HashSet<RoomCoord>();

            // Perform BFS on the start coord.
            while(searchQueue.Count > 0) {
                RoomCoord searchCoord = searchQueue.Dequeue();

                // Mark searched nodes by nulling dictionary value.
                screenCoords[searchCoord.Coord] = null;

                bool isEdge = false;

                foreach(XY offset in neighborOffsets) {
                    XY neighborCoord = searchCoord.Coord + offset;

                    RoomCoord neighbor = null;

                    bool hasKey = screenCoords.TryGetValue(neighborCoord, out neighbor);

                    if(neighbor == null) {
                        // Search coord is on edge of the screen.
                        if(!hasKey) isEdge = true;
                    } else {
                        // BIG PROBLEM HERE: it's possible to queue a node a bunch of times
                        // because I'm not marking it as visited until it gets popped from the queue.
                        if(searchCoord.Elevation == neighbor.Elevation && !visited.Contains(neighbor)) {
                            visited.Add(neighbor);
                            searchQueue.Enqueue(neighbor);
                        } else {
                            isEdge = true;
                        }
                    }
                }

                room.AddCoord(searchCoord.Coord, isEdge);
            }

            worldScreen.AddRoom(room);
        }
    }
}
