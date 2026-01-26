#version 330 core

in float vAge;

uniform vec3 uColor;
uniform float uAlpha;

out vec4 FragColor;

void main()
{
    float a = clamp(vAge, 0.0, 1.0) * uAlpha;
    a = a * a;
    FragColor = vec4(uColor, a);
}