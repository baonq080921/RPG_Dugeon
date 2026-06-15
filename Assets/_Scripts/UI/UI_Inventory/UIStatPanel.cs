using Base;
using player;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the player's computed stat values grouped by category.
/// Refreshes automatically when the inventory changes (equip/unequip updates stats).
/// </summary>
public class UIStatPanel : MonoBehaviour
{
    private EntityStat _entityStat;

    [Header("Physical Damage")]
    [SerializeField] private TextMeshProUGUI _damageTmp;
    [SerializeField] private TextMeshProUGUI _critChanceTmp;
    [SerializeField] private TextMeshProUGUI _critPowerTmp;
    [SerializeField] private TextMeshProUGUI _attackMultiplierTmp;

    [Header("Defense")]
    [SerializeField] private TextMeshProUGUI _maxHealthTmp;
    [SerializeField] private TextMeshProUGUI _healthRegenTmp;
    [SerializeField] private TextMeshProUGUI _armorTmp;
    [SerializeField] private TextMeshProUGUI _evasionTmp;

    [Header("Elemental Damage")]
    [SerializeField] private TextMeshProUGUI _lightDamageTmp;

    [Header("Elemental Resistance")]
    [SerializeField] private TextMeshProUGUI _elementalResistanceTmp;

    private EventBinding<OnInventoryChangedEvent> _inventoryChangedBinding;

    private void OnEnable()
    {
        Player.ActivePlayerChanged += OnPlayerChanged;
        _inventoryChangedBinding = new EventBinding<OnInventoryChangedEvent>(Refresh);
        EventBus<OnInventoryChangedEvent>.Register(_inventoryChangedBinding);
    }

    private void OnDisable()
    {
        Player.ActivePlayerChanged -= OnPlayerChanged;
        EventBus<OnInventoryChangedEvent>.Deregister(_inventoryChangedBinding);
    }

    private void OnPlayerChanged(Player player)
    {
        _entityStat = player.GetComponent<EntityStat>();
        Refresh();
    }

private void Refresh()
    {
        if (_entityStat == null) return;
        SetText(_damageTmp,              _entityStat.GetDamageDisplayValue(),              "0");
        SetText(_critChanceTmp,          _entityStat.GetCritChanceDisplayValue(),          "0.0'%'");
        SetText(_critPowerTmp,           _entityStat.GetCritPowerDisplayValue(),           "0.0");
        SetText(_attackMultiplierTmp,    _entityStat.GetAttackMultiplier(),                "0.0");
        SetText(_maxHealthTmp,           _entityStat.GetHealthValue(),                     "0");
        SetText(_healthRegenTmp,         _entityStat.GetHealthRegen(),                     "0.0");
        SetText(_armorTmp,               _entityStat.GetArmorDisplayValue(),               "0");
        SetText(_evasionTmp,             _entityStat.GetEnvasionValue(),                   "0.0'%'");

        SetText(_lightDamageTmp,         _entityStat.GetElementalDamage(),         "0");

        SetText(_elementalResistanceTmp, _entityStat.GetElementalResistanceDisplayValue(), "0.00'%'");
    }

    private void SetText(TextMeshProUGUI tmp, float value, string format)
    {
        if (tmp == null) return;
        tmp.text = value.ToString(format);
    }
}
