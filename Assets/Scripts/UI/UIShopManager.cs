using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UIShopManager : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;

    [Header("UI References")]
    [SerializeField] private GameObject optionPrefab; // Prefab for shop option
    [SerializeField] private GameObject closeButtonPrefab; // Prefab for close button

    private List<GameObject> spawnedOptions = new List<GameObject>();
    private GameObject spawnedCloseButton;

    void Start()
    {
        ClearOptions();
    }

    void Update()
    {
        bool shouldShow = shopManager != null && shopManager.IsOpen;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(shouldShow);
        }

        if (shouldShow && spawnedOptions.Count == 0)
        {
            ShowOptions();
        }
        else if (!shouldShow && spawnedOptions.Count > 0)
        {
            ClearOptions();
            return;
        }
        if (!shouldShow)
        {
            GetComponent<TMP_Text>().text = "";
            return;
        } else
        {
            GetComponent<TMP_Text>().text = "Choose an item to buy:";
        }
        ClearHoverEffect();
        if (shopManager.SelectedOption == 4)
        {
            var image2 = spawnedCloseButton.GetComponentInChildren<Image>();
            image2.color = new Color(image2.color.r, image2.color.g, image2.color.b, 0.5f);
        } else
        {
            var image = spawnedOptions[shopManager.SelectedOption].GetComponent<RawImage>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
        }
    }

    private void ShowOptions()
    {
        if (shopManager == null || optionPrefab == null)
            return;

        int count = Mathf.Min(4, shopManager.ItemsForSale.Count);
        for (int i = 0; i < count; i++)
        {
            GameObject option = Instantiate(optionPrefab, transform);
            option.transform.localPosition = new Vector3(-400 + 300 * i, -200, 0);
            var img = option.GetComponent<RawImage>();
            Color baseColor = img.color;
            img.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
            option.transform.GetChild(0).GetComponent<TMP_Text>().text = shopManager.ItemsForSale[i].type.ToString();
            option.transform.GetChild(1).GetComponent<TMP_Text>().text = shopManager.MapItemPrice(shopManager.ItemsForSale[i]).ToString();
            spawnedOptions.Add(option);
        }

        if (closeButtonPrefab != null)
        {
            spawnedCloseButton = Instantiate(closeButtonPrefab, transform);
            spawnedCloseButton.transform.localPosition = new Vector3(0, -400, 0);
            spawnedCloseButton.GetComponent<TMP_Text>().text = "Don't buy anything";
        }
    }

    private void ClearOptions()
    {
        foreach (var go in spawnedOptions)
        {
            if (go != null) Destroy(go);
        }
        spawnedOptions.Clear();

        if (spawnedCloseButton != null)
        {
            Destroy(spawnedCloseButton);
            spawnedCloseButton = null;
        }
    }

    private void ClearHoverEffect()
    {
        foreach (var button in spawnedOptions)
        {
            var image = button.GetComponent<RawImage>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.25f);
        }
        var image2 = spawnedCloseButton.GetComponentInChildren<Image>();
        image2.color = new Color(image2.color.r, image2.color.g, image2.color.b, 0);
    }
}