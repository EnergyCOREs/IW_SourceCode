using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMapResource : Breakable, IPlayerTarget
{
    private readonly Type _type = typeof(Breakable);

    [SerializeField] private string _breakableType = "Tree";
    [SerializeField] private SoundMaker.SoundType _soundType = SoundMaker.SoundType.Axe;
    [SerializeField] private IPlayerTarget.TargetType _weapon = IPlayerTarget.TargetType.PickAxe;
    [SerializeField] private bool _isDecorative = false;

    public IPlayerTarget.TargetType UsedWeapon => _weapon;
    public Type Typeof => _type;

    public string BreakableType => _breakableType;

    public bool IsDecorative => _isDecorative;

    public override Vector3 GetWorldPosition() => MyTransform.position;

    private void Start()
    {
        AddToTargetList();
    }

    public void AddToTargetList()
    {
        MapGlobals.Instance.AddToList(this);
    }

    public override void OnBreak()
    {
        return;
    }

    public override void TakeDamage(ResourceContainer damager, float damage)
    {
        base.TakeDamage(damager, damage);
        SoundMaker.Instance.PlaySound(_soundType, GetWorldPosition());
    }
}
