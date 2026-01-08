#version 330 core

in vec3 vWorldPos;
in vec3 vWorldNormal;

uniform vec3 uLightPos;
uniform vec3 uLightColor;
uniform vec3 uViewPos;
uniform vec3 uObjectColor;
uniform float uShininess;

out vec4 FragColor;

void main()
{
    vec3 N = normalize(vWorldNormal);
    vec3 L = normalize(uLightPos - vWorldPos);
    vec3 V = normalize(uViewPos - vWorldPos);

    float ambientStrength = 0.12;
    vec3 ambient = ambientStrength * uLightColor;

    float diff = max(dot(N, L), 0.0);
    vec3 diffuse = diff * uLightColor;

    vec3 H = normalize(L + V);
    float spec = pow(max(dot(N, H), 0.0), uShininess);
    float specStrength = 0.65;
    vec3 specular = specStrength * spec * uLightColor;

    vec3 color = (ambient + diffuse) * uObjectColor + specular;
    FragColor = vec4(color, 1.0);
}