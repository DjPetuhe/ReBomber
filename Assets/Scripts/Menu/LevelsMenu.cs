using ClipperLib;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button originalButton;
    [SerializeField] Button customButton;

    [Header("Menu Image")]
    [SerializeField] Sprite originalMenu;
    [SerializeField] Sprite customMenu;

    private void Start()
    {
        
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ToCustomLevels() => SwitchMenu(originalButton, customButton, customMenu);

    public void ToOriginalLevels() => SwitchMenu(customButton, originalButton, originalMenu);

    private void SwitchMenu(Button from, Button to, Sprite toMenu)
    {
        from.interactable = true;
        to.interactable = false;
        from.GetComponentInChildren<TextMeshProUGUI>().color = new Color(50, 50, 50);
        to.GetComponentInChildren<TextMeshProUGUI>().color = new Color(150, 150, 150);
        GetComponent<Image>().sprite = toMenu;
    }
}
