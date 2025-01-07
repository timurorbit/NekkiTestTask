using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace MageDefence
{

    public class PlayerController : MonoBehaviour
    {
        private PlayerStatsModel _playerStats;

        private IPlayerInput _playerInput;
        private ITargetLocator _targetLocator;
        private PlayerSpellCaster _playerSpellCaster;
        
        private CharacterController _characterController;

        [Inject]
        public void Construct(IPlayerInput playerInput,
            PlayerSpellCaster playerSpellCaster,
            [Inject(Id = "PlayerLocator")] ITargetLocator targetLocator
            , PlayerStatsModel playerStats)
        {
            _playerInput = playerInput;
            _playerSpellCaster = playerSpellCaster;
            _targetLocator = targetLocator;
            _playerStats = playerStats;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            Observable.EveryFixedUpdate()
                .Subscribe(_ => Move(_playerInput.MoveDirection.Value))
                .AddTo(this);

            Observable.EveryFixedUpdate()
                .Subscribe(_ => Rotate(_playerInput.MoveDirection.Value))
                .AddTo(this);

            Observable.EveryUpdate()
                .Subscribe(_ => CastSpell(_playerInput.SpellInput.Value))
                .AddTo(this);

            _playerInput.SpellChange
                .Subscribe(ChangeSpell)
                .AddTo(this);
        }

        private void Update()
        {
            _playerInput.HandleInput();
        }

        private void ChangeSpell(int direction)
        {
            _playerSpellCaster.ChangeSpell(direction);
        }

        private void Move(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                Vector3 movement = direction * (_playerStats.MoveSpeed.Value * Time.deltaTime);
                _characterController.SimpleMove(movement);
            }
        }

        private void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = (Quaternion.Slerp(transform.rotation, targetRotation,
                _playerStats.RotationSpeed.Value * Time.deltaTime));
        }

        private void CastSpell(bool spellInputValue)
        {
            if (!spellInputValue)
            {
                return;
            }

            _playerSpellCaster.CastSpell();
        }

        private void OnEnable()
        {
            _targetLocator.RegisterTarget(transform);
        }

        private void OnDisable()
        {
            _targetLocator.UnregisterTarget(transform);
        }
    }
}