using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureAtlas {
    //public Texture2D Texture { get; private set; }

    private int _rows;
    private int _cols;

    private Vector2 _tUnits;

    public TextureAtlas(int rows, int cols) {
        //Texture = texture;
        _rows = rows;
        _cols = cols;

        _tUnits = new Vector2(1.0f / cols, 1.0f / rows);
    }

    public Vector2[] getUVCoords(int index) {
        Vector2[] uvs = new Vector2[4];

        int xPos = index % _rows;
        int yPos = (int)Mathf.Floor(index / _rows);

        uvs[0] = new Vector2(_tUnits.x * xPos + _tUnits.x, _tUnits.y * yPos);
        uvs[1] = new Vector2(_tUnits.x * xPos + _tUnits.x, _tUnits.y * yPos + _tUnits.y);
        uvs[2] = new Vector2(_tUnits.x * xPos, _tUnits.y * yPos + _tUnits.y);
        uvs[3] = new Vector2(_tUnits.x * xPos, _tUnits.y * yPos);

        /*
        uvs[0] = new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y);
        uvs[1] = new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit);
        uvs[2] = new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit);
        uvs[3] = new Vector2(tUnit * texturePos.x, tUnit * texturePos.y);
        */

        return uvs;
    }
}
