using System;
using UnityEngine;

namespace Violee.Interact
{
    public class InteractCaster : MonoBehaviour
    {
        public LayerMask TarLayer;
        public float Radius = 8f;
        public float Angle = 45f;
        void Update()
        {
            DetectTargetsInSector();
        }

        void DetectTargetsInSector()
        {
            // 获取当前位置和前方方向
            var origin = transform.position;
            var forward = transform.forward;
            foreach (var col in Physics.OverlapSphere(origin, Radius, TarLayer))
            {
                var targetDir = col.transform.position - origin;
                var distance = targetDir.magnitude;

                var dot = Vector3.Dot(forward, targetDir.normalized);
                var targetAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (!(targetAngle <= Angle / 2f))
                    continue;
                if (Physics.Raycast(origin, targetDir, out var hit, distance, TarLayer)
                    && hit.collider.gameObject != col.gameObject)
                    continue;
                col.gameObject.GetComponent<InteractReceiver>()?.Interact();
                Debug.DrawLine(origin, col.transform.position, Color.green, 0.1f);
            }
        }
        
        void OnDrawGizmosSelected()
        {
            // 半径
            var origin = transform.position;
            var forward = transform.forward;
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
            Gizmos.DrawWireSphere(origin, Radius);
        
            // 边界线
            var leftBound = Quaternion.Euler(0, -Angle / 2, 0) * forward * Radius;
            var rightBound = Quaternion.Euler(0, Angle / 2, 0) * forward * Radius;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, origin + leftBound);
            Gizmos.DrawLine(origin, origin + rightBound);
        
            // 弧线
            var lastPoint = origin + leftBound;
            for (int i = 1; i <= 20; i++)
            {
                var segmentAngle = -Angle / 2 + Angle * i / 20f;
                var nextPoint = origin + Quaternion.Euler(0, segmentAngle, 0) * forward * Radius;
                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        
            // 连接弧线的直线
            Gizmos.DrawLine(origin + leftBound, origin + rightBound);
        }
    }
}