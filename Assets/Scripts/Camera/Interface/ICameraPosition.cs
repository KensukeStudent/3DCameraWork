using UnityEngine;

namespace VoxelBrave
{
    /// <summary>
    /// カメラに描画されるオブジェクトにつくインターフェース
    /// </summary>
    public interface ICameraPosition
    {
        public Vector3 GetTopTransfrom();

        public Vector3 GetCenterTransfrom();

        public Vector3 GetBottomTransfrom();
    }
}