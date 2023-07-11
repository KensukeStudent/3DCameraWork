using UnityEngine;

namespace VoxelBrave
{
    /// <summary>
    /// プレイヤーモデルデータ
    /// </summary>
    public class PlayerCameraModel : MonoBehaviour
    {
        [SerializeField]
        private CameraParameter normalParameter = null;

        [SerializeField]
        private CameraParameter lockOnParameter = null;

        [SerializeField]
        private SimpleSampleCharacterControl player = null;

        public void Init()
        {
            normalParameter.ViewTarget = player.transform;
            normalParameter.ViewLockOn = player.transform;
        }

        /// <summary>
        /// プレイヤー取得
        /// </summary>
        public CharacterBase GetPlayer => player;

        /// <summary>
        /// ロックオン対象プレイヤー周辺の敵
        /// </summary>
        public CharacterBase GetLockOnTarget => player.SearchLockOnTarget();

        public CameraParameter GetParameter(CameraMode cameraMode)
        {
            return cameraMode switch
            {
                CameraMode.Normal => normalParameter,
                CameraMode.LockOn => lockOnParameter,
                _ => throw new System.Exception("")
            };
        }

        /// <summary>
        /// 壁判定用の座標取得
        /// </summary>
        public Vector3 GetTrackTargetByWallCheakPosition()
        {
            var trackPos = GetPlayer.GetCenterTransfrom();
            trackPos.z += GetPlayer.BodySize.z / 2;
            return trackPos;
        }
    }
}