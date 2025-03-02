using UnityEngine;

namespace Player
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private GameObject _camera;

        public void IsLocalPlayer()
        {
            _playerMovement.enabled = true;
            _camera.SetActive(true);
        }
    }
}