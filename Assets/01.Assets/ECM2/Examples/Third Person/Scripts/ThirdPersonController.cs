using UnityEngine;
using ECM2.Examples.Slide;
using Photon.Pun;
using Photon.Realtime;

namespace ECM2.Examples.ThirdPerson
{
    /// <summary>
    /// This example shows how to implement a basic third person controller.
    /// This must be added to a Character.
    /// </summary>
    
    public class ThirdPersonController : MonoBehaviour
    {
        [Space(15.0f)]
        //public GameObject followTarget;

        [Tooltip("The default distance behind the Follow target.")]
        [SerializeField]
        public float followDistance = 5.0f;

        [Tooltip("The minimum distance to Follow target.")]
        [SerializeField]
        public float followMinDistance;

        [Tooltip("The maximum distance to Follow target.")]
        [SerializeField]
        public float followMaxDistance = 10.0f;

        [Space(15.0f)]
        public bool invertLook = true;

        [Tooltip("Mouse look sensitivity")]
        public Vector2 mouseSensitivity = new Vector2(1.0f, 1.0f);

        [Space(15.0f)]
        [Tooltip("How far in degrees can you move the camera down.")]
        public float minPitch = -80.0f;

        [Tooltip("How far in degrees can you move the camera up.")]
        public float maxPitch = 80.0f;

        protected float _cameraYaw;
        protected float _cameraPitch;

        protected float _currentFollowDistance;
        protected float _followDistanceSmoothVelocity;

        private PlayerCharacter _playerCharacter;
        private Rigidbody _rigidbody;
        private VariableJoystick _joystick;
  
        public float _dashSpeed;
        public float _dashTime;
        public PhotonView _photonView;

        /// <summary>
        /// Add input (affecting Yaw).
        /// This is applied to the camera's rotation.
        /// </summary>

        public virtual void AddControlYawInput(float value)
        {
            _cameraYaw = MathLib.ClampAngle(_cameraYaw + value, -180.0f, 180.0f);
        }
        
        /// <summary>
        /// Add input (affecting Pitch).
        /// This is applied to the camera's rotation.
        /// </summary>

        public virtual void AddControlPitchInput(float value, float minValue = -80.0f, float maxValue = 80.0f)
        {
            _cameraPitch = MathLib.ClampAngle(_cameraPitch + value, minValue, maxValue);
        }
        
        /// <summary>
        /// Adds input (affecting follow distance).
        /// </summary>

        public virtual void AddControlZoomInput(float value)
        {
            followDistance = Mathf.Clamp(followDistance - value, followMinDistance, followMaxDistance);
        }
        
        /// <summary>
        /// Update camera's rotation applying current _cameraPitch and _cameraYaw values.
        /// </summary>

        protected virtual void UpdateCameraRotation()
        {
            Transform cameraTransform = _playerCharacter.cameraTransform;
            cameraTransform.rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0.0f);
        }
        
        /// <summary>
        /// Update camera's position maintaining _currentFollowDistance from target. 
        /// </summary>

        protected virtual void UpdateCameraPosition()
        {
            Transform cameraTransform = _playerCharacter.cameraTransform;
            
            _currentFollowDistance =
                Mathf.SmoothDamp(_currentFollowDistance, followDistance, ref _followDistanceSmoothVelocity, 0.1f);

            //cameraTransform.position =
            //    followTarget.transform.position - cameraTransform.forward * _currentFollowDistance;
        }
        
        /// <summary>
        /// Update camera's position and rotation.
        /// </summary>

        protected virtual void UpdateCamera()
        {
            UpdateCameraRotation();
            UpdateCameraPosition();
        }

        protected virtual void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _rigidbody = GetComponent<Rigidbody>();
            _joystick = GameObject.FindWithTag("Joystick").GetComponent<VariableJoystick>();
            _photonView = GetComponent<PhotonView>();
        }

        protected virtual void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;

            //Vector3 euler = _character.cameraTransform.eulerAngles;

            //_cameraPitch = euler.x;
            //_cameraYaw = euler.y;

            _currentFollowDistance = followDistance;
            //_isDashMode = false;
            //_originPlayerSpeed = _playerCharacter.maxWalkSpeed;
        }

        protected virtual void Update()
        {
            // Movement input

            Vector2 inputKeyboardMove = new Vector2()
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };

            Vector2 inputJoystickMove = new Vector2()
            {               
                x = _joystick.Horizontal,
                y = _joystick.Vertical           
            };

            Vector3 movementDirection = Vector3.zero;

            if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
            {
                movementDirection += Vector3.right * inputKeyboardMove.x;
                movementDirection += Vector3.forward * inputKeyboardMove.y;
                movementDirection += Vector3.right * inputJoystickMove.x;
                movementDirection += Vector3.forward * inputJoystickMove.y;
            }
            else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
            {
                if (_photonView.IsMine)
                {
                    movementDirection += Vector3.right * inputKeyboardMove.x;
                    movementDirection += Vector3.forward * inputKeyboardMove.y;
                    movementDirection += Vector3.right * inputJoystickMove.x;
                    movementDirection += Vector3.forward * inputJoystickMove.y;
                }
            }

            if (_playerCharacter.cameraTransform)
                movementDirection = movementDirection.relativeTo(_playerCharacter.cameraTransform, _playerCharacter.GetUpVector());

            _playerCharacter.SetMovementDirection(movementDirection);

            // Crouch input

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
                _playerCharacter.Crouch();
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C))
                _playerCharacter.UnCrouch();

            // Jump input

            if (Input.GetButtonDown("Jump"))
                _playerCharacter.Jump();
            else if (Input.GetButtonUp("Jump"))
                _playerCharacter.StopJumping();
            
            // Look input

            Vector2 lookInput = new Vector2
            {
                x = Input.GetAxisRaw("Mouse X"),
                y = Input.GetAxisRaw("Mouse Y")
            };

            lookInput *= mouseSensitivity;

            AddControlYawInput(lookInput.x);
            AddControlPitchInput(invertLook ? -lookInput.y : lookInput.y, minPitch, maxPitch);
            
            // Zoom input

            //float mouseScrollInput = Input.GetAxisRaw("Mouse ScrollWheel");
            //AddControlZoomInput(mouseScrollInput);
        }

        protected virtual void LateUpdate()
        {
            UpdateCamera();
        }

        //public void Dash()
        //{
        //    _playerDir = transform.TransformDirection(Vector3.forward);
        //    _playerCharacter.maxWalkSpeed = _dashSpeed;
        //    _isDashMode = true;
        //    _time = 0f;
        //}
    }
}
