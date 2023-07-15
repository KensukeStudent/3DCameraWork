using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBrave
{
    /// <summary>
    /// キャラクター仮
    /// </summary>
    public class PlayerController : CharacterBase
    {
        #region Move Property -----------------------------------------

        [SerializeField]
        private float m_moveSpeed = 2;

        /// <summary>
        /// 移動Lerpパワー
        /// </summary>
        [SerializeField]
        private float moveLerpPower = 10;

        /// <summary>
        /// 回転Lerpパワー
        /// </summary>
        [SerializeField]
        private float turnLerpPower = 10;

        #endregion

        #region Jump Property -----------------------------------------

        private bool m_jumpInput = false;

        [SerializeField]
        private float m_jumpForce = 4;

        /// <summary>
        /// ジャンプインターバル用タイム
        /// </summary>
        private float m_jumpTimeStamp = 0;

        /// <summary>
        /// ジャンプインターバル
        /// </summary>
        private float m_minJumpInterval = 0.25f;

        #endregion

        #region Wall Check Property ------------------------------------

        private bool m_wasGrounded;

        private bool m_isGrounded;

        private List<Collider> m_collisions = new List<Collider>();

        #endregion

#if UNITY_EDITOR

        [Header("Editor用")]
        [SerializeField, Range(5, 20)]
        private float radius = 0;

        [SerializeField]
        private Color sphere = Color.yellow;

#endif

        private void Update()
        {
            DirectUpdate();
            if (!m_jumpInput && Input.GetKey(KeyCode.Space))
            {
                m_jumpInput = true;
            }
            animator.SetFloat("MoveSpeed", movingDirecion.magnitude);
            animator.SetBool("Grounded", m_isGrounded);
        }

        private void FixedUpdate()
        {
            DirectFixedUpdate();
            m_wasGrounded = m_isGrounded;
            m_jumpInput = false;
        }

        private void DirectUpdate()
        {
            Transform camera = Camera.main.transform;
            inputDirection.x = Input.GetAxisRaw("Horizontal");
            inputDirection.y = Input.GetAxisRaw("Vertical");
            movingDirecion = inputDirection.x * camera.right + inputDirection.y * camera.forward;
            movingDirecion.y = 0;
            movingDirecion.Normalize();
            if (IsMoving)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movingDirecion), Time.deltaTime * turnLerpPower);
            }
        }

        private void DirectFixedUpdate()
        {
            var movingVelocity = movingDirecion * m_moveSpeed;
            var move = rb.velocity;
            move.x = Mathf.Lerp(move.x, movingVelocity.x, Time.fixedDeltaTime * moveLerpPower);
            move.z = Mathf.Lerp(move.z, movingVelocity.z, Time.fixedDeltaTime * moveLerpPower);
            move.y = rb.velocity.y;
            rb.velocity = move;
            JumpingAndLanding();
        }

        private void JumpingAndLanding()
        {
            bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

            if (jumpCooldownOver && m_isGrounded && m_jumpInput)
            {
                m_jumpTimeStamp = Time.time;
                rb.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            }

            if (!m_wasGrounded && m_isGrounded)
            {
                animator.SetTrigger("Land");
            }

            if (!m_isGrounded && m_wasGrounded)
            {
                animator.SetTrigger("Jump");
            }
        }

        /// <summary>
        ///  自身から一番近い敵をターゲットにする
        /// </summary>
        public CharacterBase SearchLockOnTarget()
        {
            Collider[] colliders = new Collider[5];
            Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, LayerMask.GetMask("Enemy"));
            // Z軸が一番近い敵を取得
            var target = colliders.Where(x => x != null).OrderBy(x => transform.position.GetDiffDistance(x.transform.position)).FirstOrDefault();
            return target == null ? null : target.GetComponent<CharacterBase>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            ContactPoint[] contactPoints = collision.contacts;
            for (int i = 0; i < contactPoints.Length; i++)
            {
                if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
                {
                    if (!m_collisions.Contains(collision.collider))
                    {
                        m_collisions.Add(collision.collider);
                    }
                    m_isGrounded = true;
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            ContactPoint[] contactPoints = collision.contacts;
            bool validSurfaceNormal = false;
            for (int i = 0; i < contactPoints.Length; i++)
            {
                if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
                {
                    validSurfaceNormal = true; break;
                }
            }

            if (validSurfaceNormal)
            {
                m_isGrounded = true;
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
            }
            else
            {
                if (m_collisions.Contains(collision.collider))
                {
                    m_collisions.Remove(collision.collider);
                }
                if (m_collisions.Count == 0) { m_isGrounded = false; }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = sphere;
            Gizmos.DrawSphere(GetCenterTransfrom(), radius);
        }

#endif
    }
}