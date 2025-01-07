void quadratic_function_half(in half2 uv, in half a, in half b, in half c, in half u, out half3 Out)
{
    half fx = uv.y;
    half x = uv.x;

    half xu = x - u;
    half f = a * (xu * xu) + b * xu + c;

    const half3 red = half3(1.0, 0.0, 0.0);
    const half3 green = half3(0.0, 1.0, 0.0);

    fx -= f;

    Out = (fx > 0.0) ? red : green;
}
