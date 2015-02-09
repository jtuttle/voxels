using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for dividing world noise up into screens and rooms.
// Screen boundaries are determined by the screen sizes specified in our world
// configuration. Room boundaries are determined by both the screen boundaries 
// and any changes of elevation within a screen. We also set key levels here and
// find neighbor relationships between the rooms.

public class WorldRoomFinder {
    private World _world;

    // Cached list of offsets to use when BFS searching neighboring tiles.
    private List<XY> _neighborOffsets = new List<XY> { new XY(0, 1), new XY(-1, 0), new XY(1, 0), new XY(0, -1) };

    public WorldRoomFinder(World world) {
        //RoomNeighbors = new Dictionary<Room, List<Room>>();
        _world = world;
    }

    public void Find() {
        FindWorldRooms();
    }

    // This method divides the world's noise into screens based on the dimensions specified in the
    // world config. It also calls another method to find the rooms in each screen.
    private void FindWorldRooms() {
        XYZ screenCount = _world.Config.ScreenCount;
        XYZ screenChunks = _world.Config.ScreenChunks;
        
        // Loop over world screens.
        for(int screenZ = 0; screenZ < screenCount.Z; screenZ++) {
            for(int screenX = 0; screenX < screenCount.X; screenX++) {
                XY screenCoord = new XY(screenX, screenZ);
                
                WorldScreen worldScreen = new WorldScreen(screenCoord);
                
                // Create queue of coords to be searched for rooms.
                Dictionary<XY, RoomTile> roomTiles = new Dictionary<XY, RoomTile>();
                
                for(int roomZ = 0; roomZ < screenChunks.Z; roomZ++) {
                    for(int roomX = 0; roomX < screenChunks.X; roomX++) {
                        XY roomCoord = new XY(roomX, roomZ);
                        
                        XY noiseCoord = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z) + roomCoord;
                        float elevation = _world.Noise[noiseCoord.X, noiseCoord.Y];
                        
                        roomTiles[roomCoord] = new RoomTile(roomCoord, elevation);
                    }
                }

                FindScreenRooms(worldScreen, roomTiles);

                _world.AddScreen(screenCoord, worldScreen);
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
    private void FindScreenRooms(WorldScreen currentScreen, Dictionary<XY, RoomTile> roomTiles) {
        XY screenCoord = currentScreen.Coord;
        
        // We compare against previously created screens to find connected rooms.
        WorldScreen downScreen = _world.GetScreen(screenCoord - new XY(0, 1));
        WorldScreen leftScreen = _world.GetScreen(screenCoord - new XY(1, 0));
        
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
            
            List<Room> neighbors = new List<Room>();
            Room neighbor;

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
                            XY leftCoord = neighborCoord + new XY(_world.Config.ScreenChunks.X, 0);
                            
                            neighbor = FindNeighbor(room, leftScreen, leftCoord);
                            if(neighbor != null) neighbors.Add(neighbor);
                        }
                        
                        // Check for neighbor relationships with the previous vertical screen.
                        if(searchTile.Coord.Y == 0 && downScreen != null) {
                            XY downCoord = neighborCoord + new XY(0, _world.Config.ScreenChunks.Y);
                            
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
            if(room.Coords.Count >= _world.Config.MinRoomSize) {
                foreach(Room neighborRoom in neighbors) {
                    room.AddNeighbor(neighborRoom);
                    neighborRoom.AddNeighbor(room);
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
}
