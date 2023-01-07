﻿using _Project.Scripts.Extension;
using _Project.Scripts.Main.Wrappers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Main.Game.Weapon
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ShellBase : MonoPoolItemBase
    {
        [SerializeField] private ShellConfig _shellConfig;
        [SerializeField] private Destruction _destructionPrefab;
        [SerializeField] private float _lifeTime = 5f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var targetHealth = collision.gameObject.GetComponent<HealthBase>();
            if (targetHealth != null)
            {
                TakeDamage(targetHealth);
            }
            Destruct();
        }

        public void Shoot(Transform startPoint)
        {
           gameObject.SetActive(true);
           _transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
           _rigidbody.velocity = _transform.forward * _shellConfig.InitSpeed;
        }

        public void DestroyOnLifetimeEnd()
        {
            _ = DestroyOnLifetimeEndTask();
        }

        private async UniTask DestroyOnLifetimeEndTask()
        {
            await _lifeTime.WaitInSeconds();
            
            if (!_gameObject.activeSelf) return;
            
            Destruct();
        }

        private void Destruct()
        {
            var destruction = Services.Services.PoolService._destructionPool.Get();//Instantiate(_destructionPrefab,_transform.position, _transform.rotation );
            destruction._transform.position = _transform.position;
            destruction._transform.rotation = _transform.rotation;
            destruction._gameObject.SetActive(true);
            destruction._transform.SetParent(_transform.parent);
            var rigidbodies = destruction.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rigidbodies)
            {
                rb.velocity = _rigidbody.velocity / 5f;
            }
            ReturnToPool();
        }

        private void TakeDamage(HealthBase target)
        {
            target.GetDamage(_shellConfig.Damage);
        }
    }
}