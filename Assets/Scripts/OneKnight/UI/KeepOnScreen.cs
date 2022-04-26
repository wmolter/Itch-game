using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace OneKnight.UI {
    [RequireComponent(typeof(RectTransform))]
    public class KeepOnScreen : MonoBehaviour {
        public RectOffset padding;

        private void Start() {
        }

        private void OnEnable() {
            FixPosition();
        }

        private void LateUpdate() {
            FixPosition();
        }

        public void FixPosition() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            Rect bounds = GetComponent<RectTransform>().rect;
            bounds.position = transform.position;
            float x = GetComponent<RectTransform>().pivot.x*bounds.width;
            float y = GetComponent<RectTransform>().pivot.y*bounds.height;
            bounds.position -= new Vector2(x, y);
            float ychange = Mathf.Min(0, Screen.height-padding.top - bounds.yMax);
            float xchange = Mathf.Min(0, Screen.width-padding.right - bounds.xMax);
            //Debug.Log("X Max: " + bounds.xMax + " X Min: " + bounds.xMin + " Width: " + bounds.width);
            ychange += Mathf.Max(0, padding.bottom-bounds.yMin);
            xchange += Mathf.Max(0, padding.left-bounds.xMin);
            transform.position += new Vector3(xchange, ychange, 0);
        }

        private void OnDrawGizmos() {
            Rect bounds = GetComponent<RectTransform>().rect;
            bounds.position = transform.position;
            float x = GetComponent<RectTransform>().pivot.x*bounds.width;
            float y = GetComponent<RectTransform>().pivot.y*bounds.height;
            bounds.position -= new Vector2(x, y);
            Vector3 topLeft = new Vector3(bounds.xMin, bounds.yMax);
            Vector3 topRight = new Vector3(bounds.xMax, bounds.yMax);
            Vector3 bottomLeft = new Vector3(bounds.xMin, bounds.yMin);
            Vector3 bottomRight = new Vector3(bounds.xMax, bounds.yMin);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}