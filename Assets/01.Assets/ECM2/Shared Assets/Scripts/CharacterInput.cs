using UnityEngine;
using ECM2.Examples.Slide;

namespace ECM2.Examples
{
    public class CharacterInput : MonoBehaviour
    {
        // The controlled Character
        
        private PlayerCharacter _playerCharacter;

        private void Awake()
        {
            // Cache controlled character
            
            _playerCharacter = GetComponent<PlayerCharacter>();
        }

        private void Update()
        {
            // Movement input

            Vector2 inputKeyboardMove = new Vector2()
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };

            Vector3 movementDirection =  Vector3.zero;

            movementDirection += Vector3.right * inputKeyboardMove.x;
            movementDirection += Vector3.forward * inputKeyboardMove.y;

            // If character has a camera assigned...

            if (_playerCharacter.camera)
            {
                // Make movement direction relative to its camera view direction
                
                movementDirection = movementDirection.relativeTo(_playerCharacter.cameraTransform);
            }

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
        }
    }
}
