using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalController : MonoBehaviour
{
    public float detectDistance = 3f;
    public WeaponScriptableObject[] possibleWeapons;
    private WeaponScriptableObject myWeapon;

    private bool finished = false;

    public SpriteRenderer weapon;
    GameObject weaponSelectUI;
    
    // Start is called before the first frame update
    void Start()
    {
        myWeapon = possibleWeapons[Random.Range(0, possibleWeapons.Length)];
        weapon.sprite = myWeapon.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        var player = FindObjectOfType<PlayerController>();
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectDistance && !finished) {
            if(Input.GetKeyDown(KeyCode.E)) {
                var canvas = FindObjectOfType<Canvas>().gameObject.transform;

                weaponSelectUI = Instantiate(Resources.Load("Prefabs/UI/WeaponSelect", typeof(GameObject)) as GameObject,
                    canvas);

                weaponSelectUI.GetComponent<WeaponSelectorUI>().Initialize(myWeapon, this);

                finished = true;
            }
        }
    }

    public void MyDestroy() {
        var dust = Instantiate(Resources.Load("Prefabs/Spawn", typeof(GameObject)) as GameObject,
                            transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(gameObject);
    }
}
