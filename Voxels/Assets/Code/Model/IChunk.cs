using UnityEngine;
using System.Collections;

public interface IChunk {
    byte GetBlock(int x, int y, int z);
}
