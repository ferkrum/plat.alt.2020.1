using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    public sealed class SfxManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _fireSource;
        [SerializeField] private AudioSource _projectileHitSource;
        [SerializeField] private AudioSource _enemyExplosionSource;
        [SerializeField] private AudioSource _generatorExplosionSource;

        private void Start()
        {
            var sfxEventsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SfxEventsSystem>();
            sfxEventsSystem.OnFire += HandleFire;
            sfxEventsSystem.OnProjectileHit += HandleProjectileHit;
            sfxEventsSystem.OnEnemyExplosion += HandleEnemyExplosion;
            sfxEventsSystem.OnGeneratorExplosion += HandleGeneratorExplosion;
        }

        private void HandleFire()
        {
            _fireSource.Play();
        }

        private void HandleProjectileHit()
        {
            _projectileHitSource.Play();
        }

        private void HandleEnemyExplosion()
        {
            _enemyExplosionSource.Play();
        }

        private void HandleGeneratorExplosion()
        {
            _generatorExplosionSource.Play();
        }
    }
}
