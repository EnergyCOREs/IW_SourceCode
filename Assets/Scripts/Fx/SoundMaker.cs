using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    public static SoundMaker Instance;

    [SerializeField] private float _falloffMaxDistance = 30f;
    [SerializeField] private AudioClip[] _stepSounds, _axeSounds, _pickaxeSound, _upgradeSounds, _popSounds;

    private Transform _camera;

    private void Awake()
    {
        Instance = this;
        _camera = Camera.main.transform;
    }

    public void PlaySound(SoundType soundType, Vector3 worldPosition)
    {
        int id = Random.Range(0, GetSoundArray(soundType).Length);
        PlaySound(soundType, worldPosition, id);
    }

    public void PlaySound(SoundType soundType, Vector3 worldPosition, int id)
    {
        AudioClip[] soundArray = GetSoundArray(soundType);
        if (soundArray[id] == null)
        {
            Debug.LogError("HACTPOU 3BYK");
        }
        AudioSource.PlayClipAtPoint(soundArray[id], worldPosition, MapGlobals.Instance.UserBusinessLogic.SaveObject.SoundLevel * SquareFalloff(worldPosition));
    }

    private float SquareFalloff(Vector3 start)
    {
        return Mathf.Clamp01(1 - ((_camera.position - start) / _falloffMaxDistance).sqrMagnitude);
    }

    private AudioClip[] GetSoundArray(SoundType soundType)
    {
        return soundType switch
        {
            SoundType.Step => _stepSounds,
            SoundType.Axe => _axeSounds,
            SoundType.Pickaxe => _pickaxeSound,
            SoundType.Upgrade => _upgradeSounds,
            SoundType.Pop => _popSounds,
            _ => null
        };
    }

    public enum SoundType
    {
        Step,
        Axe,
        Pickaxe,
        Upgrade,
        Pop
    }
}
