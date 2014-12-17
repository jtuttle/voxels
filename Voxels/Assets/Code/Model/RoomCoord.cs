using System.Collections;

public class RoomCoord {
    public XY Coord { get; private set; }
    public float Elevation { get; private set; }

    public RoomCoord(XY coord, float elevation) {
        Coord = coord;
        Elevation = elevation;
    }
}
