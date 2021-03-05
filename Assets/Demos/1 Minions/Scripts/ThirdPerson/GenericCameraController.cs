using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    public abstract class GenericCameraController : MonoBehaviour
    {
        [Header("Rotation")]
        public float rotationSensitivity = 1.0f;

        [Header("Angle")]
        public float Angle = 44f;
        public float AngleSmooth = 0.3f;

        [Header("Zoom")]
        public float defaultZoom = 14.0f;
        public float minZoom = 6.0f;
        public float maxZoom = 40.0f;
        public float zoomSpeed = 12.0f;

        [Header("Field of View")]
        public float fieldOfView = 110;
        public float minFOV = 80;
        public float maxFOV = 110;

        //Properties
        public Camera Camera
        {
            get { return controlledCamera; }
        }
        public float FieldOfView
        {
            set
            {
                if (controlledCamera == null)
                {
                    controlledCamera = GetComponentInChildren<Camera>();
                }
                controlledCamera.fieldOfView = HorizontalToVerticalFOV(Mathf.Clamp(fieldOfView, minFOV, maxFOV), controlledCamera.aspect);
            }
        }

        public Vector3 Forward
        {
            get
            {
                return (Vector3.Cross(controlledCamera.transform.right, Vector3.up)).normalized;
            }
        }
        public Vector3 Right
        {
            get { return (Vector3.Cross(-Forward, Vector3.up)).normalized; }
        }

        public Quaternion Rotation
        {
            get { return Quaternion.LookRotation(transform.position - controlledCamera.transform.position); }
        }
        public Quaternion InverseRotation
        {
            get { return Quaternion.LookRotation(controlledCamera.transform.position - transform.position); }
        }
        public Quaternion FlattenedRotation
        {
            get
            {
                Vector3 direction = transform.position - controlledCamera.transform.position;
                direction.y = 0;
                return Quaternion.LookRotation(direction.normalized);
            }
        }

        //Backing fields
        protected Camera controlledCamera;

        protected float cameraAngle;
        protected float angleVelocity;

        protected float zoom;
        protected Vector3 cameraOffset;

        protected float initialMouseX;
        protected float currentMousex;
        protected float initialRotOffset;
        protected float currentRotOffset;

        protected bool rotationInput;

        //Generic methods
        protected void Awake()
        {
            controlledCamera = GetComponentInChildren<Camera>();
        }
        protected void Start()
        {
            //Set default zoom
            zoom = defaultZoom;

            //Set default angle
            cameraAngle = Angle;
        }

        //Rotation & zoom
        protected void RotationZoomInput()
        {
            //Zoom Input
            zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

            //RotationInput
            if (Input.GetMouseButtonDown(2))
            {
                initialMouseX = Input.mousePosition.x;
                initialRotOffset = currentRotOffset;
            }
            if (Input.GetMouseButton(2))
            {
                rotationInput = true;
            }
            else
            {
                rotationInput = false;
            }
        }
        protected void ApplyRotationZoom()
        {
            //Rotation
            if (rotationInput)
            {
                currentMousex = Input.mousePosition.x;
                currentRotOffset = initialRotOffset - ((currentMousex - initialMouseX) * rotationSensitivity);
            }

            //Camera Angle
            cameraAngle = Mathf.SmoothDampAngle(cameraAngle, Angle, ref angleVelocity, AngleSmooth);

            //Generate a CameraOffset from our CurrentZoom and Angle
            float FielfOfViewAdjust = Mathf.Pow(fieldOfView / maxFOV, -1);

            cameraOffset = Vector3.zero;
            cameraOffset.y = zoom * FielfOfViewAdjust * (1.41f * Mathf.Sin(Mathf.Deg2Rad * cameraAngle));
            cameraOffset.z = -zoom * FielfOfViewAdjust * (1.41f * Mathf.Cos(Mathf.Deg2Rad * cameraAngle));

            //Camera Position
            controlledCamera.transform.position = RotateAroundPoint(transform.position + cameraOffset, transform.position, Quaternion.Euler(new Vector3(0, currentRotOffset, 0)));

            //Camera Rotation
            controlledCamera.transform.rotation = Quaternion.LookRotation(transform.position - controlledCamera.transform.position);
        }

        //Utility
        protected static float HorizontalToVerticalFOV(float horizontalFOV, float aspect)
        {
            return Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan((horizontalFOV * Mathf.Deg2Rad) / 2f) / aspect);
        }
        protected Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion Angle)
        {
            return Angle * (point - pivot) + pivot;
        }
    }
}