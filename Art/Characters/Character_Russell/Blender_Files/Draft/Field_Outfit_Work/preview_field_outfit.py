import bpy, math
from mathutils import Vector
scene=bpy.context.scene
studio=bpy.data.collections.new('PREVIEW • Studio'); scene.collection.children.link(studio)
for o in scene.objects:
    if o.type=='LIGHT' and not o.name.startswith('Field_'): o.hide_render=True
def add(o):
    for c in list(o.users_collection): c.objects.unlink(o)
    studio.objects.link(o); return o
def area(name,loc,power,color,size):
    light=bpy.data.lights.new(name,'AREA'); light.energy=power; light.color=color; light.shape='DISK'; light.size=size
    o=bpy.data.objects.new(name,light); studio.objects.link(o); o.location=loc
    o.rotation_euler=(Vector((0,0,1.2))-o.location).to_track_quat('-Z','Y').to_euler()
area('Preview_Key',(-3,-4,5),330,(1,.88,.73),4)
area('Preview_Fill',(4,-2,3),240,(.72,.85,1),3)
area('Preview_Rim',(0,3,4.2),430,(.80,.91,1),3)
world=bpy.data.worlds.new('Field Outfit Studio World'); world.use_nodes=True
bg=next(n for n in world.node_tree.nodes if n.type=='BACKGROUND'); bg.inputs['Color'].default_value=(.12,.15,.18,1); bg.inputs['Strength'].default_value=.5; scene.world=world
bpy.ops.mesh.primitive_plane_add(size=200,location=(0,0,-.008)); floor=add(bpy.context.object); floor.name='Preview_Floor'
mat=bpy.data.materials.new('Preview | Slate floor'); mat.use_nodes=True
bs=next(n for n in mat.node_tree.nodes if n.type=='BSDF_PRINCIPLED'); bs.inputs['Base Color'].default_value=(.055,.066,.068,1); bs.inputs['Roughness'].default_value=.95
floor.data.materials.append(mat)
cam=bpy.data.cameras.new('Preview_Camera'); camera=bpy.data.objects.new('Preview_Camera',cam); studio.objects.link(camera)
camera.location=(2.7,-7,3.1); target=Vector((0,-.02,1.22)); camera.rotation_euler=(target-camera.location).to_track_quat('-Z','Y').to_euler()
cam.type='ORTHO'; cam.ortho_scale=3.0; cam.lens=50; scene.camera=camera
scene.render.resolution_x=1400; scene.render.resolution_y=1400; scene.render.resolution_percentage=100
scene.render.image_settings.file_format='PNG'; scene.render.film_transparent=False
scene.render.filepath='/Users/martycow/CedarStation/Art/Characters/Character_Russell/Blender_Files/Draft/Field_Outfit_Work/Russell_Field_Outfit_Preview.png'
# Show the character closely in the interactive viewport; studio helpers stay out of the way.
for o in studio.objects: o.hide_set(True)
for screen in bpy.data.screens:
    for area in screen.areas:
        if area.type=='VIEW_3D':
            space=area.spaces.active
            space.region_3d.view_location=(0,-.05,1.2)
            space.region_3d.view_distance=3.55
            space.region_3d.view_rotation=camera.rotation_euler.to_quaternion()
            space.overlay.show_extras=False
bpy.ops.object.select_all(action='DESELECT')
bpy.ops.wm.save_as_mainfile(filepath=bpy.data.filepath)
bpy.ops.render.render(write_still=True)
print('Preview saved:',scene.render.filepath)
