extends Node

var saved_outfits: Dictionary = {}

const file_path = "user://outfits.json"

func _ready():
	load_outfits_from_file()

func load_outfit(name: String) -> bool:
	if not saved_outfits.has(name): return false
		
	var player = find_player()
	if player == null: return false
	
	PlayerData.cosmetics_equipped = saved_outfits[name].duplicate()
	player._change_cosmetics()
	return true

func save_outfit(name: String) -> bool:
	saved_outfits[name] = PlayerData.cosmetics_equipped.duplicate()
	save_outfits_to_file()
	return true
	
func del_outfit(name: String):
	saved_outfits.erase(name)

func _inject(hud):
	var main: Node = hud.get_child(0)
	if main == null: return
	
	var outfit: Node = main.find_node("outfit")
	
	var button_container: HBoxContainer = outfit.get_child(1)
	if button_container.get_child_count() != 4: return
	
	var outfit_button: Button = create_button("outfits", "OUTFITS")
	outfit_button.modulate = Color(0.7, 0.7, 0.7)
	outfit_button.size_flags_horizontal = 3
	outfit_button.connect("pressed", self, "_button_clicked", [outfit])
	button_container.add_child(outfit_button)
	
	var tabs: Node = outfit.get_child(2).get_child(0)
	
	var outfits_tab = load("res://mods/WebWardrobe/outfits.tscn").instance()
	outfits_tab.name = "outfits"
	outfits_tab._setup(self)
	tabs.add_child(outfits_tab)

func _find_and_inject():
	var player = find_player()
	if player == null: return
	
	var hud: Node = player.hud
	if hud == null: return
	
	_init(hud)

func _button_clicked(menu):
	menu._change_tab("outfits")





# Utils
func find_player() -> Node:
	var nodes = get_tree().get_nodes_in_group("player")
	
	if nodes.empty(): return null
	return nodes[0]

func create_button(name: String, text: String = name) -> Button:
	var button: Button = Button.new()
	button.name = name
	button.text = text
	return button

func save_outfits_to_file() -> void:
	var file = File.new()
	if file.open(file_path, File.WRITE) == OK:
		var json = to_json(saved_outfits)
		file.store_string(json)
		file.close()
		print("Saved outfits to file!")
	else:
		print("Failed to save outfits")
		
func load_outfits_from_file() -> void:
	var file = File.new()
	if file.open(file_path, File.READ) == OK:
		var json = file.get_as_text()
		saved_outfits = parse_json(json) 
		file.close()
		print("Loaded outfits from file!")
	else:
		print("Failed to load outfits")
