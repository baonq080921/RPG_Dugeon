using System;
using System.Collections.Generic;
using Base;
using entity;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inventory
{
    public class PlayerInventory : InventoryBase
    {

        private EntityStat _entityStat;
        private IItemEffectTarget _effectTarget;
        [field: SerializeField] public List<ItemInventoryEquipment> equipList { get; private set; }
        private EventBinding<EquipEvent> _eventEquipBinding;
        private EventBinding<PlayerDiedEvent> _eventDiedBinding;
        private EventBinding<SkillPointRewardEvent> _skillPointRewardBinding;
        private EventBinding<MoneyAddRewardEvent> _moneyPointRewardBinding;

        [SerializeField] private float _startingSkillPoints = 0f;
        [SerializeField] private float _startingMoney = 0f;
        public float Money { get; private set; }
        public float SkillPoints { get; private set; }

        public event Action<float> OnSkillPointsChanged;
        public event Action<float> OnMoneyChanged;

        #region  SKill Points

        public bool CanSpendSkillPoints(float cost) => SkillPoints >= cost;

        public void SpendSkillPoints(float cost)
        {
            if (!CanSpendSkillPoints(cost)) return;
            SkillPoints -= cost;
            OnSkillPointsChanged?.Invoke(SkillPoints);
        }

        public void AddSkillPoints(float amount)
        {
            SkillPoints += amount;
            OnSkillPointsChanged?.Invoke(SkillPoints);
        }

        public void SetSkillPoints(float amount)
        {
            SkillPoints = amount;
            OnSkillPointsChanged?.Invoke(SkillPoints);
        }

        #endregion


        #region  Money asset
        public bool CanSpendMoney(float cost) => Money >= cost;

        public void SpendMoney(float cost)
        {
            if (!CanSpendMoney(cost)) return;
            Money -= cost;
            OnMoneyChanged?.Invoke(Money);
        }

        public void AddMoney(float amount)
        {
            Money += amount;
            OnMoneyChanged?.Invoke(Money);
        }

        public void SetMoney(float amout)
        {
            Money = amout;
            OnMoneyChanged?.Invoke(Money);

        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            _entityStat = GetComponent<EntityStat>();
            _effectTarget = GetComponent<IItemEffectTarget>();
            SkillPoints = _startingSkillPoints;
            Money = _startingMoney;
        }

        void OnEnable()
        {
            _eventEquipBinding = new EventBinding<EquipEvent>(TryToEquipEvent);
            EventBus<EquipEvent>.Register(_eventEquipBinding);
            _eventDiedBinding = new EventBinding<PlayerDiedEvent>(DropAllItem);
            EventBus<PlayerDiedEvent>.Register(_eventDiedBinding);
            _skillPointRewardBinding = new EventBinding<SkillPointRewardEvent>(e => AddSkillPoints(e.Amount));
            EventBus<SkillPointRewardEvent>.Register(_skillPointRewardBinding);
            _moneyPointRewardBinding = new EventBinding<MoneyAddRewardEvent>(e => AddMoney(e.Amount));
            EventBus<MoneyAddRewardEvent>.Register(_moneyPointRewardBinding);
        }

        void OnDisable()
        {
            EventBus<EquipEvent>.Deregister(_eventEquipBinding);
            EventBus<PlayerDiedEvent>.Deregister(_eventDiedBinding);
            EventBus<SkillPointRewardEvent>.Deregister(_skillPointRewardBinding);
            EventBus<MoneyAddRewardEvent>.Deregister(_moneyPointRewardBinding);

        }


        public void TryToEquipEvent(EquipEvent equipEvent) => TryEquipItem(equipEvent.itemInventory);


        private void TryEquipItem(ItemInventory item)
        {
            if (item == null || item.itemData == null) return;
            Vector2 pointerPos = Pointer.current?.position.ReadValue() ?? Vector2.zero;
            var worldPos = ServiceLocator.Get<Helper>().mainCam.WorldToScreenPoint(pointerPos);

            if (item.itemData.EquipSlot == EquipSlotType.None)
            {
                EventBus<AlertNotiEvent>.Raise(new AlertNotiEvent(GameMessages.Alert("Item Cannot Be Equipped"), worldPos, Color.red));
                return;
            }
            ItemInventory itemInventory = FindItem(item);
            if (itemInventory == null) return;

            if (item.itemData.EquipSlot == EquipSlotType.Use)
            {
                UseItem(item); // Some item like health potion may be in the future have mana or some potion that can cool down etc
                return;
            }

            ItemInventoryEquipment targetSlot = equipList.Find(slot => slot.slotType == item.itemData.EquipSlot);
            ItemInventoryEquipment slotToEquip = equipList.Find(slot => slot.slotType == item.itemData.EquipSlot);
            if (slotToEquip == null || slotToEquip.HasItem())
            {
                EventBus<AlertNotiEvent>.Raise(new AlertNotiEvent(GameMessages.Alert("Weapon Already In Slot"), worldPos, Color.red));
                return;
            }
            EquipItem(itemInventory, slotToEquip);
        }

        /// <summary>
        /// This for item that can be equip in the player equip slot like weapon amor,etc...
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        private void EquipItem(ItemInventory item, ItemInventoryEquipment slot)
        {
            if (item.stackSize > 1)
            {
                item.RemoveStackSize();
                slot.equipItem = new ItemInventory(item.itemData);
            }
            else
            {
                slot.equipItem = item;
                ClearFromInventory(item);
            }
            slot.equipItem.AddModifiers(_entityStat);
            slot.equipItem.AddItemEffect(_effectTarget);
            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent()); //Update UI Slot on the inventory
        }



        /// <summary>
        /// This for item that can increase player like health mana but right now just health
        /// </summary>
        /// <param name="item"></param>
        private void UseItem(ItemInventory item) // refactore this in the future to support many kind of item that can be used for all types
        {
            if (item.stackSize > 1)
            {
                item.RemoveStackSize();
            }
            else
            {
                ClearFromInventory(item);
            }
            ItemUse itemUse = item.itemData as ItemUse;
            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent()); // Update UI Slot on the inventory
            switch (item.itemData.ItemType)
            {

                case ItemTypes.HealingPotion:
                    EventBus<PlayerAddHealthAmountEvent>.Raise(new PlayerAddHealthAmountEvent(itemUse.Amount));
                    return;
                case ItemTypes.Dopping:
                    EventBus<PlayerBoostingAmountEvent>.Raise(new PlayerBoostingAmountEvent(itemUse.Amount));
                    return;
                    //Define here in the future
            }

        }

        /// <summary>
        /// Removes the item from the equipment slot and returns it to the inventory.
        /// If the inventory is full and cannot stack, the item is discarded.
        /// </summary>
        /// <param name="slot">The equipment slot to unequip.</param>
        public void UnequipItem(ItemInventoryEquipment slot)
        {
            if (!slot.HasItem()) return;

            ItemInventory item = slot.equipItem;
            slot.equipItem.RemoveModifiers(_entityStat);
            item.RemoveItemEffect(_effectTarget);
            slot.equipItem = null;
            ItemInventory stackable = FindItem(item);
            bool canStack = stackable != null && stackable.CanAddToStack();
            if (CanAddToIventory() || canStack)
                AddToInventory(item);
            else
                EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
        }

        /// <summary>
        /// Removes the item from the equipment slot and discards it without returning it to inventory.
        /// </summary>
        /// <param name="slot">The equipment slot to clear.</param>
        public void DropEquippedItem(ItemInventoryEquipment slot)
        {
            if (!slot.HasItem()) return;
            slot.equipItem.RemoveModifiers(_entityStat);
            slot.equipItem.RemoveItemEffect(_effectTarget);
            slot.equipItem = null;
            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
        }

        public void DropAllItem()
        {
            foreach (var item in itemInventoriesList)
                for (int i = 0; i < item.stackSize; i++)
                    SpawnDroppedItem(item.itemData);
            itemInventoriesList.Clear();

            foreach (var slot in equipList)
            {
                if (!slot.HasItem()) continue;
                SpawnDroppedItem(slot.equipItem.itemData);
                slot.equipItem.RemoveModifiers(_entityStat);
                slot.equipItem.RemoveItemEffect(_effectTarget);
                slot.equipItem = null;
            }

            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
        }

        // Raise a drop request instead of calling PoolManager directly, so Inventory (L2) does not
        // depend on the Pool package (L3). The Pool layer subscribes and performs the actual spawn.
        private void SpawnDroppedItem(ItemData itemData) =>
            EventBus<DropItemRequestedEvent>.Raise(new DropItemRequestedEvent(itemData, transform.position));
    }
}
