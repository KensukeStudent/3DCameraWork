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

        public CameraParameter GetParameter(CameraMode cameraMode)
        {
            return cameraMode switch
            {
                CameraMode.Normal => normalParameter.cameraParameter,
                CameraMode.LockOn => lockOnParameter.cameraParameter,
                _ => throw new System.Exception("")
            };
        }
    }
}