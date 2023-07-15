using UnityEngine;

namespace VoxelBrave
{
    /// <summary>
    /// プレイヤーモデルデータ
    /// </summary>
    public class PlayerCameraModel : MonoBehaviour
    {
        [SerializeField]
        private CameraParameterScriptable normalParameter = null;

        [SerializeField]
        private CameraParameterScriptable lockOnParameter = null;

        // TODO : モデルからでも参照できるように定義
        public PlayerController Player { private set; get; } = null;

        public void Init(PlayerController _player)
        {
            Player = _player;
            normalParameter.cameraParameter.ViewTarget = Player;
            normalParameter.cameraParameter.ViewLockOn = Player;
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
                CameraMode.Normal => normalParameter.cameraParameter,
                CameraMode.LockOn => lockOnParameter.cameraParameter,
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