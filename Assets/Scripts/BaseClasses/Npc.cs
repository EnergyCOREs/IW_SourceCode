using UnityEngine;
using UnityEngine.AI;

public class Npc : ResourceContainer
{

    [SerializeField] private int _damage = 15;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _attackDistance;
    [SerializeField] private Animator _animator;

    private Building _owner;
    private ResourceContainer _distanation;
    private float _timer;
    private bool _needTarget;
    private StateHandler _stateHandler;

    public delegate void StateHandler();
    public bool NeedTarget => _needTarget;

    public Building Owner { get => _owner; set => _owner = value; }
    public ResourceContainer Distanation { get => _distanation; set => _distanation = value; }

    private void Awake()
    {
        _stateHandler = WaitForTarget;
    }

    public void SetSpeed(float speed)
    {
        _agent.speed = speed*3;
    }

    internal void SetDistanation(ResourceContainer resourceContainer)
    {
        Distanation = resourceContainer;
        _stateHandler = GoToTarget;
    }

    public virtual void Update()
    {
        if (_timer >= 0)
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _stateHandler();
                _timer = Random.Range(3f / _agent.speed, 5f / _agent.speed);
            }
        }

        _animator.SetFloat("Blend", Mathf.Clamp01(_agent.velocity.magnitude));
        TradeLot.GiveAll(this, MapGlobals.Instance.Player);
    }

    public void GoToTarget()
    {
        if (MapGlobals.IsNullOrDestroyed(Distanation))
        {
            WaitForTarget();
            return;
        }

        _needTarget = false;
        _agent.SetDestination(Distanation.GetWorldPosition());

        var distance = Distanation.GetWorldPosition() - GetWorldPosition();
        distance.y = 0;

        if (distance.magnitude < _attackDistance)
        {
            _stateHandler = KillTarget;
        }
    }

    public void WaitForTarget()
    {
        if (MapGlobals.IsNullOrDestroyed(Distanation))
        {
            _needTarget = true;
            _agent.SetDestination(Owner.GetWorldPosition() + (Vector3)Random.insideUnitCircle * 15f);
            return;
        }

        _needTarget = false;
        _stateHandler = GoToTarget;
    }

    public void FindTarget<T>() where T : IPlayerTarget
    {
        Distanation = MapGlobals.Instance.GetNearest<T>(MyTransform.position, 300) as ResourceContainer;

        _stateHandler = GoToTarget;
    }

    public void KillTarget()
    {
        if (MapGlobals.IsNullOrDestroyed(Distanation))
            _stateHandler = WaitForTarget;

        _needTarget = false;
        //_agent.SetDestination(GetWorldPosition());
        if (Distanation != null)
            if ((Distanation as Breakable) != null)
                (Distanation as Breakable).TakeDamage(this, _damage);
        AttackAction();
    }

    public void AttackAction()
    {
        _animator.SetTrigger("Attack");
    }
}
