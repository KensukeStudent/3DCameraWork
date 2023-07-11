using UnityEngine;
using System.Diagnostics;

namespace VoxelBrave
{
    /// <summary>
    /// ログユーティリティークラス
    /// </summary>
    public class LogUtil
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(object o, bool isActive = true)
        {
            if (!isActive)
                return;

            UnityEngine.Debug.Log(o);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
        {
            UnityEngine.Debug.DrawRay(start, dir, color, duration);
        }
    }
}
