using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WorldTestController : MonoBehaviour {
    public GameObject Canvas;

    public Vector3 ScreenChunks;
    public Vector2 ScreenCount;

    private WorldGenerator _worldGenerator;
    //private float[,] _currentNoise;
    private World _currentWorld;

    private Texture2D _currentTexture;

    private bool _drawRooms = true;
    private bool _drawCenters = true;
    private bool _drawConnections = true;
    private bool _drawSpanningTree = true;

    private Dictionary<Room, XY> _centroids;

    protected void Start() {
        _centroids = new Dictionary<Room, XY>();

        RefreshWorld();
    }

    protected void Update() {

    }

    protected void OnGUI() {
        if(GUI.Button(new Rect(10, 10, 100, 30), "New World"))
            OnRefreshClick();

        if(GUI.Button(new Rect(10, 50, 100, 30), (_drawRooms ? "Hide" : "Show") + " Rooms")) {
            _drawRooms = !_drawRooms;
            UpdateTexture();
        }

        if(GUI.Button(new Rect(10, 90, 100, 30), (_drawCenters ? "Hide" : "Show") + " Centers")) {
            _drawCenters = !_drawCenters;
            UpdateTexture();
        }

        if(GUI.Button(new Rect(10, 130, 100, 30), (_drawConnections ? "Hide" : "Show") + " Neighbors")) {
            _drawConnections = !_drawConnections;
            UpdateTexture();
        }

        if(GUI.Button(new Rect(10, 170, 100, 30), (_drawSpanningTree ? "Hide" : "Show") + " Tree")) {
            _drawSpanningTree = !_drawSpanningTree;
            UpdateTexture();
        }

        /*
        if(GUI.Button(new Rect(20, 120, 80, 20), "Weight"))
            OnWeightClick();

        if(GUI.Button(new Rect(20, 160, 80, 20), "Blockify"))
            OnBlockifyClick();

        if(GUI.Button(new Rect(20, 200, 80, 20), "Discretize"))
            OnDiscretizeClick();
        */

        if(GUI.Button(new Rect(20, 240, 80, 20), "Save"))
            OnSaveClick();
    }

    private void OnRefreshClick() {
        RefreshWorld();
    }

    private void OnFindRoomsClick() {
        /*
        _currentNoise = _worldNoise.NormalizeAverage(_currentNoise);
        _currentTexture = GenerateTexture(Width, Height, _currentNoise);
        Canvas.renderer.material.mainTexture = _currentTexture;
        */
    }

    private void OnSaveClick() {
        SaveTextureToFile(_currentTexture, "Resources/Textures/test.png");
    }

    private void RefreshWorld() {
        XYZ screenChunks = new XYZ((int)ScreenChunks.x, (int)ScreenChunks.y, (int)ScreenChunks.z);
        XY screenCount = new XY((int)ScreenCount.x, (int)ScreenCount.y);
        WorldConfig worldConfig = new WorldConfig(8, screenChunks, screenCount);

        _worldGenerator = new WorldGenerator();
        _currentWorld = _worldGenerator.GenerateWorld(Random.Range(0, 60000).ToString(), worldConfig);

        // Pre-calculate centroids of rooms in current world.
        _centroids.Clear();

        foreach(WorldScreen screen in _currentWorld.Screens) {
            XY screenCoord = screen.Coord;
            XY screenOffset = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z);

            foreach(Room room in screen.Rooms)
                _centroids[room] = screenOffset + XY.Average(room.Perimeter.ToList());
        }

        UpdateTexture();
    }

    private void UpdateTexture() {
        _currentTexture = GenerateTexture(_currentWorld);
        Canvas.GetComponent<Renderer>().material.mainTexture = _currentTexture;
    }

    private Texture2D GenerateTexture(World world) {
        XYZ worldChunks = world.Config.WorldChunks;
        XYZ screenChunks = world.Config.ScreenChunks;
        int elevations = world.Config.ScreenChunks.Y;

        int width = worldChunks.X;
        int height = worldChunks.Z;
        float normalizeRatio = 1.0f / elevations;

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

        Color[] pixels = new Color[width * height];

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float sample = world.Noise[x, y] * normalizeRatio;
                pixels[y * width + x] = new Color(sample, sample, sample);
            }
        }

        // Prevent room border colors from changing when other features are toggled.
        Random.seed = 0;

        foreach(WorldScreen screen in world.Screens) {
            //WorldScreen screen = screenPair.Value;
            XY screenCoord = screen.Coord;
            XY screenOffset = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z);

            foreach(Room room in screen.Rooms) {
                // Draw a colored border around the perimeter of the room.
                if(_drawRooms) {
                    Color roomColor = new Color(Random.value, Random.value, Random.value);

                    foreach(XY coord in room.Perimeter) {
                        XY edgeCoord = screenOffset + coord;
                        pixels[edgeCoord.Y * width + edgeCoord.X] = roomColor;
                    }
                }
            }
        }

        foreach(WorldScreen screen in world.Screens) {
            //WorldScreen screen = screenPair.Value;
            XY screenCoord = screen.Coord;
            XY screenOffset = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z);
            
            foreach(Room room in screen.Rooms) {
                XY centroid = _centroids[room];

                // Draw lines between the centers of all connected rooms.
                if(_drawConnections) {
                    foreach(Room neighbor in room.Neighbors) {
                        List<XY> line = MathUtils.CalculateLineCoords(_centroids[room], _centroids[neighbor]);

                        foreach(XY point in line) {
                            //if(point.Y * width + point.X < pixels.Length)
                                pixels[point.Y * width + point.X] = Color.blue;
                        }
                    }
                }
            }
        }

        foreach(WorldScreen screen in world.Screens) {
            //WorldScreen screen = screenPair.Value;
            XY screenCoord = screen.Coord;
            XY screenOffset = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z);
            
            foreach(Room room in screen.Rooms) {
                // Draw lines between rooms that contain edges in the spanning tree.
                if(_drawSpanningTree) {                    
                    foreach(Room neighbor in room.Neighbors) {
                        if(room.Parent == neighbor || neighbor.Parent == room) {
                            List<XY> line = MathUtils.CalculateLineCoords(_centroids[room], _centroids[neighbor]);
                            
                            foreach(XY point in line)
                                pixels[point.Y * width + point.X] = Color.green;
                        }
                    }
                }
            }
        }

        foreach(WorldScreen screen in world.Screens) {
            //WorldScreen screen = screenPair.Value;
            XY screenCoord = screen.Coord;
            XY screenOffset = new XY(screenCoord.X * screenChunks.X, screenCoord.Y * screenChunks.Z);
            
            foreach(Room room in screen.Rooms) {
                XY centroid = _centroids[room];

                // Draw a dot in the center of the room. The method used for calculating the
                // centroid does not work well for convex polygons. A true centroid calculation
                // is currently impossible because our lists of perimeter coords are not ordered.
                if(_drawCenters) {
                    pixels[centroid.Y * width + centroid.X] = Color.red;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    private void SaveTextureToFile(Texture2D tex, string filepath) {
        string fullpath = Application.dataPath + "/" + filepath;
        FileStream file = File.Open(fullpath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        
        byte[] bytes = tex.EncodeToPNG();
        writer.Write(bytes);
        writer.Close();

        file.Close();
    }
}
