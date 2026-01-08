#version 330 core

uniform vec2  uLightScreenPx;
uniform float uRadiusPx;
uniform vec3  uColor;
uniform float uIntensity;

out vec4 FragColor;

float gauss(float x, float sigma)
{
    return exp(-(x * x) / (2.0 * sigma * sigma));
}

void main()
{
    float d = length(gl_FragCoord.xy - uLightScreenPx);

    float r = max(uRadiusPx, 1.0);

    float a = 0.0;
    a += gauss(d, r * 0.18) * 0.85;
    a += gauss(d, r * 0.45) * 0.45;
    a += gauss(d, r * 0.95) * 0.25;

    float cut = 1.0 - smoothstep(r * 1.10, r * 1.70, d);
    a *= cut;

    a = pow(a, 1.15) * uIntensity;

    FragColor = vec4(uColor, clamp(a, 0.0, 1.0));
}