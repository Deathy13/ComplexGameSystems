using UnityEngine;
using System.Collections;
using System;

using Plugins.Curves;

namespace Checkers
{
    public class CameraFly : MonoBehaviour
    {
        public Transform lookTarget;
        public Vector3 offset = new Vector3(-10f, 0, 0f);
        public BezierSpline spline;
        public float duration = 1;
        public SplineWalkerMode mode;

        private float progress;
        private bool goingForward = true;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, .1f);
        }

        private void Update()
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }

            transform.localPosition = spline.GetPoint(progress);
        }
        private void LateUpdate()
        {
            if (lookTarget)
            {
                transform.LookAt(lookTarget);
                transform.rotation *= Quaternion.AngleAxis(offset.y, Vector3.up);
            }
        }
    }
}
