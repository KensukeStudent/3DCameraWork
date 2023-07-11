using UnityEngine;

namespace VoxelBrave
{
    [CreateAssetMenu(fileName = "CameraParameter", menuName = "Camera/Parameter")]
    public class CameraParameter : ScriptableObject
    {
        /// <summary>
        /// 描画ターゲット
        /// </summary>
        public Transform ViewTarget { set; get; }

        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        public Transform ViewLockOn { set; get; }

        /// <summary>
        /// 表示オフセット
        /// </summary>
        [SerializeField]
        private Vector3 viewOffset;

        public Vector3 ViewOffset => viewOffset;

        /// <summary>
        /// 表示する角度
        /// </summary>
        [SerializeField]
        private Vector3 viewAngle;

        public Vector3 ViewAngle => viewAngle;

        /// <summary>
        /// 描画距離
        /// </summary>
        [SerializeField]
        private float viewDistance;

        public float ViewDistance => viewDistance;
    }
}