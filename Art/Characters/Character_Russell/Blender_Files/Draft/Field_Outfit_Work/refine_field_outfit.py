import bpy, math, ast
from mathutils import Vector
from math import sin,cos,pi
COL=bpy.data.collections['FIELD OUTFIT • Russell']
source=open('/Users/martycow/CedarStation/Art/Characters/Character_Russell/Blender_Files/Draft/Field_Outfit_Work/build_field_outfit.py').read()
for node in ast.parse(source).body:
    if isinstance(node,ast.FunctionDef): exec(compile(ast.Module(body=[node],type_ignores=[]),'helpers','exec'))
charcoal=bpy.data.materials['Field | Charcoal canvas']; panel=bpy.data.materials['Field | Olive equipment canvas']
gold=bpy.data.materials['Field | Ochre binding']; rubber=bpy.data.materials['Field | Rubber']
web=bpy.data.materials['Field | Woven straps']; reflect=bpy.data.materials['Field | Reflective piping']
steel=bpy.data.materials['Field | Brushed fittings']
def remove(name):
    o=bpy.data.objects.get('Field_'+name)
    if o: bpy.data.objects.remove(o,do_unlink=True)

# Fitted sleeve bands, including the asymmetric section of the original sleeve.
dg=bpy.context.evaluated_depsgraph_get()
ev=bpy.data.objects['Field_Jacket_Hood_Down'].evaluated_get(dg)
inv=ev.matrix_world.inverted()
def ray(origin,direction):
    hit,p,n,idx=ev.ray_cast(inv@Vector(origin),(inv.to_3x3()@Vector(direction)).normalized())
    return ev.matrix_world@p if hit else None
for side,label in [(1,'L'),(-1,'R')]:
    o=bpy.data.objects['Field_Sleeve_Reflector_'+label]
    for v in o.data.vertices:
        x=v.co.x; a=math.atan2((v.co.y+.119)/.122,(v.co.z-1.779)/.104)
        center=Vector((x,-.107,1.79)); d=Vector((0,sin(a),cos(a)))
        p=ray(center+d*.4,-d)
        if p is not None: v.co=p+d*.003
    # Build shaped front panels in rows instead of one flat polygon.
    remove('Vest_Panel_'+label); remove('Ochre_Binding_'+label)
    zs=[1.315,1.40,1.56,1.70,1.82,1.865]
    inside=[.065,.083,.12,.145,.175,.20]; outside=[.265,.28,.30,.33,.32,.26]
    yc=[-.405,-.41,-.40,-.405,-.37,-.30]
    vs=[]
    for z,lo,hi,y in zip(zs,inside,outside,yc):
        for t in [0,.5,1]:
            x=lo+(hi-lo)*t; pt=ray((side*x,-2,z),(0,1,0))
            yy=pt.y-.022 if pt is not None and pt.y<-.05 else y+.12*(x/.3)**2
            vs.append((side*x,yy,z))
    faces=[(r*3+c,r*3+c+1,(r+1)*3+c+1,(r+1)*3+c) for r in range(5) for c in range(2)]
    o=mesh('Vest_Panel_'+label,vs,faces,panel)
    s=o.modifiers.new('Canvas thickness','SOLIDIFY'); s.thickness=.012
    boundary=[0,1,2,5,8,11,14,17,16,15,12,9,6,3,0]
    path('Ochre_Binding_'+label,[(vs[i][0],vs[i][1]-.006,vs[i][2]) for i in boundary],.009,gold,1)
    # Broad shoulder straps are fabric ribbons, joined to a fitted back panel.
    remove('Shoulder_Webbing_'+label); remove('Shoulder_Gold_'+label)
    centers=[(side*.23,-.23,1.865),(side*.235,-.11,1.927),(side*.235,.04,1.925),(side*.225,.164,1.84)]
    vv=[(x+dx,y,z) for x,y,z in centers for dx in [-.035,.035]]
    o=mesh('Shoulder_Webbing_'+label,vv,[(i,i+1,i+3,i+2) for i in [0,2,4]],panel)
    s=o.modifiers.new('Canvas thickness','SOLIDIFY'); s.thickness=.012
    for edge in [-.035,.035]: path('Shoulder_Edge_'+label+str(edge),[(x+edge,y-.003,z+.003) for x,y,z in centers],.006,gold)
    # Pull pockets against curved canvas and rotate outer pair around the torso.
    for i in range(2):
        x=.137 if i==0 else .235
        center=Vector((side*x,-.402 if i==0 else -.345,1.435))
        old=Vector((side*(.137 if i==0 else .253),-.448 if i==0 else -.405,1.435))
        angle=side*math.radians(19 if i==0 else 35)
        from mathutils import Matrix
        rot=Matrix.Rotation(angle,3,'Z')
        for prefix in ['Vest_Pouch_','Pocket_Flap_','Pocket_Tab_']:
            item=bpy.data.objects['Field_'+prefix+label+str(i)]
            item.location=center+rot@(item.location-old); item.rotation_euler.z=angle

# Vest back with a small horizontal reflective strip.
v=[]
for z,rx,cy in [(1.32,.264,.16),(1.56,.275,.13),(1.81,.258,.14),(1.87,.20,.13)]:
    for t in [-1,-.5,0,.5,1]:
        x=rx*t; v.append((x,cy+.059*(1-t*t)+.018,z))
o=mesh('Vest_Back',v,[(r*5+c,r*5+c+1,(r+1)*5+c+1,(r+1)*5+c) for r in range(3) for c in range(4)],panel)
s=o.modifiers.new('Back canvas thickness','SOLIDIFY'); s.thickness=.015
for edge in [0,4]: path('Back_Binding_'+str(edge),[v[r*5+edge] for r in range(4)],.008,gold)
path('Back_Hem',v[:5],.008,gold)
path('Back_Reflective',[(x,.215-abs(x)*.15,1.70) for x in [-.19,-.1,0,.1,.19]],.012,reflect)

# Closed front, raised collar and zipper: no floating zip above the neckline.
vv=[]
for z,y,rx in [(1.27,-.38,.12),(1.5,-.385,.13),(1.7,-.381,.145),(1.82,-.325,.12),(1.88,-.29,.095)]:
    for t in [-1,0,1]: vv.append((rx*t,y+abs(t)*.018,z))
o=mesh('Jacket_Closed_Front',vv,[(r*3+c,r*3+c+1,(r+1)*3+c+1,(r+1)*3+c) for r in range(4) for c in range(2)],charcoal)
s=o.modifiers.new('Front thickness','SOLIDIFY'); s.thickness=.01
for n in ['Zip_Tape','Zip_Metal','Zip_Pull']: remove(n)
zippts=[(0,-.397,1.27),(0,-.402,1.5),(0,-.398,1.7),(0,-.342,1.82),(0,-.307,1.88)]
path('Zip_Tape',zippts,.010,rubber)
path('Zip_Metal',[(x,y-.01,z) for x,y,z in zippts],.0035,reflect)
box('Zip_Pull',(0,-.363,1.80),(.023,.014,.04),steel,.003)

# Tuck the fabric inside the boot shafts and cover the heel fully.
pants=bpy.data.objects['Field_Cargo_Pants']; invp=pants.matrix_world.inverted()
for vertex in pants.data.vertices:
    w=pants.matrix_world@vertex.co
    if w.z<.30:
        side=1 if w.x>0 else -1
        w.z=max(.22,w.z)
        w.x=side*.195+(w.x-side*.195)*.85
        w.y=-.13+(w.y+.13)*.70
        vertex.co=invp@w
for side,label in [(1,'L'),(-1,'R')]:
    boot=bpy.data.objects['Field_Boot_'+label]
    for vertex in boot.data.vertices:
        if vertex.co.y>-.14 and vertex.co.z>.06:
            vertex.co.y+=.035
    # Create an ankle gusset behind the laces, where the trouser cuff sits.
    ring('Boot_Ankle_Gusset_'+label,side*.195,-.106,.225,.098,.143,.13,bpy.data.materials['Field | Boot collar'],12)

# Tone down the vest to retain the dark workwear impression of the reference.
node=next(n for n in panel.node_tree.nodes if n.type=='BSDF_PRINCIPLED')
node.inputs['Base Color'].default_value=(.041,.047,.032,1); panel.diffuse_color=(.041,.047,.032,1)
bpy.context.view_layer.update()
print('Fitting pass complete')
