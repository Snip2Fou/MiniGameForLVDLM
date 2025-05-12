using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    private PlayerMovement playerMovement;

    [Header("Reference")]
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject inventoryContainer;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private InputActionReference inventoryAction;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inventoryCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        inventoryAction.action.Enable();
        inventoryAction.action.performed += ShowInventory;
    }

    private void OnDisable()
    {
        inventoryAction.action.performed -= ShowInventory;
        inventoryAction.action.performed -= HideInventory;
        inventoryAction.action.Disable();
    }

    public void AddItem(Item item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]++;
        }
        else
        {
            inventory[item] = 1;
        }
    }

    public void RemoveItem(Item item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]--;
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
            }
        }
    }

    public void ShowInventory(InputAction.CallbackContext context)
    {
        inventoryCanvas.SetActive(true);
        playerMovement.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        UpdateInventoryUI();
        inventoryAction.action.performed -= ShowInventory;
        inventoryAction.action.performed += HideInventory;
    }

    public void HideInventory(InputAction.CallbackContext context)
    {
        inventoryCanvas.SetActive(false);
        playerMovement.enabled = true;
        Cursor.lockState = CursorLockMode.Locked; 
        inventoryAction.action.performed -= HideInventory;
        inventoryAction.action.performed += ShowInventory;
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in inventory)
        {
            GameObject newItem = Instantiate(inventoryItemPrefab, inventoryContainer.transform);
            newItem.transform.Find("IconBG").transform.Find("Icon").GetComponent<Image>().sprite = item.Key.icon;
            newItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Key.name;
            newItem.transform.Find("NBText").GetComponent<TextMeshProUGUI>().text = item.Value.ToString();
        }
    }
}
