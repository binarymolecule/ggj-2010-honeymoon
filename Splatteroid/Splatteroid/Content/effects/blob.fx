//**************************************************************//
//  Effect File exported by RenderMonkey 1.6
//
//  - Although many improvements were made to RenderMonkey FX  
//    file export, there are still situations that may cause   
//    compilation problems once the file is exported, such as  
//    occasional naming conflicts for methods, since FX format 
//    does not support any notions of name spaces. You need to 
//    try to create workspaces in such a way as to minimize    
//    potential naming conflicts on export.                    
//    
//  - Note that to minimize resulting name collisions in the FX 
//    file, RenderMonkey will mangle names for passes, shaders  
//    and function names as necessary to reduce name conflicts. 
//**************************************************************//

//--------------------------------------------------------------//
// HajoBlob
//--------------------------------------------------------------//
//--------------------------------------------------------------//
// Single Pass
//--------------------------------------------------------------//
string HajoBlob_Single_Pass_ScreenAlignedQuad : ModelData = "..\\..\\Program Files\\AMD\\RenderMonkey 1.82\\Examples\\Media\\Models\\ScreenAlignedQuad.3ds";

struct VS_OUTPUT
{
   float4 pos       : POSITION0;
   float2 texCoord  : TEXCOORD0;
};

VS_OUTPUT HajoBlob_Single_Pass_Vertex_Shader_vs_main( float4 inPos: POSITION )
{
   VS_OUTPUT o = (VS_OUTPUT) 0;

   inPos.xy = sign( inPos.xy);
   o.pos = float4( inPos.xy, 0.0f, 1.0f);

   // get into range [0,1]
   o.texCoord = (float2(o.pos.x, -o.pos.y) + 1.0f)/2.0f;
   return o;
}

texture backdrop_Tex
<
   string ResourceName = "\\NGJ2010\\Share\\backdrop.jpg";
>;
sampler2D Texture0 = sampler_state
{
   Texture = (backdrop_Tex);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   ADDRESSW = WRAP;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
   MAGFILTER = LINEAR;
};
texture blobs_Tex
<
   string ResourceName = "..\\Share\\blobs.jpg";
>;
sampler2D Texture1 = sampler_state
{
   Texture = (blobs_Tex);
   MAGFILTER = ANISOTROPIC;
   MINFILTER = ANISOTROPIC;
   MAXANISOTROPY = 16;
};
texture noise_Tex
<
   string ResourceName = "..\\Share\\noise.png";
>;
sampler2D Texture2 = sampler_state
{
   Texture = (noise_Tex);
   MAGFILTER = ANISOTROPIC;
   MINFILTER = ANISOTROPIC;
   MAXANISOTROPY = 16;
   ADDRESSU = MIRROR;
   ADDRESSV = MIRROR;
};

float2 fInverseViewportDimensions : ViewportDimensionsInverse;
float4 blobColor
<
   string UIName = "blobColor";
   string UIWidget = "Color";
   bool UIVisible =  true;
> = float4( 0.28, 0.43, 1.00, 1.00 );
float blobPow
<
   string UIName = "blobPow";
   string UIWidget = "Numeric";
   bool UIVisible =  true;
   float UIMin = 0.00;
   float UIMax = 64.00;
> = float( 2.56 );
float blobNoise
<
   string UIName = "blobNoise";
   string UIWidget = "Numeric";
   bool UIVisible =  true;
   float UIMin = 0.00;
   float UIMax = 1.00;
> = float( 0.02 );
float blobNoise2
<
   string UIName = "blobNoise2";
   string UIWidget = "Numeric";
   bool UIVisible =  true;
   float UIMin = -1.00;
   float UIMax = 1.00;
> = float( 0.06 );
float blobOffset
<
   string UIName = "blobOffset";
   string UIWidget = "Numeric";
   bool UIVisible =  true;
   float UIMin = -1.00;
   float UIMax = 1.00;
> = float( 0.48 );
float3 lightDir
<
   string UIName = "lightDir";
   string UIWidget = "Numeric";
   bool UIVisible =  true;
   float UIMin = -1.00;
   float UIMax = 1.00;
> = float3( 0.78, 0.66, 0.68 );
float highlightPow
<
   string UIName = "highlightPow";
   string UIWidget = "Numeric";
   bool UIVisible =  true;
   float UIMin = 0.00;
   float UIMax = 64.00;
> = float( 5.12 );
float fTime0_X : Time0_X;

float4 HajoBlob_Single_Pass_Pixel_Shader_ps_main( float2 texCoord  : TEXCOORD0 ) : COLOR
{
 float4 backdrop = tex2D( Texture0, texCoord );
   float noiseStrength = tex2D( Texture2,texCoord + fTime0_X*0.05).r - 0.5;
   float2 tc =texCoord+  noiseStrength * blobNoise; 
   float blobStrength = tex2D( Texture1, tc ).r;
   float blobX = tex2D( Texture1, tc + float2(fInverseViewportDimensions.x, 0)*3 ).r;
   float blobY = tex2D( Texture1, tc + float2(0, fInverseViewportDimensions.y)*3 ).r;
   blobStrength = blobStrength * (1-blobNoise2*noiseStrength);
   blobX = blobX -blobStrength;
   blobY = blobY -blobStrength;
 
   blobStrength = clamp(blobStrength-0.35,0,1);
   float refractionStrength = 1-pow(1-blobStrength,blobPow);
   float3 blobDir3 = normalize(float3(blobX, blobY, refractionStrength));
   float4 blobcolor = tex2D( Texture0, texCoord + blobDir3.xy*blobOffset);
   blobcolor = blobcolor * blobColor;
   float highlight = pow(dot(lightDir,blobDir3),highlightPow);
   return lerp(backdrop, blobcolor+highlight, clamp(blobStrength*100,0,1));
}


//--------------------------------------------------------------//
// Technique Section for HajoBlob
//--------------------------------------------------------------//
technique HajoBlob
{
   pass Single_Pass
   {
      CULLMODE = NONE;
      BLENDOP = ADD;
      SRCBLEND = SRCALPHA;
      DESTBLEND = INVSRCALPHA;
      ALPHABLENDENABLE = FALSE;

      //VertexShader = compile vs_2_0 HajoBlob_Single_Pass_Vertex_Shader_vs_main();
      PixelShader = compile ps_2_0 HajoBlob_Single_Pass_Pixel_Shader_ps_main();
   }

}

