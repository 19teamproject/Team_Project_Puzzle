using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float sprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)]
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 10.0f;
        [SerializeField] private Material material;

        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [Range(0, 1)]
        [SerializeField] private float footstepAudioVolume = 0.5f;

        [Space(10)]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -15.0f;

        [Space(10)]
        [SerializeField] private float jumpTimeout = 0.50f;
        [SerializeField] private float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [SerializeField] private bool grounded = true;
        [SerializeField] private float groundedOffset = -0.14f;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private LayerMask groundLayers;

        [Header("Cinemachine")]
        [SerializeField] private List<CinemachineVirtualCamera> vCams;
        [SerializeField] private GameObject cinemachineCameraTarget;
        [SerializeField] private GameObject hat;
        [SerializeField] private float topClamp = 70.0f;
        [SerializeField] private float bottomClamp = -30.0f;
        [SerializeField] private float cameraAngleOverride = 0.0f;
        [SerializeField] private bool lockCameraPosition = false;

        // cinemachine
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;

        // player
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity;
        private readonly float terminalVelocity = 53.0f;

        // timeout deltatime
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;

        // animation IDs
        private int animIDSpeed;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDFreeFall;
        private int animIDMotionSpeed;

        private int camIndex = 0;

        private PlayerInput playerInput;
        private Interaction interaction;
        private Animator animator;
        private Tween delayedCall;
        private CharacterController controller;
        private StarterAssetsInputs input;
        private GameObject mainCamera;
        private CinemachineBrain mainCam;
        private UICondition condition;

        private const float Threshold = 0.01f;

        private bool externalJump = false;
        private bool hasAnimator;
        private bool IsCurrentDeviceMouse => playerInput.currentControlScheme == "KeyboardMouse";
        
        public PlayerInput PlayerInput => playerInput;

        // 포트 연결할 때 실행
        //public static Action OnPlayerEnterPortal;
        bool isCheating;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main.gameObject;
            }

            mainCam = mainCamera.GetComponent<CinemachineBrain>();
        }

        private void Start()
        {
            cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            hasAnimator = TryGetComponent(out animator);
            controller = GetComponent<CharacterController>();
            input = GetComponent<StarterAssetsInputs>();
            playerInput = GetComponent<PlayerInput>();
            interaction = GetComponent<Interaction>();
            condition = CharacterManager.Instance.Player.Condition.UICondition;

            AssignAnimationIDs();

            // reset our timeouts on start
            jumpTimeoutDelta = jumpTimeout;
            fallTimeoutDelta = fallTimeout;

            ChangeRenderMode(false);
        }

        private void Update()
        {
            CheatInput();

            hasAnimator = TryGetComponent(out animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CheatInput()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                isCheating = !isCheating;
                controller.enabled = !isCheating;
            }
        }

        private void AssignAnimationIDs()
        {
            animIDSpeed = Animator.StringToHash("Speed");
            animIDGrounded = Animator.StringToHash("Grounded");
            animIDJump = Animator.StringToHash("Jump");
            animIDFreeFall = Animator.StringToHash("FreeFall");
            animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (hasAnimator)
            {
                animator.SetBool(animIDGrounded, grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (input.look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
                cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = input.sprint ? sprintSpeed : moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            if (input.sprint)
            {
                if (condition.Stamina.CurValue > 10)
                {
                    if (input.move != Vector2.zero) condition.Stamina.Subtract(10f * Time.fixedDeltaTime);
                }
                else
                {
                    input.sprint = false;
                }
            }
            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);

                // round speed to 3 decimal places
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (animationBlend < 0.01f) animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (input.move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    rotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            if (!isCheating)
                // move the player
                controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                                 new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
            else
                transform.Translate(inputDirection * (10 * Time.deltaTime) +
                                 new Vector3(0.0f, Input.GetKey(KeyCode.Q) ? 10 : (Input.GetKey(KeyCode.E) ? 10 : 0), 0.0f) * Time.deltaTime);

            // update animator if using character
            if (hasAnimator)
            {
                animator.SetFloat(animIDSpeed, animationBlend);
                animator.SetFloat(animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (grounded)
            {
                // reset the fall timeout timer
                fallTimeoutDelta = fallTimeout;

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animIDJump, false);
                    animator.SetBool(animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                // Jump
                if (!externalJump && input.jump && jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                    // update animator if using character
                    if (hasAnimator)
                    {
                        animator.SetBool(animIDJump, true);
                    }
                }

                // jump timeout
                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }

                externalJump = false;
            }
            else
            {
                // reset the jump timeout timer
                jumpTimeoutDelta = jumpTimeout;

                // fall timeout
                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (hasAnimator)
                    {
                        animator.SetBool(animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
                groundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (footstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, footstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(controller.center), footstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(controller.center), footstepAudioVolume);
            }
        }

        /// <summary>
        /// 점프대 스크립트가 사용
        /// </summary>
        /// <param name="force"></param>
        public void AddJumpForce(Vector3 force)
        {
            externalJump = true;  // 외부 점프 활성화
            verticalVelocity = force.y;

            // 점프 애니메이션 강제 적용
            if (hasAnimator)
            {
                animator.SetBool(animIDJump, true);
            }
        }

        public void OnToggleViewInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed && vCams.Count > 0 && !mainCam.IsBlending)
            {
                vCams[camIndex].Priority = 0;

                camIndex = (camIndex + 1) % vCams.Count;

                vCams[camIndex].Priority = 10;

                Cinemachine3rdPersonFollow cinemachine3rdPersonFollow = vCams[camIndex].GetCinemachineComponent<Cinemachine3rdPersonFollow>();

                if (camIndex < 2)
                {
                    delayedCall.Kill();
                    hat.SetActive(true);
                    ChangeRenderMode(false);
                    interaction.CheckDistanceBonus = cinemachine3rdPersonFollow.CameraDistance;
                }
                else
                {
                    delayedCall = DOVirtual.DelayedCall(mainCam.m_DefaultBlend.BlendTime, () => {
                        hat.SetActive(false);
                        ChangeRenderMode(true);
                    });
                    interaction.CheckDistanceBonus = 0f;
                }
            }
        }

        private void ChangeRenderMode(bool isChanged)
        {
            if (material != null)
            {
                if (isChanged)
                {
                    Debug.Log("Transparent Mode");
                    material.SetFloat("_Mode", 2);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    SetAlpa(0f);
                }
                else if(!isChanged)
                {
                    Debug.Log("Opaque Mode");
                    material.SetFloat("_Mode", 0);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    SetAlpa(1f);
                }
            }
        }

        private void SetAlpa(float alpa)
        {
            Color color = material.color;
            color.a = alpa;
            material.color = color;
        }
    }
}