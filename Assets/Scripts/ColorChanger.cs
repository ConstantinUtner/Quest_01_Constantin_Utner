using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField]
    private Color color;

    private MaterialPropertyBlock mpb;
    private MeshRenderer mr;

    void Start()
    {
        this.mpb = new MaterialPropertyBlock();
        this.mr = this.GetComponent<MeshRenderer>();
        this.mr.SetPropertyBlock(this.mpb);
    }

    void Update()
    {
        this.mpb.SetColor("_BaseColor", this.color);
        this.mr.SetPropertyBlock(this.mpb);
    }
}
