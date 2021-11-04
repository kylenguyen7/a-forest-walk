using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectorUI : MonoBehaviour
{
    public Image primaryWeaponCard;
    public Image offhandWeaponCard;
    public Image newCard;

    public Image primaryWeaponArrow;
    public Image offhandWeaponArrow;

    Image selectedCard;

    public Sprite weaponCard;
    public Sprite weaponCardOutlined;

    bool finished = false;
    PedestalController myPedestal;
    
    // Start is called before the first frame update
    void Start()
    {
        selectedCard = primaryWeaponCard;

        primaryWeaponCard.sprite = weaponCardOutlined;
        offhandWeaponCard.sprite = weaponCard;

        primaryWeaponArrow.enabled = true;
        offhandWeaponArrow.enabled = false;

        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (finished) return;

        if(Input.GetAxisRaw("Vertical") == 1) {
            selectedCard = primaryWeaponCard;

            primaryWeaponCard.sprite = weaponCardOutlined;
            offhandWeaponCard.sprite = weaponCard;

            primaryWeaponArrow.enabled = true;
            offhandWeaponArrow.enabled = false;
        }

        if(Input.GetAxisRaw("Vertical") == -1) {
            selectedCard = offhandWeaponCard;

            primaryWeaponCard.sprite = weaponCard;
            offhandWeaponCard.sprite = weaponCardOutlined;

            primaryWeaponArrow.enabled = false;
            offhandWeaponArrow.enabled = true;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            Destroy(gameObject);
        }

        if(Input.GetKeyDown(KeyCode.E)) {
            WeaponScriptableObject chosenWeapon = newCard.gameObject.GetComponent<WeaponCardUI>().myWeapon;
            Debug.Log(chosenWeapon);

            WeaponControllerPlayer weapon = FindObjectOfType<WeaponControllerPlayer>();

            if(selectedCard == primaryWeaponCard) {
                weapon.primaryWeapon = chosenWeapon;
            } else {
                weapon.offhandWeapon = chosenWeapon;
            }

            // Extra reset to refresh weapon to new primary/offhand, if it has changed
            weapon.myWeapon = weapon.holdingPrimary ? weapon.primaryWeapon : weapon.offhandWeapon;
            weapon.Reset();

            finished = true;
            StartFirstAnim();
        }
    }

    public RectTransform topLeft;
    public RectTransform topRight;
    public RectTransform bottomLeft;
    public RectTransform bottomRight;
    private void StartFirstAnim() {
        Debug.Log("starting first anim");
        var newCardController = newCard.GetComponent<WeaponCardUI>();

        if (selectedCard == primaryWeaponCard) {
            primaryWeaponCard.GetComponent<WeaponCardUI>().MoveToFirstWaypoint();
            newCardController.firstWaypoint = topLeft;
            newCardController.MoveToFirstWaypoint();
        } else {
            offhandWeaponCard.GetComponent<WeaponCardUI>().MoveToFirstWaypoint();
            newCardController.firstWaypoint = bottomLeft;
            newCardController.MoveToFirstWaypoint();
        }
        StartCoroutine(StartSecondAnim());
    }

    private IEnumerator StartSecondAnim() {
        yield return new WaitForSecondsRealtime(0.3f);
        var newCardController = newCard.GetComponent<WeaponCardUI>();

        if (selectedCard == primaryWeaponCard) {
            primaryWeaponCard.GetComponent<WeaponCardUI>().MoveToSecondWaypoint();
            newCardController.firstWaypoint = topRight;
            newCardController.MoveToFirstWaypoint();
        } else {
            offhandWeaponCard.GetComponent<WeaponCardUI>().MoveToSecondWaypoint();
            newCardController.firstWaypoint = bottomRight;
            newCardController.MoveToFirstWaypoint();
        }
        StartCoroutine(MyDestroy());
    }

    private IEnumerator MyDestroy() {
        yield return new WaitForSecondsRealtime(1f);
        myPedestal.MyDestroy();
        Destroy(gameObject);
    }

    private void OnDestroy() {
        Time.timeScale = 1;
    }

    // Updates cards with correct info based on weapons player is carrying
    // and the weapon that was found.
    public void Initialize(WeaponScriptableObject newWeapon, PedestalController pedestal) {
        var newCardController = newCard.GetComponent<WeaponCardUI>();
        newCardController.myWeapon = newWeapon;
        newCardController.UpdateDisplay();

        var primary = FindObjectOfType<WeaponControllerPlayer>().primaryWeapon;
        var primaryCardController = primaryWeaponCard.GetComponent<WeaponCardUI>();
        primaryCardController.myWeapon = primary;
        primaryCardController.UpdateDisplay();

        var offhand = FindObjectOfType<WeaponControllerPlayer>().offhandWeapon;
        var offhandCardController = offhandWeaponCard.GetComponent<WeaponCardUI>();
        offhandCardController.myWeapon = offhand;
        offhandCardController.UpdateDisplay();

        // Reference to pedestal which can be destroyed
        myPedestal = pedestal;
    }
}
