extends BaseDialogueTestScene


func _ready() -> void:
	var balloon = load("res://UI/DialogueUI/Balloon.tscn").instantiate()
	get_tree().current_scene.add_child(balloon)
	balloon.start(resource, title)
