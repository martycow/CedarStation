import bpy, math
from mathutils import Vector, Matrix
from math import sin, cos, pi

# Run inside Blender. Source objects remain intact and hidden in this version.
COL = bpy.data.collections.new('FIELD OUTFIT • Russell')
bpy.context.scene.collection.children.link(COL)

def material(name, color, rough=.8, metal=0):
    m=bpy.data.materials.new('Field | '+name); m.use_nodes=True
    m.diffuse_color=(*color,1)
    p=next(n for n in m.node_tree.nodes if n.type=='BSDF_PRINCIPLED')
    p.inputs['Base Color'].default_value=(*color,1)
    p.inputs['Roughness'].default_value=rough; p.inputs['Metallic'].default_value=metal
    return m

charcoal=material('Charcoal canvas',(.035,.046,.043))
panel=material('Olive equipment canvas',(.075,.079,.052))
gold=material('Ochre binding',(.43,.29,.065))
teal=material('Petrol cargo fabric',(.075,.16,.175))
teal2=material('Cargo reinforcement',(.056,.12,.13))
rubber=material('Rubber',(.018,.022,.019),.9)
armor=material('Knee shell',(.065,.07,.046),.65)
web=material('Woven straps',(.042,.05,.035))
leather=material('Ochre boot leather',(.27,.16,.045))
leather2=material('Boot collar',(.13,.105,.045))
steel=material('Brushed fittings',(.23,.27,.26),.4,.65)
reflect=material('Reflective piping',(.62,.68,.63),.4,.15)
lens=material('Headlamp lens',(.68,.87,.92),.2,.15)
p=next(n for n in lens.node_tree.nodes if n.type=='BSDF_PRINCIPLED')
p.inputs['Emission Color'].default_value=(.70,.9,1,1)
p.inputs['Emission Strength'].default_value=4

def finish(o,name,mat):
    o.name='Field_'+name
    for c in list(o.users_collection): c.objects.unlink(o)
    COL.objects.link(o)
    if mat: o.data.materials.append(mat)
    return o

def mesh(name, verts, faces, mat):
    me=bpy.data.meshes.new('FieldMesh_'+name); me.from_pydata(verts,[],faces); me.update()
    o=bpy.data.objects.new('Field_'+name,me); COL.objects.link(o)
    if mat: me.materials.append(mat)
    return o

def box(name, loc, size, mat, bevel=.01):
    bpy.ops.mesh.primitive_cube_add(size=1,location=loc)
    o=finish(bpy.context.object,name,mat); o.dimensions=size
    bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
    if bevel:
        mod=o.modifiers.new('Soft manufactured edges','BEVEL'); mod.width=bevel; mod.segments=1
    return o

def path(name, pts, radius, mat, res=0):
    cu=bpy.data.curves.new(name,'CURVE'); cu.dimensions='3D'; cu.resolution_u=1
    cu.bevel_depth=radius; cu.bevel_resolution=res
    sp=cu.splines.new('POLY'); sp.points.add(len(pts)-1)
    for p,co in zip(sp.points,pts): p.co=(*co,1)
    o=bpy.data.objects.new('Field_'+name,cu); COL.objects.link(o); cu.materials.append(mat)
    return o

def ring(name,cx,cy,z,rx,ry,height,mat,n=16):
    v=[(cx+rx*cos(i*2*pi/n),cy+ry*sin(i*2*pi/n),z+dz) for dz in [-height/2,height/2] for i in range(n)]
    f=[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
    o=mesh(name,v,f,mat); s=o.modifiers.new('Strap thickness','SOLIDIFY'); s.thickness=.008
    return o

def cylinder(name, loc, radius, depth, mat, vertices=16):
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices,radius=radius,depth=depth,location=loc,rotation=(pi/2,0,0))
    o=finish(bpy.context.object,name,mat)
    b=o.modifiers.new('Rim bevel','BEVEL'); b.width=.003; b.segments=1
    return o

def duplicate_garment(src,name,mat,key=None):
    original=bpy.data.objects[src]
    o=original.copy(); o.data=original.data.copy(); COL.objects.link(o); o.name='Field_'+name
    o.hide_set(False); o.hide_render=False
    if key is not None: o.data.shape_keys.key_blocks[1].value=key
    # Keep the existing vertex groups and UVs. Rig is not present in this source file.
    for m in list(o.modifiers):
        if m.type=='ARMATURE': o.modifiers.remove(m)
        elif m.type=='SUBSURF': m.levels=1; m.render_levels=1
    o.data.materials.clear(); o.data.materials.append(mat)
    original.hide_set(True); original.hide_render=True
    return o

jacket=duplicate_garment('Outfit_Jacket','Jacket_Hood_Down',charcoal,1)
inv=jacket.matrix_world.inverted()
for key in jacket.data.shape_keys.key_blocks:
    for v in key.data:
        w=jacket.matrix_world@v.co
        if w.z<1.70: w.z=1.27+(w.z-1.0434)*(.43/.6566)
        v.co=inv@w

shirt=duplicate_garment('Outfit_Tshirt','Jacket_Centre',charcoal)
pants=duplicate_garment('Outfit_Pants','Cargo_Pants',teal)
inv=pants.matrix_world.inverted()
for v in pants.data.vertices:
    w=pants.matrix_world@v.co
    if w.z<.95:
        side=1 if w.x>0 else -1
        strength=1.17 if w.z<.77 else 1.08
        w.x=side*.195+(w.x-side*.195)*strength
        w.y=-.125+(w.y+.125)*1.13
    v.co=inv@w
for p in pants.data.polygons: p.use_smooth=False
bpy.data.objects['Outfit_Shoes'].hide_set(True); bpy.data.objects['Outfit_Shoes'].hide_render=True

# Zipper follows the center front of the fitted jacket.
path('Zip_Tape',[(0,-.372,1.26),(0,-.382,1.48),(0,-.369,1.69),(0,-.32,1.85)],.014,rubber)
path('Zip_Metal',[(0,-.388,1.27),(0,-.398,1.48),(0,-.385,1.69),(0,-.336,1.85)],.0045,reflect)
box('Zip_Pull',(0,-.35,1.79),(.025,.015,.053),steel,.004)

# Vest panels and back yoke, with actual ochre edge binding.
for side,label in [(1,'L'),(-1,'R')]:
    outline=[(.065,-.418,1.30),(.279,-.366,1.31),(.315,-.31,1.50),(.325,-.26,1.69),(.342,-.23,1.82),(.238,-.311,1.88),(.147,-.392,1.68)]
    verts=[(side*x,y,z) for x,y,z in outline]
    o=mesh('Vest_Panel_'+label,verts,[tuple(range(len(verts)))],panel)
    s=o.modifiers.new('Canvas thickness','SOLIDIFY'); s.thickness=.016
    b=o.modifiers.new('Bound edges','BEVEL'); b.width=.008; b.segments=1
    path('Ochre_Binding_'+label,verts+[verts[0]],.012,gold,1)
    pts=[(side*.238,-.311,1.88),(side*.26,-.16,1.925),(side*.26,.025,1.91),(side*.24,.154,1.81),(side*.22,.18,1.43)]
    path('Shoulder_Webbing_'+label,pts,.029,panel)
    path('Shoulder_Gold_'+label,[(x+side*.028,y,z+.004) for x,y,z in pts],.007,gold)
    # Two box pockets with flaps and retaining tabs.
    for i,x in enumerate([.137,.253]):
        y=-.448 if i==0 else -.405
        pocket=box('Vest_Pouch_'+label+str(i),(side*x,y,1.435),(.095,.075,.155),panel,.012)
        box('Pocket_Flap_'+label+str(i),(side*x,y-.044,1.49),(.101,.018,.054),web,.006)
        box('Pocket_Tab_'+label+str(i),(side*x,y-.057,1.453),(.018,.012,.055),rubber,.002)
    # Sleeve reflective band fitted to upper-arm cross section.
    n=16; vs=[]
    for x in [.546,.581]:
        for i in range(n):
            a=2*pi*i/n
            vs.append((side*x,-.119+.122*sin(a),1.779+.104*cos(a)))
    band=mesh('Sleeve_Reflector_'+label,vs,[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)],reflect)
    # Cargo thigh pocket on outer leg.
    box('Cargo_Pocket_'+label,(side*.306,-.112,.96),(.068,.20,.22),teal2,.015)
    box('Cargo_Flap_'+label,(side*.345,-.112,1.038),(.016,.208,.052),teal,.005)
    # Knee straps, broad cushion and raised faceted protective cap.
    for z in [.60,.73]: ring('Knee_Strap_'+label+str(z),side*.205,-.133,z,.119,.155,.047,web)
    box('Knee_Cushion_'+label,(side*.205,-.30,.666),(.22,.067,.267),rubber,.035)
    shape=[(-.075,-.135),(.075,-.135),(.112,-.075),(.112,.078),(.072,.135),(-.072,.135),(-.112,.075),(-.112,-.075)]
    v=[(side*.205+x,-.326,z+.666) for x,z in shape]+[(side*.205+x*.80,-.383,z*.81+.666) for x,z in shape]
    f=[tuple(range(8,16))]+[(i,(i+1)%8,(i+1)%8+8,i+8) for i in range(8)]+[tuple(reversed(range(8)))]
    mesh('Knee_Armor_'+label,v,f,armor)
    for dx in [-.083,.083]: cylinder('Knee_Rivet_'+label+str(dx),(side*.205+dx,-.375,.67),.009,.008,steel,8)
    # Work boots: broad toe, ankle shaft, reinforced sole.
    n=12; v=[]
    levels=[(.015,.115,.247,-.235),(.055,.119,.25,-.235),(.125,.112,.237,-.229),(.19,.098,.16,-.165),(.29,.091,.118,-.119)]
    for z,rx,ry,cy in levels:
        for i in range(n):
            a=2*pi*i/n
            v.append((side*.195+rx*cos(a),cy+ry*sin(a),z))
    f=[tuple(reversed(range(n)))]
    for k in range(len(levels)-1): f.extend([(k*n+i,k*n+(i+1)%n,(k+1)*n+(i+1)%n,(k+1)*n+i) for i in range(n)])
    f.append(tuple(range((len(levels)-1)*n,len(levels)*n)))
    boot=mesh('Boot_'+label,v,f,leather); boot.data.materials.append(rubber); boot.data.materials.append(leather2)
    for p in boot.data.polygons:
        if p.center.z<.06: p.material_index=1
        elif p.center.z>.22: p.material_index=2
    ring('Boot_Cuff_'+label,side*.195,-.119,.278,.094,.122,.045,leather2,12)
    for z,y in [(.153,-.331),(.184,-.295),(.216,-.26)]:
        path('Boot_Lace_'+label+str(z),[(side*.195-.052,y,z),(side*.195+.052,y,z+.008)],.006,web)
    for y in [-.40,-.29,-.18,-.07]: box('Sole_Tread_'+label+str(y),(side*.195,y,.014),(.219,.043,.025),rubber,.005)

# Belt and vest closures.
ring('Utility_Belt',0,-.105,1.27,.302,.275,.065,web)
box('Belt_Buckle',(0,-.394,1.265),(.105,.037,.073),steel,.008)
box('Buckle_Inset',(0,-.416,1.265),(.067,.009,.041),rubber,.003)
for z in [1.48,1.59]:
    box('Chest_Strap_'+str(z),(0,-.431,z),(.15,.023,.032),web,.004)
    box('Chest_Clip_'+str(z),(.018,-.45,z),(.052,.025,.044),steel,.004)

# A closed, fitted headband, front mount and concentric lamp components.
ring('Headband',0,-.117,2.287,.198,.245,.062,web,32)
box('Lamp_Backplate',(0,-.369,2.302),(.129,.028,.099),rubber,.012)
cylinder('Lamp_Housing',(0,-.404,2.311),.059,.066,armor,16)
cylinder('Lamp_Bezel',(0,-.442,2.311),.052,.018,steel,16)
cylinder('Lamp_Gasket',(0,-.454,2.311),.044,.012,rubber,20)
cylinder('Lamp_Lens',(0,-.462,2.311),.037,.009,lens,24)
box('Lamp_Switch',(0,-.404,2.373),(.031,.025,.009),gold,.003)
box('Headband_Battery',(0,.137,2.29),(.124,.039,.061),rubber,.008)

# Discreet utility radio matching the reference, with no readable branding.
radio=box('Belt_Radio',(-.266,-.389,1.205),(.103,.068,.17),rubber,.009)
box('Radio_Display',(-.266,-.428,1.24),(.072,.012,.041),reflect,.003)
path('Radio_Antenna',[(-.294,-.383,1.29),(-.307,-.376,1.405)],.007,rubber)
for row in range(3):
    for col in range(3): box('Radio_Key_'+str(row)+str(col),(-.29+col*.024,-.43,1.20-row*.019),(.014,.008,.011),armor,.001)

for o in COL.objects:
    o['outfit']='Russell field outfit / supplied reference'
    o['stage']='Editable modeling pass; new attachments are not rigged'
bpy.ops.object.select_all(action='DESELECT')
bpy.context.view_layer.update()
print('Built',len(COL.objects),'outfit objects')
