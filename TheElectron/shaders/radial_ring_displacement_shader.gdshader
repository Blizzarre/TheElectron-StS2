shader_type canvas_item;

uniform bool shaking       = false;
uniform float amplitude    = 1.0;
uniform float noise_scale_x  = 1.0;
uniform float noise_speed_x  = 10.0;
uniform float noise_scale_y  = 1.0;
uniform float noise_speed_y  = 10.0;
uniform vec2 inv_scaling = vec2(1.0, 1.0);

uniform sampler2D noise: filter_nearest, repeat_enable;

varying vec4 modulate_color;

void vertex() {
	modulate_color = COLOR;
}

void fragment() {
	if (shaking) {
		vec2 uv = (UV - 0.5) * inv_scaling;
		vec2 dir = normalize(uv); // direction from center
		float rad = atan(uv.y, uv.x); //rotation
		float dist = length(uv) * 2.0; // dist from center
		uv += 0.5;
		
		vec2 offset = vec2(TIME * noise_speed_x, TIME * noise_speed_y);
		
		//mat2 rot_max = mat2(vec2(cos(rad), sin(rad)),
						//-vec2(sin(rad), cos(rad)));
		
		float phi = rad * 0.5 / PI + 0.5; // from -PI => PI to 0 to 1
		vec2 polar = vec2(phi, dist);
		
		// Generate two noise values to construct a random direction.
		// sample noise using polar coordinate
		float nx = texture(noise, polar * noise_scale_x + vec2(offset.x, 0.0)).x;
		float ny = texture(noise, polar * noise_scale_y + vec2(0.0, offset.y)).y;
		// Map [0,1] to [-1,1]
		vec2 rand_dir = normalize(vec2(nx * 2.0 - 1.0, ny * 2.0 - 1.0));

		// Use another noise value to determine the jitter amplitude sign.
		float n = texture(noise, (polar + vec2(TIME * noise_speed_x, TIME * noise_speed_y)) * vec2(noise_scale_x, noise_scale_y)).b;
		float off = (n * 2.0 - 1.0) * amplitude;

		// Offset along a random direction
		vec2 uv2 = uv + rand_dir * off * TEXTURE_PIXEL_SIZE;
		COLOR = texture(TEXTURE, uv2);
		COLOR *= modulate_color;
		
		//COLOR = vec4(nx, ny, 0.0, 1.0);
	}
}
