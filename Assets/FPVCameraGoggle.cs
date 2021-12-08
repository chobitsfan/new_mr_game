using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVCameraGoggle : MonoBehaviour
{
    public FPV_CAM fpvCamera;
    public Shader LenDistortShader;

    Material mat;
    Material DistortMat;

    // Start is called before the first frame update
    void Start()
    {
        DistortMat = new Material(LenDistortShader);
        mat = fpvCamera.mat;
    }

    void OnPreRender()
    {
        Graphics.Blit(null, mat);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, DistortMat);
    }
}
