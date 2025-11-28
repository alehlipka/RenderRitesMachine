using RenderRitesDemo.Configuration;
using RenderRitesDemo.Scenes.Demo;
using RenderRitesMachine;

_ = RenderRites.Machine
    .ConfigureRenderSettings(DemoRenderConfiguration.Create())
    .Scenes
    .AddScene<DemoScene>("demo");

RenderRites.Machine.RunWindow("RenderRites Machine Demo");
