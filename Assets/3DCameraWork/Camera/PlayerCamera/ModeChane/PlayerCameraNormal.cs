using UnityEngine;

namespace CameraWork
{
    /// <summary>
    /// プレイヤーカメラ通常時の切り替え定義クラス
    /// </summary>
    public class PlayerCameraNormal : PlayerCameraModeBase
    {
        public PlayerCameraNormal(PlayerCameraModel _model, PlayerCameraParameter _param) : base(_model, _param)
        {

        }

        public override SwitchCameraModeInfo SwitchCameraMode(CameraMode nextMode)
        {
            return nextMode switch
            {
                CameraMode.LockOn => GetLockOnInfo(),
                _ => throw DefineCameraModeException()
            };
        }

        private SwitchCameraModeInfo GetLockOnInfo()
        {
            CharacterBase lockTarget = model.Player.SearchLockOnTarget();
            var lockOnTarget = lockTarget != null ? lockTarget : model.Player;
            var nextMode = lockOnTarget.GetInstanceID() == model.Player.GetInstanceID() ? CameraMode.Normal : CameraMode.LockOn;
            var lockOnParam = model.GetParameter(CameraMode.LockOn);
            var angle = new Vector3(lockOnParam.ViewAngle.x, originParam.ViewAngle.y, 0);

            var nextParam = defaultParam;
            nextParam.ViewLockOn = lockOnTarget;
            nextParam.ViewAngle = angle;
            nextParam.ViewDistance = lockOnParam.ViewDistance;
            nextParam.AngleLerpPower = lockOnParam.AngleLerpPower;
            return CreateCameraModeInfo(nextMode, nextParam);
        }
    }
}
