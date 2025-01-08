#define PI 3.14159265358
#define TWO_PI 6.28318530718

float sinusoidal(float x, float a, float b, float c)
{
    const float u = 0.5;
    return a * sin((x + c) * b) * u + u;
}

float sine_wave(float2 uv, float a, float b, float c)
{
    float fx = uv.y;
    float x = uv.x;

    float f = sinusoidal(x, a, b, c);
    fx -= f;

    return (fx > 0.0) ? 0.0 : 1.0;
}

float tan_wave(float2 uv, float a, float b, float c)
{
    float fx = uv.y;
    float x = uv.x;

    const float px = 0.5;
    float py = sinusoidal(px, a, b, c);
    float m = a * tan(PI / 4.0 * cos((px + c) * b)) * (b / 2.0);

    float f = m * (x - px) + py;
    fx -= f;

    return (fx > 0.0) ? 1.0 : 0.0;
}

float normalized_time(float t, float s)
{
    return frac(t / s);
}

void trigonometric_operators_float(
    in half2 uv,
    in half a, in half b, in half c,
    in float time,
    out float Sin, out float Tan, out float Py
)
{
    float t = normalized_time(time, c) * (TWO_PI / b);

    Sin = sine_wave(uv, a, b, t);
    Tan = tan_wave(uv, a, b, t);
    Py = sinusoidal(0.5, a, b, t);
}
