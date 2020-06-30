using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace SpaceshipWarrior
{
    [ConverterVersion("Franco", 1)]
    public sealed class GeneratorSpawnSystemSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        [SerializeField] private GameObject _generatorGameObjectPrefab;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
        {
            destinationManager.AddComponentData(entity, new GeneratorsInitializationSystem.Settings { GeneratorPrefab = conversionSystem.GetPrimaryEntity(_generatorGameObjectPrefab) });
        }

        void IDeclareReferencedPrefabs.DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(_generatorGameObjectPrefab);
        }
    }
}
