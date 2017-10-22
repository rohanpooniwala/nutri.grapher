Shader "Custom/CameraBackground" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "black" {}
    }
    SubShader {
        //Tags {"Queue"="overlay+1" "RenderType"="overlay" }
        Pass {
            // Render the teapot
            SetTexture [_MainTex] {
                combine texture 
            }
        }
    } 
    FallBack "Diffuse"
}
