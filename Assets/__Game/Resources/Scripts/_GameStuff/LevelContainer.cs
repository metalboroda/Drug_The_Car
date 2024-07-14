using System.Collections.Generic;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class LevelContainer : MonoBehaviour
  {
    [Header("Car Spawning")]
    [SerializeField] private List<GameObject> _carsToSpawn = new List<GameObject>();
    [Header("")]
    [Range(0, 5)]
    [SerializeField] private int _spawnCarsAmount;
    [Space]
    [SerializeField] private Transform _carsContainer;

    private void Awake() {
      SpawnRandomCars();
    }

    private void SpawnRandomCars() {
      if (_spawnCarsAmount == 0) return;

      for (int i = 0; i < _spawnCarsAmount; i++) {
        Instantiate(_carsToSpawn[Random.Range(0, _carsToSpawn.Count)], _carsContainer);
      }
    }
  }
}