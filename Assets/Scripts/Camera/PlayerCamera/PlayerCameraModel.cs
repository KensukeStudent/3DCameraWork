using UnityEngine;

namespace VoxelBrave
{
    /// <summary>
    /// プレイヤーモデルデータ
    /// </summary>
    public class PlayerCameraModel : MonoBehaviour
    {
        /// <summary>
        /// 通常カメラワークデータ
        /// </summary>
        [SerializeField]
        private PlayerCameraParameterScriptable normalParameter = null;

        /// <summary>
        /// ロックオンカメラワークデータ
        /// </summary>
        [SerializeField]
        private PlayerCameraParameterScriptable lockOnParameter = null;

        // TODO : モデルからでも参照できるように定義
        public PlayerController Player { private set; get; } = null;

        public void Init(PlayerController _player)
        {
            Player = _player;
            normalParameter.parameter.ViewTarget = Player;
            normalParameter.parameter.ViewLockOn = Player;

            normalParameter.CheckValidation(CameraMode.Normal, () => normalParameter);
            lockOnParameter.CheckValidation(CameraMode.LockOn, () => lockOnParameter);
        }

        public CameraParameter GetParameter(CameraMode cameraMode)
        {
            return cameraMode switch
            {
                CameraMode.Normal => normalParameter.parameter,
                CameraMode.LockOn => lockOnParameter.parameter,
                _ => throw new System.Exception("")
            };
        }
    }
}