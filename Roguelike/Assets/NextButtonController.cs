using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextButtonController : ButtonController
{
    protected override void OnClick() {
        MenuManager.instance.ToggleMenu(MenuManager.menus.upgrades);
        Invoke("Restart", 1f);
    }

    private void Restart() {
        MusicPlayer.instance.Pause();
        TransitionController.instance.Transition("procedural generators only!");
    }
}
