Shader "Kao/KaoToon"
{
	Properties
	{
		[Header(Face Setting)]
		[ToggleUI] _IsFace("Is Face? (顔の陰軽減)", Float) = 0

		[Header(Base Color)]
		[MainTexture] _BaseMap("Texture", 2D) = "white" {}
		[HDR][MainColor]_BaseColor("_BaseColor", Color) = (1,1,1,1)

		[Header(Alpha)]
		[Toggle(_UseAlphaClipping)]_UseAlphaClipping("_UseAlphaClipping", Float) = 0
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

		[Header(Emission)]
		[Toggle]_UseEmission("_UseEmission (on/off Emission completely)", Float) = 0
		[HDR] _EmissionColor("_EmissionColor", Color) = (0,0,0)
		_EmissionMulByBaseColor("_EmissionMulByBaseColor", Range(0,1)) = 0
		[NoScaleOffset]_EmissionMap("_EmissionMap", 2D) = "white" {}
		_EmissionMapChannelMask("_EmissionMapChannelMask", Vector) = (1,1,1,0)

		[Header(Occlusion)]
		[Toggle]_UseOcclusion("_UseOcclusion (on/off Occlusion completely)", Float) = 0
		_OcclusionStrength("_OcclusionStrength", Range(0.0, 1.0)) = 1.0
		[NoScaleOffset]_OcclusionMap("_OcclusionMap", 2D) = "white" {}
		_OcclusionMapChannelMask("_OcclusionMapChannelMask", Vector) = (1,0,0,0)
		_OcclusionRemapStart("_OcclusionRemapStart", Range(0,1)) = 0
		_OcclusionRemapEnd("_OcclusionRemapEnd", Range(0,1)) = 1

		[Header(Lighting)]
		_IndirectLightMinColor("_IndirectLightMinColor", Color) = (0.1,0.1,0.1,1) // ライトプローブがBakeされていない場合、完全に黒になるのを防ぐことができます
		_IndirectLightMultiplier("_IndirectLightMultiplier", Range(0,1)) = 1
		_DirectLightMultiplier("_DirectLightMultiplier", Range(0,1)) = 1
		_CelShadeMidPoint("_CelShadeMidPoint", Range(-1,1)) = -0.5
		_CelShadeSoftness("_CelShadeSoftness", Range(0,1)) = 0.05
		_MainLightIgnoreCelShade("_MainLightIgnoreCelShade", Range(0,1)) = 0
		_AdditionalLightIgnoreCelShade("_AdditionalLightIgnoreCelShade", Range(0,1)) = 0.9

		 [Header(Shadow mapping)]
		_ReceiveShadowMappingAmount("_ReceiveShadowMappingAmount", Range(0,1)) = 0.65
		_ReceiveShadowMappingPosOffset("_ReceiveShadowMappingPosOffset", Float) = 0
		_ShadowMapColor("_ShadowMapColor", Color) = (1,0.825,0.78)

		[Header(Outline)]
		_OutlineWidth("_OutlineWidth (World Space)", Range(0,4)) = 1
		_OutlineColor("_OutlineColor", Color) = (0.5,0.5,0.5,1)
		_OutlineZOffset("_OutlineZOffset (View Space)", Range(0,1)) = 0.0001
		[NoScaleOffset]_OutlineZOffsetMaskTex("_OutlineZOffsetMask (black is apply ZOffset)", 2D) = "black" {}
		_OutlineZOffsetMaskRemapStart("_OutlineZOffsetMaskRemapStart", Range(0,1)) = 0
		_OutlineZOffsetMaskRemapEnd("_OutlineZOffsetMaskRemapEnd", Range(0,1)) = 1					  
	}
		SubShader
		{
			Tags {
				"RenderPipeline" = "UniversalPipeline"
				"RenderType" = "Opaque"
				"UniversalMaterialType" = "Lit"
				"Queue" = "Geometry"}

			HLSLINCLUDE

			// 全てのパスはこれを使う
			#pragma shader_feature_local_fragment _UseAlphaClipping

			ENDHLSL

			// [#0 Pass - MainColor]
			Pass
			{
				Name "FowerdLit"
				Tags{"LightMode" = "UniversalForward"}

				Cull Back
				ZTest LEqual
				ZWrite On
				Blend One Zero

				HLSLPROGRAM
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
				#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
				#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
				#pragma multi_compile_fragment _ _SHADOWS_SOFT

				// ---------------------------------------------------------------------------------------------
				// multi_compile = シェーダバリアント
				#pragma multi_compile_fog
				// ---------------------------------------------------------------------------------------------
				
				#pragma vertex VertexShaderWork
				#pragma fragment ShadeFinalColor

			    //シェーダーが書かれたHLSLファイル
				#include "KaoToonOutline_Shared.hlsl"
				ENDHLSL
			}

			// [#1 Pass - Outline]
			// 上記の「ForwardLit」パスと同じですが、
			// -頂点の位置は法線方向に基づいて少しだけプッシュアウトされます
			// -また、色もtintedされます
			// -Cull Backの代わりにCull Frontを使用します。これは、すべての追加のパスアウトライン方法に必要です
			
			Pass
			{
				Name "Outline"
				Tags
				{
					// 重要: どのカスタムパスにもこの行を書かないでください! そうしないと、このアウトラインパスはURPによってレンダリングされません!
					//"LightMode" = "UniversalForward" 

					// [重要なCPUパフォーマンス ノート]
					// シェーダーにカスタムパスを追加する必要がある場合（アウトラインパス、planar shadowパス、ブロックされたときのXRayパスなど）、
					// (0) 新しいPass {}をシェーダーに追加する
					// (1) 新しいPassのTags {}内に "LightMode" = "YourCustomPassTag" を書く
					// (2) 新しいカスタムRendererFeature（C＃）をレンダラーに追加する
					// (3) cmd.DrawRenderers() with ShaderPassName = "YourCustomPassTag" で書く
					// (4) 正しく実行されると、URPはシェーダーの新しいPass {}をSRP-batcherフレンドリーな方法でレンダリングします(通常は1つの大きなSRPバッチで、)

					//"LightMode" = "PostTransparentPass"
					// チュートリアル目的で、現在のすべてはC＃がないシェーダーファイルだけであるため、このOutline passは実際にはSRP-batcherにはフレンドリーではありません。
					// 多くのキャラクターを扱うプロジェクトで作業している場合は、Outline passをSRP-batcherにフレンドリーにするために上記の方法を使用するようにしてください!
				}

				Cull Front // Cull Front is a must for extra pass outline method

				HLSLPROGRAM

				// "ForwardLit" パスから直接コピー
				// ---------------------------------------------------------------------------------------------
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
				#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
				#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
				#pragma multi_compile_fragment _ _SHADOWS_SOFT
				// ---------------------------------------------------------------------------------------------
				#pragma multi_compile_fog
				// ---------------------------------------------------------------------------------------------

				#pragma vertex VertexShaderWork
				#pragma fragment ShadeFinalColor

				// 「ToonShaderIsOutline」を定義(define)することで、「VertexShaderWork()」と「ShadeFinalColor()」の両方に、輪郭線に関連するコードを挿入することができます。
				#define ToonShaderIsOutline

				// この .hlsl ファイル内にすべてのシェーダーのロジックが記述されています。すべての #define を #include の前に記述することを忘れないでください。
				#include "KaoToonOutline_Shared.hlsl"

				ENDHLSL
			}
			
			// [#2 Pass - ShadowCaster]
			Pass
			{
				Name "ShadowCaster"
				Tags
				{
					"LightMode" = "ShadowCaster"
				}

				ZWrite On
				ZTest LEqual
				ColorMask 0
				Cull Back


				HLSLPROGRAM

				#pragma vertex VertexShaderWork
				#pragma fragment BaseColorAlphaClipTest

				#define ToonShaderApplyShadowBiasFix

				#include "KaoToonOutline_Shared.hlsl"


				ENDHLSL
			}
			
			// [#3 Pass - DepthOnly]
			Pass
			{
				Name "DepthOnly"
				Tags{"LightMode" = "DepthOnly"}

				// より明確な「render state」で混乱することを回避する
				ZWrite On    // このパスの唯一の目的は、深度を書き込むことです。
				ZTest LEqual // 「Early-Z」ステージで処理を早期に終了することができる場合はそのようにする          
				ColorMask 0  // 色については気にしないが、深度だけを書き込むことが望ましい。「ColorMask 0」を使用することで、書き込み帯域を節約することができる。
				Cull Back    // 「Cull[_Cull]」をサポートするには、フラグメントシェーダー内で「VFACE」を使用して、「flip vertex normal」する必要があります。これは、単純なチュートリアルシェーダーの範囲を超えるかもしれません。

				HLSLPROGRAM

				// このパスで必要なキーワードは、「_UseAlphaClipping」のみです。これは、すでに「HLSLINCLUDE」ブロック内で定義されています。
				// したがって、このパスでは「multi_compile」や「shader_feature」を書く必要はありません。

				#pragma vertex VertexShaderWork
				#pragma fragment BaseColorAlphaClipTest // 「Clip()」を行うだけで、色の描画は必要ありません。

				// アウトラインエリアも深度を書き込む必要があるため、「ToonShaderIsOutline」を定義(define)して、「VertexShaderWork()」にアウトライン関連のコードを挿入します。
				#define ToonShaderIsOutline

				// すべてのシェーダーロジックは、この「.hlsl」ファイル内で記述されています。「#include」を記述する前に、すべての「#define」を記述することを忘れないでください。
				#include "KaoToonOutline_Shared.hlsl"

				ENDHLSL
			}
			
		}
			FallBack "Hidden/Universal Render Pipeline/FallbackError"
}