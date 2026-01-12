#version 330 core

in vec3 vWorldPos;
in vec3 vWorldNormal;

uniform vec3 uLightPos[2];
uniform vec3 uLightColor[2];

uniform vec3 uViewPos;
uniform vec3 uObjectColor;
uniform float uShininess;

out vec4 FragColor;

float attenuation(float dist)
{
    const float k1 = 0.09;
    const float k2 = 0.032;
    return 1.0 / (1.0 + k1 * dist + k2 * dist * dist);
}

void main()
{
    vec3 N = normalize(vWorldNormal);
    vec3 V = normalize(uViewPos - vWorldPos);

    const float ambientStrength = 0.08;
    const float specStrength    = 0.55;

    vec3 color = vec3(0.0);

    for (int i = 0; i < 2; i++)
    {
        vec3 Lvec = uLightPos[i] - vWorldPos;
        float dist = length(Lvec);

        vec3 L = Lvec / max(dist, 1e-4);

        float att = attenuation(dist);

        vec3 ambient = ambientStrength * uLightColor[i];

        float diff = max(dot(N, L), 0.0);
        vec3 diffuse = diff * uLightColor[i];

        vec3 H = normalize(L + V);
        float spec = pow(max(dot(N, H), 0.0), max(uShininess, 1.0));
        vec3 specular = specStrength * spec * uLightColor[i];

        color += (ambient + diffuse) * uObjectColor * att
              + specular * att;
    }

    FragColor = vec4(color, 1.0);
}