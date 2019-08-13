using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

    public string name = "MoonBullet";

    public int damage = 10;

    public float range = 200f;

    public float fireRate = 0f;

    public int reloadTime = 1;

    public int bullets = 10;

    public int maxBullets = 10;

    public GameObject graphics;
}
