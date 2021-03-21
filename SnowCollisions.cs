using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowCollisions : MonoBehaviour
{
    public Shader _drawShader;
    public GameObject snow;
    public Transform mesh;
    private RenderTexture _disptex;
    private Material _snowMaterial, _drawMaterial;
    private RaycastHit _hit;
    private Rigidbody rb;
    private Vector3 normal;
    private float start_y; 
    private bool stopped;
    private int layerMask;
    private float strength;

    void Start()
    {
        _drawMaterial = new Material(_drawShader);
        _snowMaterial = snow.GetComponent<MeshRenderer>().material;
        _snowMaterial.SetTexture(
            "_DispTex", 
            _disptex = new RenderTexture(2048, 2048, 0, RenderTextureFormat.ARGBFloat)
        );
        rb = GetComponent<Rigidbody>();
        strength = rb.mass * 0.0055f;
        layerMask = LayerMask.GetMask("Snow Rendering");
    }

    private void OnTriggerEnter(Collider col)
    {
        switch (rb.mass <= 1 | rb.mass + rb.drag <= 2) { 
            case true: rb.isKinematic = true; stopped = true; break;
            case false: StartCoroutine(RunOpposingForce(col)); DrawDisplacements(); break;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (!stopped) { StartCoroutine(RunOpposingForce(col)); DrawDisplacements(); }
    }

    IEnumerator RunOpposingForce(Collider col) 
    {
        yield return new WaitForFixedUpdate();
        if (!rb.isKinematic) {
            switch (rb.velocity.y < 0) {
                case true:
                    normal = -(rb.mass * rb.velocity);
                    rb.AddForce(normal, ForceMode.Force);
                    rb.AddForce(-(Physics.gravity), ForceMode.Impulse);
                    break;
                case false: rb.isKinematic = true; stopped = true; break;
            }
        }
    }

    private void DrawDisplacements() {
        Physics.Raycast(mesh.position, -Vector3.up, out _hit, 10f, layerMask);
        _drawMaterial.SetVector(
            "_Coordinate",
            new Vector4(_hit.textureCoord.x, _hit.textureCoord.y, 0, 0)
        );
        _drawMaterial.SetVector("_Color", Color.red);
        _drawMaterial.SetFloat("_Strength", strength);
        _drawMaterial.SetFloat("_Size ", 10.0f);
        RenderTexture temp = RenderTexture.GetTemporary(
            _disptex.width,
            _disptex.height,
            0,
            RenderTextureFormat.ARGBFloat
        );
        Graphics.Blit(_disptex, temp);
        Graphics.Blit(temp, _disptex, _drawMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 128, 128), _disptex, ScaleMode.ScaleToFit, false, 1);
    }
}
