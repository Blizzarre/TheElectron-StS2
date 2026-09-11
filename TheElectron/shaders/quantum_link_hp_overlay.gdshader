shader_type canvas_item;

uniform sampler2D noise: repeat_enable;
uniform sampler2D gradient_tex;
uniform float inverse_speed = 10.0;
uniform float line_length = 1000.0;
uniform float scale = 1.0;
uniform float angle: hint_range(0.0, 360.0) = 0.0;

void fragment() {
	float ratio = SCREEN_PIXEL_SIZE.x / SCREEN_PIXEL_SIZE.y;
	vec2 pix_uv = SCREEN_UV * scale;
	pix_uv.y *= ratio;
	vec2 uv = vec2(pix_uv.x * cos(radians(angle)) - pix_uv.y * sin(radians(angle)), pix_uv.x * sin(radians(angle)) + pix_uv.y * cos(radians(angle)));
	vec4 noise_line = texture(noise, vec2(uv.x / line_length + TIME / inverse_speed, uv.y));
	vec4 color;
	color = texture(gradient_tex, vec2(1.0 - noise_line.r));
	COLOR.rgb = color.rgb;
	
	//COLOR = vec4(pix_uv.rg, 0.0, 1.0);
}