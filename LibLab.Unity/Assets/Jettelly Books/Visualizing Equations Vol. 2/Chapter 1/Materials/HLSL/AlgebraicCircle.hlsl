#define RADIUS 0.01

void algebraic_circle_half(in half2 uv, in half u, in half b, out half Out)
{
    const half r = RADIUS * RADIUS;

    half x = uv.x - u;
    half y = uv.y - b;

    half circle = x * x + y * y;

    Out = (circle <= r) ? 0.0 : 1.0;
}
