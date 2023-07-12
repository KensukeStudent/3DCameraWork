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

    [Serializable]
    public class PlayerCameraParameter
    {
        /// <summary>
        /// 追跡するターゲット
        /// </summary>
        [SerializeField, Header("追跡ターゲット")]
        public CharacterBase trackTarget = null;

        /// <summary>
        /// オフセット座標
        /// </summary>
        [SerializeField, Header("オフセット座標")]
        public Vector3 trackOffsetPos = Vector3.zero;

        /// <summary>
        /// オフセットアングル
        /// </summary>
        [SerializeField, Header("オフセット角度")]
        public Vector3 trackOffsetAngle = Vector3.zero;

        /// <summary>
        /// 追跡ターゲットとの距離
        /// </summary>
        [SerializeField, Header("追跡ターゲットとの距離")]
        public float trackDistance = 0;

        /// <summary>
        /// 追跡ターゲットとの角度
        /// </summary>
        [SerializeField, Header("追跡ターゲットとの角度")]
        public Vector3 trackAngle = Vector3.zero;

        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        [SerializeField, Header("ロックオン対象")]
        public CharacterBase lockOnTarget = null;
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

        private CameraMode cameraMode = CameraMode.Normal;

        /// <summary>
        /// 現在制御中のカメラ
        /// </summary>
        [SerializeField]
        private Camera currentCamera = null;

        #region Camera Parameter Property --------------------------------------------------------------------------

        /// <summary>
        /// カメラパラメータ
        /// </summary>
        [SerializeField, Header("カメラパラメータ")]
        private PlayerCameraParameter param = new PlayerCameraParameter();

        /// <summary>
        /// 追跡するターゲット
        /// </summary>
        private CharacterBase TrackTarget { set => param.trackTarget = value; get => param.trackTarget; }

        /// <summary>
        /// オフセット座標
        /// </summary>
        private Vector3 TrackOffsetPos { set => param.trackOffsetPos = value; get => param.trackOffsetPos; }

        /// <summary>
        /// オフセットアングル
        /// </summary>
        private Vector3 TrackOffsetAngle { set => param.trackOffsetAngle = value; get => param.trackOffsetAngle; }

        /// <summary>
        /// 追跡ターゲットとの距離
        /// </summary>
        private float TrackDistance { set => param.trackDistance = value; get => param.trackDistance; }

        /// <summary>
        /// 追跡ターゲットとの角度
        /// </summary>
        private Vector3 TrackAngle { set => param.trackAngle = value; get => param.trackAngle; }

        /// <summary>
        /// ロックオンターゲット
        /// </summary>
        private CharacterBase LockOnTarget { set => param.lockOnTarget = value; get => param.lockOnTarget; }

        /// <summary>
        /// カメラ感度
        /// </summary>
        private readonly Vector2 mouseSenitivity = new Vector2(3, 1.5f);

        #endregion

        #region  Camera Lerp Property

        /// <summary>
        /// カメラクオータニオン
        /// <para>最後にカメラに代入される角度</para>
        /// </summary>
        private Quaternion cameraQ;

        /// <summary>
        /// カメラ距離
        /// <para>最後にカメラに代入される距離</para>
        /// </summary>
        private float cameraD;

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
        private const float LerpPower = 25;

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
        /// カメラロックオンLerp用
        /// </summary>
        private Vector3 l;

        #endregion

        private void Start()
        {
            model.Init();
            UpdateParameter(model.GetParameter(CameraMode.Normal));
            hitMask = LayerMask.GetMask("CameraHit");
        }

        /// <summary>
        /// キャラクター座標以外の処理をLateUpdateで行う
        /// </summary>
        private void LateUpdate()
        {
            // 追尾するターゲットの移動後に処理をしたいのでLateUpdateで処理する
            if (currentCamera == null)
                return;

            ClacCameraPosition();
            SetCameraAngle();
            SwicthCameraMode();
        }

        /// <summary>
        /// キャラクター座標取得/代入はFixedUpdateで行う (RigidBodyの計算の都合上)
        /// </summary>
        private void FixedUpdate()
        {
            if (currentCamera == null)
                return;

            t = Vector3.Lerp(t, TrackOffsetPos, Time.deltaTime * LerpPower);
            v = Vector3.Lerp(v, TrackTarget.GetCenterTransfrom(), Time.deltaTime * LerpPower);
            IsWallCheck(nextPosition);
            currentCamera.transform.position = VT + cameraQ * Vector3.forward * cameraD;
        }

        /// <summary>
        /// 通常座標・壁衝突時の座標計算を行う
        /// <para>カメラ座標のセットはFixedUpdateで行う</para>
        /// </summary>
        private void ClacCameraPosition()
        {
            ClacNextPosition();
            cameraQ = q;
            // 壁当たり判定用のカメラ座標
            if (cameraHit.collider == null)
            {
                cameraD = Mathf.Lerp(cameraD, d, Time.deltaTime * LerpPower);
            }
            else
            {
                float dWall = -(Vector3.Distance(v, cameraHit.point) - cameraHitRadius);
                cameraD = Mathf.Lerp(cameraD, dWall, Time.deltaTime * LerpPower);
            }
        }

        /// <summary>
        /// 次の座標計算用関数
        /// <para>壁判定などを計算するために早い段階で計算</para>
        /// </summary>
        private void ClacNextPosition()
        {
            q = Quaternion.Lerp(q, GetRotateQuaternion(), Time.deltaTime * LerpPower);
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
            angle.x = Input.GetAxis("Mouse Y") * mouseSenitivity.y;
            angle.y = Input.GetAxis("Mouse X") * mouseSenitivity.x;
            return angle;
        }

        private Vector2 GetNormalAngle(Vector2 trackAngle)
        {
            Vector2 mouseAngle = GetMouseAngle();
            trackAngle.x -= mouseAngle.x;
            trackAngle.x = Mathf.Clamp(trackAngle.x, -MAX_ANGLE, MAX_ANGLE);
            trackAngle.y += mouseAngle.y;
            return trackAngle;
        }

        /// <summary>
        /// ロックオン時のオート角度設定
        /// </summary>
        private Vector2 GetLockOnAutoAngle(Vector2 trackAngle)
        {
            // プレイヤーを基点としたカメラ座標とロックオンターゲット座標からcosΘを計算
            var playerPos = model.GetPlayer.GetBottomTransfrom();
            var camPos = currentCamera.transform.position;
            var lockOn = LockOnTarget.GetBottomTransfrom();
            var signedPlayerΘ = MathfExtension.GetSignedAngle(playerPos, camPos, lockOn);

            // プレイヤーとロックオンターゲットとの距離
            var diff_PL_distance = Vector3.Distance(model.GetPlayer.GetBottomTransfrom(), LockOnTarget.GetBottomTransfrom());

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
                if (MathfExtension.IsRange(Mathf.Abs(signedPlayerΘ), autoLockAngle, LOCKON_ANGLE_CENTER) && model.GetPlayer.IsLeftRight)
                {
                    return trackAngle;
                }
            }

            // 左右に角度を足していく
            var sign = signedPlayerΘ > 0 ? 1 : -1;
            diff = (autoLockAngle * sign - signedPlayerΘ);
            trackAngle.y += diff * Time.deltaTime;
            return trackAngle;
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
            Vector3 direction = Vector3.zero;
            switch (cameraMode)
            {
                case CameraMode.Normal:
                    // プレイヤーとカメラ方向のベクトル成分を取得
                    direction = VT - currentCamera.transform.position;
                    break;

                case CameraMode.LockOn:
                    // ２点間の中点
                    var lookCenter = (LockOnTarget.GetCenterTransfrom() + v) / 2;
                    lookCenter.y = Mathf.Max(LockOnTarget.GetCenterTransfrom().y, v.y);
                    direction = lookCenter - currentCamera.transform.position;
                    break;
            }
            currentCamera.transform.rotation = Quaternion.LookRotation(direction);
        }

        /// <summary>
        /// カメラモード切り替え
        /// </summary>
        private void SwicthCameraMode()
        {
            if (Input.GetMouseButtonUp(1))
            {
                CameraMode mode = CameraMode.Normal;
                if (cameraMode == CameraMode.LockOn)
                {
                    mode = CameraMode.Normal;
                    LockOnTarget = model.GetPlayer;
                }
                else
                {
                    CharacterBase lockTarget = model.GetLockOnTarget;
                    LockOnTarget = lockTarget != null ? lockTarget : model.GetPlayer;
                    mode = LockOnTarget == model.GetPlayer ? CameraMode.Normal : CameraMode.LockOn;
                }

                // カメラパラメータ更新
                SwitchCameraParameter(mode);
            }
        }

        /// <summary>
        /// カメラパラメータ更新
        /// </summary>
        private void SwitchCameraParameter(CameraMode mode)
        {
            if (mode == cameraMode)
                return;

            UpdateCameraModeParameter(mode);
            cameraMode = mode;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // カメラ当たり判定
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentCamera.transform.position, cameraHitRadius);
        }
#endif

        /// <summary>
        /// カメラモードに応じたパラメータ更新
        /// </summary>
        private void UpdateCameraModeParameter(CameraMode mode)
        {
            TrackOffsetPos = model.GetParameter(mode).ViewOffset;
            TrackAngle = model.GetParameter(mode).ViewAngle;
            TrackDistance = model.GetParameter(mode).ViewDistance;
        }

        /// <summary>
        /// カメラパラメーター更新
        /// </summary>
        public void UpdateParameter(CameraParameter parameter)
        {
            param.trackTarget = parameter.ViewTarget.GetComponent<CharacterBase>();
            param.trackOffsetPos = parameter.ViewOffset;
            param.trackOffsetAngle = parameter.ViewAngle;
            param.trackAngle = parameter.ViewAngle;
            param.trackDistance = parameter.ViewDistance;
            param.lockOnTarget = parameter.ViewLockOn.GetComponent<CharacterBase>();
        }
    }
}
