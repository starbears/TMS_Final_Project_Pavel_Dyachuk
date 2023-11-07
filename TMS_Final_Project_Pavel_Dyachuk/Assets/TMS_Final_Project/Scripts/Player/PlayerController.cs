using Platformer.Player;
using System;
using Platformer.Common;
using Platformer.Pickables;
using UnityEngine;
using System.Collections;

namespace Platformer.Player
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private HealthBarManager _healthBarManager;

        [SerializeField]
        private float _currentHealth, _maxHealth;
        [SerializeField]
        float invulnerabilityTime;

        private bool _invulnerableAfterHit;
        private bool _invulnerableByCherry;

        [SerializeField]
        private PlayerView _playerView;

        [SerializeField]
        private PlayerPhysics _playerPhysics;

        [SerializeField]
        private SpriteRenderer _invulnerabilityView;

        private bool _isRunningLeft;
        private bool _isRunningRight;
        private bool _isJumping;

        public static event Action OnCurrentHealthChanged;
        public event Action OnDied;
        public event Action<IPickable> OnPickableCollected;

        private void Awake()
        {
             _playerPhysics.OnCollided += OnCollided;
        }

        private void Start()
        {
            _healthBarManager.CurrentHealth = _currentHealth;
            _healthBarManager.MaxHealth = _maxHealth;
            _healthBarManager.DrawHearts();
            _playerPhysics.OnCollided += OnCollided;
            _invulnerabilityView.enabled = false;


        }

        private void OnDestroy()
        {
            _playerPhysics.OnCollided -= OnCollided;
        }


        private void Update()
        {

            if (Input.GetKey(KeyCode.A))
            {
                _isRunningLeft = true;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _isRunningRight = true;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                _isJumping = true;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _isJumping = true;
            }

            _playerView.Tick(_playerPhysics.Velocity, _playerPhysics.IsOnGround);
        }

        private void FixedUpdate()
        {
            _playerPhysics.Tick(_isRunningLeft, _isRunningRight, _isJumping);

            // Reset input for movement
            _isRunningLeft = false;
            _isRunningRight = false;
            _isJumping = false;
        }


        private void OnCollided(GameObject collidedObject)
        {
            if (collidedObject.TryGetComponent<IDamageSource>(out var damageSource) && !_invulnerableAfterHit && !_invulnerableByCherry)
            {
                _healthBarManager.CurrentHealth -= damageSource.Damage;
                Debug.Log(_invulnerableAfterHit);

                if (_healthBarManager.CurrentHealth < 1)
                {
                    // Death
                    _playerPhysics.Disable();
                    _playerView.PlayDieAnimation(OnDieAnimationFinished);
                    SoundManager.Sound_Manager.PlayPlayerDeathSound();
                }
                else
                {
                    // Hit
                    _playerPhysics.Disable();
                    _playerView.PlayHitAnimation(OnHitAnimationFinished);
                    SoundManager.Sound_Manager.PlayPlayerHitByEnemySound();
                    _invulnerableAfterHit = true;
                    StartCoroutine(DisableInvulnerabilityAfterHitCoroutine());
                }

                OnCurrentHealthChanged?.Invoke();
            }
        }

        private void OnHitAnimationFinished()
        {
        }

        private void OnDieAnimationFinished()
        {
            OnDied?.Invoke();
        }

        public void Pick(IPickable pickable)
        {
            OnPickableCollected?.Invoke(pickable);
        }

        private IEnumerator DisableInvulnerabilityAfterHitCoroutine()
        {
            yield return new WaitForSeconds(1f);
            _invulnerableAfterHit = false;
        }
    }
}