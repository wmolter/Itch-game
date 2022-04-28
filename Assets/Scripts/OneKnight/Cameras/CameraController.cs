using UnityEngine;
using System.Collections;

namespace OneKnight.Cameras {
    public class CameraController : MonoBehaviour {

        [System.Serializable]
        public enum Mode {
            Focus, Free
        }

        public Camera cam;
        public bool zoomAllowed = true;
        public bool zoomWithFov = true;
        public bool radiusMode;
        [Range(1, 2)]
        public float clipPlaneBuffer = 1.1f;
        public float zoomSpeed = 20;
        public float minView = .001f;
        public float maxView = 150;
        public float defaultZoomFactor = .3f;
        public float cameraSmoothTime = 1f;
        public bool dependOnDistance = true;
        [Range(.5f,1f)]
        public float quadInterpCenter = .6f;
        [Range(.1f, 10f)]
        public float logisticSteepness = 1f;

        public bool panAllowed = true;
        public bool panWithObject = true;
        public MoveToPan moveToPan;
        public float zoomPanSpeedFactor = .5f;
        public Vector2 speedExponentBounds;
        public float speedExponentChangeSpeed = 1;
        [SerializeField]
        float speedExponent = 0;
        float panSpeedFactor = 4f;
        public bool panSpeedDependOnZoom = true;
        public bool rotationAllowed = true;
        public float rotationSpeed = 1.5f;
        public bool rotateWithoutFocus = false;
        public bool autoRotate = false;
        public bool autoWhileUnfocused = false;
        public float autoRotateSpeed = .1f;
        public bool lockVerticalAxis = false;
        public Vector3 verticalAxis;
        public bool boundVerticalAngle = false;
        public Vector2 angleBounds;

        public float ZoomFactor {
            get {
                if(zoomWithFov)
                    return cam.fieldOfView;
                else
                    return moveZoomFactor;
            }
        }

        public float PanSpeed {
            get {
                if(panSpeedDependOnZoom) {
                    return ZoomFactor * zoomPanSpeedFactor;
                } else {
                    return panSpeedFactor;
                }
            }
        }

        private float moveZoomFactor;
        public bool enableInputs = true;
        Mode mode;
        float startView;
        float defaultView;
        Vector3 defaultPosition;
        Quaternion defaultRotation;
        Vector3 rotatedDefaultPos;
        Selectable currentSelected;
        Selectable defaultSelected;
        public Transform followInPlane;
        Vector3 oldFollowPos;
        Coroutine movementRoutine;
        // Use this for initialization

        public delegate float Interpolator(float tSoFar, float maxT);

        private Interpolator PosInterp;
        private Interpolator ZoomInInterp;
        private Interpolator ZoomOutInterp;

        private void Awake() {
            moveZoomFactor = Mathf.Abs(cam.transform.localPosition.z);
            Debug.Log("Starting zoom: " + moveZoomFactor);
            if(cam.orthographic)
                startView = cam.orthographicSize;
            else
                startView = ZoomFactor;
            Quaternion defaultRotation = transform.localRotation;
            Vector3 defaultPosition = transform.localPosition;
            rotatedDefaultPos = defaultRotation*defaultPosition;

            PosInterp = delegate (float tSoFar, float maxT) {
                return Utils.LogisticInterp(tSoFar, maxT, logisticSteepness);
            };

            ZoomInInterp = delegate (float tSoFar, float maxT) {
                return Utils.QuadInterp(tSoFar, maxT, 1-quadInterpCenter);
            };

            ZoomOutInterp = delegate (float tSoFar, float maxT) {
                return Utils.QuadInterp(tSoFar, maxT, quadInterpCenter);
            };
        }

        void Start() {
            if(cam.orthographic)
                defaultView = cam.orthographicSize;
            else
                defaultView = cam.fieldOfView;
            defaultPosition = transform.position;
            defaultRotation = transform.rotation;
            if(followInPlane != null)
                FocusCameraImmediate(followInPlane);

        }

        // Update is called once per frame
        void Update() {
            if(autoRotate && (followInPlane != null || autoWhileUnfocused)) {
                RotateCamera(Time.deltaTime*autoRotateSpeed, 0);
            }
            if(!enableInputs)
                return;
            speedExponent += speedExponentChangeSpeed*Input.GetAxis("Camera Speed");
            speedExponent = Mathf.Clamp(speedExponent, speedExponentBounds.x, speedExponentBounds.y);
            panSpeedFactor = Mathf.Pow(10, speedExponent);
            switch(mode) {
                case Mode.Focus:
                    if(zoomAllowed) {
                        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") + Input.GetAxis("Key Zoom");
                        if(cam.orthographic)
                            ZoomOrtho(zoomAmount);
                        else
                            Zoom(zoomAmount);
                    }
                    break;
                case Mode.Free:
                    if(panAllowed) {
                        PanView(-panSpeedFactor*(transform.forward*Utils.PowInterp(Input.GetAxis("Vertical"), 1) + transform.right*Utils.PowInterp(Input.GetAxis("Horizontal"), 1)));
                    }
                    break;
            }

            if(panAllowed) {
                //MoveViewBy(-forwardSpeedFactor*transform.forward*Utils.PowInterp(Input.GetAxis("Vertical"), 1));
                //if(Input.GetKeyDown(RESET_CAMERA))
                //ResetCamera();
                if(Input.GetMouseButton(1)) {

                    CancelSelect();
                    MoveView(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                }
            }
            if(rotationAllowed) { 
                if(Input.GetMouseButton(2)) {
                    if(followInPlane != null || rotateWithoutFocus)
                        RotateCamera(rotationSpeed*Input.GetAxis("Mouse X"), rotationSpeed*Input.GetAxis("Mouse Y"));
                }
            }
        }

        void LateUpdate() {
            if(followInPlane != null) {
                Vector3 newPos = GetTargetPosition(followInPlane);
                MoveViewBy(newPos - GetCurrentPosition());
            }
            //new Vector3(followInPlane.position.x, transform.position.y, followInPlane.position.z);

        }

        public Vector3 GetCurrentPosition() {
            return panWithObject ? followInPlane.position : transform.position;
        }
    
        public Vector3 GetTargetPosition(Transform t) {
            return GetTargetPosition(t.position);
        }

        public Vector3 GetTargetPosition(Vector3 pos) {

            if(panWithObject)
                return Vector3.zero;
            else {
                Vector3 translate = transform.TransformVector(rotatedDefaultPos);
                return pos - translate;
            }

        }

        /*
        public Vector3 GetTargetPosition(float scale, Vector3 position, Quaternion rotation) {
            GameObject empty = new GameObject();
            empty.transform.localScale = scale*Vector3.one;
            empty.transform.rotation = rotation;
            empty.transform.position = position;
            Vector3 pos = GetTargetPosition(empty.transform);
            Destroy(empty);
            return pos;
        }*/

        public void Zoom(float zoomAmount) {
            if(zoomWithFov) {
                float nextView = cam.fieldOfView - cam.fieldOfView*zoomAmount*zoomSpeed;
                SetFOV(nextView);
            } else {
                float factor = moveZoomFactor - moveZoomFactor*zoomAmount*zoomSpeed;
                SetMoveZoom(factor);
            }
        }

        public void ZoomOrtho(float zoomAmount) {
            float nextSize = cam.orthographicSize - cam.orthographicSize*zoomAmount*zoomSpeed;
            SetOrthoSize(nextSize);
        }

        public void MoveViewBy(Vector3 positionChange) {
            if(panWithObject) {
                moveToPan.Move(positionChange);
            } else {
                transform.position += positionChange;
            }
        }

        public void MoveView(float x, float y) {
            if(panWithObject)
                PanWithObject(x, y);
            else
                DragCamera(x, y);
        }

        public void DragCamera(float x, float y) {
            transform.position += (transform.right*x + transform.up*y) * PanFactor;

        }

        public void PanWithObject(float x, float y) {
            moveToPan.Move(-(transform.right*x + transform.up*y) * PanFactor);
        }

        public void PanView(Vector3 change) {
            MoveViewBy(PanFactor*change);
        }

        private float PanFactor {
            get {
                return -PanSpeed*Mathf.Tan(Mathf.Deg2Rad*cam.fieldOfView/2);
            }
        }

        public void RotateCamera(float x, float y) {
            Vector3 rotateAround;
            if(currentSelected == null && panWithObject) {
                //inverted, since the objects need to rotate the other way to make it look like the camera rotated
                moveToPan.RotateAround(cam.transform.position, transform.up, -x);
                moveToPan.RotateAround(cam.transform.position, transform.right, y);

            } else {
                if(currentSelected == null) {
                    rotateAround = transform.parent == null ? transform.position : transform.parent.position;
                } else {
                    rotateAround = currentSelected.transform.position;
                }
                if(lockVerticalAxis)
                    transform.RotateAround(rotateAround, verticalAxis, x);
                else
                    transform.RotateAround(rotateAround, transform.up, x);
                transform.RotateAround(rotateAround, transform.right, -y);
                if(lockVerticalAxis && boundVerticalAngle) {
                    float angle = Vector3.Angle(transform.up, verticalAxis);
                    if(angle < angleBounds.x || angle > angleBounds.y) {
                        float angleDiff = 0;
                        if(angle < angleBounds.x) {
                            angleDiff = angleBounds.x - angle;
                        } else {
                            angleDiff = angleBounds.y - angle;
                        }

                        transform.RotateAround(rotateAround, transform.right, angleDiff);
                    }
                }
            }
        }

        private void FocusMode() {
            mode = Mode.Focus;
            panSpeedDependOnZoom = true;
        }

        private void FreeMode() {
            mode = Mode.Free;
            //make the pan speed the same as it was 
            panSpeedFactor = PanSpeed;
            speedExponent = Mathf.Log10(panSpeedFactor);
            panSpeedDependOnZoom = false;
        }

        public void FocusCameraImmediate(Transform target) {
            if(panWithObject) {
                moveToPan.Move(-target.position);
            } else {
                transform.position = GetTargetPosition(target);
            }
            float newZoom = CalculateZoom(target); 
            SetZoom(newZoom);
            FocusMode();
        }

        public void SelectImmediate(Selectable sel) {
            CancelSelect();
            FocusCameraImmediate(sel.transform);
            sel.Selected = true;
            currentSelected = sel;
            followInPlane = sel.transform;
            FocusMode();
        }

        public void Select(Selectable sel) {
            CancelSelect();
            currentSelected = sel;
            SmoothObjectToTarget(sel.transform);
        }

        void SetZoom(float factor) {
            if(zoomWithFov) {
                SetFOV(factor);
            } else {
                SetMoveZoom(factor);
            }
        }

        void SetMoveZoom(float factor) {
            float camZPos;
            if(radiusMode && followInPlane != null) {
                moveZoomFactor = Mathf.Clamp(factor, minView*followInPlane.lossyScale.x*.5f, maxView);
                moveZoomFactor = Mathf.Max(moveZoomFactor, cam.nearClipPlane*clipPlaneBuffer);
                camZPos = moveZoomFactor + followInPlane.lossyScale.x*.5f;
            } else {
                moveZoomFactor = Mathf.Clamp(factor, minView, maxView);
                camZPos = moveZoomFactor;
            }
            cam.transform.localPosition = camZPos*Vector3.back;
        }

        void SetFOV(float fov) {

            if(fov < minView)
                cam.fieldOfView = minView;
            else if(fov > maxView)
                cam.fieldOfView = maxView;
            else
                cam.fieldOfView = fov;
        }

        void SetOrthoSize(float size) {
            if(size < minView)
                cam.orthographicSize = minView;
            else if(size > maxView)
                cam.orthographicSize = maxView;
            else
                cam.orthographicSize = size;
        }

        public void CancelSelect() {

            if(currentSelected != null) {
                currentSelected.Selected = false;
                currentSelected = null;
            }
            followInPlane = null;
            FreeMode();
        }

        /*
        public void ResetCamera() {
            if(defaultSelected == null) {
                followInPlane = null;
                StartCoroutine(SmoothCameraToTarget(defaultView, defaultPosition, defaultRotation, cameraSmoothTime));
                if(panWithObject)
                    StartCoroutine(SmoothObjectToTarget(moveToPan.topLevel.transform));
            } else {
                transform.localRotation = defaultRotation;
                Select(defaultSelected);
            }
        }*/
        
            /*
        IEnumerator SmoothCameraToTarget(Transform target, float seconds) {
            Vector3 oldPos = panWithObject? target.position : transform.position;
            float newFOV = CalculateFOV(target);
            float oldFOV = cam.fieldOfView;

            Vector3 prevPos = oldPos;
            for(float t = 0; t < seconds && currentSelected != null; t += Time.deltaTime) {
                float f = LNInterp(t, seconds);

                Vector3 newPos = Vector3.Lerp(oldPos, GetTargetPosition(target), f);
                MoveViewBy(newPos - prevPos);
                prevPos = newPos;
                SetFOV(Mathf.Lerp(oldFOV, newFOV, f));
                yield return null;
            }

            if(currentSelected != null) {
                followInPlane = currentSelected.transform;
            }
        }
        */

        public void SetFOVFor(Transform obj) {
            cam.fieldOfView = CalculateZoom(obj);
        }

        private float CalculateZoom(Transform obj) {
            return Mathf.Abs(obj.lossyScale.x) * startView * defaultZoomFactor;
        }

        /*
        IEnumerator SmoothCameraToTarget(float fov, Vector3 position, Quaternion rotation, float seconds) {
            Vector3 oldPos = transform.position;
            float newFOV = fov;
            float oldFOV = cam.fieldOfView;
            Quaternion oldRot = transform.rotation;
            
            for(float t = 0; t < seconds; t += Time.deltaTime) {
                float f = LNInterp(t, seconds);
                Vector3 newPos = Vector3.Lerp(oldPos, position, f);
                transform.position = newPos;
                transform.rotation = Quaternion.Lerp(oldRot, rotation, f);
                SetFOV(Mathf.Lerp(oldFOV, newFOV, f));
                yield return null;
            }

        }*/
        public void SmoothObjectToTarget(Transform target){
            FocusMode();
            if (panWithObject)
                SmoothObjectToTarget(target, target);
            else
                SmoothObjectToTarget(target, transform);
        }

        void SmoothObjectToTarget(Transform target, Transform adjustAnchor) {
            if(movementRoutine != null)
                StopCoroutine(movementRoutine);
            float time = CalcSmoothTime((adjustAnchor.position - GetTargetPosition(target)).sqrMagnitude);
            movementRoutine = StartCoroutine(SmoothObjectToTarget(target, adjustAnchor, time));
        }

        float CalcSmoothTime(float distanceSqr){
            float time = cameraSmoothTime;
            if (dependOnDistance){
                //.5 for the squared magnitude
                float distanceFactor = .5f * Mathf.Log10(1 + distanceSqr);
                time = Mathf.Max(distanceFactor * time, time);
            }
            return time;
        }

        IEnumerator SmoothObjectToTarget(Transform target, Transform adjustAnchor, float seconds) {
            float newFOV = CalculateZoom(target);
            float oldFOV = ZoomFactor;

            Vector3 prevPos = adjustAnchor.position;
            for(float t = 0; t < seconds && currentSelected != null; t += Time.deltaTime) {
                float posf;
                float cameraf;
                if(newFOV > oldFOV) {
                    posf = PosInterp(t, seconds); //t/seconds;
                    cameraf = ZoomOutInterp(t, seconds);
                } else {
                    posf = PosInterp(t, seconds);// LNInterp(t, seconds);
                    cameraf = ZoomInInterp(t, seconds);
                }
                //prevPos = obj.position;
                Vector3 newPos = Vector3.LerpUnclamped(prevPos, GetTargetPosition(target), posf);
                MoveViewBy(newPos - adjustAnchor.position);
                SetZoom(Mathf.LerpUnclamped(oldFOV, newFOV, cameraf));
                yield return null;
            }

            if(currentSelected != null) {
                followInPlane = currentSelected.transform;
            }
        }

        public void SmoothToPosition(Vector3 pos){
            if (panWithObject)
                SmoothToPosition(pos, currentSelected.transform);
            else
                SmoothToPosition(pos, transform);
        }

        private void SmoothToPosition(Vector3 pos, Transform adjustAnchor) {
            if(movementRoutine != null)
                StopCoroutine(movementRoutine);
            float time = CalcSmoothTime((adjustAnchor.position - pos).sqrMagnitude);
            movementRoutine = StartCoroutine(SmoothToPosition(pos, adjustAnchor, time));
        }

        IEnumerator SmoothToPosition(Vector3 pos, Transform adjustAnchor, float seconds){
            Vector3 prevPos = adjustAnchor.position;
            for (float t = 0; t < seconds; t += Time.deltaTime){
                float posf = PosInterp(t, seconds);
                Vector3 newPos = Vector3.LerpUnclamped(prevPos, pos, posf);
                MoveViewBy(newPos - adjustAnchor.position);
                yield return null;
            }
            //end in correct pos
            MoveViewBy(pos - adjustAnchor.position);
        }

    }
}
