using System;
using UnityEngine;

namespace VoxelBrave
{
    public enum CameraMode
    {
        /// <summary>
        /// 通常カメラワーク
        /// </summary>
        Normal,

        /// <summary>
        /// ロックオンカメラワーク
        /// </summary>
        LockOn
    }

    /// <summary>
    /// プレイヤー操作クラス
    /// </summary>
    public partial class PlayerCameraController : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーカメラモデルデータ
        /// </summary>
        [SerializeField]
        private PlayerCameraModel model = null;

        /// <summary>
        /// 現在制御中のカメラ
        /// </summary>
        [SerializeField]
        private Camera currentCamera = null;

        [SerializeField]
        private PlayerController player = null;

        private CameraMode cameraMode = CameraMode.Normal;

        /// <summary>
        /// カメラモード切り替え時のヘルパークラス
        /// </summary>
        private CameraModeChangeHelper cameraModeChangeHelper = null;

        #region Camera Parameter Property --------------------------------------------------------------------------

        /// <summary>
        /// カメラパラメータ
        /// </summary>
        [SerializeField, Header("カメラパラメータ")]
        private CameraParameter param = new CameraParameter();

        /// <summary>
        /// 追跡するターゲット
        /// </summary>
        private ICameraPosition TrackTarget { set => param.ViewTarget = value; get => param.ViewTarget; }

        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        private ICameraPosition LockOnTarget { set => param.ViewLockOn = value; get => param.ViewLockOn; }

        /// <summary>
        /// オフセット座標
        /// </summary>
        private Vector3 TrackOffsetPos { set => param.ViewOffset = value; get => param.ViewOffset; }

        /// <summary>
        /// 追跡ターゲットとの角度
        /// </summary>
        private Vector3 TrackAngle { set => param.ViewAngle = value; get => param.ViewAngle; }

        /// <summary>
        /// 追跡ターゲットとの距離
        /// </summary>
        private float TrackDistance { set => param.ViewDistance = value; get => param.ViewDistance; }

        #endregion

        #region Camera Property

        /// <summary>
        /// カメラ感度
        /// </summary>
        private readonly Vector2 mouseSensitive = new Vector2(3, 1.5f);

        #endregion

        #region Wall Check Property --------------------------------------------------------------------------------

        /// <summary>
        /// カメラ当たり判定の半径
        /// </summary>
        private float cameraHitRadius = 0.25f;

        /// <summary>
        /// カメラと衝突判定レイヤー
        /// </summary>
        private LayerMask hitMask;

        /// <summary>
        /// 壁判定用Raycast
        /// </summary>
        private RaycastHit cameraHit;

        #endregion

        #region Normal Lerp Property --------------------------------------------------------------------------------------

        [Header("Lerp量"), SerializeField, Range(1f, 100f)]
        private float LerpPower = 25;

        [Header("カメラ注視のLerp量"), SerializeField, Range(1, 100)]
        private float LookAtLerpPower = 25;

        [Header("カメラ角度のLerp量"), SerializeField, Range(1, 100)]
        private float AngleLerpPower = 25;

        /// <summary>
        /// カメラ描画座標Lerp用
        /// </summary>
        private Vector3 nextPosition;

        /// <summary>
        /// TrackTargetとTrackOffsetを足し合わせた座標
        /// </summary>
        private Vector3 VT => v + t;

        /// <summary>
        /// カメラ追尾Lerp用
        /// </summary>
        private Vector3 v;

        /// <summary>
        /// カメラoffsetPosのLerp用
        /// </summary>
        private Vector3 t;

        /// <summary>
        /// カメラ回転角度Lerp用
        /// </summary>
        private Quaternion q;

        /// <summary>
        /// カメラ距離Lerp用
        /// </summary>
        private float d;

        /// <summary>
        /// 通常時のカメラ距離と壁判定時のカメラ距離をLerpで切り替える用の変数
        /// </summary>
        private float cameraDistance;

        /// <summary>
        /// カメラロックオンLerp用
        /// </summary>
        private Vector3 l;

        #endregion

        private void Start()
        {
            model.Init(player);
            InitCameraParameter(model.GetParameter(CameraMode.Normal));
            cameraModeChangeHelper = new(model, param);
            hitMask = LayerMask.GetMask("CameraHit");
        }

        // -------------------------------------------------------------
        // LateUpdateで処理
        // -------------------------------------------------------------

        /// <summary>
        /// キャラクター座標以外の処理をLateUpdateで行う
        /// </summary>
        private void LateUpdate()
        {
            // 追尾するターゲットの移動後に処理をしたいのでLateUpdateで処理する
            if (currentCamera == null)
                return;

            ClacCameraPosition();
            SwicthLockOnCameraMode();
        }

        /// <summary>
        /// 通常座標・壁衝突時の座標計算を行う
        /// <para>カメラ座標のセットはFixedUpdateで行う</para>
        /// </summary>
        private void ClacCameraPosition()
        {
            ClacNextPosition();
            // 通常時の距離と壁判定時の距離をLerpで求める
            if (cameraHit.collider == null)
            {
                cameraDistance = Mathf.Lerp(cameraDistance, d, Time.deltaTime * LerpPower);
            }
            else
            {
                float dWall = -(Vector3.Distance(v, cameraHit.point) - cameraHitRadius);
                cameraDistance = Mathf.Lerp(cameraDistance, dWall, Time.deltaTime * LerpPower);
            }
        }

        /// <summary>
        /// 次の座標計算用関数
        /// <para>壁判定などを計算するために早い段階で計算</para>
        /// </summary>
        private void ClacNextPosition()
        {
            q = Quaternion.Lerp(q, GetRotateQuaternion(), Time.deltaTime * AngleLerpPower);
            d = Mathf.Lerp(d, GetDistance(), Time.deltaTime * LerpPower);
            // forward : カメラの正面方向がクォータニオンによって回転された方向とする
            nextPosition = VT + q * Vector3.forward * d;
        }

        /// <summary>
        /// カメラ角度をクオータニオンとして取得
        /// </summary>
        private Quaternion GetRotateQuaternion()
        {
            var angle = TrackAngle;
            switch (cameraMode)
            {
                // 任意角度指定
                case CameraMode.Normal:
                    angle = GetNormalAngle(angle);
                    break;
                case CameraMode.LockOn:
                    angle = GetLockOnAutoAngle(angle);
                    break;
            }
            TrackAngle = angle;
            return Quaternion.Euler(TrackAngle.x, TrackAngle.y, 0);
        }

        /// <summary>
        /// ユーザー任意角度設定
        /// </summary>
        private Vector3 GetMouseAngle()
        {
            var angle = Vector2.zero;
            angle.x = Input.GetAxis("Mouse Y") * mouseSensitive.y;
            angle.y = Input.GetAxis("Mouse X") * mouseSensitive.x;
            return angle;
        }

        private Vector2 GetNormalAngle(Vector2 viewAngle)
        {
            Vector2 mouseAngle = GetMouseAngle();
            viewAngle.x -= mouseAngle.x;
            viewAngle.x = Mathf.Clamp(viewAngle.x, -MAX_ANGLE, MAX_ANGLE);
            viewAngle.y += mouseAngle.y;
            return viewAngle;
        }

        /// <summary>
        /// ロックオン時のオート角度設定
        /// </summary>
        private Vector2 GetLockOnAutoAngle(Vector2 viewAngle)
        {
            // プレイヤーを基点としたカメラ座標とロックオンターゲット座標からcosΘを計算
            var playerPos = player.GetBottomTransfrom();
            var camPos = currentCamera.transform.position;
            var lockOn = LockOnTarget.GetBottomTransfrom();
            var signedPlayerΘ = MathfExtension.GetSignedAngle(playerPos, camPos, lockOn);

            // プレイヤーとロックオンターゲットとの距離
            var diff_PL_distance = Vector3.Distance(player.GetBottomTransfrom(), LockOnTarget.GetBottomTransfrom());

            float diff = 0;
            float autoLockAngle = 0;
            // 指定座標までプレイヤーとターゲットが正面になるようなカメラアングル
            if (diff_PL_distance > EXPAND_LOCKON_DISTANCE)
            {
                autoLockAngle = LOCKON_ANGLE_CENTER;
            }
            else
            {
                // プレイヤーと敵が近すぎると正面に来てしまい、見えずらくなるので角度を少し足した位置にカメラを配置する
                var perDistance = MathfExtension.ClampPercent(diff_PL_distance, LINIT_LOCKON_DISTANCE, EXPAND_LOCKON_DISTANCE);
                var diffAngle = (LOCKON_ANGLE_CENTER - MAX_LOCKON_ANGLE.x) * perDistance;
                autoLockAngle = MAX_LOCKON_ANGLE.x + diffAngle;

                // 自動オート角度範囲内で左右キーが入力されていれば、角度更新しない
                if (MathfExtension.IsRange(Mathf.Abs(signedPlayerΘ), autoLockAngle, LOCKON_ANGLE_CENTER) && player.IsLeftRight)
                {
                    return viewAngle;
                }
            }

            // 左右に角度を足していく
            var sign = signedPlayerΘ > 0 ? 1 : -1;
            diff = (autoLockAngle * sign - signedPlayerΘ);
            viewAngle.y += diff * Time.deltaTime;
            return viewAngle;
        }

        /// <summary>
        /// 距離設定
        /// </summary>
        private float GetDistance()
        {
            switch (cameraMode)
            {
                case CameraMode.Normal:
                    var wheel = -Input.GetAxis("Mouse ScrollWheel") + TrackDistance;
                    wheel = Mathf.Clamp(wheel, MIN_DISTANCE, MAX_DISTANCE);
                    TrackDistance = wheel;
                    break;
            }
            return -TrackDistance;
        }

        /// <summary>
        /// ロックオンカメラモード切り替え
        /// </summary>
        private void SwicthLockOnCameraMode()
        {
            if (Input.GetMouseButtonUp(1))
            {
                var preMode = cameraMode;
                var nextMode = cameraMode == CameraMode.Normal ? CameraMode.LockOn : CameraMode.Normal;
                UpdateCameraParameter(cameraModeChangeHelper.SwitchCameraMode(preMode, nextMode));
            }
        }

        /// <summary>
        /// パラメータ初期化
        /// </summary>
        private void InitCameraParameter(CameraParameter _param)
        {
            TrackTarget = player;
            UpdateCameraModeParameter(_param);
        }

        /// <summary>
        /// カメラパラメータ更新
        /// </summary>
        private void UpdateCameraParameter(SwitchCameraModeInfo info)
        {
            if (cameraMode == info.nextMode)
                return;

            UpdateCameraModeParameter(info.param);
            cameraMode = info.nextMode;
        }

        /// <summary>
        /// カメラモードに応じたパラメータ更新
        /// </summary>
        private void UpdateCameraModeParameter(CameraParameter _param)
        {
            LockOnTarget = _param.ViewLockOn;
            TrackOffsetPos = _param.ViewOffset;
            TrackAngle = _param.ViewAngle;
            TrackDistance = _param.ViewDistance;
        }

        // -------------------------------------------------------------
        // FixedUpdateで処理
        // -------------------------------------------------------------

        /// <summary>
        /// キャラクター座標取得/代入はFixedUpdateで行う (RigidBodyの計算の都合上)
        /// </summary>
        private void FixedUpdate()
        {
            if (currentCamera == null)
                return;

            t = Vector3.Lerp(t, TrackOffsetPos, Time.fixedDeltaTime * LerpPower);
            v = Vector3.Lerp(v, TrackTarget.GetCenterTransfrom(), Time.fixedDeltaTime * LerpPower);
            IsWallCheck(nextPosition);
            currentCamera.transform.position = VT + q * Vector3.forward * cameraDistance;
            SetCameraAngle();
        }

        /// <summary>
        /// 壁チェック
        /// </summary>
        /// <param name="movementVec">カメラ表示座標</param>
        private bool IsWallCheck(Vector3 camMovement)
        {
            Vector3 center = camMovement;
            var target = v;
            var direction = center - target;
            var distance = Vector3.Distance(center, target);
            Physics.SphereCast(target, cameraHitRadius, direction, out cameraHit, distance, hitMask);
            LogUtil.DrawRay(target, direction, Color.red, 0.2f);
            return cameraHit.collider != null;
        }

        /// <summary>
        /// 対象の角度セット
        /// </summary>
        private void SetCameraAngle()
        {
            var target = Vector3.zero;
            if (cameraMode == CameraMode.Normal)
            {
                target = VT;
            }
            else
            {
                // ２点間の中点
                var lookCenter = (LockOnTarget.GetCenterTransfrom() + VT) / 2;
                // 中心より少し上を中点
                lookCenter.y = Mathf.Max(LockOnTarget.GetCenterTransfrom().y, VT.y) / 2.5f;
                target = lookCenter;
            }
            l = Vector3.Lerp(l, target, Time.fixedDeltaTime * LookAtLerpPower);
            currentCamera.transform.rotation = Quaternion.LookRotation(l - currentCamera.transform.position);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // カメラ当たり判定
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentCamera.transform.position, cameraHitRadius);
        }

#endif
    }
}
