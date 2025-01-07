half tangent(half2 a, half2 b)
{
    return (b.y - a.y) / (b.x - a.x);
}

void segment_half(in half2 uv, in half2 p0, in half2 p1, out half Out)
{
    half fx = uv.y;
    half x = uv.x;
    half m = tangent(p0, p1);

    half f = m * (x - p0.x) + p0.y;
    fx -= f;

    Out = (fx > 0.0) ? 1.0 : 0.0;
}
