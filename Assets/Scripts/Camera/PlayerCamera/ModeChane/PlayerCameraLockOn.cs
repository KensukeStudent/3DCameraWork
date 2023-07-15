namespace VoxelBrave
{
    /// <summary>
    /// プレイヤーカメラロックオンモード時の切り替え定義クラス
    /// </summary>
    public class PlayerCameraLockOn : PlayerCameraModeBase
    {
        public PlayerCameraLockOn(PlayerCameraModel _model, CameraParameter _param) : base(_model, _param)
        {

        }

        public override SwitchCameraModeInfo SwitchCameraMode(CameraMode nextMode)
        {
            return nextMode switch
            {
                CameraMode.Normal => GetNormalInfo(),
                _ => throw DefineCameraModeException()
            };
        }

        private SwitchCameraModeInfo GetNormalInfo()
        {
            var normalParam = model.GetParameter(CameraMode.Normal);
            return CreateCameraModeInfo(CameraMode.Normal, lockOnTarget: normalParam.ViewLockOn, ViewDistance: normalParam.ViewDistance);
        }
    }
}
