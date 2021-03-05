using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.Misc
{
    [ExecuteInEditMode]
    public class FirstPersonCharacterController : MonoBehaviour
    {
        [Header("Look")]
        public float lookSensitivity = 3f;

        [Header("Move")]
        public float moveAcceleration = 0.2f;
        public float moveSpeed = 8;

        [Header("Jump")]
        public float jumpAcceleration = 1;

        [Header("Camera")]
        public Camera cameraControlled;
        public float cameraSmooth = 0.2f;
        public Vector3 cameraOffset = new Vector3(0, 0.6f, 0);

        [Header("Weapon")]
        public WeaponController weapon;

        //Properties
        public bool Grounded
        {
            get { return grounded; }
        }

        //Backing fields
        private Rigidbody attachedRigidbody;
        private CapsuleCollider capsuleCollider;

        private Vector3 cameraRotation;
        private Vector2 lookDelta;

        private float recoil;
        private float recoilDuration;
        private float recoilVelocity;

        private Vector3 moveDelta;

        private bool grounded;
        private int collisions;

        private bool jumpInput;
        private Vector3 cameraVelocity;

        //Generic methods
        private void Awake()
        {
            //Grab our components
            attachedRigidbody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();

            if (Application.isPlaying)
            {
                //Lock Cursor
                Cursor.lockState = CursorLockMode.Confined;

                //Hide Cursor
                Cursor.visible = false;
            }
        }
        private void OnEnable()
        {
            if (cameraControlled == null) cameraControlled = Camera.main;
            cameraRotation = cameraControlled.transform.rotation.eulerAngles;
        }
        private void Update()
        {
            //Look Input
            lookDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            //Move Input
            moveDelta = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            //Jump Input
            if (Input.GetKey(KeyCode.Space)) jumpInput = true;
            else jumpInput = false;
        }

        //Physics methods
        private void FixedUpdate()
        {
            //Update character rotation
            Vector3 characterRotation = transform.rotation.eulerAngles;
            characterRotation.y += lookDelta.x * lookSensitivity;
            transform.rotation = Quaternion.Euler(characterRotation);

            //Get velocity
            Vector3 velocity = attachedRigidbody.velocity;

            //Update horizontal velocity
            Vector3 goalAcceleration = transform.rotation * moveDelta.normalized * moveAcceleration;
            velocity.x += goalAcceleration.x;
            velocity.z += goalAcceleration.z;

            //Clamp to max speed
            Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);
            if (horizontalVelocity.magnitude > moveSpeed)
            {
                velocity.x *= moveSpeed / horizontalVelocity.magnitude;
                velocity.z *= moveSpeed / horizontalVelocity.magnitude;
            }

            //Grounded
            grounded = CheckGrounded();

            //Jumping
            if (jumpInput && grounded)
            {
                velocity.y += jumpAcceleration;
            }

            //Set velocity
            attachedRigidbody.velocity = velocity;

            //Update camera rotation
            cameraRotation.x -= lookDelta.y * lookSensitivity;
            cameraRotation.y += lookDelta.x * lookSensitivity;
            cameraRotation.z = 0;

            //Clamp looking too high/low
            if (cameraRotation.x < 200) cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);

            //Update recoil
            recoil = Mathf.SmoothDamp(recoil, 0, ref recoilVelocity, recoilDuration);
            Vector3 RecoiledRotation = cameraRotation;
            RecoiledRotation.x -= recoil;

            cameraControlled.transform.rotation = Quaternion.Euler(RecoiledRotation);

            //Update camera position
            cameraControlled.transform.position = Vector3.SmoothDamp(cameraControlled.transform.position, transform.TransformPoint(cameraOffset), ref cameraVelocity, cameraSmooth);

            //Update weapon - Called here instead of within its own FixedUpdate because we need to guarentee it's not updated until after the camera position has been
            if (weapon != null) weapon.UpdateWeapon();
        }
        private void OnCollisionEnter(Collision collision)
        {
            collisions++;
        }
        private void OnCollisionExit(Collision collision)
        {
            collisions--;
        }

        private bool CheckGrounded()
        {
            return (collisions > 0 && Physics.Raycast(transform.position, -Vector3.up, capsuleCollider.bounds.extents.y * 1.4f));
        }

        //Recoil
        public void ApplyRecoil(float RecoilStrength, float RecoilDuration)
        {
            recoilDuration = RecoilDuration;
            recoilVelocity += RecoilStrength;
        }
    }
}
