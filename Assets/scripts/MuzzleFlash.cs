using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {

    public GameObject muzzleFlash;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public float flashTime;

    void Start()
    {
        deactivate();
    }

    public void activate()
    {
        muzzleFlash.SetActive(true);

        int flashSpriteIndex = Random.Range(0, flashSprites.Length);
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }

        Invoke("deactivate", flashTime);
    }

    void deactivate()
    {
        muzzleFlash.SetActive(false);
    }
}
