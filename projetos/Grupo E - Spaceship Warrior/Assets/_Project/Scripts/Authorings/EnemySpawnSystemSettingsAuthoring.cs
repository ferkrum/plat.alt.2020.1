using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    [ConverterVersion("Franco", 1)]
    public sealed class EnemySpawnSystemSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        [SerializeField] private GameObject _enemyGameObjectPrefab;
        [SerializeField] private AnimationCurve _spawnTimerCurve;
        [SerializeField] private AnimationCurve _spawnCountCurve;
        [SerializeField] private uint _maxWaveCount;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddComponentObject(entity, new EnemySpawnSystem.Settings
            {
                MaxWaveCount = _maxWaveCount,
                EnemyPrefab = conversionSystem.GetPrimaryEntity(_enemyGameObjectPrefab),
                SpawnTimerCurve = _spawnTimerCurve,
                SpawnCountCurve = _spawnCountCurve
            });
        }

        void IDeclareReferencedPrefabs.DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(_enemyGameObjectPrefab);
        }
    }
}
