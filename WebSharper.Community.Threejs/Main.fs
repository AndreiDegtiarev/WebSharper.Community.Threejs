namespace ExtTreejs

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let Vector3 = Class "Vector3"
                   |+> Instance [
                                "x" =@ T<double>
                                "z" =@ T<double>
                                "y" =@ T<double>
                                "set" => T<double> * T<double> * T<double> ^-> T<unit>
                                "normalize" => T<unit> ^-> T<unit>

                                ]
    let Object3D = Class "THREE.Object3D"
    Object3D|+> Static [Constructor (T<unit>)]|>ignore
    Object3D|+> Instance [
                        "add" => Object3D.Type ^-> T<unit>
                        "position" =@ Vector3.Type
                        "rotation" =@ Vector3.Type
                          ]|>ignore
    let DirectionalLight = Class "THREE.DirectionalLight"
                            |=> Inherits Object3D
                            |+> Static [Constructor (T<int> * T<double>)]
    let PointLight = Class "THREE.PointLight"
                            |=> Inherits Object3D
                            |+> Static [
                                        Constructor (T<int> * T<double>)
                                        Constructor (T<int> * T<double> * T<double> * T<double>)
                                       ]
    let Camera = Class "THREE.Camera"
                 |=> Inherits Object3D
    let PerspectiveCamera = Class "THREE.PerspectiveCamera"
                            |=> Inherits Camera
                            |+> Static [Constructor (T<double> * T<double> * T<double> * T<double>)]
    let OrthographicCamera = Class "THREE.OrthographicCamera"
                             |=> Inherits Camera
                             |+> Static [Constructor (T<double> * T<double> * T<double> * T<double> * T<double> * T<double>)]
    let Fog = Class "THREE.Fog" 
                            |+> Static [Constructor ( T<int> * T<double>* T<double>)]
                            |+> Instance [
                                "color" =@ T<int>
                                ]
    let MaterialProp = Pattern.Config "MaterialProp" {
            Required = ["color" , T<int>]
            Optional = ["wireframe" , T<bool> ]
        } 
    let Material = Class "THREE.Material" 
                            |+> Static [Constructor (MaterialProp.Type)]
    let MeshBasicMaterial = Class "THREE.MeshBasicMaterial" 
                            |=> Inherits Material
                            |+> Static [Constructor (MaterialProp.Type)]
    let MeshPhongMaterial = Class "THREE.MeshPhongMaterial" 
                            |=> Inherits Material
                            |+> Static [Constructor (MaterialProp.Type)]
    let Scene = Class "THREE.Scene"
                |=> Inherits Object3D
                |+> Static [Constructor (T<unit>)]
                |+> Instance ["fog" =@ Fog.Type]

    let AxisHelper = Class "THREE.AxisHelper"
                     |=> Inherits Object3D
                     |+> Static [Constructor (T<unit>)]

    let WebGLRenderer = Class "THREE.WebGLRenderer"
                        |+> Static [Constructor (T<unit>)]
                        |+> Instance [
                                        "setSize" => T<double> * T<double> ^-> T<unit>
                                        "setClearColor" => T<int> ^-> T<unit>
                                        "domElement" =@ T<Dom.Node>
                                        "render" => Scene.Type * Camera.Type ^-> T<unit>
                                      ]
    let Geometry = Class "Geometry"
    let BoxGeometry = Class "THREE.BoxGeometry"
                      |=> Inherits Geometry
                      |+> Static [Constructor (T<double> * T<double> * T<double>)]
    let CylinderGeometry = Class "THREE.CylinderGeometry"
                           |=> Inherits Geometry
                           |+> Static [Constructor (T<double> * T<double> * T<double> * T<int>)]
    let Mesh =  Class "THREE.Mesh"
                |=> Inherits Object3D
                |+> Static [Constructor (Geometry.Type * Material.Type)]

    let OrbitControls = Class "THREE.OrbitControls"
                        |+> Static [Constructor (Camera.Type * T<Dom.Node>)]
                        |+> Instance ["update" => T<unit> ^-> T<unit>]
    let TrackballControls = Class "THREE.TrackballControls"
                            |+> Static [Constructor (Camera.Type * T<Dom.Node>)]
                            |+> Instance ["update" => T<unit> ^-> T<unit>]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Community.Threejs" [
                Vector3
                Object3D
                PointLight
                DirectionalLight
                Camera
                OrthographicCamera
                PerspectiveCamera
                Fog
                MaterialProp
                Material
                MeshBasicMaterial
                MeshPhongMaterial
                Mesh
                WebGLRenderer
                Scene
                AxisHelper
                Geometry
                BoxGeometry
                CylinderGeometry
                OrbitControls
                TrackballControls
            ]
            Namespace "WebSharper.Community.Threejs.Resources" [
                Resource "ExtTreeJs" "three.js"
                |> fun r -> r.AssemblyWide()
                Resource "ExtTreeJsOrbitControl" "OrbitControls.js"
                |> fun r -> r.AssemblyWide()
                Resource "ExtTreeJsTrackballControl" "TrackballControls.js"
                |> fun r -> r.AssemblyWide()
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
