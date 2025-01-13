#include "Assets/Jettelly Books/Visualizing Equations Vol. 2/Chapter 1/Materials/HLSL/Segment.hlsl"
#include "Assets/Jettelly Books/Visualizing Equations Vol. 2/Chapter 3/Materials/HLSL/AntiAliasingStep.hlsl"

float shuriken(float2 uv, float x0, float radius)
{
    float y = abs(uv.y);
    float x = abs(uv.x);

    float2 p0 = float2(0.0, 0.5);
    float2 p1 = float2(x0, x0);

    float segment_p0_p1 = y - (tangent(p0, p1) * (x - p0.x) + p0.y);
    segment_p0_p1 = 1.0 - anti_aliasing_step(segment_p0_p1, 0);

    float2 n = float2(-1 / sqrt(2), 1 / sqrt(2));
    float2 p2 = p0 - 2.0 * n * dot(p0, n);

    float segment_p2_p1 = y - (tangent(p2, p1) * (x - p2.x) + p2.y);
    segment_p2_p1 = 1.0 - anti_aliasing_step(segment_p2_p1, 0);

    // circles
    float r = radius * radius;
    float reflected_circle = (x - x0) * (x - x0) + (y - x0) * (y - x0);
    reflected_circle = anti_aliasing_step(reflected_circle, r);

    float circle = (x * x) + (y * y);
    circle = anti_aliasing_step(circle, r);

    segment_p0_p1 += segment_p2_p1;
    segment_p0_p1 *= reflected_circle;
    segment_p0_p1 *= circle;

    return saturate(segment_p0_p1);
}

void procedural_shuriken_float(
    in half2 uv,
    in float x0, in float radius, in float size,
    in float3 colorA, in float3 colorB,
    out float4 Out)
{
    uv -= 0.5;
    uv *= lerp(2.0, 1.0, size);

    const float d0 = 0.9;
    float sharp = shuriken(uv, x0, radius * d0);
    float body = shuriken(uv, x0 * d0, radius);

    float v0 = (sharp * d0) * body;
    float3 l0 = anti_aliasing_step(float3(uv, 1), 0);
    float l0_gray = saturate(dot(l0, float3(0.126, 0.7152, 0.0722)));

    float4 shape = 0;
    shape.rgb = lerp(colorA, colorB, v0);
    shape.rgb *= l0_gray;
    shape.a = sharp;

    // float4 shape = shuriken(uv, x0, radius);
    // shape.rgb *= colorA;

    Out = shape;
}
