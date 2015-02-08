using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Implement critical path, otherwise there's going to be way too much backtracking.

public class WorldGenerator {
    public World World { get; private set; }

    private WorldConfig _worldConfig;

    // Cached list of offsets to use when BFS searching neighboring tiles.
    private List<XY> _neighborOffsets = new List<XY> { new XY(0, 1), new XY(-1, 0), new XY(1, 0), new XY(0, -1) };

    // We must temporarily store all room neighbor relationships in order
    // to build a spanning tree over the world. This knowledge is no longer
    // useful following dungeon creation (I think). For now, I'm exposing it
    // for debug visualizations.
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
        //float[,] worldNoise = GenerateWorldNoise(_worldConfig);
        float[,] worldNoise = GenerateFakeWorldNoise(_worldConfig);

        // 2. create world object
        World = new World(_worldConfig, worldNoise);

        // 3. split world into "rooms"
        FindWorldRooms(World);

        // 4. Create spanning tree
        CreateSpanningTree(World);

        // 5. Place keys
        // After assigning key levels, we'll want to place keys 
        // randomly in each group of key level nodes
        PlaceKeys(World);


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

    private float[,] GenerateFakeWorldNoise(WorldConfig worldConfig) {

        float[,] worldNoise = new float[16 * 2, 12 * 2];

        for(int y = 0; y < 24; y++) {
            for(int x = 0; x < 32; x++) {
                int index = y * 32 + x;
                worldNoise[x,y] = (y < 8 ? 0 : 1);
            }
        }

        return worldNoise;
    }

    // This method divides the world's noise into screens based on the dimensions specified in the
    // world config. It also calls another method to find the rooms in each screen.
    private void FindWorldRooms(World world) {
        XYZ screenCount = world.Config.ScreenCount;
        XYZ screenChunks = world.Config.ScreenChunks;

        // Loop over world screens.
        for(int screenZ = 0; screenZ < screenCount.Z; screenZ++) {
            for(int screenX = 0; screenX < screenCount.X; screenX++) {
                XY screenCoord = new XY(screenX, screenZ);

                WorldScreen screen = new WorldScreen(screenCoord);

                // Create queue of coords to be searched for rooms.
                Dictionary<XY, RoomTile> roomTiles = new Dictionary<XY, RoomTile>();

                for(int roomZ = 0; roomZ < screenChunks.Z; roomZ++) {
                    for(int roomX = 0; roomX < screenChunks.X; roomX++) {
                        XY roomCoord = new XY(roomX, roomZ);

                        XY noiseCoord = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z) + roomCoord;
                        float elevation = world.Noise[noiseCoord.X, noiseCoord.Y];

                        roomTiles[roomCoord] = new RoomTile(roomCoord, elevation);
                    }
                }

                world.AddScreen(screenCoord, screen);

                FindScreenRooms(world, screenCoord, roomTiles);
            }
        }
    }

    // TODO: Could probably save some memory here by not using RoomCoords at all. Since we are
    // only storing XY values in our rooms, they aren't really necessary. They are somewhat useful
    // as a temp structure to store elevations while searching...

    // IDEA: pre-divide screencoords by elevation, so that each time you have to remove or 
    // check whether you've already seen a coord you're searching a smaller list.

    // This method uses repeated BFS to divide a screen into rooms based on the screen's edges 
    // and any internal elevation changes. It also builds a dictionary of connections between 
    // rooms to be used when generating the spanning tree in the next step.
    private void FindScreenRooms(World world, XY screenCoord, Dictionary<XY, RoomTile> roomTiles) {
        WorldScreen currentScreen = world.GetScreen(screenCoord);

        // We compare against previously created screens to find connected rooms.
        WorldScreen downScreen = world.GetScreen(screenCoord - new XY(0, 1));
        WorldScreen leftScreen = world.GetScreen(screenCoord - new XY(1, 0));

        // Queue used to choose first coord of each new room from which to search.
        // Start with all tiles and remove as we go.
        Queue<RoomTile> tileQueue = new Queue<RoomTile>(roomTiles.Values);

        // Queue used to fill out a room during the BFS stage.
        Queue<RoomTile> searchQueue = new Queue<RoomTile>();

        HashSet<RoomTile> visited = new HashSet<RoomTile>();

        while(tileQueue.Count > 0) {
            RoomTile startTile = tileQueue.Dequeue();

            if(visited.Contains(startTile)) continue;

            searchQueue.Enqueue(startTile);
            visited.Add(startTile);

            Room room = new Room(startTile.Elevation);

            HashSet<Room> neighbors = new HashSet<Room>();
            Room neighbor;

            RoomNeighbors[room] = new List<Room>();

            // Perform BFS on the chosen tile, stopping at screen edges and elevation changes.
            while(searchQueue.Count > 0) {
                RoomTile searchTile = searchQueue.Dequeue();

                bool isEdge = false;

                foreach(XY offset in _neighborOffsets) {
                    XY neighborCoord = searchTile.Coord + offset;

                    RoomTile neighborTile = null;

                    bool hasKey = roomTiles.TryGetValue(neighborCoord, out neighborTile);

                    // Search coord is on edge of the screen.
                    if(neighborTile == null) {
                        if(!hasKey) isEdge = true;

                        // Check for neighbor relationships with the previous horizontal screen.
                        if(searchTile.Coord.X == 0 && leftScreen != null) {
                            XY leftCoord = neighborCoord + new XY(world.Config.ScreenChunks.X, 0);

                            neighbor = FindNeighbor(room, leftScreen, leftCoord);
                            if(neighbor != null) neighbors.Add(neighbor);
                        }

                        // Check for neighbor relationships with the previous vertical screen.
                        if(searchTile.Coord.Y == 0 && downScreen != null) {
                            XY downCoord = neighborCoord + new XY(0, world.Config.ScreenChunks.Y);

                            neighbor = FindNeighbor(room, downScreen, downCoord);
                            if(neighbor != null) neighbors.Add(neighbor);
                        }
                    } else {
                        if(searchTile.Elevation == neighborTile.Elevation && !visited.Contains(neighborTile)) {
                            visited.Add(neighborTile);
                            searchQueue.Enqueue(neighborTile);
                        }

                        // Search tile is on the edge of an elevation change.
                        if(searchTile.Elevation != neighborTile.Elevation) {
                            isEdge = true;

                            // Check for internal neighbor relationships.
                            neighbor = FindNeighbor(room, currentScreen, neighborCoord);
                            if(neighbor != null) neighbors.Add(neighbor);
                        }
                    }
                }

                room.AddCoord(searchTile.Coord, isEdge);
            }

            // Only add room and neighbors if room meets minimum size requirement.
            // TODO: check min width / height instead of total area.
            if(room.Coords.Count >= _worldConfig.MinRoomSize) {
                foreach(Room neighborRoom in neighbors.ToList()) {
                    RoomNeighbors[room].Add(neighborRoom);
                    RoomNeighbors[neighborRoom].Add(room);
                }

                currentScreen.AddRoom(room);
            }
        }
    }

    private Room FindNeighbor(Room room, WorldScreen screen, XY neighborCoord) {
        foreach(Room neighborRoom in screen.Rooms) {
            if(neighborRoom.Coords.Contains(neighborCoord))
                return neighborRoom;
        }
        return null;
    }

    private void CreateSpanningTree(World world) {
        int keyLevel = 0;
        int keyLevelCount = 0;

        // The number of rooms after which to increment the key level.
        // TODO: find some way to vary this reasonably
        int keyLevelInterval = (int)Mathf.Ceil(world.Rooms.Count / world.Config.KeyLevels);

        // TODO: exclude rooms that are too small from this process (though we will
        // need to remember them as neighbors so we can block them off).
        List<Room> rooms = world.Rooms;

        // Keep track of rooms that have been expanded TO.
        HashSet<Room> visited = new HashSet<Room>();

        // This list will represent visited nodes with unvisited neighbors.
        List<Room> expandables = new List<Room>();

        Room firstRoom = rooms[Random.Range(0, world.Rooms.Count - 1)]; 
        expandables.Add(firstRoom);
        visited.Add(firstRoom);

        firstRoom.KeyLevel = keyLevel;
        keyLevelCount++;

        Room currentRoom;
        Room nextRoom;

        while(visited.Count != rooms.Count) {
            // Select random expandable room.
            currentRoom = expandables[Random.Range(0, expandables.Count - 1)];

            // Find unvisited neighbors of current room.
            List<Room> neighbors = RoomNeighbors[currentRoom].Where(x => !visited.Contains(x)).ToList();

            // Remove old room from expandables if this is the last unvisited neighbor.
            // NOTE: This is redundant with the step near the end of the method, though perhaps a tad faster.
            //if(neighbors.Count == 1)
            //    expandables.Remove(currentRoom);

            // Select a random unvisited neighbor.
            nextRoom = neighbors[Random.Range(0, neighbors.Count - 1)];

            // Set up parent-child relationship.
            nextRoom.SetParent(currentRoom);
            currentRoom.AddChild(nextRoom);

            // Assign a key level to the new room and increment if necessary.
            nextRoom.KeyLevel = keyLevel;

            if(++keyLevelCount == keyLevelInterval) {
                keyLevel++;
                keyLevelCount = 0;
            }

            // edge?

            visited.Add(nextRoom);

            List<Room> nextRoomNeighbors = RoomNeighbors[nextRoom];

            // Add newly-visited room to expandables if it has unvisited neighbors.
            if(nextRoomNeighbors.Where(x => !visited.Contains(x)).ToList().Count > 0)
                expandables.Add(nextRoom);

            // Remove expandable neighbors if this room was the last option for expansion.
            // TODO: This seems like it would be quite expensive. Find a way to avoid it?
            foreach(Room room in nextRoomNeighbors.Where(x => expandables.Contains(x)).ToList()) {
                if(RoomNeighbors[room].Where(x => !visited.Contains(x)).ToList().Count == 0) 
                    expandables.Remove(room);
            }
        }
    }

    // Choose a random room from each key level and mark it as the key location.
    private void PlaceKeys(World world) {
        Dictionary<int, List<Room>> keyLevels = new Dictionary<int, List<Room>>();

        foreach(Room room in world.Rooms) {
            if(keyLevels[room.KeyLevel] == null)
                keyLevels[room.KeyLevel] = new List<Room>();

            keyLevels[room.KeyLevel].Add(room);
        }

        for(int i = 0; i < world.Config.KeyLevels - 1; i++) {
            List<Room> rooms = keyLevels[i];

            Room keyRoom = rooms[Random.Range(0, rooms.Count)];
            keyRoom.AddSymbol(RoomSymbols.HAS_KEY);
        }
    }
}
