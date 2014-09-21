using System.Collections;

public class MathUtils {
    public static float ConvertRange(float oldMin, float oldMax, float newMin, float newMax, float value) {
        float scale = (float)(newMax - newMin) / (oldMax - oldMin);
        return newMin + ((value - oldMin) * scale);
    }
}
