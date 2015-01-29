using System.Collections;

public class RoomTile {
    public XY Coord { get; private set; }
    public float Elevation { get; private set; }

    public RoomTile(XY coord, float elevation) {
        Coord = coord;
        Elevation = elevation;
    }
}
