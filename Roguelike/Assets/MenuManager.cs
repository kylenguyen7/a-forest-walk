using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MenuManager : MonoBehaviour
{
    #region singleton
    public static MenuManager instance;
    private void Awake() {
        if(instance != null) {
            Debug.LogError("Found more than one MenuManager in the scene!");
            return;
        }
        instance = this;
    }
    #endregion

    bool menuEnabled = false;
    CanvasGroup menusCanvasGroup;
    [SerializeField] CanvasGroup hotbar;
    [SerializeField] CanvasGroup inventory;
    [SerializeField] CanvasGroup shop;

    Animator animator;

    public enum menus {
        inventory,
        shop,
        upgrades,
        none
    }
    menus activeMenu = menus.none;
    bool menuActive = false;

    private void Start() {
        menusCanvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
    }

    public void ToggleMenu(menus menu) {
        if(activeMenu == menu) {
            DeactivateMenu();
        } else {
            if (activeMenu != menus.none) DeactivateMenu();
            ActivateMenu(menu);
        }
    }

    void ActivateMenu(menus menu) {
        switch(menu) {
            case menus.inventory: {
                    animator.SetTrigger("inventory");
                    activeMenu = menu;
                } break;
            case menus.shop: {
                    animator.SetTrigger("shop");
                    activeMenu = menu;
                } break;
            case menus.upgrades: {
                    animator.SetTrigger("upgrades");
                    activeMenu = menu;
                }
                break;
            default: {
                    Debug.LogError("Something went wrong activating a menu.");
                } break;
        }

        // if a menu was activated
        if(activeMenu != menus.none) {
            ToggleCanvasGroup(hotbar, false);
            Time.timeScale = 0f;
        }
    }

    void DeactivateMenu() {
        if (activeMenu != menus.none) {
            animator.SetTrigger("reset");
            ToggleCanvasGroup(hotbar, true);
            Time.timeScale = 1f;

            activeMenu = menus.none;
        }
    }

    static void ToggleCanvasGroup(CanvasGroup cg, bool on) {
        if(on) {
            // cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        } else {
            // cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }
        
    }
}
