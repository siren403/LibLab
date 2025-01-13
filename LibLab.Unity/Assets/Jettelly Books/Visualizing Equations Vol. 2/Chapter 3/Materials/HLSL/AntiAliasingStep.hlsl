float3 anti_aliasing_step(float3 gradient, float edge)
{
    float3 rate_of_change = fwidth(gradient);

    float3 lower_edge = edge - rate_of_change;
    float3 upper_edge = edge + rate_of_change;

    float3 stepped = (gradient - lower_edge) / (upper_edge - lower_edge);
    stepped = saturate(stepped);

    return stepped;
}
