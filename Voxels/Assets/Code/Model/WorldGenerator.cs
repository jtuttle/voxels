using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenerator {
    public World World { get; private set; }

    private WorldConfig _worldConfig;

    // We must temporarily store all room neighbor relationships in order
    // to build a spanning tree over the world. This knowledge is no longer
    // useful following dungeon creation (I think).
    public Dictionary<Room, List<Room>> RoomNeighbors { get; private set; }

    public WorldGenerator(WorldConfig worldConfig) {
        _worldConfig = worldConfig;

        RoomNeighbors = new Dictionary<Room, List<Room>>();
    }

    public void GenerateWorld() {
        // 0. validate world config
        // Maybe store the information such that it shouldn't need to be
        // validated and the potentially invalid values are computed.
        //worldConfig.Validate();

        // 1. generate noise
        float[,] worldNoise = GenerateWorldNoise(_worldConfig);

        // 2. create world object
        World = new World(_worldConfig, worldNoise);

        // 3. split world into "rooms"
        FindWorldRooms(World);

        // 4. Create spanning tree
        CreateSpanningTree(World);



        // 4. build graph from rooms
        // - for each room, create one graph node
        // - for each edge node in each room, create one graph edge
        //   if it doesn't already exist

        // 5. apply lock and key algorithm to rooms
        // - keys will be dungeons for the overworld

    }

    private float[,] GenerateWorldNoise(WorldConfig worldConfig) {
        XYZ worldChunks = worldConfig.WorldChunks;

        WorldNoiseGenerator worldNoiseGen = new WorldNoiseGenerator(Random.Range(1, 65536));
        float[,] worldNoise = worldNoiseGen.GenerateWorldNoise(worldChunks.X, 
                                                               worldChunks.Z,
                                                               worldChunks.Y);

        worldNoise = worldNoiseGen.ShiftNoise(0, 1, 0, worldChunks.Y, worldNoise);
        worldNoise = worldNoiseGen.DiscretizeDenormalizedNoise(worldNoise);

        return worldNoise;
    }

    private void FindWorldRooms(World world) {
        XYZ screenCount = world.Config.ScreenCount;
        XYZ screenChunks = world.Config.ScreenChunks;

        // Loop over world screens.
        for(int screenZ = 0; screenZ < screenCount.Z; screenZ++) {
            for(int screenX = 0; screenX < screenCount.X; screenX++) {
                XY screenCoord = new XY(screenX, screenZ);

                WorldScreen screen = new WorldScreen(screenCoord);

                // Create queue of coords to be searched for rooms.
                Dictionary<XY, RoomCoord> roomCoords = new Dictionary<XY, RoomCoord>();

                for(int roomZ = 0; roomZ < screenChunks.Z; roomZ++) {
                    for(int roomX = 0; roomX < screenChunks.X; roomX++) {
                        XY roomCoord = new XY(roomX, roomZ);

                        XY noiseCoord = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z) + roomCoord;
                        float elevation = world.Noise[noiseCoord.X, noiseCoord.Y];

                        roomCoords[roomCoord] = new RoomCoord(roomCoord, elevation);
                    }
                }

                world.AddScreen(screenCoord, screen);

                FindScreenRooms(world, screenCoord, roomCoords);
            }
        }
    }

    // TODO: Could probably save some memory here by not using RoomCoords at all. Since we are
    // only storing XY values in our rooms, they aren't really necessary. They are somewhat useful
    // as a temp structure to store elevations while searching...

    // IDEA: pre-divide screencoords by elevation, so that each time you have to remove or 
    // check whether you've already seen a coord you're searching a smaller list.
    private void FindScreenRooms(World world, XY screenCoord, Dictionary<XY, RoomCoord> roomCoords) {
        WorldScreen currentScreen = world.GetScreen(screenCoord);

        // We compare against previously created screens to find connected rooms.
        WorldScreen downScreen = world.GetScreen(screenCoord - new XY(0, 1));
        WorldScreen leftScreen = world.GetScreen(screenCoord - new XY(1, 0));

        // Use queue to choose first coord of each new room from which to search.
        Queue<RoomCoord> coordQueue = new Queue<RoomCoord>(roomCoords.Values);

        // Use queue to fill out a room during the BFS stage.
        Queue<RoomCoord> searchQueue = new Queue<RoomCoord>();

        List<XY> neighborOffsets = new List<XY> { new XY(0, 1), new XY(-1, 0), new XY(1, 0), new XY(0, -1) };

        HashSet<RoomCoord> visited = new HashSet<RoomCoord>();

        while(coordQueue.Count > 0) {
            RoomCoord startCoord = coordQueue.Dequeue();

            // Skip this coord if it has already been visited.
            if(visited.Contains(startCoord)) continue;

            // Place first coord of room in search queue.
            searchQueue.Enqueue(startCoord);
            visited.Add(startCoord);

            Room room = new Room(startCoord.Elevation, 1.0f);

            HashSet<Room> neighbors = new HashSet<Room>();
            Room neighbor;

            // Create neighbor entry for new room.
            RoomNeighbors[room] = new List<Room>();

            // Perform BFS on the start coord.
            while(searchQueue.Count > 0) {
                RoomCoord searchCoord = searchQueue.Dequeue();

                bool isEdge = false;

                foreach(XY offset in neighborOffsets) {
                    XY neighborCoord = searchCoord.Coord + offset;

                    RoomCoord neighborRoomCoord = null;

                    bool hasKey = roomCoords.TryGetValue(neighborCoord, out neighborRoomCoord);

                    if(neighborRoomCoord == null) {
                        // Search coord is on edge of the screen.
                        if(!hasKey) isEdge = true;

                        // Check for neighbor relationships with the previous horizontal screen.
                        if(searchCoord.Coord.X == 0 && leftScreen != null) {
                            XY leftCoord = neighborCoord + new XY(world.Config.ScreenChunks.X, 0);

                            neighbor = FindNeighbor(room, leftScreen, leftCoord);
                            if(neighbor != null) neighbors.Add(neighbor);
                        }

                        // Check for neighbor relationships with the previous vertical screen.
                        if(searchCoord.Coord.Y == 0 && downScreen != null) {
                            XY downCoord = neighborCoord + new XY(0, world.Config.ScreenChunks.Y);

                            neighbor = FindNeighbor(room, downScreen, downCoord);
                            if(neighbor != null) neighbors.Add(neighbor);
                        }
                    } else {
                        if(searchCoord.Elevation == neighborRoomCoord.Elevation && !visited.Contains(neighborRoomCoord)) {
                            visited.Add(neighborRoomCoord);
                            searchQueue.Enqueue(neighborRoomCoord);
                        }

                        if(searchCoord.Elevation != neighborRoomCoord.Elevation) {
                            isEdge = true;

                            // Check for internal neighbor relationships.
                            neighbor = FindNeighbor(room, currentScreen, neighborCoord);
                            if(neighbor != null) neighbors.Add(neighbor);
                        }
                    }
                }

                room.AddCoord(searchCoord.Coord, isEdge);
            }

            // IDEA: store neighbors in a local list until this point, then only add them
            // to the neighbor map if the min room size condition is met.
            if(room.Coords.Count >= _worldConfig.MinRoomSize) {
                foreach(Room neigh in neighbors.ToList()) {
                    RoomNeighbors[room].Add(neigh);
                    RoomNeighbors[neigh].Add(room);
                }

                currentScreen.AddRoom(room);
            }
        }
    }

    private Room FindNeighbor(Room room, WorldScreen screen, XY neighborCoord) {
        foreach(Room neighborRoom in screen.Rooms) {
            if(neighborRoom.Coords.Contains(neighborCoord)) {
                return neighborRoom;

                /*
                if(!RoomNeighbors[room].Contains(neighborRoom)) 
                    RoomNeighbors[room].Add(neighborRoom);
                if(!RoomNeighbors[neighborRoom].Contains(room)) 
                    RoomNeighbors[neighborRoom].Add(room);
                return;
                */
            }
        }
        return null;
    }

    private void CreateSpanningTree(World world) {
        // do we need neighbor relationships beyond generating this graph?

        foreach(Room room in world.Rooms) {

        }
    }
}
