using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace MageDefence
{
   [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
   {
       [SerializeField] private float moveSpeed = 5f;
       [SerializeField] private float rotationSpeed = 700f;
       // todo add cooldown on spells
       // and spells SO;
   
       private PlayerInput _playerInput;
       private Rigidbody _rigidbody;
   
       [Inject]
       public void Construct(PlayerInput playerInput)
       {
           _playerInput = playerInput;
       }
   
       private void Awake()
       {
           _rigidbody = GetComponent<Rigidbody>();
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

       }
   
       private void Move(Vector3 direction)
       {
           if (direction != Vector3.zero)
           {
               Vector3 movement = direction * moveSpeed * Time.deltaTime;
               _rigidbody.MovePosition(_rigidbody.position + movement);  
           }
       }

       private void Rotate(Vector3 direction)
       {
           if (direction != Vector3.zero)
           {
               Quaternion targetRotation = Quaternion.LookRotation(direction);
               _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
           }
       }

       private void CastSpell(bool spellInputValue)
       {
           if (!spellInputValue)
           {
               return;
           }

           Debug.Log("CastSpell");
       }
   } 
}