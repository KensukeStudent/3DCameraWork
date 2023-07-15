using System;
using UnityEngine;

namespace VoxelBrave
{
    public struct SwitchCameraModeInfo
    {
        public CameraMode nextMode;

        public PlayerCameraParameter param;
    }

    /// <summary>
    /// カメラ切り替え用ベースクラス
    /// </summary>
    public abstract class PlayerCameraModeBase
    {
        protected PlayerCameraModel model;

        protected PlayerCameraParameter originParam;

        protected PlayerCameraParameter defaultParam => new()
        {
            ViewTarget = null,
            ViewLockOn = null,
            ViewOffset = CommonConst.INIT_VECTOR3,
            ViewAngle = CommonConst.INIT_VECTOR3,
            ViewDistance = CommonConst.INIT_INT,
            LerpPower = CommonConst.INIT_INT,
            AngleLerpPower = CommonConst.INIT_INT
        };

        public PlayerCameraModeBase(PlayerCameraModel _model, PlayerCameraParameter _param)
        {
            model = _model;
            originParam = _param;
        }

        public abstract SwitchCameraModeInfo SwitchCameraMode(CameraMode nextMode);

        protected virtual SwitchCameraModeInfo CreateCameraModeInfo(CameraMode nextMode, PlayerCameraParameter nextParam)
        {
            nextParam.ViewTarget = nextParam.ViewTarget;
            nextParam.ViewLockOn = nextParam.ViewLockOn;
            nextParam.ViewOffset = ToUseVector3(nextParam.ViewOffset, originParam.ViewOffset);
            nextParam.ViewAngle = ToUseVector3(nextParam.ViewAngle, originParam.ViewAngle);
            nextParam.ViewDistance = ToUseFloat(nextParam.ViewDistance, originParam.ViewDistance);
            nextParam.LerpPower = ToUseFloat(nextParam.LerpPower, originParam.LerpPower);
            nextParam.AngleLerpPower = ToUseFloat(nextParam.AngleLerpPower, originParam.AngleLerpPower);

            return new SwitchCameraModeInfo()
            {
                nextMode = nextMode,
                param = nextParam
            };
        }

        private Vector3 ToUseVector3(Vector3 nextVector3, Vector3 origin)
        {
            nextVector3.x = nextVector3.x == CommonConst.INIT_INT ? origin.x : nextVector3.x;
            nextVector3.y = nextVector3.y == CommonConst.INIT_INT ? origin.y : nextVector3.y;
            nextVector3.z = nextVector3.z == CommonConst.INIT_INT ? origin.z : nextVector3.z;
            return nextVector3;
        }

        private float ToUseFloat(float nextFloat, float origin)
        {
            return nextFloat == CommonConst.INIT_INT ? origin : nextFloat;
        }

        public Exception DefineCameraModeException()
        {
            return new Exception($"{this.ToString()}クラスにはnextModeは定義されていません");
        }
    }
}