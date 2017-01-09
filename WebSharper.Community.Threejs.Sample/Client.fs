namespace WebSharper.Community.Threejs.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client
open WebSharper.Community.Threejs

[<JavaScript>]
module Client =

    let Start input k =
        async {
            let! data = Server.DoSomething input
            return k data
        }
        |> Async.Start

    let Main () =

        let viewPane=Div[Attr.Style "width:100%;height:100%"]
        Div [Attr.Style "width:100%;height:100%" :?> Element
             viewPane]|>! OnAfterRender (fun div ->
                                let scene=new Scene()
                                scene.Fog <- new Fog( 0xadd8e6, 1.0, 1000.0 )

                                let dirLight = new DirectionalLight( 0xffffff, 0.125 )
                                dirLight.Position.Set( 0.0, 0.0, 1.0 )
                                dirLight.Position.Normalize()
                                scene.Add(dirLight)
                                let pointLight = new PointLight( 0xffffff, 1.5 )
                                pointLight.Position.Set( 0.0, 100.0, 90.0 )
                                scene.Add( pointLight )

                                let camera=new PerspectiveCamera(75.0, 1.0, 1.0, 1000.0 )
                                camera.Position.Z <- 200.0;
                                let geometry=new CylinderGeometry(20.0, 40.0, 50.0,20)
                                let material = new MeshPhongMaterial(MaterialProp(color=0xbebebe) )
                                let mesh = new Mesh( geometry, material )
                                let obj=new Object3D()
                                obj.Add(mesh)
                                scene.Add( obj ) 

                                let renderer = new WebGLRenderer()
                                renderer.SetSize(viewPane.Dom.ClientWidth, viewPane.Dom.ClientHeight )
                                viewPane.OnResize((fun el evnt->renderer.SetSize(el.Dom.ClientWidth, el.Dom.ClientHeight )))|>ignore
                                viewPane.Append(renderer.DomElement)
                                //renderer.SetClearColor( 0xf0f0f0 );
                                renderer.SetClearColor(scene.Fog.Color );
                                renderer.Render( scene, camera )

                                let rec loop (start:double) (now:double) = 
                                         mesh.Rotation.X<-mesh.Rotation.X+0.005
                                         mesh.Rotation.Y<-mesh.Rotation.Y+0.01
                                         renderer.Render( scene, camera )
                                         JS.RequestAnimationFrame (fun t -> loop start t) |> ignore 
                                JS.RequestAnimationFrame (fun t -> loop t t) |> ignore 
        )
