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
// Screen-AlignedQuad
//--------------------------------------------------------------//
//--------------------------------------------------------------//
// Twitch
//--------------------------------------------------------------//
string Screen_AlignedQuad_Twitch_ScreenAlignedQuad : ModelData = "D:\\Program Files (x86)\\AMD\\RenderMonkey 1.81\\Examples\\Advanced\\HeatHaze\\ScreenAlignedQuad.3ds";

texture p0_result_Tex : RenderColorTarget
<
   float2 ViewportRatio={1.0,1.0};
   string Format="D3DFMT_A8R8G8B8";
   float  ClearDepth=1.000000;
   int    ClearColor=-16777216;
>;
struct VS_OUTPUT
{
   float4 pos       : POSITION0;
   float2 texCoord  : TEXCOORD0;
};

VS_OUTPUT Screen_AlignedQuad_Twitch_Vertex_Shader_vs_main( float4 inPos: POSITION )
{
   VS_OUTPUT o = (VS_OUTPUT) 0;

   inPos.xy = sign( inPos.xy);
   o.pos = float4( inPos.xy, 0.0f, 1.0f);

   // get into range [0,1]
   o.texCoord = (float2(o.pos.x, -o.pos.y) + 1.0f)/2.0f;
   return o;
}

texture scene_Tex
<
   string ResourceName = "C:\\Users\\Marcel\\Desktop\\bg_spacy.png";
>;
sampler2D Texture0 = sampler_state
{
   Texture = (scene_Tex);
   ADDRESSU = WRAP;
   ADDRESSV = WRAP;
   ADDRESSW = WRAP;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
   MAGFILTER = LINEAR;
};
texture noise_tex_Tex
<
   string ResourceName = "C:\\Users\\Marcel\\Desktop\\bnoise.png";
>;
sampler2D Texture1 = sampler_state
{
   Texture = (noise_tex_Tex);
};
float fTime0_X : Time0_X = 0.0;
float fCosTime0_X = 0.0;
float rgbTwitchWidth
<
   string UIName = "rgbTwitchWidth";
   string UIWidget = "Numeric";
   bool UIVisible =  true;
   float UIMin = -1.00;
   float UIMax = 1.00;
> = float( 0.44 ) ;

float4 Screen_AlignedQuad_Twitch_Pixel_Shader_ps_main( float2 texCoord  : TEXCOORD0 ) : COLOR
{
   float strength = abs(2 * ((fTime0_X % 1) - 0.5));
   //strength = 0;
   
   float2 twitchDir = float2(0.1, 0) * rgbTwitchWidth * strength;
   float4 noised = tex2D( Texture1, float2(strength, texCoord.y) );
   float2 coord = texCoord + float2((0.5-noised.r) * 0.2f, 0) * strength;
   float4 orig = tex2D( Texture0, coord);
   float4 orig_l = tex2D( Texture0, coord + twitchDir);
   float4 orig_r = tex2D( Texture0, coord - twitchDir);   
   float4 rgbResult = float4(orig_l.r, orig.g, orig_r.b, 1);
   
   return lerp(rgbResult, (float4)1, strength * 0.7);
}


//--------------------------------------------------------------//
// ScaleDown
//--------------------------------------------------------------//
string Screen_AlignedQuad_ScaleDown_ScreenAlignedQuad : ModelData = "D:\\Program Files (x86)\\AMD\\RenderMonkey 1.81\\Examples\\Advanced\\HeatHaze\\ScreenAlignedQuad.3ds";

texture p0_blurry_Tex : RenderColorTarget
<
   float2 RenderTargetDimensions = {128,128};
   string Format="D3DFMT_A8R8G8B8";
   float  ClearDepth=1.000000;
   int    ClearColor=-16777216;
>;
struct Screen_AlignedQuad_ScaleDown_Vertex_Shader_VS_OUTPUT
{
   float4 pos       : POSITION0;
   float2 texCoord  : TEXCOORD0;
};

Screen_AlignedQuad_ScaleDown_Vertex_Shader_VS_OUTPUT Screen_AlignedQuad_ScaleDown_Vertex_Shader_vs_main( float4 inPos: POSITION )
{
   Screen_AlignedQuad_ScaleDown_Vertex_Shader_VS_OUTPUT o = (Screen_AlignedQuad_ScaleDown_Vertex_Shader_VS_OUTPUT) 0;

   inPos.xy = sign( inPos.xy);
   o.pos = float4( inPos.xy, 0.0f, 1.0f);

   // get into range [0,1]
   o.texCoord = (float2(o.pos.x, -o.pos.y) + 1.0f)/2.0f;
   return o;
}

sampler2D Screen_AlignedQuad_ScaleDown_Pixel_Shader_Texture0 = sampler_state
{
   Texture = (scene_Tex);
};

float4 Screen_AlignedQuad_ScaleDown_Pixel_Shader_ps_main( float2 texCoord  : TEXCOORD0 ) : COLOR
{
   return tex2D(Screen_AlignedQuad_ScaleDown_Pixel_Shader_Texture0, texCoord); 
}
//--------------------------------------------------------------//
// AfterTwitch
//--------------------------------------------------------------//
string Screen_AlignedQuad_AfterTwitch_ScreenAlignedQuad : ModelData = "D:\\Program Files (x86)\\AMD\\RenderMonkey 1.81\\Examples\\Advanced\\HeatHaze\\ScreenAlignedQuad.3ds";

struct Screen_AlignedQuad_AfterTwitch_Vertex_Shader_VS_OUTPUT
{
   float4 pos       : POSITION0;
   float2 texCoord  : TEXCOORD0;
};

Screen_AlignedQuad_AfterTwitch_Vertex_Shader_VS_OUTPUT Screen_AlignedQuad_AfterTwitch_Vertex_Shader_vs_main( float4 inPos: POSITION )
{
   Screen_AlignedQuad_AfterTwitch_Vertex_Shader_VS_OUTPUT o = (Screen_AlignedQuad_AfterTwitch_Vertex_Shader_VS_OUTPUT) 0;

   inPos.xy = sign( inPos.xy);
   o.pos = float4( inPos.xy, 0.0f, 1.0f);

   // get into range [0,1]
   o.texCoord = (float2(o.pos.x, -o.pos.y) + 1.0f)/2.0f;
   return o;
}

sampler2D Screen_AlignedQuad_AfterTwitch_Pixel_Shader_Texture0 = sampler_state
{
   Texture = (p0_result_Tex);
};
sampler2D Screen_AlignedQuad_AfterTwitch_Pixel_Shader_Texture1 = sampler_state
{
   Texture = (p0_blurry_Tex);
   MAGFILTER = ANISOTROPIC;
   MINFILTER = ANISOTROPIC;
   MIPFILTER = NONE;
};
float Screen_AlignedQuad_AfterTwitch_Pixel_Shader_fTime0_X : Time0_X = 0;
const float2 blurStep = float2(0, 0.02f);

float4 Screen_AlignedQuad_AfterTwitch_Pixel_Shader_ps_main( float2 texCoord  : TEXCOORD0 ) : COLOR
{
   float strength = abs(2 * ((Screen_AlignedQuad_AfterTwitch_Pixel_Shader_fTime0_X % 1) - 0.5));

   float4 orig = tex2D(Screen_AlignedQuad_AfterTwitch_Pixel_Shader_Texture0, texCoord); 
   float4 sum = orig * 0.5;
   
   for(int i=1; i<8; i++)
   {
       float4 v1 = tex2D(Screen_AlignedQuad_AfterTwitch_Pixel_Shader_Texture1, texCoord + blurStep * i * strength);
       float4 v2 = tex2D(Screen_AlignedQuad_AfterTwitch_Pixel_Shader_Texture1, texCoord - blurStep * i * strength);
       sum += (v1 + v2) * pow(0.5, i + 1);
   }
  
   return lerp(orig, sum, strength);
}




//--------------------------------------------------------------//
// Technique Section for Screen-AlignedQuad
//--------------------------------------------------------------//
technique Screen_AlignedQuad
{
   pass Twitch
   <
      string Script = "RenderColorTarget0 = p0_result_Tex;"
                      "ClearColor = (0, 0, 0, 255);"
                      "ClearDepth = 1.000000;";
   >
   {
      CULLMODE = NONE;

      //VertexShader = compile vs_2_0 Screen_AlignedQuad_Twitch_Vertex_Shader_vs_main();
      PixelShader = compile ps_2_0 Screen_AlignedQuad_Twitch_Pixel_Shader_ps_main();
   }

   pass ScaleDown
   <
      string Script = "RenderColorTarget0 = p0_blurry_Tex;"
                      "ClearColor = (0, 0, 0, 255);"
                      "ClearDepth = 1.000000;";
   >
   {
      //VertexShader = compile vs_2_0 Screen_AlignedQuad_ScaleDown_Vertex_Shader_vs_main();
      PixelShader = compile ps_2_0 Screen_AlignedQuad_ScaleDown_Pixel_Shader_ps_main();
   }

   pass AfterTwitch
   {
      //VertexShader = compile vs_2_0 Screen_AlignedQuad_AfterTwitch_Vertex_Shader_vs_main();
      PixelShader = compile ps_2_0 Screen_AlignedQuad_AfterTwitch_Pixel_Shader_ps_main();
   }

}

