using UnityEngine;

namespace Player
{
    public class MouseLook : MonoBehaviour
    {
        #region Settings

        [Header("Settings")]
        [SerializeField] private Vector2 clampInDegrees = new Vector2(360, 180);
        [SerializeField] private bool lockCursor = true;
        [SerializeField] private Vector2 sensitivity = new Vector2(2, 2);
        [SerializeField] private Vector2 smoothing = new Vector2(3, 3);

        [Header("First Person")]
        [SerializeField] private GameObject characterBody;

        #endregion

        #region Private Fields

        private Vector2 targetDirection;
        private Vector2 targetCharacterDirection;
        private Vector2 mouseDelta;
        private Vector2 smoothMouse;
        private Vector2 mouseAbsolute;

        private bool isScoped;

        #endregion

        #region Properties

        public static MouseLook Instance { get; private set; }
        public bool IsScoped { get => isScoped; set => isScoped = value; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            Instance = this;

            InitializeDirections();
            if (lockCursor)
            {
                LockCursor();
            }
        }

        private void Update()
        {
            HandleMouseLook();
        }

        #endregion

        #region Public Methods

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        #endregion

        #region Private Methods

        private void InitializeDirections()
        {
            targetDirection = transform.localRotation.eulerAngles;
            if (characterBody)
            {
                targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
            }
        }

        private void HandleMouseLook()
        {
            var targetOrientation = Quaternion.Euler(targetDirection);
            var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

            mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            mouseAbsolute += smoothMouse;

            if (clampInDegrees.x < 360)
            {
                mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
            }

            if (clampInDegrees.y < 360)
            {
                mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
            }

            transform.localRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

            if (characterBody)
            {
                var yRotation = Quaternion.AngleAxis(mouseAbsolute.x, Vector3.up);
                characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
            }
            else
            {
                var yRotation = Quaternion.AngleAxis(mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
                transform.localRotation *= yRotation;
            }
        }

        #endregion
    }
}