extends Control

const option = preload("res://mods/WebWardrobe/outfitoption.tscn")

onready var line_edit: LineEdit = find_node("LineEdit")
onready var cosmetic_menu = get_parent().get_parent().get_parent()
onready var hud = cosmetic_menu.get_parent().get_parent().get_parent().get_parent()
onready var outfits_cont = find_node("outfits_cont")

var mod = null

func _setup(mod):
	self.mod = mod

func _ready():
	_refresh()

func _refresh():
	for child in outfits_cont.get_children():
		child.queue_free()
	
	for key in mod.saved_outfits.keys():
		var outfit = mod.saved_outfits[key]
		
		var o = option.instance()
		var outfit_button = o.get_child(0)
		outfit_button.text = key
		outfit_button.connect("pressed", self, "_outfit_click", [key])
		var del_button = o.get_child(2)
		del_button.connect("pressed", self, "_del_click", [key])
		
		outfits_cont.add_child(o)

func _outfit_click(name: String) -> void:
	if mod.load_outfit(name):
		PlayerData._send_notification("loaded outfit " + name, 0)
	else:
		PlayerData._send_notification("failed to load outfit " + name, 1)
	if cosmetic_menu.model: cosmetic_menu.model._change_cosmetics()
	
func _del_click(name: String) -> void:
	mod.del_outfit(name)
	_refresh()

func _on_LineEdit_focus_entered():
	hud.typing = true

func _on_LineEdit_focus_exited():
	hud.typing = false

func _on_SaveButton_pressed():
	var text = line_edit.text
	line_edit.clear()
	
	if text == null or text.empty(): return
	
	if mod.save_outfit(text):
		PlayerData._send_notification("saved outfit " + text, 0)
	else:
		PlayerData._send_notification("failed to save outfit", 1)
	
	_refresh()


func _on_ReloadButton_pressed():
	if mod.reload_outfits():
		PlayerData._send_notification("reloaded outfits", 0)
	else:
		PlayerData._send_notification("failed to reload outfits", 1)
	_refresh()
