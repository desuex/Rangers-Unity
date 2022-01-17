using Assets.Graphics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private HAI _hai;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private uint currentFrame = 0;
    // Start is called before the first frame update
    void Start()
    {
        this._hai = new HAI("PelengRs.hai");
        this.sprites = this._hai.LoadBytes();

        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.spriteRenderer.sprite = this.sprites[0];
        this.AdvanceAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        this.AdvanceAnimation();
    }
    void AdvanceAnimation()
    {
        if (this.currentFrame%10 == 0)
        {
            this.spriteRenderer.sprite = this.sprites[this.currentFrame / 10];
        }
        
        this.currentFrame++;
        if (this.currentFrame >= this.sprites.Length*10)
        {
            this.currentFrame = 0;
        }
    }

}
