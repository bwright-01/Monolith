using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour {
    [SerializeField] Color color = Color.white;
    [SerializeField][Range(0, 16)] int outlineSize = 1;

    MaterialPropertyBlock mpb;
    SpriteRenderer spriteRenderer;

    Color lastColor;
    int lastSize;

    void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateOutline(true);
        lastColor = color;
        lastSize = outlineSize;
    }

    void OnDisable() {
        UpdateOutline(false);
    }

    void Update() {
        if (lastColor != color || lastSize != outlineSize) {
            UpdateOutline(true);
            lastColor = color;
            lastSize = outlineSize;
        }
    }

    void UpdateOutline(bool outline) {
        mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
