using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// A single clickable entry in the store's category list.
    /// </summary>
    public class UI_StoreItemEntry : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _itemName;
    }
}
