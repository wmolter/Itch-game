using UnityEngine;
using System.Collections;

namespace OneKnight.UI {
    [RequireComponent(typeof(RectTransform))]
    public class EdgeIndicator : MonoBehaviour {
        public Vector3 targetPos;
        public Vector3 offset;
        public float minScale;
        public float minScaleDistance=1;
        public RectTransform.Edge defaultEdge;
        public Transform toRotate;
        public Transform target;
        public bool changeParent = true;
        //public Camera reference;
        // Use this for initialization
        void Start() {
            if(changeParent) {
                //transform.SetParent(UIMaster.IndicatorParent, false);
            }
        }


        private void Update() {
            MoveIndicator();
            ResizeIndicator();
        }

        void ResizeIndicator() {
            Vector3 refPos = Camera.main.transform.position;
            transform.localScale = Vector3.one * ((minScaleDistance - Vector3.Distance(refPos, targetPos))/minScaleDistance * (1-minScale) + minScale);
        }
        // Update is called once per frame
        void RotateIndicator(RectTransform.Edge edge) {
            RectTransform rect = GetComponent<RectTransform>();
            switch(edge) {
                case RectTransform.Edge.Left:
                    rect.pivot = new Vector2(0, .5f);
                    toRotate.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case RectTransform.Edge.Right:
                    rect.pivot = new Vector2(1, .5f);
                    toRotate.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case RectTransform.Edge.Top:
                    rect.pivot = new Vector2(.5f, 1);
                    toRotate.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case RectTransform.Edge.Bottom:
                    rect.pivot = new Vector2(.5f, 0);
                    toRotate.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
        }

        private void MoveIndicator() {
            RectTransform rect = GetComponent<RectTransform>();
            if(target != null)
                targetPos = target.position + target.TransformVector(offset);
            Vector3 pos = targetPos;
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(pos);
            screenPoint = screenPoint * 2 + new Vector3(-1, -1, 0);
            screenPoint /= Mathf.Max(Mathf.Abs(screenPoint.x), Mathf.Abs(screenPoint.y), 1);
            if(screenPoint.z < 0) {
                screenPoint /= -Mathf.Max(Mathf.Abs(screenPoint.x), Mathf.Abs(screenPoint.y));
            }
            rect.anchorMax = (screenPoint + new Vector3(1, 1, 0)) * .5f;
            if(!(Mathf.Abs(screenPoint.x) < 1 && Mathf.Abs(screenPoint.y) < 1))
                RotateIndicator(CalculateEdge(screenPoint));
            else
                RotateIndicator(defaultEdge);
            rect.anchorMin = rect.anchorMax;
            rect.anchoredPosition = Vector2.zero;
        }

        public RectTransform.Edge CalculateEdge(Vector3 screenPoint) {
            if(Mathf.Abs(screenPoint.y) > Mathf.Abs(screenPoint.x)) {
                if(screenPoint.y > 0)
                    return RectTransform.Edge.Top;
                else
                    return RectTransform.Edge.Bottom;
            } else {
                if(screenPoint.x > 0)
                    return RectTransform.Edge.Right;
                else
                    return RectTransform.Edge.Left;
            }
        }
    }
}