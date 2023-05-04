using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private float _timeToSpawn;

    private void Update()
    {

        if (_timeToSpawn > 0)
        {
            _timeToSpawn -= Time.deltaTime;
            if (_timeToSpawn <= 0)
            {
                int randomInt = Random.Range(0, 1000) % _prefabs.Length;
                Instantiate(_prefabs[randomInt], transform.position, _prefabs[randomInt].transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}
