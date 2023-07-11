using UnityEngine;

public static class MathfExtension
{
    /// <summary>
    /// X軸とZ軸の距離の差を取得
    /// </summary>
    public static float GetDiffDistance(this Vector3 value, Vector3 target)
    {
        float diffX = Mathf.Abs(Mathf.Abs(value.x) - Mathf.Abs(target.x));
        float diffZ = Mathf.Abs(Mathf.Abs(value.z) - Mathf.Abs(target.z));
        return diffX + diffZ;
    }

    /// <summary>
    /// minからmax範囲内からvalueが何パーセントかを取得
    /// </summary>
    public static float ClampPercent(float value, float min, float max)
    {
        // Clamp内の範囲内の値にする
        var clampValue = Mathf.Clamp(value, min, max);
        var range = Mathf.Abs(Mathf.Abs(max) - Mathf.Abs(min));
        var clipValue = Mathf.Abs(Mathf.Abs(clampValue) - Mathf.Abs(min));
        return clipValue / range;
    }

    /// <summary>
    /// 3点の座標から角度を取得
    /// <para>角度計算はVector.SignedAngleで計算(左右どちらの角度かを考慮)</para>
    /// </summary>
    /// <param name="origin">求められる角度</param>
    /// <param name="pointA">originからpointAに直線を引くベクトル</param>
    /// <param name="pointB">originからpointBに直線を引くベクトル</param>
    public static float GetSignedAngle(Vector3 origin, Vector3 pointA, Vector3 pointB)
    {
        Vector3 vectorOA3 = pointA - origin;
        Vector3 vectorOB3 = pointB - origin;

        Vector2 vectorOA2 = new Vector2(vectorOA3.x, vectorOA3.z);
        Vector2 vectorOB2 = new Vector2(vectorOB3.x, vectorOB3.z);
        return Vector2.SignedAngle(vectorOA2, vectorOB2);
    }

    /// <summary>
    /// 3点の座標から角度を取得
    /// <para>角度計算はVector.Angleで計算(左右どちらの角度かを考慮しない)</para>
    /// </summary>
    /// <param name="origin">求められる角度</param>
    /// <param name="pointA">originからpointAに直線を引くベクトル</param>
    /// <param name="pointB">originからpointBに直線を引くベクトル</param>
    public static float GetAngle(Vector3 origin, Vector3 pointA, Vector3 pointB)
    {
        Vector3 vectorOA3 = pointA - origin;
        Vector3 vectorOB3 = pointB - origin;

        Vector2 vectorOA2 = new Vector2(vectorOA3.x, vectorOA3.z);
        Vector2 vectorOB2 = new Vector2(vectorOB3.x, vectorOB3.z);
        return Vector2.Angle(vectorOA2, vectorOB2);
    }
}
