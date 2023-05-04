using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class SimpleCharacterController : ResourceContainer, ISavable
{
    [SerializeField] private Animator _animator;

    [SerializeField] private float _walkingSpeed = 3f;
    [SerializeField] private float _runningSpeed = 8f;
    [SerializeField] private float _gravityForce = 2000f;
    [SerializeField] private float _attackCooldown = 0.25f;
    [SerializeField] private bool _rotateToVelocity = true;
    [SerializeField] private DynamicJoystick _touchscreenInput;

    [SerializeField] private float _stepDistance = 1.5f;

    [SerializeField] private GameObject _pickaxe, _axe;

    private CharacterController _characterController;
    private Vector3 _moveDirection;

    private float _characterTempSpeed;
    private float _inputHorizontal, _inputVertical;
    private Vector3 _lastPosition;
    private float _cooldown;
    private Breakable _finded;
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _moveDirection = Vector3.zero;
    }

    public void FindTarget()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
            return;
        }

        _finded = (Breakable)MapGlobals.Instance.GetNearest(MyTransform.position, 3);

        if (_finded != null)
            if (_finded.gameObject != null)
            {
                _cooldown = _attackCooldown;
                _animator.SetTrigger("Attack");
                _finded.TakeDamage(this, 15);
                CameraShaker.Instance.ShakeOnce(0.5f, 2, 0.1f, 0.75f);

                var playerTarget = (_finded as IPlayerTarget);
                if (playerTarget != null)
                {
                    _pickaxe.SetActive(playerTarget.UsedWeapon == IPlayerTarget.TargetType.PickAxe);
                    _axe.SetActive(playerTarget.UsedWeapon == IPlayerTarget.TargetType.Axe);
                }
            }
    }

    void Update()
    {
        FindTarget();
        //Инпут с клавиатуры, TODO - перенести в отдельный модуль для поддержки тача

        //Выбираем скорость, спринт или шаг
        _characterTempSpeed = (Input.GetKey(KeyCode.LeftShift) ? _walkingSpeed : _runningSpeed);
        if (MyTransform.position.y < -2f)
            _characterTempSpeed = _walkingSpeed / 3f;

        //Направление движения
        _inputVertical = (Input.GetAxis("Vertical") + _touchscreenInput.Vertical);
        _inputHorizontal = (Input.GetAxis("Horizontal") + _touchscreenInput.Horizontal);

        //Компутим направление по осям
        _moveDirection = Vector3.forward * _inputVertical + Vector3.right * _inputHorizontal;

        _animator.SetFloat("Blend", Mathf.Clamp01(_moveDirection.magnitude));

        if (_moveDirection.magnitude > 1)
            _moveDirection.Normalize();

        _moveDirection *= _characterTempSpeed;

        //Если игрок не на земле то двигаем его имитируя гравитацию
        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= _gravityForce * Time.deltaTime;
        }

        //Двигаем чарактер контроллер
        _characterController.Move(_moveDirection * Time.deltaTime);


        if (_rotateToVelocity)
        {
            //Уберем движение по Y для нормальной работы вращения игрока
            _moveDirection.y = 0;

            //Если вектор движения не пустой то повернем игрока по вектору движения;
            if (_moveDirection.magnitude > 0)
            {
                MyTransform.rotation = Quaternion.RotateTowards(MyTransform.rotation, Quaternion.LookRotation(-_moveDirection), 650 * Time.deltaTime);
            }
            else
            {
                if (_finded != null)
                {
                    MyTransform.rotation = Quaternion.RotateTowards(MyTransform.rotation,
                        Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, GetWorldPosition() - _finded.GetWorldPosition(), Vector3.up), 0), 5);
                }
            }
        }
    }


    void LateUpdate()
    {
        if (Vector3.Distance(transform.position, _lastPosition) > _stepDistance)
        {
            SoundMaker.Instance.PlaySound(SoundMaker.SoundType.Step, GetWorldPosition());
            _lastPosition = MyTransform.position;
        }
    }



    public bool IsDynamic => false;
    public int ID { get => _saverId; set => _saverId = value; }
    [SerializeField] private int _saverId;

    public string GetJson()
    {
        SaveData data = new SaveData();

        //==========================
        data.Position = MyTransform.position;
        data.Rotation = MyTransform.rotation;
        data.Skin = MapGlobals.Instance.SkinSelector.CurrentSkin;
        data.ResourceCounts = new List<int>();
        data.ResourceTypes = new List<ResourceType>();
        for (int i = 0; i < ResourceObjects.Count; i++)
        {
            data.ResourceTypes.Add(ResourceObjects[i].ResourceType);
            data.ResourceCounts.Add(ResourceObjects[i].Count);
        }
        //==========================

        return JsonUtility.ToJson(data);

    }



    public void SetJson(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);
         Debug.Log(json);
        //==========================
        _characterController.enabled = false;
        MyTransform.position = data.Position;
        MyTransform.rotation = data.Rotation;
        _characterController.enabled = true;

        foreach (var item in ResourceObjects)
        {
            item.Count = 0;
        }

        for (int i = 0; i < data.ResourceTypes.Count; i++)
        {
            var finded = ResourceObjects.Find(a => a.ResourceType == data.ResourceTypes[i]);

            if (finded != null)
                finded.Count = data.ResourceCounts[i];
            else
            {
                Debug.Log($"cannot find {data.ResourceTypes[i].Name}");
                TradeLot.AddResource(data.ResourceTypes[i], this, data.ResourceCounts[i]);
            }
        }
        MapGlobals.Instance.SkinSelector.SetSkin(data.Skin);

        InventoryShower inventory = FindObjectOfType<InventoryShower>();
        if (inventory != null)
            inventory.Regenerate();
        //==========================
    }

    public struct SaveData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public List<ResourceType> ResourceTypes;
        public List<int> ResourceCounts;
        public string Skin;
    }
}
