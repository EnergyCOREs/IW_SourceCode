using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] private GameObject[] _bubbles;
    [SerializeField] private float _bubbleTime;
    [SerializeField] private float _waterLevel;
    [SerializeField] private Transform _homePosition;

    [SerializeField] private AudioSource _deathSound;

    private bool _underwater;
    private int _bubbleCount, _bubblesLeft;
    private SimpleCharacterController _player;
    private float _timer;

    private void Start()
    {
        _bubbleCount = _bubblesLeft = _bubbles.Length;
        _timer = _bubbleTime;
        _player = MapGlobals.Instance.Player;
        HideBubbles();
    }

    private void Update()
    {
        if (_player.GetWorldPosition().y < _waterLevel)
        {
            _underwater = true;
            UpdateVisual();

            if (_timer > 0)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0)
                {
                    _bubblesLeft--;
                    _timer = _bubbleTime;

                    if (_bubblesLeft <= 0)
                    {
                        TeleportPlayer();
                    }
                }
            }
        }
        else
        {
            if (_underwater)
            {
                _timer = _bubbleTime;
                _bubblesLeft = _bubbleCount;
                _underwater = false;
                HideBubbles();
            }
        }
    }

    private void TeleportPlayer()
    {
        var controller = _player.GetComponent<CharacterController>();
        if (controller == null)
            return;
        _deathSound.Play();
        controller.enabled = false;
        MapGlobals.Instance.Player.transform.position = _homePosition.position;
        MapGlobals.Instance.Player.transform.rotation = Quaternion.Euler(Vector3.zero);
        controller.enabled = true;

        HideBubbles();
    }

    private void HideBubbles()
    {
        foreach (var item in _bubbles)
        {
            if (item != null)
                item.SetActive(false);
        }
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < _bubbles.Length; i++)
        {
            if (_bubbles[i] != null)
                _bubbles[i].SetActive(!((i + 1) >= _bubblesLeft));
        }
    }
}
