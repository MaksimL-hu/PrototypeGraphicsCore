#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in float aAge;

uniform mat4 uView;
uniform mat4 uProjection;

out float vAge;

void main()
{
    vAge = aAge;
    gl_Position = uProjection * uView * vec4(aPos, 1.0);
}