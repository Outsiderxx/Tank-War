public static class MyUtils
{
    public static float NormalizedAngle(float angle)
    {
        float result = angle % 360;
        return result < 0 ? result + 360 : result;
    }
}
