extends Node3D

const PLAYER_SPEED := 5.0
const TOM_SPEED := 2.7
const LOOK_SENSITIVITY := 0.004

var player: CharacterBody3D
var camera: Camera3D
var tom: CharacterBody3D
var cheese_count := 0
var snack_count := 0
var fridge_door: Node3D
var fridge_open := false
var fridge_food_spawned := false
var tom_visual: Node3D
var animation_clock := 0.0
var sfx_player: AudioStreamPlayer
var music_player: AudioStreamPlayer
var footstep_clock := 0.0
var tom_alert_clock := 0.0
var trap_baited := false
var level_complete := false
var game_over := false
var move_touch_id := -1
var look_touch_id := -1
var move_origin := Vector2.ZERO
var move_current := Vector2.ZERO
var pending_look := Vector2.ZERO
var mission_label: Label
var cheese_label: Label
var hint_label: Label
var use_button: Button

func _ready() -> void:
	_build_audio()
	_build_world()
	_build_house_details()
	_build_player()
	_build_tom()
	_build_collectibles()
	_build_trap()
	_build_hud()
	_update_hud()

func _build_audio() -> void:
	music_player = AudioStreamPlayer.new()
	music_player.name = "OriginalCartoonMusic"
	music_player.volume_db = -3.0
	music_player.stream = _make_audio_stream([523.25, 659.25, 783.99, 659.25, 587.33, 698.46, 880.0, 698.46, 523.25, 659.25, 783.99, 1046.5], 0.24, 0.055, true)
	add_child(music_player)
	music_player.play()
	sfx_player = AudioStreamPlayer.new()
	sfx_player.name = "GameSoundEffects"
	sfx_player.volume_db = 0.0
	add_child(sfx_player)

func _make_audio_stream(notes: Array, note_duration: float, volume: float, looped: bool) -> AudioStreamWAV:
	var rate := 22050
	var frames_per_note := int(rate * note_duration)
	var total_frames := frames_per_note * notes.size()
	var pcm := PackedByteArray()
	pcm.resize(total_frames * 2)
	for frame in range(total_frames):
		var note_index := int(frame / frames_per_note)
		var local_frame := frame % frames_per_note
		var frequency := float(notes[note_index])
		var phase := TAU * frequency * float(local_frame) / float(rate)
		var envelope := minf(1.0, float(local_frame) / 220.0) * minf(1.0, float(frames_per_note - local_frame) / 500.0)
		var wave := (sin(phase) + 0.18 * sin(phase * 2.0)) * volume * envelope
		pcm.encode_s16(frame * 2, int(clampf(wave, -1.0, 1.0) * 32767.0))
	var stream := AudioStreamWAV.new()
	stream.data = pcm
	stream.format = AudioStreamWAV.FORMAT_16_BITS
	stream.mix_rate = rate
	stream.stereo = false
	if looped:
		stream.loop_mode = AudioStreamWAV.LOOP_FORWARD
		stream.loop_begin = 0
		stream.loop_end = total_frames
	return stream

func _play_sfx(notes: Array, note_duration := 0.11, volume := 0.22) -> void:
	if not is_instance_valid(sfx_player):
		return
	sfx_player.stream = _make_audio_stream(notes, note_duration, volume, false)
	sfx_player.play()

func _build_world() -> void:
	var env := WorldEnvironment.new()
	var environment := Environment.new()
	environment.background_mode = Environment.BG_COLOR
	environment.background_color = Color(0.48, 0.68, 0.82)
	environment.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	environment.ambient_light_color = Color(0.72, 0.76, 0.82)
	environment.ambient_light_energy = 0.8
	env.environment = environment
	add_child(env)

	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-48, -32, 0)
	sun.light_energy = 1.15
	sun.shadow_enabled = true
	add_child(sun)

	_make_box("Floor", Vector3(0, -0.25, 0), Vector3(18, 0.5, 14), Color(0.66, 0.48, 0.31))
	_make_box("Back wall", Vector3(0, 2, -7), Vector3(18, 4, 0.35), Color(0.91, 0.82, 0.68))
	_make_box("Front wall left", Vector3(-6.5, 2, 7), Vector3(5, 4, 0.35), Color(0.91, 0.82, 0.68))
	_make_box("Front wall right", Vector3(6.5, 2, 7), Vector3(5, 4, 0.35), Color(0.91, 0.82, 0.68))
	_make_box("Left wall", Vector3(-9, 2, 0), Vector3(0.35, 4, 14), Color(0.86, 0.75, 0.62))
	_make_box("Right wall", Vector3(9, 2, 0), Vector3(0.35, 4, 14), Color(0.86, 0.75, 0.62))

	_make_box("Kitchen counter", Vector3(-5.5, 0.8, -4.5), Vector3(4.5, 1.6, 1.1), Color(0.42, 0.25, 0.16))
	_make_box("Dining table", Vector3(4.2, 0.9, 1.5), Vector3(3.2, 0.18, 1.8), Color(0.48, 0.27, 0.13))
	_make_box("Table leg 1", Vector3(3, 0.35, 0.9), Vector3(0.18, 0.9, 0.18), Color(0.3, 0.18, 0.1))
	_make_box("Table leg 2", Vector3(5.4, 0.35, 0.9), Vector3(0.18, 0.9, 0.18), Color(0.3, 0.18, 0.1))
	_make_box("Table leg 3", Vector3(3, 0.35, 2.1), Vector3(0.18, 0.9, 0.18), Color(0.3, 0.18, 0.1))
	_make_box("Table leg 4", Vector3(5.4, 0.35, 2.1), Vector3(0.18, 0.9, 0.18), Color(0.3, 0.18, 0.1))
	_make_box("Fridge body", Vector3(-7.3, 1.4, -2.2), Vector3(1.2, 2.8, 1.3), Color(0.72, 0.82, 0.88))
	_build_fridge_door()
	_make_box("Rug", Vector3(3.5, 0.015, -2.5), Vector3(4.5, 0.04, 3.2), Color(0.58, 0.22, 0.18), false)

func _make_box(label: String, pos: Vector3, dimensions: Vector3, tint: Color, solid := true) -> void:
	var root: Node3D
	if solid:
		var body := StaticBody3D.new()
		body.name = label
		root = body
	else:
		root = Node3D.new()
		root.name = label
	root.position = pos
	var mesh_instance := MeshInstance3D.new()
	var mesh := BoxMesh.new()
	mesh.size = dimensions
	mesh_instance.mesh = mesh
	var material := StandardMaterial3D.new()
	material.albedo_color = tint
	material.roughness = 0.86
	mesh_instance.material_override = material
	root.add_child(mesh_instance)
	if solid:
		var collision := CollisionShape3D.new()
		var shape := BoxShape3D.new()
		shape.size = dimensions
		collision.shape = shape
		root.add_child(collision)
	add_child(root)



func _build_fridge_door() -> void:
	fridge_door = Node3D.new()
	fridge_door.name = "FridgeDoorHinge"
	fridge_door.position = Vector3(-7.3, 1.4, -1.53)
	add_child(fridge_door)
	var door := MeshInstance3D.new()
	var door_mesh := BoxMesh.new()
	door_mesh.size = Vector3(1.12, 2.65, 0.12)
	door.mesh = door_mesh
	var door_material := StandardMaterial3D.new()
	door_material.albedo_color = Color(0.82, 0.9, 0.94)
	door.material_override = door_material
	door.position = Vector3(0, 0, 0.58)
	fridge_door.add_child(door)
	var handle := MeshInstance3D.new()
	var handle_mesh := BoxMesh.new()
	handle_mesh.size = Vector3(0.07, 0.75, 0.08)
	handle.mesh = handle_mesh
	var handle_material := StandardMaterial3D.new()
	handle_material.albedo_color = Color(0.35, 0.42, 0.48)
	handle.material_override = handle_material
	handle.position = Vector3(0.42, 0, 0.5)
	fridge_door.add_child(handle)

func _build_house_details() -> void:
	_make_box("Window frame", Vector3(0, 2.25, -6.78), Vector3(3.2, 1.7, 0.08), Color(0.28, 0.52, 0.68), false)
	_make_box("Sofa seat", Vector3(5.8, 0.55, -4.4), Vector3(3.2, 0.55, 1.15), Color(0.22, 0.42, 0.62))
	_make_box("Sofa back", Vector3(5.8, 1.25, -4.95), Vector3(3.2, 1.25, 0.28), Color(0.18, 0.34, 0.54))
	_make_box("Cupboard", Vector3(-2.7, 1.05, -6.1), Vector3(2.0, 2.1, 0.65), Color(0.52, 0.30, 0.16))
	_make_box("Wall clock", Vector3(6.8, 2.9, -6.72), Vector3(0.8, 0.8, 0.08), Color(0.95, 0.86, 0.62), false)
	_make_box("Kitchen island", Vector3(-5.2, 0.55, 1.4), Vector3(2.4, 1.1, 1.5), Color(0.55, 0.34, 0.22))
	_make_box("Chair seat", Vector3(2.6, 0.55, 3.1), Vector3(0.9, 0.16, 0.9), Color(0.48, 0.28, 0.14))
	_make_box("Chair back", Vector3(2.6, 1.05, 3.5), Vector3(0.9, 1.0, 0.14), Color(0.48, 0.28, 0.14))
	_add_snack(Vector3(-5.0, 1.45, 1.4), "Apple", Color(0.82, 0.16, 0.12))
	_add_snack(Vector3(5.8, 0.95, -4.4), "Cookie", Color(0.72, 0.48, 0.24))

func _add_snack(pos: Vector3, snack_name: String, tint: Color) -> void:
	var pickup := Area3D.new()
	pickup.name = snack_name
	pickup.position = pos
	pickup.add_to_group("snacks")
	var visual := MeshInstance3D.new()
	var mesh := SphereMesh.new()
	mesh.radius = 0.22
	mesh.height = 0.4
	visual.mesh = mesh
	var material := StandardMaterial3D.new()
	material.albedo_color = tint
	visual.material_override = material
	pickup.add_child(visual)
	var collision := CollisionShape3D.new()
	var shape := SphereShape3D.new()
	shape.radius = 0.45
	collision.shape = shape
	pickup.add_child(collision)
	pickup.body_entered.connect(_on_snack_body_entered.bind(pickup))
	add_child(pickup)

func _on_snack_body_entered(body: Node3D, pickup: Area3D) -> void:
	if body == player and is_instance_valid(pickup):
		snack_count += 1
		_play_sfx([523.25, 783.99], 0.13, 0.24)
		pickup.queue_free()
		_update_hud()

func _build_player() -> void:
	player = CharacterBody3D.new()
	player.name = "JerryPlayer"
	player.position = Vector3(0, 0.58, 4.5)
	var shape_node := CollisionShape3D.new()
	var capsule := CapsuleShape3D.new()
	capsule.radius = 0.28
	capsule.height = 1.15
	shape_node.shape = capsule
	player.add_child(shape_node)
	camera = Camera3D.new()
	camera.position = Vector3(0, 0.48, 0)
	camera.current = true
	player.add_child(camera)
	_build_jerry_hand()
	add_child(player)

func _build_tom() -> void:
	tom = CharacterBody3D.new()
	tom.name = "Tom"
	tom.position = Vector3(4.5, 0.65, -3.5)
	tom_visual = Node3D.new()
	tom_visual.name = "Visual"
	tom.add_child(tom_visual)
	var body_mesh := MeshInstance3D.new()
	var capsule_mesh := CapsuleMesh.new()
	capsule_mesh.radius = 0.48
	capsule_mesh.height = 1.35
	body_mesh.mesh = capsule_mesh
	var gray := StandardMaterial3D.new()
	gray.albedo_color = Color(0.42, 0.45, 0.5)
	body_mesh.material_override = gray
	tom_visual.add_child(body_mesh)
	_decorate_tom(tom_visual)
	var collision := CollisionShape3D.new()
	var body_shape := CapsuleShape3D.new()
	body_shape.radius = 0.42
	body_shape.height = 1.3
	collision.shape = body_shape
	tom.add_child(collision)
	add_child(tom)


func _decorate_tom(root: Node3D) -> void:
	_add_sphere(root, Vector3(0, 0.68, -0.12), Vector3(0.38, 0.34, 0.34), Color(0.5, 0.53, 0.58))
	_add_sphere(root, Vector3(-0.23, 0.98, -0.12), Vector3(0.16, 0.22, 0.12), Color(0.5, 0.53, 0.58))
	_add_sphere(root, Vector3(0.23, 0.98, -0.12), Vector3(0.16, 0.22, 0.12), Color(0.5, 0.53, 0.58))
	_add_sphere(root, Vector3(-0.13, 0.72, -0.42), Vector3(0.07, 0.09, 0.045), Color(0.95, 0.96, 0.88))
	_add_sphere(root, Vector3(0.13, 0.72, -0.42), Vector3(0.07, 0.09, 0.045), Color(0.95, 0.96, 0.88))
	_add_sphere(root, Vector3(-0.13, 0.72, -0.46), Vector3(0.035, 0.055, 0.025), Color(0.08, 0.08, 0.1))
	_add_sphere(root, Vector3(0.13, 0.72, -0.46), Vector3(0.035, 0.055, 0.025), Color(0.08, 0.08, 0.1))
	_add_sphere(root, Vector3(0, 0.58, -0.46), Vector3(0.07, 0.045, 0.035), Color(0.88, 0.55, 0.62))
	var tail := MeshInstance3D.new()
	var tail_mesh := CapsuleMesh.new()
	tail_mesh.radius = 0.075
	tail_mesh.height = 0.9
	tail.mesh = tail_mesh
	tail.position = Vector3(0, 0.12, 0.58)
	tail.rotation_degrees = Vector3(65, 0, 0)
	var tail_material := StandardMaterial3D.new()
	tail_material.albedo_color = Color(0.5, 0.53, 0.58)
	tail.material_override = tail_material
	root.add_child(tail)

func _add_sphere(parent: Node3D, pos: Vector3, scale_value: Vector3, tint: Color) -> void:
	var item := MeshInstance3D.new()
	var sphere := SphereMesh.new()
	sphere.radius = 0.5
	sphere.height = 1.0
	item.mesh = sphere
	item.position = pos
	item.scale = scale_value
	var material := StandardMaterial3D.new()
	material.albedo_color = tint
	material.roughness = 0.8
	item.material_override = material
	parent.add_child(item)

func _build_jerry_hand() -> void:
	var hand := Node3D.new()
	hand.name = "JerryHandView"
	hand.position = Vector3(0.34, -0.32, -0.62)
	camera.add_child(hand)
	_add_sphere(hand, Vector3.ZERO, Vector3(0.16, 0.13, 0.2), Color(0.62, 0.48, 0.38))
	for i in range(3):
		_add_sphere(hand, Vector3(-0.1 + i * 0.1, -0.08, -0.1), Vector3(0.045, 0.045, 0.11), Color(0.72, 0.56, 0.45))

func _process(delta: float) -> void:
	animation_clock += delta
	for item in get_tree().get_nodes_in_group("spinning_cheese"):
		item.rotate_y(delta * 1.4)
	if is_instance_valid(tom_visual) and not level_complete:
		tom_visual.position.y = 0.05 + absf(sin(animation_clock * 7.0)) * 0.09
		tom_visual.rotation.z = sin(animation_clock * 3.5) * 0.035
	if is_instance_valid(fridge_door):
		var target_angle := -1.15 if fridge_open else 0.0
		fridge_door.rotation.y = move_toward(fridge_door.rotation.y, target_angle, delta * 3.0)

func _build_collectibles() -> void:
	_add_cheese(Vector3(-3.5, 0.35, -4.2))
	_add_cheese(Vector3(5.2, 0.35, -4.8))
	_add_cheese(Vector3(6.5, 0.35, 4.5))

func _add_cheese(pos: Vector3) -> void:
	var area := Area3D.new()
	area.position = pos
	area.name = "Cheese"
	area.add_to_group("spinning_cheese")
	var mesh_instance := MeshInstance3D.new()
	var mesh := PrismMesh.new()
	mesh.size = Vector3(0.62, 0.42, 0.18)
	mesh_instance.mesh = mesh
	var material := StandardMaterial3D.new()
	material.albedo_color = Color(1.0, 0.72, 0.12)
	material.metallic = 0.05
	mesh_instance.material_override = material
	area.add_child(mesh_instance)
	var collision := CollisionShape3D.new()
	var sphere := SphereShape3D.new()
	sphere.radius = 0.58
	collision.shape = sphere
	area.add_child(collision)
	area.body_entered.connect(_on_cheese_body_entered.bind(area))
	add_child(area)

func _on_cheese_body_entered(body: Node3D, cheese: Area3D) -> void:
	if body == player and is_instance_valid(cheese):
		cheese_count += 1
		_play_sfx([659.25, 880.0, 987.77], 0.10, 0.24)
		cheese.queue_free()
		_update_hud()

func _build_trap() -> void:
	var trap := Node3D.new()
	trap.name = "Mousetrap"
	trap.position = Vector3(-1.5, 0.08, -5.2)
	add_child(trap)
	var base := MeshInstance3D.new()
	var base_mesh := BoxMesh.new()
	base_mesh.size = Vector3(1.25, 0.16, 0.85)
	base.mesh = base_mesh
	var wood := StandardMaterial3D.new()
	wood.albedo_color = Color(0.65, 0.42, 0.2)
	base.material_override = wood
	trap.add_child(base)
	var spring := MeshInstance3D.new()
	var spring_mesh := BoxMesh.new()
	spring_mesh.size = Vector3(0.08, 0.32, 0.65)
	spring.mesh = spring_mesh
	spring.position.y = 0.2
	var metal := StandardMaterial3D.new()
	metal.albedo_color = Color(0.72, 0.75, 0.8)
	spring.material_override = metal
	trap.add_child(spring)

func _build_hud() -> void:
	var layer := CanvasLayer.new()
	add_child(layer)
	var root := Control.new()
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	root.mouse_filter = Control.MOUSE_FILTER_PASS
	layer.add_child(root)

	cheese_label = Label.new()
	cheese_label.position = Vector2(22, 18)
	cheese_label.add_theme_font_size_override("font_size", 28)
	root.add_child(cheese_label)

	mission_label = Label.new()
	mission_label.position = Vector2(22, 58)
	mission_label.add_theme_font_size_override("font_size", 22)
	root.add_child(mission_label)

	hint_label = Label.new()
	hint_label.text = "LEFT SIDE: MOVE     RIGHT SIDE: LOOK"
	hint_label.position = Vector2(22, -42)
	hint_label.set_anchors_preset(Control.PRESET_BOTTOM_LEFT)
	hint_label.add_theme_font_size_override("font_size", 18)
	root.add_child(hint_label)

	use_button = _make_hud_button(root, "USE", Vector2(-154, -92))
	use_button.pressed.connect(_on_use_pressed)
	var restart_button := _make_hud_button(root, "RESTART", Vector2(-154, -164))
	restart_button.pressed.connect(_restart_level)

func _make_hud_button(parent: Control, caption: String, offset: Vector2) -> Button:
	var button := Button.new()
	button.text = caption
	button.size = Vector2(132, 58)
	button.set_anchors_preset(Control.PRESET_BOTTOM_RIGHT)
	button.position = offset
	button.add_theme_font_size_override("font_size", 20)
	parent.add_child(button)
	return button

func _update_hud() -> void:
	if cheese_label:
		cheese_label.text = "CHEESE: %d / 3    SNACKS: %d / 2" % [cheese_count, snack_count]
	if mission_label:
		if level_complete:
			mission_label.text = "LEVEL COMPLETE! Jerry outsmarted Tom!"
		elif game_over:
			mission_label.text = "TOM CAUGHT JERRY! Tap RESTART."
		elif cheese_count < 3:
			mission_label.text = "Find 3 pieces of cheese."
		elif not trap_baited:
			mission_label.text = "Go to the mousetrap and tap USE."
		else:
			mission_label.text = "Trap baited! Lead Tom to the trap."

func _on_use_pressed() -> void:
	if level_complete or game_over:
		return
	if player.global_position.distance_to(Vector3(-7.3, 0.6, -2.2)) < 3.0:
		fridge_open = not fridge_open
		_play_sfx([196.0, 246.94] if fridge_open else [246.94, 196.0], 0.16, 0.17)
		if fridge_open and not fridge_food_spawned:
			fridge_food_spawned = true
			_add_snack(Vector3(-6.2, 0.35, -0.7), "Fridge apple", Color(0.82, 0.16, 0.12))
			_add_snack(Vector3(-7.7, 0.35, -0.55), "Fridge cheese snack", Color(0.96, 0.72, 0.18))
		mission_label.text = "Fridge opened!" if fridge_open else "Fridge closed."
		return
	if cheese_count >= 3 and not trap_baited and player.global_position.distance_to(Vector3(-1.5, 0, -5.2)) < 3.0:
		trap_baited = true
		_play_sfx([392.0, 523.25, 659.25], 0.15, 0.25)
		_update_hud()
	elif cheese_count < 3:
		mission_label.text = "You need 3 cheese pieces first!"
	else:
		mission_label.text = "Move closer to the mousetrap or fridge and tap USE."

func _restart_level() -> void:
	get_tree().reload_current_scene()

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if event.pressed:
			if event.position.x < get_viewport().get_visible_rect().size.x * 0.5:
				move_touch_id = event.index
				move_origin = event.position
				move_current = event.position
			else:
				look_touch_id = event.index
		else:
			if event.index == move_touch_id:
				move_touch_id = -1
			if event.index == look_touch_id:
				look_touch_id = -1
	elif event is InputEventScreenDrag:
		if event.index == move_touch_id:
			move_current = event.position
		elif event.index == look_touch_id:
			pending_look += event.relative
	elif event is InputEventMouseMotion and Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
		pending_look += event.relative

func _physics_process(delta: float) -> void:
	if not is_instance_valid(player) or level_complete or game_over:
		return
	var move_axis := Vector2.ZERO
	if move_touch_id != -1:
		move_axis = ((move_current - move_origin) / 90.0).limit_length(1.0)
	else:
		move_axis.y = float(Input.is_key_pressed(KEY_S)) - float(Input.is_key_pressed(KEY_W))
		move_axis.x = float(Input.is_key_pressed(KEY_D)) - float(Input.is_key_pressed(KEY_A))

	var look := pending_look
	pending_look = Vector2.ZERO
	player.rotate_y(-look.x * LOOK_SENSITIVITY)
	camera.rotation.x = clampf(camera.rotation.x - look.y * LOOK_SENSITIVITY, -1.1, 1.1)

	var direction := (player.global_transform.basis * Vector3(move_axis.x, 0, move_axis.y)).normalized()
	player.velocity.x = direction.x * PLAYER_SPEED
	player.velocity.z = direction.z * PLAYER_SPEED
	if not player.is_on_floor():
		player.velocity.y -= 18.0 * delta
	else:
		player.velocity.y = -0.2
	player.move_and_slide()
	if direction.length() > 0.1:
		footstep_clock += delta
		if footstep_clock >= 0.38:
			footstep_clock = 0.0
			_play_sfx([165.0, 190.0], 0.055, 0.075)
	else:
		footstep_clock = 0.0
	_update_tom(delta)
	if not trap_baited and tom.global_position.distance_to(player.global_position) < 1.05:
		game_over = true
		_play_sfx([392.0, 311.13, 233.08, 174.61], 0.20, 0.25)
		_update_hud()

func _update_tom(delta: float) -> void:
	if not is_instance_valid(tom):
		return
	var target := player.global_position
	if trap_baited:
		var trap_position := Vector3(-1.5, 0.65, -5.2)
		if tom.global_position.distance_to(trap_position) > 1.15:
			target = trap_position
		else:
			level_complete = true
			_play_sfx([523.25, 659.25, 783.99, 1046.5], 0.22, 0.27)
			tom.velocity = Vector3.ZERO
			_update_hud()
			return
	elif tom.global_position.distance_to(player.global_position) > 8.0:
		target = Vector3(4.5, 0.65, -3.5)
	var chase_distance := tom.global_position.distance_to(player.global_position)
	tom_alert_clock += delta
	if chase_distance < 3.8 and tom_alert_clock >= 2.7:
		tom_alert_clock = 0.0
		_play_sfx([110.0, 130.81, 98.0], 0.16, 0.10)
	var flat := target - tom.global_position
	flat.y = 0
	if flat.length() > 0.2:
		var dir := flat.normalized()
		var chase_speed := TOM_SPEED + minf(float(cheese_count) * 0.18, 0.54)
		tom.velocity.x = dir.x * chase_speed
		tom.velocity.z = dir.z * chase_speed
		tom.look_at(tom.global_position + dir, Vector3.UP)
	else:
		tom.velocity.x = move_toward(tom.velocity.x, 0.0, TOM_SPEED)
		tom.velocity.z = move_toward(tom.velocity.z, 0.0, TOM_SPEED)
	if not tom.is_on_floor():
		tom.velocity.y -= 18.0 * delta
	else:
		tom.velocity.y = -0.2
	tom.move_and_slide()
