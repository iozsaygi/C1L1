using UnityEngine;

namespace C1L1.Core.Systems.Player
{
    [DisallowMultipleComponent]
    internal sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target = null;
        [SerializeField] private Vector2 focusAreaSize = Vector2.zero;
        [SerializeField] private float verticalOffset = 0.1f;

        public bool IsChaseEnabled = true;

        private FocusArea focusArea;
        private Collider2D targetCollider = null;

        private void Start()
        {
            targetCollider = target.GetComponent<Collider2D>();
            focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);
        }

        private void LateUpdate()
        {
            if (IsChaseEnabled)
            {
                focusArea.Update(targetCollider.bounds);
                Vector2 focusPosition = focusArea.Center + Vector2.up * verticalOffset;
                transform.position = (Vector3)focusPosition + Vector3.forward * -10.0f;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            Gizmos.DrawCube(focusArea.Center, focusAreaSize);
        }

        public void UpdateCameraOffset(float offset)
        {
            verticalOffset = offset;
            focusArea.Update(targetCollider.bounds);
        }

        public void UpdateHorizontalFocusAreaSize(float x)
        {
            focusAreaSize = new Vector2(x, focusAreaSize.y);
            focusArea.Update(targetCollider.bounds);
        }

        public void UpdateVerticalFocusAreaSize(float y)
        {
            focusAreaSize = new Vector2(focusAreaSize.x, y);
            focusArea.Update(targetCollider.bounds);
        }

        public void EnableChase()
        {
            IsChaseEnabled = true;
        }

        public void DisableChase()
        {
            IsChaseEnabled = false;
        }

        public void SetPosition(Transform other)
        {
            transform.position = other.position;
        }

        private struct FocusArea
        {
            public Vector2 Center;
            public Vector2 Velocity;
            private float left, right, top, bottom;

            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                left = targetBounds.center.x - size.x / 2;
                right = targetBounds.center.x + size.x / 2;
                bottom = targetBounds.min.y;
                top = targetBounds.min.y + size.y;

                Velocity = Vector2.zero;
                Center = new Vector2((left + right) / 2, (top + bottom) / 2);
            }

            public void Update(Bounds targetBounds)
            {
                float shiftX = 0;
                if (targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if (targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }

                left += shiftX;
                right += shiftX;

                float shiftY = 0;
                if (targetBounds.min.y < bottom)
                {
                    shiftY = targetBounds.min.y - bottom;
                }
                else if (targetBounds.max.y > top)
                {
                    shiftY = targetBounds.max.y - top;
                }

                top += shiftY;
                bottom += shiftY;

                Center = new Vector2((left + right) / 2, (top + bottom) / 2);
                Velocity = new Vector2(shiftX, shiftY);
            }
        }
    }
}