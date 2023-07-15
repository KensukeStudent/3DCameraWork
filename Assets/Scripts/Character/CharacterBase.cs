using UnityEngine;

namespace VoxelBrave
{
    /// <summary>
    /// キャラクターベースクラス
    /// <para>移動やカメラのロックオン、HPなどを持っているキャラクターに継承させる</para>
    /// </summary>
    public abstract class CharacterBase : MonoBehaviour, ICameraPosition
    {
        /// <summary>
        /// 3DモデルのRenderer
        /// </summary>
        [SerializeField]
        private Renderer modelRenderer = null;

        [SerializeField]
        private float centerPosY = 0;

        /// <summary>
        /// modelのboundsサイズ
        /// </summary>
        public Vector3 BodySize { private set; get; }

        /// <summary>
        /// モデルのPivotが初めから中心である
        /// </summary>
        [Header("モデルのPivotが初めから中心である"), SerializeField]
        private bool isCenter = false;

        [SerializeField]
        protected Animator animator = null;

        [SerializeField]
        protected Rigidbody rb = null;

        protected Vector3 movingDirecion;

        /// <summary>
        /// 移動中
        /// </summary>
        public bool IsMoving => movingDirecion.magnitude > 0;

        /// <summary>
        /// 入力方向
        /// </summary>
        protected Vector2 inputDirection = Vector2.zero;

        /// <summary>
        /// 左右に入力中であるか
        /// </summary>
        public bool IsLeftRight => inputDirection.x != 0;

        /// <summary>
        /// 上下に入力中であるか
        /// </summary>
        public bool IsUpDown => inputDirection.y != 0;

        protected virtual void Start()
        {
            if (modelRenderer == null)
            {
                BodySize = new Vector3(0, centerPosY, 0);
            }
            else
            {
                BodySize = modelRenderer.bounds.size;
            }
        }

        /// <summary>
        /// 頂点座標を取得
        /// </summary>
        public Vector3 GetTopTransfrom()
        {
            var pos = transform.position;
            pos.y += BodySize.y;
            return pos;
        }

        /// <summary>
        /// 中点座標を取得
        /// </summary>
        public Vector3 GetCenterTransfrom()
        {
            if (isCenter)
            {
                return transform.position;
            }

            var pos = transform.position;
            pos.y += BodySize.y / 2;
            return pos;
        }

        /// <summary>
        /// 底点座標を取得
        /// </summary>
        public Vector3 GetBottomTransfrom()
        {
            return transform.position;
        }
    }
}