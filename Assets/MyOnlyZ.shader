Shader "Custom/MyOnlyZ"
{
    SubShader
    {

        Tags { "Queue" = "Geometry-1" }  // Write to the stencil buffer before drawing any geometry to the screen
        ColorMask 0 // Don't write to any colour channels
        ZWrite On

        Pass
        {
            //do nothing
        }
    }
}