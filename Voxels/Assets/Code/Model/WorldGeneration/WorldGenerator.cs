using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class WorldGenerator {
    //public World World { get; private set; }

    private string _worldName;
    private WorldConfig _worldConfig;

    public WorldGenerator(string worldName, WorldConfig worldConfig) {
        _worldName = worldName;
        _worldConfig = worldConfig;
    }

    public World GenerateWorld() {
        // 0. validate world config
        // Maybe store the information such that it shouldn't need to be
        // validated and the potentially invalid values are computed.
        //worldConfig.Validate();

        // 1. generate noise
        float[,] worldNoise = GenerateWorldNoise(_worldConfig);
        //float[,] worldNoise = GenerateFakeWorldNoise(_worldConfig);

        // 2. create world object
        World world = new World(_worldConfig, worldNoise);

        // 3. split world into "rooms"
        new WorldRoomFinder(world).Find();

        // 3.5 Choose initial room
        ChooseInitialRoom(world);

        // 4. Create spanning tree
        CreateSpanningTree(world);

        // 5. Place keys
        // After assigning key levels, we'll want to place keys 
        // randomly in each group of key level nodes
        //PlaceKeys(World);

        return world;
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

    private void ChooseInitialRoom(World world) {

    }

    // TODO: Implement critical path, otherwise there's going to be way too much backtracking.
    private void CreateSpanningTree(World world) {
        int keyLevel = 0;
        int keyLevelCount = 0;

        // The number of rooms after which to increment the key level.
        // TODO: find some way to vary this reasonably
        int keyLevelInterval = (int)Mathf.Ceil(world.Rooms.Count / world.Config.KeyLevels);

        // TODO: exclude rooms that are too small from this process (though we will
        // need to remember them as neighbors so we can block them off).
        ReadOnlyCollection<Room> rooms = world.Rooms;

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
            List<Room> neighbors = currentRoom.Neighbors.Where(x => !visited.Contains(x)).ToList();

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

            ReadOnlyCollection<Room> nextRoomNeighbors = nextRoom.Neighbors;

            // Add newly-visited room to expandables if it has unvisited neighbors.
            if(nextRoomNeighbors.Where(x => !visited.Contains(x)).ToList().Count > 0)
                expandables.Add(nextRoom);

            // Remove expandable neighbors if this room was the last option for expansion.
            // TODO: This seems like it would be quite expensive. Find a way to avoid it?
            foreach(Room room in nextRoomNeighbors.Where(x => expandables.Contains(x)).ToList()) {
                if(room.Neighbors.Where(x => !visited.Contains(x)).ToList().Count == 0) 
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
