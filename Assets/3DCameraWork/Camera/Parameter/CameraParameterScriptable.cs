using System;
using UnityEngine;

namespace CameraWork
{
    [CreateAssetMenu(fileName = "名前を変更してね(CameraParameter)", menuName = "Camera/Parameter")]
    public class CameraParameterScriptable : ScriptableObject
    {
        [SerializeField]
        public CameraParameter parameter;
    }


    [Serializable]
    public class CameraParameter
    {
        /// <summary>
        /// 描画ターゲット
        /// </summary>
        public ICameraPosition ViewTarget { set; get; }

        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        public ICameraPosition ViewLockOn { set; get; }

        /// <summary>
        /// オフセット座標
        /// </summary>
        [SerializeField, Header("オフセット座標")]
        public Vector3 ViewOffset;

        /// <summary>
        /// 表示する角度
        /// </summary>
        [SerializeField, Header("追跡ターゲットとの角度")]
        public Vector3 ViewAngle;

        /// <summary>
        /// 描画距離
        /// </summary>
        [SerializeField, Range(1, 10), Header("追跡ターゲットとの距離")]
        public float ViewDistance;
    }
}