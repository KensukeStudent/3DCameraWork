using System;
using UnityEngine;

namespace VoxelBrave
{
    public struct SwitchCameraModeInfo
    {
        public CameraMode nextMode;

        public CameraParameter param;
    }

    /// <summary>
    /// カメラ切り替え用ベースクラス
    /// </summary>
    public abstract class PlayerCameraModeBase
    {
        protected PlayerCameraModel model;

        protected CameraParameter param;

        public PlayerCameraModeBase(PlayerCameraModel _model, CameraParameter _param)
        {
            model = _model;
            param = _param;
        }

        public abstract SwitchCameraModeInfo SwitchCameraMode(CameraMode nextMode);

        protected SwitchCameraModeInfo CreateCameraModeInfo(CameraMode nextMode, ICameraPosition trackTarget = null, ICameraPosition lockOnTarget = null, Vector3? viewOffset = null, Vector3? ViewAngle = null, float? ViewDistance = null)
        {
            return new SwitchCameraModeInfo()
            {
                nextMode = nextMode,
                param = new CameraParameter()
                {
                    ViewTarget = trackTarget,
                    ViewLockOn = lockOnTarget,
                    ViewOffset = viewOffset == null ? param.ViewOffset : (Vector3)viewOffset,
                    ViewAngle = ViewAngle == null ? param.ViewAngle : (Vector3)ViewAngle,
                    ViewDistance = ViewDistance == null ? param.ViewDistance : (float)ViewDistance,
                }
            };
        }

        public Exception DefineCameraModeException()
        {
            return new Exception($"{this.ToString()}クラスにはnextModeは定義されていません");
        }
    }
}