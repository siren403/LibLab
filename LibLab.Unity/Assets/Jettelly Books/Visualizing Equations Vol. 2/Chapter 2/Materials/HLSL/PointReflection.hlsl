#define PI 3.14159265358
#include "Assets/Jettelly Books/Visualizing Equations Vol. 2/Chapter 1/Materials/HLSL/AlgebraicCircle.hlsl"

float linear_function(float2 uv, float a)
{
    float fx = uv.y;
    float x = uv.x;

    float m = tan(a);
    float f = m * x;
    fx -= f;

    float h = (a < PI / 2) ? 1 : -1;

    return (h * fx > 0.0) ? 1.0 : 0.0;
}

float to_degree(float a)
{
    return a * (PI / 180.0);
}

void point_reflection_float(in float2 uv, in float2 p, in float a, out float3 Out)
{
    uv -= 0.5;
    a = to_degree(a);

    const float3 color_g = float3(0.0, 1.0, 0.0);
    const float3 color_y = float3(1, 1, 0);
    const float3 color_m = float3(1, 0, 1);

    float3 l = linear_function(uv, a) * color_g;

    // normal
    float2 n = float2(-sin(a), cos(a));
    float2 pr = p - 2 * n * (-sin(a) * p.x + cos(a) * p.y);

    float p0 = 0;
    float p1 = 0;

    algebraic_circle_half(uv, p.x, p.y, p0);
    algebraic_circle_half(uv, pr.x, pr.y, p1);

    float3 c = (1 - float3(p0.xxx)) * color_y;
    float3 cr = (1 - float3(p1.xxx)) * color_m;

    float3 render = l + c + cr;

    Out = render;
}
