shader_type canvas_item;

uniform float intensity: hint_range(0.0, 1.0, 0.01) = 1.0;

varying mat4 to_canvas_space;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;

void vertex(){
	to_canvas_space = CANVAS_MATRIX * MODEL_MATRIX;
}

void fragment() {
	vec2 dir = normalize(vec2(0.5) - UV);
	float effect = COLOR.r * (exp(intensity) - 1.0);
	
	vec2 r_displacement = effect * dir * 1.5;
	vec2 g_displacement = effect * dir * 1.0;
	vec2 b_displacement = effect * dir * 0.5;
	
	vec2 r_screen_displacement = (to_canvas_space * vec4((UV + r_displacement - 0.5)/TEXTURE_PIXEL_SIZE, 0.0, 1.0)).xy * SCREEN_PIXEL_SIZE;
	vec2 g_screen_displacement = (to_canvas_space * vec4((UV + g_displacement - 0.5)/TEXTURE_PIXEL_SIZE, 0.0, 1.0)).xy * SCREEN_PIXEL_SIZE;
	vec2 b_screen_displacement = (to_canvas_space * vec4((UV + b_displacement - 0.5)/TEXTURE_PIXEL_SIZE, 0.0, 1.0)).xy * SCREEN_PIXEL_SIZE;
	
	float r = texture(screen_texture, r_screen_displacement).r;
	float g = texture(screen_texture, g_screen_displacement).g;
	float b = texture(screen_texture, b_screen_displacement).b;
	float a = texture(screen_texture, SCREEN_UV).a;
	
	COLOR = vec4(r, g, b, a);
}